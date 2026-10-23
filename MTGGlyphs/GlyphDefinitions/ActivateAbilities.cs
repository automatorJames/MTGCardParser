namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permission to activate abilities outside their usual timing: "activate equip abilities any time you could cast an instant", "activate loyalty abilities of teferi on any player's turn any time you could cast an instant", "activate the loyalty abilities of planeswalkers you control twice each turn rather than only once". A predicate, used after "you may".</summary>
/// <exampledoc>Leonin Shikari</exampledoc>
/// <examplecapture>activate equip abilities any time you could cast an instant</examplecapture>
public class ActivateAbilities : Glyph, IPredicate
{
    public override Nib[] Nibs => ["activate", Prop(Abilities), Prop(AnyPlayersTurn), Prop(Timing)];

    public ActivatableAbilities Abilities { get; set; }
    [Optional]
    [RegexPattern("on any player's turn")]
    public bool? AnyPlayersTurn { get; set; }
    public OneOf<InstantSpeed?, ExtraActivation> Timing { get; set; }
}
