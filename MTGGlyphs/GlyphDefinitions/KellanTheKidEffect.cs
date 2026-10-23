namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kellan, the Kid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kellan, the Kid</exampledoc>
/// <examplecapture>you may cast a permanent spell with equal or lesser mana value from your hand without paying its mana cost</examplecapture>
[Dependent]
public class KellanTheKidEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a permanent spell with equal or lesser mana value from your hand without paying its mana cost"];
}
