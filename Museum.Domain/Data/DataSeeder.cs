using Bogus;
using Museum.Domain.Entities;
using Museum.Domain.Enums;

namespace Museum.Domain.Data;

/// <summary>
/// Генерирует тестовые данные для предметной области музея.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Создаёт и заполняет контекст тестовыми данными.
    /// </summary>
    /// <returns>Заполненный контекст музея.</returns>
    public static MuseumContext Seed()
    {
        Randomizer.Seed = new Random(12345);

        var context = new MuseumContext();

        CreateExhibitions(context);
        CreateVisitors(context);
        CreateTours(context);
        CreateTickets(context);

        return context;
    }

    /// <summary>
    /// Создаёт выставки музея.
    /// </summary>
    private static void CreateExhibitions(MuseumContext context)
    {
        var themes = Enum.GetValues<ExhibitionTheme>();

        var exhibitionFaker = new Faker<Exhibition>("ru")
            .RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.Name, f => $"Выставка «{f.Commerce.ProductName()}»")
            .RuleFor(x => x.Theme, f => f.PickRandom(themes))
            .RuleFor(x => x.HallNumber, f => f.Random.Int(1, 5))
            .RuleFor(x => x.StartDate, f => DateOnly.FromDateTime(
                f.Date.Between(
                    new DateTime(2025, 1, 1),
                    new DateTime(2026, 6, 1))))
            .RuleFor(x => x.EndDate, (f, exhibition) =>
                exhibition.StartDate.AddDays(f.Random.Int(30, 180)));

        context.Exhibitions.AddRange(exhibitionFaker.Generate(10));
    }

    /// <summary>
    /// Создаёт посетителей музея.
    /// </summary>
    private static void CreateVisitors(MuseumContext context)
    {
        var visitorFaker = new Faker<Visitor>("ru")
            .RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.FullName, f => f.Person.FullName)
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.BirthDate, f => DateOnly.FromDateTime(
                f.Date.Between(
                    new DateTime(1950, 1, 1),
                    new DateTime(2007, 12, 31))));

        context.Visitors.AddRange(visitorFaker.Generate(50));
    }

    /// <summary>
    /// Создаёт экскурсии и связывает их с выставками.
    /// </summary>
    private static void CreateTours(MuseumContext context)
    {
        var tourFaker = new Faker<Tour>()
            .RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.Date, f => DateOnly.FromDateTime(
                f.Date.Between(
                    new DateTime(2026, 1, 1),
                    new DateTime(2026, 6, 30))))
            .RuleFor(x => x.StartTime, f => new TimeOnly(
                f.Random.Int(10, 18),
                f.Random.Int(0, 3) * 15))
            .RuleFor(x => x.Duration, f => TimeSpan.FromMinutes(
                f.Random.Int(45, 120)));

        var tours = tourFaker.Generate(30);

        foreach (var tour in tours)
        {
            var exhibitions = context.Exhibitions
                .Where(x => x.StartDate <= tour.Date && x.EndDate >= tour.Date)
                .OrderBy(_ => Guid.NewGuid())
                .Take(Random.Shared.Next(1, 4))
                .ToList();

            foreach (var exhibition in exhibitions)
            {
                var tourExhibition = new TourExhibition
                {
                    TourId = tour.Id,
                    ExhibitionId = exhibition.Id,
                    Tour = tour,
                    Exhibition = exhibition
                };

                tour.TourExhibitions.Add(tourExhibition);
                exhibition.TourExhibitions.Add(tourExhibition);
                context.TourExhibitions.Add(tourExhibition);
            }
        }

        context.Tours.AddRange(tours);
    }

    /// <summary>
    /// Создаёт билеты на экскурсии.
    /// </summary>
    private static void CreateTickets(MuseumContext context)
    {
        var ticketFaker = new Faker<Ticket>()
            .RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.TourId, f => f.PickRandom(context.Tours).Id)
            .RuleFor(x => x.VisitorId, f => f.PickRandom(context.Visitors).Id)
            .RuleFor(x => x.Type, f => f.Random.Bool()
                ? TicketType.Adult
                : TicketType.Concessionary)
            .RuleFor(x => x.Price, (f, ticket) =>
                ticket.Type == TicketType.Adult
                    ? f.Random.Decimal(500, 1000)
                    : f.Random.Decimal(200, 500));

        var tickets = ticketFaker.Generate(150);

        foreach (var ticket in tickets)
        {
            ticket.Tour = context.Tours.First(x => x.Id == ticket.TourId);
            ticket.Visitor = context.Visitors.First(x => x.Id == ticket.VisitorId);

            ticket.Tour.Tickets.Add(ticket);
            ticket.Visitor.Tickets.Add(ticket);
        }

        context.Tickets.AddRange(tickets);
    }
}