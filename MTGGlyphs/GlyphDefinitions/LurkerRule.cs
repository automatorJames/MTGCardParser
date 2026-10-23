namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lurker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lurker</exampledoc>
[TokenizationOrder(-1)]
public class LurkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't be the target of spells unless it attacked or blocked this turn"];
}
