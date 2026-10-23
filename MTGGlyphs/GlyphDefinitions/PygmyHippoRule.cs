namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pygmy Hippo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pygmy Hippo</exampledoc>
/// <examplecapture>if you do, this creature assigns no combat damage this turn and at the beginning of your next main phase this turn, you add an amount of {c} equal to the amount of mana that player lost this way</examplecapture>
[TokenizationOrder(-1)]
public class PygmyHippoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, this creature assigns no combat damage this turn and at the beginning of your next main phase this turn, you add an amount of {c} equal to the amount of mana that player lost this way"];
}
