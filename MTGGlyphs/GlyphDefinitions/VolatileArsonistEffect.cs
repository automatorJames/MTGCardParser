namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Volatile Arsonist // Dire-Strain Anarchist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volatile Arsonist // Dire-Strain Anarchist</exampledoc>
/// <examplecapture>it deals 1 damage to each of up to one target creature, up to one target player, and/or up to one target planeswalker</examplecapture>
[Dependent]
public class VolatileArsonistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 1 damage to each of up to one target creature, up to one target player, and/or up to one target planeswalker"];
}
