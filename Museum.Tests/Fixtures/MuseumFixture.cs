using Museum.Domain.Data;

namespace Museum.Tests.Fixtures;

/// <summary>
/// Предоставляет общий контекст музея с тестовыми данными.
/// </summary>
public class MuseumFixture
{
    public MuseumContext Context { get; }

    public MuseumFixture()
    {
        Context = DataSeeder.Seed();
    }
}