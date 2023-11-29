namespace CarBuilder.Models;

public class Order 
{
    // Setting default value for TimeStamp because Postman was giving it default value
    public Order()
    {
        TimeStamp = DateTime.Now;
    }
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int WheelId { get; set; }
    public int TechnologyId { get; set; }
    public int PaintId { get; set; }
    public int InteriorId { get; set; }
    public Wheels Wheels { get; set; }
    public Interior Interior { get; set;}
     public Technology Technology { get; set; }
    public PaintColor Paint { get; set;}
}