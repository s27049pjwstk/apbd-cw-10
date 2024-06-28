using Cw10.Context;
using Cw10.Models;
using Cw10.Models.Dto;
using Cw10.Models.View;
using Microsoft.EntityFrameworkCore;

namespace Cw10.Services;

public class MedicamentService : IMedicamentService {
    public async Task AddPrescription(PrescriptionDto prescriptionDto) {
        if (prescriptionDto.Medicaments.Count > 10)
            throw new Exception("More than 10 Medicaments in prescription!");

        if (prescriptionDto.DueDate < prescriptionDto.Date)
            throw new Exception("DueDate before Date!");


        var dbContext = new HospitalDbContext();

        var doctor = await dbContext.GetDoctor(prescriptionDto.IdDoctor);
        if (doctor == null)
            throw new Exception("Doctor does not exist!");


        foreach (var medicament in prescriptionDto.Medicaments) {
            var temp = await dbContext.GetMedicament(medicament.IdMedicament);
            if (temp == null)
                throw new Exception("Medicament does not exist!");
        }

        var findPatient = await dbContext.Patients
            .Where(e => e.FirstName.Equals(prescriptionDto.Patient.FirstName) &&
                        e.LastName.Equals(prescriptionDto.Patient.LastName) &&
                        e.BirthDate.Equals(prescriptionDto.Patient.BirthDate))
            .FirstOrDefaultAsync();

        int patientId;

        if (findPatient == null) {
            var newPatient = new Patient {
                FirstName = prescriptionDto.Patient.FirstName,
                LastName = prescriptionDto.Patient.LastName,
                BirthDate = prescriptionDto.Patient.BirthDate
            };

            await dbContext.Patients.AddAsync(newPatient);
            await dbContext.SaveChangesAsync();

            patientId = newPatient.IdPatient;
        } else {
            patientId = findPatient.IdPatient;
        }

        var newPrescription = new Prescription {
            Date = prescriptionDto.Date,
            DueDate = prescriptionDto.DueDate,
            IdPatient = patientId,
            IdDoctor = doctor.IdDoctor
        };
        await dbContext.Prescriptions.AddAsync(newPrescription);
        await dbContext.SaveChangesAsync();

        var newPrescriptionId = newPrescription.IdPrescription;

        foreach (var medicament in prescriptionDto.Medicaments) {
            await dbContext.PrescriptionsMedicaments.AddAsync(new Prescription_Medicament {
                IdMedicament = medicament.IdMedicament,
                IdPrescription = newPrescriptionId,
                Dose = medicament.Dose,
                Details = medicament.Details
            });
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<PatientView> GetPatientData(int idPatient) {
        var result = new PatientView();
        var dbContext = new HospitalDbContext();

        var patient = await dbContext.GetPatient(idPatient);
        if (patient == null)
            throw new Exception("Patient does not exist!");


        result.IdPatient = patient.IdPatient;
        result.FirstName = patient.FirstName;
        result.LastName = patient.LastName;
        result.BirthDate = patient.BirthDate;

        var prescriptions = await dbContext.Prescriptions
            .Where(e => e.IdPatient.Equals(idPatient))
            .OrderBy(e => e.DueDate)
            .ToListAsync();

        var prescriptionViews = new List<PrescriptionView>();

        foreach (var prescription in prescriptions) {
            var pv = new PrescriptionView();
            pv.IdPrescription = prescription.IdPrescription;
            pv.Date = prescription.Date;
            pv.DueDate = prescription.DueDate;
            var meds = await dbContext.PrescriptionsMedicaments
                .Join(dbContext.Medicaments, pm => pm.IdMedicament, m => m.IdMedicament,
                    (pm, m) => new { pm, m })
                .Where(t => t.pm.IdPrescription == prescription.IdPrescription)
                .Select(t => new MedicamentView {
                    IdMedicament = t.pm.IdMedicament,
                    Name = t.m.Name,
                    Dose = t.pm.Dose,
                    Description = t.m.Description
                }).ToListAsync();
            pv.Medicaments = meds;

            var doc = await dbContext.Doctors
                .Where(e => e.IdDoctor.Equals(prescription.IdDoctor)).Select(t => new DoctorView {
                    IdDoctor = t.IdDoctor,
                    FirstName = t.FirstName
                })
                .SingleAsync();
            pv.Doctor = doc;

            prescriptionViews.Add(pv);
        }

        result.Prescriptions = prescriptionViews;

        return result;
    }
}