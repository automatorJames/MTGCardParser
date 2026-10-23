namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Assemble the Players, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Assemble the Players</exampledoc>
[TokenizationOrder(-1)]
public class AssembleThePlayersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once each turn, you may cast a creature spell with power 2 or less from the top of your library"];
}
