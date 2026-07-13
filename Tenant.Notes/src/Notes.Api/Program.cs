using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Notes.Api.Data;
using Notes.Api.Domain;
using Notes.Api.DTOs;
using Notes.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHybridCache();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("notes-db")));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

app.MapDefaultEndpoints();

//app.mapdefaul

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    await DataSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/me", async (IUserContext userContext) =>
{
    return Results.Ok(new { userId = userContext.UserId, tenantId = await userContext.GetTenantId() });
}).RequireAuthorization();

app.MapGet("/notes", async (ApplicationDbContext dbContext, IUserContext userContext) =>
{
    var tenantId = await userContext.GetTenantId();

    var notes = await dbContext.Notes
        .ForTenant(tenantId)
        .Select(n => new NoteDto(n.Id, n.Content, n.CreatedAt))
        .ToListAsync();

    return Results.Ok(notes);
}).RequireAuthorization();


app.MapGet("/notes/{id:guid}", async (Guid id, ApplicationDbContext dbContext, IUserContext userContext) =>
{
    var tenantId = await userContext.GetTenantId();

    var note = await dbContext.Notes
        .ForTenant(tenantId)
        .FirstOrDefaultAsync(n => n.Id == id);

    if (note == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new NoteDto(note.Id, note.Content, note.CreatedAt));
}).RequireAuthorization();


app.MapPost("/notes", async (CreateNoteRequest request, ApplicationDbContext dbContext, IUserContext userContext) =>
{
    var tenantId = await userContext.GetTenantId();

    var note = new Note
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        UserId = userContext.UserId,
        Content = request.Content,
        CreatedAt = DateTime.UtcNow
    };

    dbContext.Notes.Add(note);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/notes/{note.Id}", new NoteDto(note.Id, note.Content, note.CreatedAt));
}).RequireAuthorization();

app.MapDelete("/notes/{id:guid}", async (
    Guid id,
    ApplicationDbContext dbContext,
    IUserContext userContext) =>
{
    var tenantId = await userContext.GetTenantId();

    var note = await dbContext.Notes
        .ForTenant(tenantId)
        .FirstOrDefaultAsync(n => n.Id == id);

    if (note == null)
    {
        return Results.NotFound();
    }

    dbContext.Notes.Remove(note);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

app.Run();
