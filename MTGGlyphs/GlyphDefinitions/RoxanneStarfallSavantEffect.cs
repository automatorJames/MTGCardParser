namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Roxanne, Starfall Savant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roxanne, Starfall Savant</exampledoc>
/// <examplecapture>create a tapped colorless artifact token named meteorite with "when this token enters, it deals 2 damage to any target" and "{t}: add one mana of any color</examplecapture>
[Dependent]
public class RoxanneStarfallSavantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a tapped colorless artifact token named meteorite with \"when this token enters, it deals 2 damage to any target\" and \"{t}: add one mana of any color"];
}
