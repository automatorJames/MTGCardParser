namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Personal Incarnation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Personal Incarnation</exampledoc>
[TokenizationOrder(-1)]
public class PersonalIncarnationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only this creature's owner may activate this ability"];
}
