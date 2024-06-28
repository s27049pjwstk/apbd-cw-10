using Cw10.Models;
using Cw10.Models.Dto;
using Cw10.Models.View;

namespace Cw10.Services;

public interface IMedicamentService {
    Task AddPrescription(PrescriptionDto prescriptionDto);
    Task<PatientView> GetPatientData(int idPatient);
}