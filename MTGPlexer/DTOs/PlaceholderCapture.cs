namespace MTGPlexer.DTOs;

/// <summary>
/// A wrapper that represents a property whose value is a simple string of captured text.
/// This record exists primarily as a marker for instances where the definer of some TokenUnit wants
/// to capture a given pattern, but doesn't yet know how they want to decompose captures or otherwise 
/// handle them (i.e. a placeholder). This record may also be used by TokenUnit types that override
/// SetPropertiesFromMatch(), and need a place to store captured text during instantiation to be 
/// processed later.
/// </summary>
public record PlaceholderCapture
(
    string Text
)
{
    public override string ToString() => Text;
}

