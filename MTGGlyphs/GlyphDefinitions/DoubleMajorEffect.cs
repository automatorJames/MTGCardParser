namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Double Major, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Double Major</exampledoc>
[Dependent]
public class DoubleMajorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a copy of a creature spell becomes a token"];
}
