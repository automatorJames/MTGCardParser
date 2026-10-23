namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Krovikan Vampire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Krovikan Vampire</exampledoc>
/// <examplecapture>if a creature dealt damage by this creature this turn died, put that card onto the battlefield under your control</examplecapture>
[Dependent]
public class KrovikanVampireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature dealt damage by this creature this turn died, put that card onto the battlefield under your control"];
}
