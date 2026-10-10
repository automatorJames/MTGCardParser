namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"damage", or "damage equal to …": damage whose amount is measured rather than counted (the measure may instead follow the recipient, see <see cref="DamageTo"/>).</summary>
/// <exampledoc>Creature Bond</exampledoc>
/// <examplecapture>damage equal to that creature's toughness</examplecapture>
[Dependent]
public class MeasuredDamage : Glyph
{
    public override Nib[] Nibs => ["damage", Prop(Measure)];

    [Optional]
    public EqualTo Measure { get; set; }
}
