namespace Cw10.Models.View;

public class PatientView {
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public List<PrescriptionView> Prescriptions { get; set; }
}