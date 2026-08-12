namespace FifaPressApp.Models;

// Every event in this app comes from here — invented sample data, not a
// database or an outside service.
public static class MockEventData
{
    private static readonly string[] Cities =
    {
        "Austin", "Chicago", "Seattle", "Portland", "Denver", "Miami", "Boston", "Phoenix", "Dallas", "Atlanta"
    };

    public static List<EventModel> GetSampleEvents()
    {
        var events = new List<EventModel>
        {
            new() { Id = 1, Name = "Annual Tech Summit", Date = new DateTime(2026, 9, 14), Location = "Grand Convention Center, Austin" },
            new() { Id = 2, Name = "Product Launch Party", Date = new DateTime(2026, 9, 22), Location = "Skyline Rooftop, Chicago" },
            new() { Id = 3, Name = "Community Meetup", Date = new DateTime(2026, 10, 3), Location = "Downtown Library Hall, Seattle" },
            new() { Id = 4, Name = "Charity Gala", Date = new DateTime(2026, 10, 18), Location = "Riverside Ballroom, Portland" },
            new() { Id = 5, Name = "Developer Workshop", Date = new DateTime(2026, 11, 2), Location = "Innovation Hub, Denver" },
        };

        // Generated to give the rendering optimization (Activity 2) a large-enough list to matter.
        for (var i = 6; i <= 50; i++)
        {
            events.Add(new EventModel
            {
                Id = i,
                Name = $"Regional Conference #{i}",
                Date = new DateTime(2026, 9, 1).AddDays(i * 3),
                Location = $"Event Hall, {Cities[i % Cities.Length]}"
            });
        }

        return events;
    }
}
