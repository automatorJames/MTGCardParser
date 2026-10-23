namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Virtue of Courage // Embereth Blaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Virtue of Courage // Embereth Blaze</exampledoc>
[TokenizationOrder(-1)]
public class VirtueOfCourageCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source you control deals noncombat damage to an opponent, you may exile that many cards from the top of your library"];
}
