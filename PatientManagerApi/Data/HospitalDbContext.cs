using Microsoft.EntityFrameworkCore;
using PatientManagerApi.Models.MedicUser;

namespace PatientManagerApi.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<UserContract> UserLoginCredentials{ get; set; }
    public DbSet<PatientContract> PatientData { get; set; }
}