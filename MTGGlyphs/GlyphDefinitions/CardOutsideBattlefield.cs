namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card or spell in a zone other than the battlefield: "card from your graveyard", "card in a graveyard".</summary>
/// <exampledoc>Argivian Archaeologist</exampledoc>
/// <examplecapture>card from your graveyard</examplecapture>
[Dependent]
public class CardOutsideBattlefield : Glyph
{
    public override Nib[] Nibs => [Alt("card", "spell"), Opt(Alt("in", "from")), Prop(Whose), Prop(Zone)];

    public Whose? Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}

 