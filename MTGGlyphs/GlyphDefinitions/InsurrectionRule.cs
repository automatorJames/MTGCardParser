namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Insurrection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Insurrection</exampledoc>
[TokenizationOrder(-1)]
public class InsurrectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures and gain control of them until end of turn"];
}
