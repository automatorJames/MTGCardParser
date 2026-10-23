namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Angelic Arbiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angelic Arbiter</exampledoc>
[TokenizationOrder(-1)]
public class AngelicArbiterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who cast a spell this turn can't attack with creatures"];
}
