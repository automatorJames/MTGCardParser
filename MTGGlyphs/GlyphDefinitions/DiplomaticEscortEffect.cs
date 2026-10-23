namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Diplomatic Escort, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diplomatic Escort</exampledoc>
[Dependent]
public class DiplomaticEscortEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell or ability that targets a creature"];
}
