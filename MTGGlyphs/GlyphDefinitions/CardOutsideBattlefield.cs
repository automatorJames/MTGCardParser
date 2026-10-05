namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card in a zone other than the battlefield: "card in a graveyard", "card you own outside the game".</summary>
[Dependent]
public class CardOutsideBattlefield : Glyph
{
    public override Nib[] Nibs => ["card", Opt("in", "from"), Prop(Whose), Prop(Zone)];

    [Optional]
    public Whose? Whose { get; set; }

    public NonBattlefieldZone Zone { get; set; }
}
