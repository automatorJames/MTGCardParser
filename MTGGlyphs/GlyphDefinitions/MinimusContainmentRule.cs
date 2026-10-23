namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Minimus Containment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minimus Containment</exampledoc>
[TokenizationOrder(-1)]
public class MinimusContainmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted permanent is a treasure artifact with \"{t}, sacrifice this artifact: add one mana of any color,\" and it loses all other abilities"];
}
