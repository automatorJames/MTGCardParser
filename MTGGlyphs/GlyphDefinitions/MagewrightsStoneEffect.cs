namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Magewright's Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magewright's Stone</exampledoc>
[Dependent]
public class MagewrightsStoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap target creature that has an activated ability with {t} in its cost"];
}
