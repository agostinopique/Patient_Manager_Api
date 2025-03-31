namespace PatientManagerApi.Models;

public class ParametersDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool Alarm { get; set; }
}