using Museum.Tests.Fixtures;

namespace Museum.Tests;

/// <summary>
/// Проверяет корректность сгенерированной предметной модели музея.
/// </summary>
public class DomainTests(MuseumFixture fixture)
    : IClassFixture<MuseumFixture>
{
    [Fact]
    public void SeederShouldCreateEnoughData()
    {
        var context = fixture.Context;

        Assert.True(context.Exhibitions.Count >= 10);
        Assert.True(context.Tours.Count >= 30);
        Assert.True(context.Visitors.Count >= 50);
        Assert.True(context.Tickets.Count >= 100);
    }

    [Fact]
    public void TicketsShouldHaveVisitors()
    {
        var context = fixture.Context;

        Assert.All(
            context.Tickets,
            ticket => Assert.NotNull(ticket.Visitor));
    }

    [Fact]
    public void TicketsShouldHaveTours()
    {
        var context = fixture.Context;

        Assert.All(
            context.Tickets,
            ticket => Assert.NotNull(ticket.Tour));
    }

    [Fact]
    public void TourExhibitionsShouldHaveRelations()
    {
        var context = fixture.Context;

        Assert.All(context.TourExhibitions, relation =>
        {
            Assert.NotNull(relation.Tour);
            Assert.NotNull(relation.Exhibition);
        });
    }
}