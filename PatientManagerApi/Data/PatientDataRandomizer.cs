using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace PatientManagerApi.Data;

public class PatientDataRandomizer : BackgroundService
{

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PatientDataRandomizer> _logger;

    public PatientDataRandomizer(
        IServiceScopeFactory scopeFactory,
        ILogger<PatientDataRandomizer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Random Data Updater Service is starting.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a new scope for each iteration
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider
                        .GetRequiredService<HospitalDbContext>();
                    
                    await UpdateRandomData(dbContext, stoppingToken);
                }
                
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Random Data Updater");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    private async Task UpdateRandomData(
        HospitalDbContext dbContext,
        CancellationToken token)
    {
        var random = new Random();
        var patients = await dbContext.PatientData
            .Include(p => p.Parameters)
            .ToListAsync(token);
        
        foreach (var patient in patients)
        {
            foreach (var parameter in patient.Parameters)
            {
                // Example random updates
                switch (parameter.Name)
                {
                    case "Cholesterol":
                        parameter.Value = random.Next(120, 240).ToString();
                        parameter.Alarm = int.Parse(parameter.Value) > 200;
                        break;
                    case "Blood pressure":
                        parameter.Value = $"{random.Next(70, 90)} - {random.Next(120, 160)}";
                        parameter.Alarm = random.NextDouble() > 0.8;
                        break;
                    case "Oxygen saturation":
                        parameter.Value = random.Next(85, 100).ToString();
                        parameter.Alarm = int.Parse(parameter.Value) < 90;
                        break;
                }
            }
        }

        await dbContext.SaveChangesAsync(token);
        _logger.LogInformation("Updated patient parameters at {Time}", DateTime.Now);
    }
}
