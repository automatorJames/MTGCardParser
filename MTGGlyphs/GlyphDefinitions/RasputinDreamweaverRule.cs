namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rasputin Dreamweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rasputin Dreamweaver</exampledoc>
[TokenizationOrder(-1)]
public class RasputinDreamweaverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["rasputin can't have more than seven dream counters on it"];
}
