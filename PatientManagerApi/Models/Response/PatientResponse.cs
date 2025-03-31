namespace PatientManagerApi.Models.Response;

public class PatientResponse
{
    public int Id { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Sex { get; set; }
    public DateTime DateOfBirth { get; set; }
    public IList<ParametersDto> Parameters { get; set; }
}