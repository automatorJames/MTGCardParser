namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Powerleech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Powerleech</exampledoc>
[Dependent]
public class PowerleechEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 1 life"];
}
