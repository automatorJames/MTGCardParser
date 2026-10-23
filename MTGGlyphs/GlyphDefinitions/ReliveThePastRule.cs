namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Relive the Past, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Relive the Past</exampledoc>
/// <examplecapture>return up to one target artifact card, up to one target land card, and up to one target non-aura enchantment card from your graveyard to the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class ReliveThePastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return up to one target artifact card, up to one target land card, and up to one target non-aura enchantment card from your graveyard to the battlefield"];
}
