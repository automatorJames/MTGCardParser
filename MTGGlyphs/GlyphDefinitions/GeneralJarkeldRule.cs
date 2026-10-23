namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of General Jarkeld, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>General Jarkeld</exampledoc>
/// <examplecapture>if each of those creatures could be blocked by all creatures that the other is blocked by, each creature that's blocking exactly one of those attacking creatures stops blocking it and is blocking the other attacking creature</examplecapture>
[TokenizationOrder(-1)]
public class GeneralJarkeldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if each of those creatures could be blocked by all creatures that the other is blocked by, each creature that's blocking exactly one of those attacking creatures stops blocking it and is blocking the other attacking creature"];
}
