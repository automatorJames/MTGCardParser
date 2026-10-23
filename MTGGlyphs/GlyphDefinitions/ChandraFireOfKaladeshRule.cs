namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra, Fire of Kaladesh // Chandra, Roaring Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Fire of Kaladesh // Chandra, Roaring Flame</exampledoc>
[TokenizationOrder(-1)]
public class ChandraFireOfKaladeshRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if chandra has dealt 3 or more damage this turn, exile her, then return her to the battlefield transformed under her owner's control"];
}
