using CustomerManager.Api.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

public sealed class JwtOptionsConfigueOptions : IConfigureOptions<JwtOptions>
{
    private const string ConfigurationSectionName = "Jwt";
    private readonly IConfiguration _configuration;

    public JwtOptionsConfigueOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options) => _configuration.GetSection(ConfigurationSectionName).Bind(options);
}
