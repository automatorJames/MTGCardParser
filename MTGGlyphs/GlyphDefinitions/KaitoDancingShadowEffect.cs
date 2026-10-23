namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaito, Dancing Shadow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaito, Dancing Shadow</exampledoc>
[Dependent]
public class KaitoDancingShadowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return one of them to its owner's hand"];
}
