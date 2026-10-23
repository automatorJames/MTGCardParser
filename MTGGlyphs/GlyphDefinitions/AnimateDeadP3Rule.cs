namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class AnimateDeadP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this aura enters, if it's on the battlefield, it loses \"enchant creature card in a graveyard\" and gains \"enchant creature put onto the battlefield with this aura"];
}
