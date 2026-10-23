namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Puppet Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Puppet Master</exampledoc>
[TokenizationOrder(-1)]
public class PuppetMasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that card is returned to its owner's hand this way, you may pay {u}{u}{u}"];
}
