namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"return enchanted creature card to the battlefield under your control and attach {this} to it": putting a card back onto the battlefield, possibly naming the zone it comes from ("this card from your graveyard").</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>return enchanted creature card to the battlefield under your control and attach {this} to it</examplecapture>
public class ReturnToBattlefield : Glyph
{
    public override Nib[] Nibs => ["return", Prop(Returned), Opt("card"), Prop(Destination), Prop(AndThen)];

    public OneOf<PermanentFromZone, TargetCardInZone, PermanentPhrase> Returned { get; set; }
    public ToTheBattlefieldUnderControl Destination { get; set; }
    [Optional]
    public AndThen AndThen { get; set; }
}
