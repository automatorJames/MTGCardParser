namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Warrior's Blades, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warrior's Blades</exampledoc>
[TokenizationOrder(-1)]
public class WarriorsBladesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability costs {1} less to activate for each +1/+1 counter on the creature it targets"];
}
