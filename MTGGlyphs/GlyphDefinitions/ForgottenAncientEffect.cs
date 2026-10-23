namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Forgotten Ancient, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forgotten Ancient</exampledoc>
[Dependent]
public class ForgottenAncientEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may move any number of +1/+1 counters from this creature onto other creatures"];
}
