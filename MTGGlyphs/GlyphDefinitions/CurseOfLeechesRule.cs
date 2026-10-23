namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Curse of Leeches // Leeching Lurker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of Leeches // Leeching Lurker</exampledoc>
[TokenizationOrder(-1)]
public class CurseOfLeechesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this permanent transforms into curse of leeches, attach it to a player"];
}
