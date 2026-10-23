namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Weaver of Lies, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Weaver of Lies</exampledoc>
[Dependent]
public class WeaverOfLiesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["turn any number of target creatures with morph abilities other than this creature face down"];
}
