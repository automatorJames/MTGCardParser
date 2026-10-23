namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Qarsi Deceiver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Qarsi Deceiver</exampledoc>
[Dependent]
public class QarsiDeceiverCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a megamorph cost is a morph cost"];
}
