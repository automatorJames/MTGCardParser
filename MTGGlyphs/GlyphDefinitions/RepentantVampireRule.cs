namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Repentant Vampire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Repentant Vampire</exampledoc>
[TokenizationOrder(-1)]
public class RepentantVampireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold — as long as there are seven or more cards in your graveyard, this creature is white and has \"{t}: destroy target black creature"];
}
