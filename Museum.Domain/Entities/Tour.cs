
namespace Museum.Domain.Entities;

/// <summary>
/// Представляет экскурсию в музее.
/// </summary>
public class Tour
{
    /// <summary>
    /// Уникальный идентификатор экскурсии.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Дата и время начала экскурсии.
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// Продолжительность экскурсии.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Выставки, посещаемые во время экскурсии.
    /// </summary>
    public ICollection<TourExhibition> TourExhibitions { get; set; } = [];

    /// <summary>
    /// Билеты на экскурсию.
    /// </summary>
    public ICollection<Ticket> Tickets { get; set; } = [];
}
