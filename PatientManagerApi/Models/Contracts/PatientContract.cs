using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientManagerApi.Models.MedicUser;

public class PatientContract
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Sex { get; set; }
    public DateTime DateOfBirth { get; set; }
    public IList<ParametersContract> Parameters { get; set; }
}