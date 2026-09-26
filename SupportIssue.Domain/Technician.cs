namespace SupportIssue.Domain;

public class Technician
{
    public string? TechnicianName { get; set; }
    public int TechnicianId { get; set; }

    List<Technician> technicianList = new()
{
    new Technician { TechnicianId = 1, TechnicianName = "Anna" },
    new Technician { TechnicianId = 2, TechnicianName = "Erik" },
    new Technician { TechnicianId = 3, TechnicianName = "Sara" },
    new Technician { TechnicianId = 4, TechnicianName = "Johan" }
};
}
