using Museum.Domain.Enums;
using Museum.Tests.Fixtures;

namespace Museum.Tests;

/// <summary>
/// Проверяет запросы к предметной модели музея.
/// </summary>
public class QueriesTests(MuseumFixture fixture) : IClassFixture<MuseumFixture>
{

    /// <summary>
    /// Проверяет получение пяти выставок с наибольшим количеством посетителей.
    /// </summary>
    [Fact]
    public void TopFiveExhibitionsByVisitorCount()
    {
        var context = fixture.Context;

        var result = context.Exhibitions
            .Select(exhibition => new
            {
                exhibition.Id,
                VisitorCount = exhibition.TourExhibitions
                    .SelectMany(te => te.Tour.Tickets)
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.VisitorCount)
            .ThenBy(x => x.Id)
            .Take(5)
            .Select(x => (x.Id, x.VisitorCount))
            .ToArray();

        var expected = new[]
        {
            (1, 4),
            (2, 3),
            (3, 3),
            (5, 2),
            (4, 1)
        };

        Assert.Equal(expected, result);
    }



    /// <summary>
    /// Проверяет поиск экскурсий с минимальным количеством участников.
    /// </summary>
    [Fact]
    public void ToursWithMinimumParticipantCount()
    {
        var context = fixture.Context;

        var participantCounts = context.Tours
            .Select(tour => new
            {
                tour.Id,
                ParticipantCount = tour.Tickets
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .ToList();

        var minimumCount = participantCounts
            .Min(x => x.ParticipantCount);

        var result = participantCounts
            .Where(x => x.ParticipantCount == minimumCount)
            .OrderBy(x => x.Id)
            .Select(x => (x.Id, x.ParticipantCount))
            .ToArray();

        var expected = new[]
        {
            (9, 0),
            (10, 0)
        };

        Assert.Equal(expected, result);
    }


    /// <summary>
    /// Проверяет получение статистики посещаемости по темам выставок за выбранный период.
    /// </summary>
    [Fact]
    public void AttendanceSummaryByExhibitionTheme()
    {
        var context = fixture.Context;

        var periodStart = new DateOnly(2026, 1, 10);
        var periodEnd = new DateOnly(2026, 1, 11);

        var result = context.Exhibitions
            .Select(exhibition => exhibition.Theme)
            .Distinct()
            .OrderBy(theme => theme)
            .Select(theme =>
            {
                var tickets = context.Exhibitions
                    .Where(exhibition => exhibition.Theme == theme)
                    .SelectMany(exhibition => exhibition.TourExhibitions)
                    .Where(relation =>
                        DateOnly.FromDateTime(relation.Tour.StartDateTime)
                            >= periodStart &&
                        DateOnly.FromDateTime(relation.Tour.StartDateTime)
                            <= periodEnd)
                    .SelectMany(relation => relation.Tour.Tickets)
                    .DistinctBy(ticket => ticket.Id)
                    .ToArray();

                var visitorsPerDay = Enumerable
                    .Range(0, periodEnd.DayNumber - periodStart.DayNumber + 1)
                    .Select(offset => periodStart.AddDays(offset))
                    .Select(date => tickets
                        .Where(ticket =>
                            DateOnly.FromDateTime(ticket.Tour.StartDateTime)
                                == date)
                        .Select(ticket => ticket.VisitorId)
                        .Distinct()
                        .Count())
                    .ToArray();

                return (
                    Theme: theme,
                    TotalVisitors: tickets
                        .Select(ticket => ticket.VisitorId)
                        .Distinct()
                        .Count(),
                    TotalTicketCost: tickets.Sum(ticket => ticket.Price),
                    AverageVisitorsPerDay: visitorsPerDay.Average(),
                    MinVisitorsPerDay: visitorsPerDay.Min(),
                    MaxVisitorsPerDay: visitorsPerDay.Max()
                );
            })
            .ToArray();

        var expected = new[]
        {
            (ExhibitionTheme.History, 5, 600m, 2.5, 1, 4),
            (ExhibitionTheme.Art, 3, 300m, 1.5, 0, 3),
            (ExhibitionTheme.Archaeology, 0, 0m, 0.0, 0, 0),
            (ExhibitionTheme.Science, 0, 0m, 0.0, 0, 0),
            (ExhibitionTheme.Ethnography, 0, 0m, 0.0, 0, 0)
        };

        Assert.Equal(expected, result);
    }


    /// <summary>
    /// Проверяет получение всех экскурсий в выбранном зале за выбранный период.
    /// </summary>
    [Fact]
    public void ToursInSelectedHallForPeriod()
    {
        var context = fixture.Context;

        const int selectedHall = 1;
        var periodStart = new DateOnly(2026, 1, 10);
        var periodEnd = new DateOnly(2026, 1, 12);

        var result = context.Tours
            .Where(tour =>
                DateOnly.FromDateTime(tour.StartDateTime) >= periodStart &&
                DateOnly.FromDateTime(tour.StartDateTime) <= periodEnd &&
                tour.TourExhibitions.Any(relation =>
                    relation.Exhibition.HallNumber == selectedHall))
            .OrderBy(tour => tour.StartDateTime)
            .ThenBy(tour => tour.Id)
            .Select(tour => (
                tour.Id,
                tour.StartDateTime,
                tour.Duration
            ))
            .ToArray();

        var expected = new[]
        {
            (1, new DateTime(2026, 1, 10, 10, 0, 0),
                TimeSpan.FromMinutes(60)),
            (2, new DateTime(2026, 1, 10, 10, 0, 0),
                TimeSpan.FromMinutes(60)),
            (3, new DateTime(2026, 1, 11, 10, 0, 0),
                TimeSpan.FromMinutes(60))
        };

        Assert.Equal(expected, result);
    }


    /// <summary>
    /// Проверяет получение посетителей выбранной экскурсии
    /// с сортировкой по ФИО.
    /// </summary>
    [Fact]
    public void VisitorsOfSelectedTourSortedByFullName()
    {
        var context = fixture.Context;

        const int selectedTourId = 1;

        var result = context.Tickets
            .Where(ticket => ticket.TourId == selectedTourId)
            .Select(ticket => ticket.Visitor)
            .DistinctBy(visitor => visitor.Id)
            .OrderBy(visitor => visitor.FullName, StringComparer.Ordinal)
            .Select(visitor => (
                visitor.Id,
                visitor.FullName,
                visitor.Phone,
                visitor.BirthDate
            ))
            .ToArray();

        var expected = new[]
        {
            (
                1,
                "Анна Иванова",
                "+79990000001",
                new DateOnly(1990, 1, 1)
            ),
            (
                2,
                "Борис Петров",
                "+79990000002",
                new DateOnly(1990, 1, 2)
            ),
            (
                3,
                "Вера Смирнова",
                "+79990000003",
                new DateOnly(1990, 1, 3)
            )
        };

        Assert.Equal(expected, result);
    }

}
