using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using ThumbnailGenerator.Models;
using ThumbnailGenerator.Services;

namespace ThumbnailGenerator.Controllers;

[ApiController]
[Route("thumbnails")]
public sealed class ThumbnailsController(
    IConfiguration configuration,
    LinkGenerator linkGenerator,
    ImageService imageService,
    Channel<ThumbnailGenerationJob> channel,
    ConcurrentDictionary<string, ThumbnailGenerationStatus> statusDictionary) : ControllerBase
{
    private readonly string _uploadDirectory = configuration["UploadDirectory"] ?? "uploads";

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile? file)
    {
        if (file is null)
        {
            return BadRequest("No file uploaded.");
        }

        if (!imageService.IsValidImage(file))
        {
            return BadRequest("Invalid image file. Only JPG, PNG, and GIF formats are allowed.");
        }

        var id = Guid.NewGuid().ToString();
        var folderPath = Path.Combine(_uploadDirectory, "images", id);
        var fileName = $"{id}{Path.GetExtension(file.FileName)}";

        var originalFilePath = await imageService.SaveOriginalImageAsync(file, folderPath, fileName);

        var job = new ThumbnailGenerationJob(id, originalFilePath, folderPath);
        await channel.Writer.WriteAsync(job);

        statusDictionary[id] = ThumbnailGenerationStatus.Queued;

        var statusUrl = GetFullyQualifiedUrl(nameof(GetStatus), new { id });
        return Accepted(statusUrl, new { id, status = ThumbnailGenerationStatus.Queued });

        //await imageService.GenerateThumbnailsAsync(originalFilePath, folderPath, id);

        //var thumbnailLinks = ImageService.ThumbnailWidths.ToDictionary(
        //    width => $"w{width}",
        //    width => GetFullyQualifiedUrl(nameof(GetImage), new { id = id, width }));

        //thumbnailLinks.Add("original", GetFullyQualifiedUrl(nameof(GetImage), new { id = id }));

        //return Ok(new { id = id, links = thumbnailLinks });
    }

    [HttpGet("{id}/status")]
    public IActionResult GetStatus(string id)
    {
        if (!statusDictionary.TryGetValue(id, out var status))
        {
            return NotFound();
        }

        var response = new { id, status, links = new Dictionary<string, string>() };

        if (status == ThumbnailGenerationStatus.Completed)
        {
            var thumbnailLinks = ImageService.ThumbnailWidths.ToDictionary(
                width => $"w{width}",
                width => GetFullyQualifiedUrl(nameof(GetImage), new { id, width }));

            thumbnailLinks.Add("original", GetFullyQualifiedUrl(nameof(GetImage), new { id }));

            response = response with { links = thumbnailLinks };

        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetImage(string id, int? width = null)
    {
        var folderPath = Path.Combine(_uploadDirectory, "images", id);

        if (!Directory.Exists(folderPath))
        {
            return NotFound();
        }

        string fileName;

        if (width is null)
        {
            fileName = Directory.GetFiles(folderPath, $"{id}.*").FirstOrDefault() ?? string.Empty;
        }
        else
        {
            fileName = Directory.GetFiles(folderPath, $"{id}_w{width}.*").FirstOrDefault() ?? string.Empty;
        }

        if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName))
        {
            return NotFound();
        }

        var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

        return File(fileStream, "image/jpeg");
    }

    private string GetFullyQualifiedUrl(string actionName, object values)
    {
        return linkGenerator.GetUriByAction(
            HttpContext,
            action: actionName,
            controller: "Thumbnails",
            values: values)
            ?? throw new InvalidOperationException("Failed to generate URL.");
    }
}
