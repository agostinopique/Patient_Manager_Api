using PatientManagerApi.Models;
using PatientManagerApi.Models.Response;

namespace PatientManagerApi.Service.Interfaces;

public interface IPatientService
{
    Task<PatientDto> GetPatientData(int patientId);
    Task<IList<PatientDto>> GetAllPatientsData();
}