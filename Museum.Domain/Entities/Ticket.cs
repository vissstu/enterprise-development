namespace Museum.Domain.Entities;

using Museum.Domain.Enums;

/// <summary>
/// Представляет билет на экскурсию.
/// </summary>
public class Ticket
{
    /// <summary>
    /// Уникальный идентификатор билета.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор экскурсии.
    /// </summary>
    public int TourId { get; set; }

    /// <summary>
    /// Идентификатор посетителя.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Тип билета.
    /// </summary>
    public TicketType Type { get; set; }

    /// <summary>
    /// Стоимость билета.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Экскурсия, на которую приобретён билет.
    /// </summary>
    public Tour Tour { get; set; } = null!;

    /// <summary>
    /// Посетитель, которому принадлежит билет.
    /// </summary>
    public Visitor Visitor { get; set; } = null!;
}