namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Obosh, the Preypiercer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Obosh, the Preypiercer</exampledoc>
/// <examplecapture>if a source you control with an odd mana value would deal damage to a permanent or player, it deals double that damage to that permanent or player instead</examplecapture>
[TokenizationOrder(-1)]
public class OboshThePreypiercerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a source you control with an odd mana value would deal damage to a permanent or player, it deals double that damage to that permanent or player instead"];
}
