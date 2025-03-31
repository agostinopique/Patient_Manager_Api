using System.ComponentModel.DataAnnotations;

namespace PatientManagerApi.Models.MedicUser;

public class UserContract
{
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}