namespace MTGPlexer.DTOs;

/// <summary>
/// Serves as a mechanism for parent TokenUnits to define a group of two or more property names whose associated Regexes serve as
/// alternates where exactly one alternate is expected to match. The types of all named properties should be TokenUnit 
/// (i.e. child token properties). This record makes it easier for callers to to define such groups in RegexTemplatedeclarations, 
/// which are typically short expression-bodied properties.
/// </summary>
public record AlternativeTokenUnits
{
    public string[] Names;

    public AlternativeTokenUnits(params string[] names)
    {
        if (names.Length < 2)
            throw new ArgumentException("At least 2 capture alternatives required");

        Names = names;
    }
}

