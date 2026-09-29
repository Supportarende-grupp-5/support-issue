namespace SupportIssue.Domain;

public class Technician
{
    public string? TechnicianName { get; set; }
    public int TechnicianId { get; set; }

    public Technician(string name, int id)
    {
        TechnicianName = name;
        TechnicianId = id;
    }
    public static List<Technician> CreateTechnicianList()
    {
        var list = new List<Technician>
        {
            new Technician ("Anna",1),
            new Technician ("Johan",2),
            new Technician ("Sara", 3),
            new Technician ("Erik", 4)
        }; 
        return list;
    }
}

