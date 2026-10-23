namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of Cruelties, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Cruelties</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfCrueltiesCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature attacks a player and isn't blocked, that player's life total becomes 1"];
}
