using Librelia.Database;
using Librelia.Models;
using MongoDB.Driver;

namespace Librelia.Services;

public class CleanUpResetPassword : BackgroundService
{
    private readonly ILogger<CheckExpiredBooksService> _logger;
    private readonly IMongoCollection<PasswordResetToken> _resetPasswordRepo;

    public CleanUpResetPassword(ILogger<CheckExpiredBooksService> logger, MongoDBContext database)
    {
        _logger = logger;
        _resetPasswordRepo = database.GetCollection<PasswordResetToken>("Reset-Password");

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DeleteExipiredElement();
            }
            catch (Exception ex)
            {
                _logger.LogError($"(Reset-Password) Errore durante la degli elementi scaduti di Reset-Password : {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Esegue il controllo ogni 24 ore
        }
    }
    
    private async Task DeleteExipiredElement()
    {
        var now = DateTime.UtcNow;
        var filter = Builders<PasswordResetToken>.Filter.Lt(x => x.ExpiresAt, now);
        var result = await _resetPasswordRepo.DeleteManyAsync(filter);
        _logger.LogInformation($"(Reset-Password) Elementi scaduti eliminati: {result.DeletedCount}");
    }

}