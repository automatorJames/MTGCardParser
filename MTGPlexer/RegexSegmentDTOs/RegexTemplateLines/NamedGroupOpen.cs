namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record NamedGroupOpen(string Name, string Path, int Indentation, string CaptureType, DeterministicPalette Palette) 
    : RegexTemplateLine($"(?<{Name}>", Path, Indentation, Palette, Name.ToFriendlyCase(TitleDisplayOption.Title), $": {CaptureType}");