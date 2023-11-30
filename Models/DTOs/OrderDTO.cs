namespace CarBuilder.Models.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int WheelId { get; set; }
    public int TechnologyId { get; set; }
    public int PaintId { get; set; }
    public int InteriorId { get; set; }
     public WheelsDTO Wheels { get; set; }
    public InteriorDTO Interior { get; set;}
     public TechnologyDTO Technology { get; set; }
    public PaintColorDTO Paint { get; set;}
    public decimal TotalCost { get; set;}
    public bool Completed { get; set; }
}