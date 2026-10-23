namespace MTGGlyphs.GlyphDefinitions;

[TokenizationOrder(-1)]
public class DragonsApproachP3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile this spell and four cards named", Nib.This, "from your graveyard"];
}
