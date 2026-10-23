namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Party Dude, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Party Dude</exampledoc>
/// <examplecapture>up to one target attacking creature gets +x/+x until end of turn, where x is the number of cards in your hand</examplecapture>
[Dependent]
public class PartyDudeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["up to one target attacking creature gets +x/+x until end of turn, where x is the number of cards in your hand"];
}
