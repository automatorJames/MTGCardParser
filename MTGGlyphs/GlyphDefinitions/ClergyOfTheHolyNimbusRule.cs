namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Clergy of the Holy Nimbus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clergy of the Holy Nimbus</exampledoc>
[TokenizationOrder(-1)]
public class ClergyOfTheHolyNimbusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only your opponents may activate this ability"];
}
