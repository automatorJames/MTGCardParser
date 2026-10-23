namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The other player of an exchange: "with target opponent".</summary>
/// <exampledoc>Mirror Universe</exampledoc>
/// <examplecapture>with target opponent</examplecapture>
[Dependent]
public class WithPlayer : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Player)];

    public TargetedPlayer Player { get; set; }
}
