namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Officious Interrogation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Officious Interrogation</exampledoc>
[TokenizationOrder(-1)]
public class OfficiousInterrogationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["investigate x times, where x is the total number of creatures those players control"];
}
