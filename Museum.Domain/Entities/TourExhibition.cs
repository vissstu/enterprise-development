namespace Museum.Domain.Entities;

/// <summary>
/// Связывает экскурсию с выставкой, посещаемой в рамках экскурсии.
/// </summary>
public class TourExhibition
{
    /// <summary>
    /// Идентификатор экскурсии.
    /// </summary>
    ///
    public int TourId { get; set; }

    /// <summary>
    /// Идентификатор выставки.
    /// </summary>
    public int ExhibitionId { get; set; }

    /// <summary>
    /// Экскурсия.
    /// </summary>
    public required Tour Tour { get; set; }

    /// <summary>
    /// Выставка.
    /// </summary>
    public required Exhibition Exhibition { get; set; }
}
