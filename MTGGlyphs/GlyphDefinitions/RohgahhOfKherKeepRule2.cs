namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rohgahh of Kher Keep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rohgahh of Kher Keep</exampledoc>
[TokenizationOrder(-1)]
public class RohgahhOfKherKeepRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control named kobolds of kher keep get +2/+2"];
}
