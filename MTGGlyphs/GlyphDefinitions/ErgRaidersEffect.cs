namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Erg Raiders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Erg Raiders</exampledoc>
/// <examplecapture>if this creature didn't attack this turn, it deals 2 damage to you unless it came under your control this turn</examplecapture>
[Dependent]
public class ErgRaidersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature didn't attack this turn, it deals 2 damage to you unless it came under your control this turn"];
}
