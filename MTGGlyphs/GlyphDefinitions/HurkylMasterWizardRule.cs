namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hurkyl, Master Wizard, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hurkyl, Master Wizard</exampledoc>
/// <examplecapture>for each card type among noncreature spells you've cast this turn, you may put a card of that type from among the revealed cards into your hand</examplecapture>
[TokenizationOrder(-1)]
public class HurkylMasterWizardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each card type among noncreature spells you've cast this turn, you may put a card of that type from among the revealed cards into your hand"];
}
