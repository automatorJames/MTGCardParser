namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Renegade Doppelganger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Renegade Doppelganger</exampledoc>
[Dependent]
public class RenegadeDoppelgangerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it does, it loses this ability for the rest of the turn"];
}
