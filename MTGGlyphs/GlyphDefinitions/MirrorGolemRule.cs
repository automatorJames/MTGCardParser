namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Golem</exampledoc>
[TokenizationOrder(-1)]
public class MirrorGolemRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has protection from each of the exiled card's card types"];
}
