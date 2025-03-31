using Microsoft.EntityFrameworkCore;
using PatientManagerApi.Data;
using PatientManagerApi.Models;
using PatientManagerApi.Models.Response;
using PatientManagerApi.Service.Interfaces;

namespace PatientManagerApi.Service;

public class PatientService :IPatientService
{
    private readonly HospitalDbContext  _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<PatientDto> GetPatientData(int patientId)
    {
        if (patientId == null)
        {
            return null;
        }
        
        var result = await _context.PatientData.Include(x => x.Parameters).Where(x => x.Id == patientId).FirstOrDefaultAsync();
        if (result == null)
        {
            return null;
        }

        IList<ParametersDto> parameters = new List<ParametersDto>();

        foreach (var parameter in result.Parameters)
        {
            parameters.Add(new  ParametersDto()
            {
                Id =  parameter.Id,
                Alarm = parameter.Alarm,
                Name = parameter.Name,
                Value = parameter.Value
            });
        }
        
        return new PatientDto()
        {
            Id = result.Id,
            GivenName = result.GivenName,
            FamilyName = result.FamilyName,
            Sex = result.Sex,
            DateOfBirth =  result.DateOfBirth,
            Parameters =  parameters,
        };
    }

    public async Task<IList<PatientDto>> GetAllPatientsData()
    {
        var result = await _context.PatientData.Include(x => x.Parameters).ToListAsync();
        if (result == null || result.Count == 0)
        {
            return null;
        }

        IList<PatientDto> patients = new List<PatientDto>();
        foreach (var patient in result)
        {

            IList<ParametersDto> parameters = new List<ParametersDto>();

            foreach (var parameter in patient.Parameters)
            {
                parameters.Add(new ParametersDto()
                {
                    Id = parameter.Id,
                    Alarm = parameter.Alarm,
                    Name = parameter.Name,
                    Value = parameter.Value
                });
            }

            patients.Add(new PatientDto()
            {
                Id = patient.Id,
                GivenName = patient.GivenName,
                FamilyName = patient.FamilyName,
                Sex = patient.Sex,
                DateOfBirth = patient.DateOfBirth,
                Parameters = parameters,
            });
        }
        
        return patients;
    }
}