namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lasting Tarfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lasting Tarfire</exampledoc>
/// <examplecapture>if you put a counter on a creature this turn, this enchantment deals 2 damage to each opponent</examplecapture>
[Dependent]
public class LastingTarfireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a counter on a creature this turn, this enchantment deals 2 damage to each opponent"];
}
