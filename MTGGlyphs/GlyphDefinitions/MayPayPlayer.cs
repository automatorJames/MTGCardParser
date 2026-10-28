namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The player offered an optional payment, where that's not you: "that player may pay …", "any player may pay …", "its controller may pay …", "they may pay …" - opening an <see cref="OptionalPayCost"/>.</summary>
/// <exampledoc>Dash Hopes</exampledoc>
/// <examplecapture>any player may pay</examplecapture>
[Dependent]
public class MayPayPlayer : Glyph
{
    public override Nib[] Nibs => [Prop(Payer), "may pay"];

    public OneOf<WhichPlayer?, PlayerReference, TargetedPlayer, TheyPlayer> Payer { get; set; }
}
