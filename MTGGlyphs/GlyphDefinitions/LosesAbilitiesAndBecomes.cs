namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent stripped of its abilities and turned into a vanilla creature: "loses all abilities and becomes a green elk creature with base power and toughness 3/3". Its own glyph because the general "X and Y" pair splits at the "and" inside "power and toughness".</summary>
/// <exampledoc>Oko, Thief of Crowns</exampledoc>
/// <examplecapture>loses all abilities and becomes a green elk creature with base power and toughness 3/3</examplecapture>
[Dependent]
public class LosesAbilitiesAndBecomes : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("loses? all abilities and"), Prop(Becomes)];

    public BecomesCreature Becomes { get; set; }
}
