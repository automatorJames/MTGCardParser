namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"unless" and the player who may pay to stop it, where that's not you or "they": "unless its controller …", "unless that player …", "unless any player …".</summary>
/// <exampledoc>Force Spike</exampledoc>
/// <examplecapture>unless its controller</examplecapture>
[Dependent]
public class UnlessPlayer : Glyph
{
    public override Nib[] Nibs => ["unless", Prop(Payer)];

    public OneOf<WhichPlayer?, PlayerReference> Payer { get; set; }
}
