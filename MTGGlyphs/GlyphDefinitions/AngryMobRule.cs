namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Angry Mob, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angry Mob</exampledoc>
[TokenizationOrder(-1)]
public class AngryMobRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during turns other than yours,", Nib.This, "'s power and toughness are each 2"];
}
