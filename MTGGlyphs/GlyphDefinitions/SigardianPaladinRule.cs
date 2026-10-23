namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sigardian Paladin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sigardian Paladin</exampledoc>
[TokenizationOrder(-1)]
public class SigardianPaladinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you've put one or more +1/+1 counters on a creature this turn, this creature has trample and lifelink"];
}
