using EventSignup.Models;

namespace EventSignup.Data
{
    public static class DbInitializer
    {
        public static void Initialize(EventDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Events.Any())
            {
                return; // DB has been seeded
            }

            var events = new Event[]
            {
                new Event
                {
                    Title = "Tech Conference 2026",
                    Description = "A full-day conference about cloud, AI, and enterprise apps.",
                    Date = new DateTime(2026, 3, 27),
                    Location = "Ottawa Convention Centre",
                    BannerUrl = "" // To be updated with Blob storage URL
                },
                new Event
                {
                    Title ="Web Development Workshop",
                    Description = "Learn the latest in .NET and React development.",
                    Date = new DateTime(2026, 4, 15),
                    Location = "Algonquin Tech Hub",
                    BannerUrl = ""
                },
                new Event
                {
                    Title ="AI & Machine Learning Summit",
                    Description = "Deep dive into LLMs and generative AI.",
                    Date = new DateTime(2026, 5, 10),
                    Location = "Toronto Innovation Lab",
                    BannerUrl = ""
                }
            };

            foreach (var e in events)
            {
                context.Events.Add(e);
            }
            context.SaveChanges();

            var attendees = new Attendee[]
            {
                new Attendee { Name = "Alice Smith", Email = "alice@example.com", EventId = events[0].Id },
                new Attendee { Name = "Bob Jones", Email = "bob@example.com", EventId = events[0].Id },
                new Attendee { Name = "Charlie Brown", Email = "charlie@example.com", EventId = events[1].Id },
                new Attendee { Name = "Dana White", Email = "dana@example.com", EventId = events[1].Id },
                new Attendee { Name = "Eve Black", Email = "eve@example.com", EventId = events[2].Id },
                new Attendee { Name = "Frank Miller", Email = "frank@example.com", EventId = events[2].Id }
            };

            foreach (var a in attendees)
            {
                context.Attendees.Add(a);
            }
            context.SaveChanges();
        }
    }
}
