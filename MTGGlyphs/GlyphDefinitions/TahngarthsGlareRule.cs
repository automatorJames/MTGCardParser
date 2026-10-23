namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tahngarth's Glare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tahngarth's Glare</exampledoc>
[TokenizationOrder(-1)]
public class TahngarthsGlareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player looks at the top three cards of your library, then puts them back in any order"];
}
