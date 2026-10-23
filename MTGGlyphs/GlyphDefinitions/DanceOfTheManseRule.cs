namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dance of the Manse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dance of the Manse</exampledoc>
[TokenizationOrder(-1)]
public class DanceOfTheManseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 6 or more, those permanents are 4/4 creatures in addition to their other types"];
}
