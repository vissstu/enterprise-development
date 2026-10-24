using Museum.Tests.Fixtures;

namespace Museum.Tests;

/// <summary>
/// Проверяет корректность тестовых данных и связей между сущностями музея.
/// </summary>
public class DomainTests(MuseumFixture fixture) : IClassFixture<MuseumFixture>
{
    /// <summary>
    /// Проверяет точное количество созданных объектов.
    /// </summary>
    [Fact]
    public void SeederShouldCreateExpectedData()
    {
        var context = fixture.Context;

        Assert.Equal(10, context.Exhibitions.Count);
        Assert.Equal(10, context.Tours.Count);
        Assert.Equal(10, context.Visitors.Count);
        Assert.Equal(12, context.Tickets.Count);
        Assert.Equal(12, context.TourExhibitions.Count);
    }

    /// <summary>
    /// Проверяет связи билетов с посетителями.
    /// </summary>
    [Fact]
    public void TicketsShouldHaveVisitors()
    {
        var context = fixture.Context;

        Assert.All(context.Tickets, ticket =>
        {
            var expectedVisitor = context.Visitors.Single(
                visitor => visitor.Id == ticket.VisitorId);

            Assert.Same(expectedVisitor, ticket.Visitor);
            Assert.Contains(ticket, ticket.Visitor.Tickets);
        });
    }

    /// <summary>
    /// Проверяет связи билетов с экскурсиями.
    /// </summary>
    [Fact]
    public void TicketsShouldHaveTours()
    {
        var context = fixture.Context;

        Assert.All(context.Tickets, ticket =>
        {
            var expectedTour = context.Tours.Single(
                tour => tour.Id == ticket.TourId);

            Assert.Same(expectedTour, ticket.Tour);
            Assert.Contains(ticket, ticket.Tour.Tickets);
        });
    }

    /// <summary>
    /// Проверяет связи экскурсий с выставками.
    /// </summary>
    [Fact]
    public void TourExhibitionsShouldHaveRelations()
    {
        var context = fixture.Context;

        Assert.All(context.TourExhibitions, relation =>
        {
            var expectedTour = context.Tours.Single(
                tour => tour.Id == relation.TourId);

            var expectedExhibition = context.Exhibitions.Single(
                exhibition => exhibition.Id == relation.ExhibitionId);

            Assert.Same(expectedTour, relation.Tour);
            Assert.Same(expectedExhibition, relation.Exhibition);

            Assert.Contains(relation, relation.Tour.TourExhibitions);
            Assert.Contains(relation, relation.Exhibition.TourExhibitions);
        });
    }
}
