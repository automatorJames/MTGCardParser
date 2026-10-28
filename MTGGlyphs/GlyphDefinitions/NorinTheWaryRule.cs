namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Norin the Wary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Norin the Wary</exampledoc>
[TokenizationOrder(-1)]
public class NorinTheWaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a player casts a spell or a creature attacks, exile norin"];
}
