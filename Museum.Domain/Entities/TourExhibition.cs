namespace Museum.Domain.Entities;

/// <summary>
/// Связывает экскурсию с выставкой, посещаемой в рамках экскурсии.
/// </summary>
public class TourExhibition
{
    /// <summary>
    /// Идентификатор экскурсии.
    /// </summary>
    public int TourId { get; set; }

    /// <summary>
    /// Идентификатор выставки.
    /// </summary>
    public int ExhibitionId { get; set; }

    /// <summary>
    /// Экскурсия.
    /// </summary>
    public Tour Tour { get; set; } = null!;

    /// <summary>
    /// Выставка.
    /// </summary>
    public Exhibition Exhibition { get; set; } = null!;
}