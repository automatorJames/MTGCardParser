namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's attacking you", "that's blocking it", "that's attacking or blocking": a permanent narrowed by its part in combat, and optionally whom it attacks or blocks.</summary>
/// <exampledoc>Ghostly Prison</exampledoc>
/// <examplecapture>that's attacking you</examplecapture>
[Dependent]
public class ThatsInCombat : Glyph
{
    public override Nib[] Nibs => ["that's", Prop(Status), Prop(Whom)];

    public InCombatStatus Status { get; set; }
    [Optional]
    public OneOf<AttackRecipient?, It> Whom { get; set; }
}
