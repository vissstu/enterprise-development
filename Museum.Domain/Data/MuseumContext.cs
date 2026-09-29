using Museum.Domain.Entities;

namespace Museum.Domain.Data;

/// <summary>
/// Представляет контекст данных музея в памяти.
/// </summary>
public class MuseumContext
{
    /// <summary>
    /// Коллекция выставок.
    /// </summary>
    public List<Exhibition> Exhibitions { get; set; } = [];

    /// <summary>
    /// Коллекция экскурсий.
    /// </summary>
    public List<Tour> Tours { get; set; } = [];

    /// <summary>
    /// Коллекция посетителей.
    /// </summary>
    public List<Visitor> Visitors { get; set; } = [];

    /// <summary>
    /// Коллекция билетов.
    /// </summary>
    public List<Ticket> Tickets { get; set; } = [];

    /// <summary>
    /// Коллекция связей между экскурсиями и выставками.
    /// </summary>
    public List<TourExhibition> TourExhibitions { get; set; } = [];
}