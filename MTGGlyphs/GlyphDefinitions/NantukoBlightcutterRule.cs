namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nantuko Blightcutter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nantuko Blightcutter</exampledoc>
[TokenizationOrder(-1)]
public class NantukoBlightcutterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold — this creature gets +1/+1 for each black permanent your opponents control as long as there are seven or more cards in your graveyard"];
}
