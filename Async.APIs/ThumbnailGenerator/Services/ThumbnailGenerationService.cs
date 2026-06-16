using System.Collections.Concurrent;
using System.Threading.Channels;
using ThumbnailGenerator.Models;

namespace ThumbnailGenerator.Services;

public class ThumbnailGenerationService(
    ILogger<ThumbnailGenerationService> logger,
    ImageService imageService,
    Channel<ThumbnailGenerationJob> channel,
    ConcurrentDictionary<string, ThumbnailGenerationStatus> statusDictionary) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessJobAsync(job);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing thumbnail generation job");
            }
        }
    }

    private async Task ProcessJobAsync(ThumbnailGenerationJob job)
    {
        statusDictionary[job.Id] = ThumbnailGenerationStatus.Processing;

        try
        {
            await imageService.GenerateThumbnailsAsync(job.OriginalFilePath, job.FolderPath, job.Id);
            statusDictionary[job.Id] = ThumbnailGenerationStatus.Completed;
        }
        catch (Exception e)
        {
            statusDictionary[job.Id] = ThumbnailGenerationStatus.Failed;
            throw;
        }

    }
}