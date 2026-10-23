namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Backdraft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Backdraft</exampledoc>
[TokenizationOrder(-1)]
public class BackdraftCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a player who cast one or more sorcery spells this turn"];
}
