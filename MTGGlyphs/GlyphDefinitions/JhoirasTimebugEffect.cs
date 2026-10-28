namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jhoira's Timebug, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jhoira's Timebug</exampledoc>
[Dependent]
public class JhoirasTimebugEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target permanent you control or suspended card you own"];
}
