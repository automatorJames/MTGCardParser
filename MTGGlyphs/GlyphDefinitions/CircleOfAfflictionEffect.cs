namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Circle of Affliction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Circle of Affliction</exampledoc>
[Dependent]
public class CircleOfAfflictionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {1}"];
}
