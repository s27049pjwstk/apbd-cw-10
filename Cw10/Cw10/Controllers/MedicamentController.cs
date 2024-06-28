using Cw10.Models;
using Cw10.Models.Dto;
using Cw10.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw10.Controllers;

[ApiController]
[Route("api/")]
public class MedicamentController : ControllerBase {
    private readonly IMedicamentService _medicamentService;

    public MedicamentController(IMedicamentService medicamentService) {
        _medicamentService = medicamentService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription(PrescriptionDto prescriptionDto) {
        try {
            await _medicamentService.AddPrescription(prescriptionDto);
            return Ok("Prescription added");
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("patient/{idPatient}")]
    public async Task<IActionResult> GetPatientData(int idPatient) {
        try {
            return Ok(await _medicamentService.GetPatientData(idPatient));
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
}