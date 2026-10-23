namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stolen Uniform, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stolen Uniform</exampledoc>
[Dependent]
public class StolenUniformEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's attached to a creature you control, unattach it"];
}
