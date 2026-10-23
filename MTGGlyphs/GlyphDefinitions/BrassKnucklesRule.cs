namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brass Knuckles, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brass Knuckles</exampledoc>
[TokenizationOrder(-1)]
public class BrassKnucklesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature has double strike as long as two or more equipment are attached to it"];
}
