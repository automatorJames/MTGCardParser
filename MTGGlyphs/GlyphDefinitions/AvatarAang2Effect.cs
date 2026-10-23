namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Avatar Aang // Aang, Master of Elements, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar Aang // Aang, Master of Elements</exampledoc>
[Dependent]
public class AvatarAang2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["earthbend, firebend, or airbend, draw a card"];
}
