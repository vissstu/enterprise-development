using Museum.Tests.Fixtures;

namespace Museum.Tests;

/// <summary>
/// Проверяет запросы к предметной модели музея.
/// </summary>
public class QueriesTests(MuseumFixture fixture)
    : IClassFixture<MuseumFixture>
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
                Exhibition = exhibition,
                VisitorCount = exhibition.TourExhibitions
                    .SelectMany(te => te.Tour.Tickets)
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.VisitorCount)
            .Take(5)
            .ToList();

        Assert.Equal(5, result.Count);

        Assert.True(
            result.Zip(result.Skip(1))
                .All(pair =>
                    pair.First.VisitorCount >= pair.Second.VisitorCount));
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
                Tour = tour,
                ParticipantCount = tour.Tickets
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .ToList();

        Assert.NotEmpty(participantCounts);

        var minimumCount = participantCounts
            .Min(x => x.ParticipantCount);

        var result = participantCounts
            .Where(x => x.ParticipantCount == minimumCount)
            .ToList();

        Assert.NotEmpty(result);

        Assert.All(
            result,
            item => Assert.Equal(
                minimumCount,
                item.ParticipantCount));
    }

    /// <summary>
    /// Проверяет получение статистики посещаемости по темам выставок за выбранный период.
    /// </summary>
    [Fact]
    public void AttendanceSummaryByExhibitionTheme()
    {
        var context = fixture.Context;

        var periodStart = new DateOnly(2026, 1, 1);
        var periodEnd = new DateOnly(2026, 6, 30);

        var result = context.Exhibitions
            .SelectMany(exhibition => exhibition.TourExhibitions
                .SelectMany(te => te.Tour.Tickets.Select(ticket => new
                {
                    Theme = exhibition.Theme,
                    TourDate = te.Tour.Date,
                    VisitorId = ticket.VisitorId,
                    Price = ticket.Price
                })))
            .Where(x =>
                x.TourDate >= periodStart &&
                x.TourDate <= periodEnd)
            .GroupBy(x => x.Theme)
            .Select(group => new
            {
                Theme = group.Key,

                TotalVisitors = group
                    .Select(x => x.VisitorId)
                    .Distinct()
                    .Count(),

                TotalTicketCost = group
                    .Sum(x => x.Price),

                AverageVisitorsPerDay = group
                    .GroupBy(x => x.TourDate)
                    .Average(day =>
                        day.Select(x => x.VisitorId)
                            .Distinct()
                            .Count()),

                MinVisitorsPerDay = group
                    .GroupBy(x => x.TourDate)
                    .Min(day =>
                        day.Select(x => x.VisitorId)
                            .Distinct()
                            .Count()),

                MaxVisitorsPerDay = group
                    .GroupBy(x => x.TourDate)
                    .Max(day =>
                        day.Select(x => x.VisitorId)
                            .Distinct()
                            .Count())
            })
            .ToList();

        Assert.NotEmpty(result);

        Assert.All(result, summary =>
        {
            Assert.True(summary.TotalVisitors > 0);
            Assert.True(summary.TotalTicketCost > 0);
            Assert.True(summary.AverageVisitorsPerDay > 0);
            Assert.True(summary.MinVisitorsPerDay > 0);
            Assert.True(
                summary.MaxVisitorsPerDay >=
                summary.MinVisitorsPerDay);
        });
    }

    /// <summary>
    /// Проверяет получение всех экскурсий в выбранном зале за выбранный период.
    /// </summary>
    [Fact]
    public void ToursInSelectedHallForPeriod()
    {
        var context = fixture.Context;

        var selectedHall = context.Exhibitions
            .Select(exhibition => exhibition.HallNumber)
            .First();

        var periodStart = new DateOnly(2026, 1, 1);
        var periodEnd = new DateOnly(2026, 6, 30);

        var result = context.Tours
            .Where(tour =>
                tour.Date >= periodStart &&
                tour.Date <= periodEnd &&
                tour.TourExhibitions.Any(te =>
                    te.Exhibition.HallNumber == selectedHall))
            .Distinct()
            .ToList();

        Assert.All(result, tour =>
        {
            Assert.InRange(
                tour.Date,
                periodStart,
                periodEnd);

            Assert.Contains(
                tour.TourExhibitions,
                te => te.Exhibition.HallNumber == selectedHall);
        });
    }

    /// <summary>
    /// Проверяет получение посетителей выбранной экскурсии
    /// с сортировкой по ФИО.
    /// </summary>
    [Fact]
    public void VisitorsOfSelectedTourSortedByFullName()
    {
        var context = fixture.Context;

        var selectedTour = context.Tours
            .First(tour => tour.Tickets.Count > 0);

        var result = selectedTour.Tickets
            .Select(ticket => ticket.Visitor)
            .DistinctBy(visitor => visitor.Id)
            .OrderBy(visitor => visitor.FullName)
            .ToList();

        Assert.NotEmpty(result);

        Assert.True(
            result.Zip(result.Skip(1))
                .All(pair =>
                    string.Compare(
                        pair.First.FullName,
                        pair.Second.FullName,
                        StringComparison.Ordinal) <= 0));
    }
}
