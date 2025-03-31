using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagerApi.Models.Response;
using PatientManagerApi.Service.Interfaces;

namespace PatientManageApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("{patientId}/singlePatient")]
    public async Task<ActionResult<PatientResponse>> GetPatientData([FromRoute]int patientId)
    {
        if (patientId == null)
            return BadRequest("No id was provided");

        var result = await _patientService.GetPatientData(patientId);
        if (result == null)
            return NotFound($"Unable to find patient with id {patientId}");

        return Ok(result);
    }

    [HttpGet("allPatients")]
    public async Task<ActionResult<List<PatientResponse>>> GetAllPatientsData()
    {
        var result = await _patientService.GetAllPatientsData();
        if (result == null)
            return NotFound($"No data is available");

        return Ok(result);
    }
}
