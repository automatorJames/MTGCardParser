namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Conduit of Worlds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conduit of Worlds</exampledoc>
[TokenizationOrder(-1)]
public class ConduitOfWorldsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you can't cast additional spells this turn"];
}
