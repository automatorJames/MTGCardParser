namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rot-Curse Rakshasa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rot-Curse Rakshasa</exampledoc>
[Dependent]
public class RotCurseRakshasaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a decayed counter on each of x target creatures"];
}
