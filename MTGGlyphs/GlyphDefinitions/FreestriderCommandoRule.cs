namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Freestrider Commando, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Freestrider Commando</exampledoc>
[TokenizationOrder(-1)]
public class FreestriderCommandoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature enters with two +1/+1 counters on it if it wasn't cast or no mana was spent to cast it"];
}
