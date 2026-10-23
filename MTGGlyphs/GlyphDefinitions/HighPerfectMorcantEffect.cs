namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of High Perfect Morcant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>High Perfect Morcant</exampledoc>
[Dependent]
public class HighPerfectMorcantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent blights 1"];
}
