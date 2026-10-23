namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chaos Orb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaos Orb</exampledoc>
[Dependent]
public class ChaosOrbEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this artifact is on the battlefield, flip it onto the battlefield from a height of at least one foot"];
}
