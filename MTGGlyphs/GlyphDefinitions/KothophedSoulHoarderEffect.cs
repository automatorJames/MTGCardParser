namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kothophed, Soul Hoarder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kothophed, Soul Hoarder</exampledoc>
[Dependent]
public class KothophedSoulHoarderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you draw a card and you lose 1 life"];
}
