namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Graxiplon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Graxiplon</exampledoc>
[TokenizationOrder(-1)]
public class GraxiplonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't be blocked unless defending player controls three or more creatures that share a creature type"];
}
