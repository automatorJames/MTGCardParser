namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Living Inferno, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Inferno</exampledoc>
[Dependent]
public class LivingInfernoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals damage equal to its power divided as you choose among any number of target creatures"];
}
