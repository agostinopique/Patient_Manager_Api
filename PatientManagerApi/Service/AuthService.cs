using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientManagerApi.Data;
using PatientManagerApi.Models.MedicUser;
using PatientManagerApi.Service.Interfaces;

namespace PatientManagerApi.Service;

public class AuthService : IAuthService
{
    private readonly HospitalDbContext  _context;
    private readonly IPasswordHasher<UserRequest> _passwordHasher;
    public AuthService(HospitalDbContext context, IPasswordHasher<UserRequest> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> RegisterUser(UserRequest userRequest)
    {
        // Verifica se l'utente esiste già
        var existingUser = await _context.UserLoginCredentials.FirstOrDefaultAsync(u => u.Username == userRequest.Email);
        if (existingUser != null)
        {
            return false;
        }

        // Hash della password
        var passwordHash = _passwordHasher.HashPassword(userRequest, userRequest.Password);

        // Crea un nuovo utente
        var user = new UserContract()
        {
            Username = userRequest.Email,
            Password = passwordHash
        };
        
        _context.UserLoginCredentials.Add(user);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> LoginUser(UserRequest userRequest)
    {
        var userContract = await _context.UserLoginCredentials.FirstOrDefaultAsync(u => u.Username == userRequest.Email);
        if (userContract == null)
        {
            return false;
        }
        
        var passwordIsCorrect = _passwordHasher.VerifyHashedPassword(userRequest, userContract.Password, userRequest.Password); 
        if (passwordIsCorrect == PasswordVerificationResult.Failed)
        {
            return false;
        }

        return true;
    }
}