namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sleight of Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sleight of Mind</exampledoc>
[TokenizationOrder(-1)]
public class SleightOfMindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(for example, you may change \"target black spell\" to \"target blue spell"];
}
