namespace Museum.Domain.Entities;

/// <summary>
/// Представляет посетителя музея.
/// </summary>
public class Visitor
{
    /// <summary>
    /// Уникальный идентификатор посетителя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Полное имя посетителя.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Номер телефона посетителя.
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// Дата рождения посетителя.
    /// </summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// Билеты, приобретённые посетителем.
    /// </summary>
    public ICollection<Ticket> Tickets { get; set; } = [];
}