namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class DanceOfTheDeadCardP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put enchanted creature card onto the battlefield tapped under your control and attach this aura to it"];
}
