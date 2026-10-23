namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Firestorm Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firestorm Phoenix</exampledoc>
[TokenizationOrder(-1)]
public class FirestormPhoenixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until that player's next turn, that player plays with that card revealed in their hand and can't play it"];
}
