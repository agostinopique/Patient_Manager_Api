using PatientManagerApi.Models.MedicUser;

namespace PatientManagerApi.Service.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterUser(UserRequest userRequest);
    Task<bool> LoginUser(UserRequest userRequest);
}