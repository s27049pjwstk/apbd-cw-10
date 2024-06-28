namespace Cw10.Models.View;

public class PrescriptionView
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentView> Medicaments { get; set; }
    public DoctorView Doctor { get; set; }
}