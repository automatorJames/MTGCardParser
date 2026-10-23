namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tyvar the Bellicose, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tyvar the Bellicose</exampledoc>
/// <examplecapture>each creature you control has "whenever a mana ability of this creature resolves, put a number of +1/+1 counters on it equal to the amount of mana this creature produced</examplecapture>
[TokenizationOrder(-1)]
public class TyvarTheBellicoseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control has \"whenever a mana ability of this creature resolves, put a number of +1/+1 counters on it equal to the amount of mana this creature produced"];
}
