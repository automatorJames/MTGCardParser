namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Time Stop, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Stop</exampledoc>
[Dependent]
public class TimeStopEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all spells and abilities, including this spell"];
}
