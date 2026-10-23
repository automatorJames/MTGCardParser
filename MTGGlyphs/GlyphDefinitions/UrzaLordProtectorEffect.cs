namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Urza, Lord Protector // Urza, Planeswalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza, Lord Protector // Urza, Planeswalker</exampledoc>
/// <examplecapture>if you both own and control urza, lord protector and an artifact named the mightstone and weakstone, exile them, then meld them into urza, planeswalker</examplecapture>
[Dependent]
public class UrzaLordProtectorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you both own and control urza, lord protector and an artifact named the mightstone and weakstone, exile them, then meld them into urza, planeswalker"];
}
