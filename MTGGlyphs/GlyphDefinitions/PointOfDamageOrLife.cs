namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A unit of life or damage counted one point at a time, as "for each" takes it: "1 damage prevented this way", "1 life you lost", "1 life your opponents have lost this turn", "1 damage dealt to you this turn", "1 life they paid this way".</summary>
/// <exampledoc>Temper</exampledoc>
/// <examplecapture>1 damage prevented this way</examplecapture>
[Dependent]
public class PointOfDamageOrLife : Glyph
{
    public override Nib[] Nibs => ["1", Prop(Unit), Prop(Event)];

    public DamageOrLife Unit { get; set; }
    public DamageOrLifeEvent Event { get; set; }
}
