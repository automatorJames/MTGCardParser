namespace MTGGlyphs.GlyphDefinitions;

public class CardOutsideBattlefield : Glyph
{
    public override Nib[] Nibs => ["(card|spell)", "((in|from) )?", Prop(Whose), Prop(Zone)];

    public Whose? Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}

 