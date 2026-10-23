namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Archmage's Newt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Archmage's Newt</exampledoc>
[TokenizationOrder(-1)]
public class ArchmagesNewtRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that card gains flashback {0} until end of turn instead if this creature is saddled"];
}
