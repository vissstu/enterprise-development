
using Museum.Domain.Entities;
using Museum.Domain.Enums;

namespace Museum.Domain.Data;

/// <summary>
/// Создаёт фиксированные тестовые данные музея.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Заполняет контекст заранее определёнными данными.
    /// </summary>
    public static MuseumContext Seed()
    {
        var context = new MuseumContext();

        CreateExhibitions(context);
        CreateVisitors(context);
        CreateTours(context);
        CreateTourExhibitions(context);
        CreateTickets(context);

        return context;
    }

    private static void CreateExhibitions(MuseumContext context)
    {
        ExhibitionTheme[] themes =
        [
            ExhibitionTheme.History,
            ExhibitionTheme.Art,
            ExhibitionTheme.History,
            ExhibitionTheme.Science,
            ExhibitionTheme.Science,
            ExhibitionTheme.Art,
            ExhibitionTheme.Archaeology,
            ExhibitionTheme.Ethnography,
            ExhibitionTheme.Archaeology,
            ExhibitionTheme.Ethnography
        ];

        int[] halls = [1, 2, 1, 3, 3, 2, 4, 5, 4, 5];

        for (var i = 1; i <= 10; i++)
        {
            context.Exhibitions.Add(new Exhibition
            {
                Id = i,
                Name = $"Выставка {i}",
                Theme = themes[i - 1],
                HallNumber = halls[i - 1],
                StartDate = new DateOnly(2026, 1, 1),
                EndDate = new DateOnly(2026, 1, 31)
            });
        }
    }

    private static void CreateVisitors(MuseumContext context)
    {
        string[] names =
        [
            "Анна Иванова",
            "Борис Петров",
            "Вера Смирнова",
            "Галина Орлова",
            "Дмитрий Соколов",
            "Елена Кузнецова",
            "Жанна Волкова",
            "Зоя Алексеева",
            "Иван Морозов",
            "Кира Федорова"
        ];

        for (var i = 1; i <= 10; i++)
        {
            context.Visitors.Add(new Visitor
            {
                Id = i,
                FullName = names[i - 1],
                Phone = $"+799900000{i:00}",
                BirthDate = new DateOnly(1990, 1, i)
            });
        }
    }

    private static void CreateTours(MuseumContext context)
    {
        int[] tourDays = [10, 10, 11, 12, 12, 13, 14, 15, 16, 17];

        for (var i = 1; i <= 10; i++)
        {
            context.Tours.Add(new Tour
            {
                Id = i,
                StartDateTime = new DateTime(
                    2026, 1, tourDays[i - 1], 10, 0, 0),
                Duration = TimeSpan.FromMinutes(60)
            });
        }
    }

    private static void CreateTourExhibitions(MuseumContext context)
    {
        (int tourId, int exhibitionId)[] connections =
        [
            (1, 1),
            (1, 2),
            (2, 1),
            (2, 3),
            (3, 3),
            (4, 4),
            (5, 5),
            (6, 6),
            (7, 7),
            (8, 8),
            (9, 9),
            (10, 10)
        ];

        foreach (var (tourId, exhibitionId) in connections)
        {
            var tour = context.Tours.Single(x => x.Id == tourId);
            var exhibition = context.Exhibitions.Single(
                x => x.Id == exhibitionId);

            var relation = new TourExhibition
            {
                TourId = tourId,
                ExhibitionId = exhibitionId,
                Tour = tour,
                Exhibition = exhibition
            };

            context.TourExhibitions.Add(relation);
            tour.TourExhibitions.Add(relation);
            exhibition.TourExhibitions.Add(relation);
        }
    }

    private static void CreateTickets(MuseumContext context)
    {
        (int tourId, int visitorId)[] purchases =
        [
            (1, 3),
            (1, 2),
            (1, 1),
            (2, 2),
            (2, 4),
            (3, 5),
            (4, 6),
            (5, 7),
            (5, 8),
            (6, 9),
            (7, 10),
            (8, 1)
        ];

        for (var i = 0; i < purchases.Length; i++)
        {
            var (tourId, visitorId) = purchases[i];

            var tour = context.Tours.Single(x => x.Id == tourId);
            var visitor = context.Visitors.Single(
                x => x.Id == visitorId);

            var ticket = new Ticket
            {
                Id = i + 1,
                TourId = tourId,
                VisitorId = visitorId,
                Type = TicketType.Adult,
                Price = 100m,
                Tour = tour,
                Visitor = visitor
            };

            context.Tickets.Add(ticket);
            tour.Tickets.Add(ticket);
            visitor.Tickets.Add(ticket);
        }
    }
}
