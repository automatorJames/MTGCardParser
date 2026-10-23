namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unexpected Fangs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unexpected Fangs</exampledoc>
[TokenizationOrder(-1)]
public class UnexpectedFangsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter and a lifelink counter on target creature"];
}
