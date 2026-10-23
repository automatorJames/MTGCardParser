namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pursued Whale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pursued Whale</exampledoc>
/// <examplecapture>each opponent creates a 1/1 red pirate creature token with "this token can't block" and "creatures you control attack each combat if able</examplecapture>
[Dependent]
public class PursuedWhaleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent creates a 1/1 red pirate creature token with \"this token can't block\" and \"creatures you control attack each combat if able"];
}
