namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Defense Grid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Defense Grid</exampledoc>
[TokenizationOrder(-1)]
public class DefenseGridRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each spell costs {3} more to cast except during its controller's turn"];
}
