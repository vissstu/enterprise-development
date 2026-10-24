namespace Museum.Domain.Entities;

using Museum.Domain.Enums;

/// <summary>
/// Представляет выставку музея.
/// </summary>
public class Exhibition
{
    /// <summary>
    /// Уникальный идентификатор выставки.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название выставки.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Тематика выставки.
    /// </summary>
    public ExhibitionTheme Theme { get; set; }

    /// <summary>
    /// Номер зала, в котором проходит выставка.
    /// </summary>
    public int HallNumber { get; set; }

    /// <summary>
    /// Дата начала работы выставки.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Дата окончания работы выставки.
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Экскурсии, в рамках которых посещается выставка.
    /// </summary>
    public ICollection<TourExhibition> TourExhibitions { get; set; } = [];
}