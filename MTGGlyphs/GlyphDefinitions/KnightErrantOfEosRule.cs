namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Knight-Errant of Eos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Knight-Errant of Eos</exampledoc>
/// <examplecapture>you may reveal up to two creature cards with mana value x or less from among them, where x is the number of creatures that convoked this creature</examplecapture>
[TokenizationOrder(-1)]
public class KnightErrantOfEosRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal up to two creature cards with mana value x or less from among them, where x is the number of creatures that convoked this creature"];
}
