namespace EventEase.Models;

public static class MockEventData
{
    public static List<EventModel> GetSampleEvents() => new()
    {
        new EventModel { Id = 1, Name = "Annual Tech Summit", Date = new DateTime(2026, 9, 14), Location = "Grand Convention Center, Austin" },
        new EventModel { Id = 2, Name = "Product Launch Party", Date = new DateTime(2026, 9, 22), Location = "Skyline Rooftop, Chicago" },
        new EventModel { Id = 3, Name = "Community Meetup", Date = new DateTime(2026, 10, 3), Location = "Downtown Library Hall, Seattle" },
        new EventModel { Id = 4, Name = "Charity Gala", Date = new DateTime(2026, 10, 18), Location = "Riverside Ballroom, Portland" },
        new EventModel { Id = 5, Name = "Developer Workshop", Date = new DateTime(2026, 11, 2), Location = "Innovation Hub, Denver" },
    };
}
