namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vexing Arcanix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vexing Arcanix</exampledoc>
[TokenizationOrder(-1)]
public class VexingArcanixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, they put it into their graveyard and this artifact deals 2 damage to them"];
}
