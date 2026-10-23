namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Getaway Glamer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Getaway Glamer</exampledoc>
[TokenizationOrder(-1)]
public class GetawayGlamerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["+ {2} — destroy target creature if no other creature has greater power"];
}
