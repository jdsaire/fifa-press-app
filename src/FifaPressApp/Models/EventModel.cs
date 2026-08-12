namespace FifaPressApp.Models;

// The shape of one event. No logic lives here — just data.
public class EventModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
}
