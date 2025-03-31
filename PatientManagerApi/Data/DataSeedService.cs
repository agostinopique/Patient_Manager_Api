


using Microsoft.AspNetCore.Identity;
using PatientManagerApi.Data;
using PatientManagerApi.Models.MedicUser;

namespace MyApi.Services;

public static class DataSeedService
{
    // private static readonly IPasswordHasher<UserContract> _passwordHasher;
    
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
        SeedPatients(dbContext);
    }

    private static void SeedPatients(HospitalDbContext context)
    {
        if (context.PatientData.Any()) return;
        var patients = new List<PatientContract>
        {
            new() { GivenName = "Lawrence", FamilyName = "Parker", DateOfBirth =  DateTime.Now.AddYears(-30), Sex = "male", Parameters =  new List<ParametersContract>()},
            new() { GivenName = "Pamela", FamilyName = "Clide", DateOfBirth =  DateTime.Now.AddYears(-30), Sex = "female", Parameters =  new List<ParametersContract>()},
            new() { GivenName = "Andrew", FamilyName = "Lee", DateOfBirth =  DateTime.Now.AddYears(-30), Sex = "male", Parameters =  new List<ParametersContract>()},
            new() { GivenName = "Arthur", FamilyName = "Smith", DateOfBirth =  DateTime.Now.AddYears(-30), Sex = "male", Parameters =  new List<ParametersContract>() }
        };
        
        foreach (var patient in patients)
        {
            patient.Parameters = new List<ParametersContract>()
            {
               new() {
                    // Id = 0,
                    Name = "Cholesterol",
                    Value = "146",
                    Alarm = false
                },
                new() {
                    // Id = 1,
                    Name = "Blood pressure",
                    Value = "90 - 150",
                    Alarm = true
                },
                new() {
                   //Id = 2,
                    Name = "Oxygen saturation",
                    Value = "95",
                    Alarm = false
                }
            };
        }
        
        context.PatientData.AddRange(patients);
        context.SaveChanges();
    }
}