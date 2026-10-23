namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Profaner of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Profaner of the Dead</exampledoc>
[Dependent]
public class ProfanerOfTheDeadEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return to their owners' hands all creatures your opponents control with toughness less than the exploited creature's toughness"];
}
