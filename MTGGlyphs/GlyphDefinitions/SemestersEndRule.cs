namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Semester's End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Semester's End</exampledoc>
/// <examplecapture>each of them enters with an additional +1/+1 counter on it if it's a creature and an additional loyalty counter on it if it's a planeswalker</examplecapture>
[TokenizationOrder(-1)]
public class SemestersEndRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each of them enters with an additional +1/+1 counter on it if it's a creature and an additional loyalty counter on it if it's a planeswalker"];
}
