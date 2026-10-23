namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vraska, the Silencer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vraska, the Silencer</exampledoc>
[TokenizationOrder(-1)]
public class VraskaTheSilencerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's a treasure artifact with \"{t}, sacrifice this artifact: add one mana of any color,\" and it loses all other card types"];
}
