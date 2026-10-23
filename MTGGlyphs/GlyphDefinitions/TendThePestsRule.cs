namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tend the Pests, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tend the Pests</exampledoc>
[TokenizationOrder(-1)]
public class TendThePestsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x 1/1 black and green pest creature tokens with \"when this token dies, you gain 1 life,\" where x is the sacrificed creature's power"];
}
