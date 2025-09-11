namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record NamedGroupOpen(string Name, string Path, int Indentation, string CaptureType, DeterministicPalette Palette, RegexPropInfo Group) 
    : RegexTemplateLine($"(?<{Name}>", Path, Indentation, Palette, Name.ToFriendlyCase(TitleDisplayOption.Title), $": {CaptureType}", Group);