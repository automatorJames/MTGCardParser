namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one of them into your hand and the rest on the bottom of your library in any order", "any number of them into your graveyard and the rest on top of your library in any order", "that card into your hand and the rest on the bottom of your library in a random order", "one of them into your hand and the other into your graveyard", "a land card from among them onto the battlefield and the rest …", "… and the rest of the revealed cards into your graveyard": splitting looked-at or revealed cards between two places, as scry, surveil and impulse effects do.</summary>
/// <exampledoc>Horizon Scholar</exampledoc>
/// <examplecapture>any number of them on the bottom of your library and the rest on top in any order</examplecapture>
[Dependent]
public class SplitLookedAtCards : Glyph
{
    public override Nib[] Nibs => [Prop(Cards), Prop(Chosen), Alt("and the rest of the revealed cards", "and the rest", "and the other"), Prop(Rest)];

    public OneOf<ChosenLookedAtCards, ThatCardPhrase> Cards { get; set; }
    public OneOf<LibraryPlacement, IntoZone, ToTheBattlefieldUnderControl> Chosen { get; set; }
    public OneOf<LibraryPlacement, IntoZone> Rest { get; set; }
}
