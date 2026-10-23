namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Party Dude, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Party Dude</exampledoc>
[TokenizationOrder(-1)]
public class PartyDudeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more of your opponents are attacked, up to one target attacking creature gets +x/+x until end of turn, where x is the number of cards in your hand"];
}
