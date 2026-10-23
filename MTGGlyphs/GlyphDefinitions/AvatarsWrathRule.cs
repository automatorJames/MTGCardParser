namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Avatar's Wrath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar's Wrath</exampledoc>
[TokenizationOrder(-1)]
public class AvatarsWrathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose up to one target creature, then airbend all other creatures"];
}
