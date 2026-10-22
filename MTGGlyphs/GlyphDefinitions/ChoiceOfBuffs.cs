namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice among keyword buffs, made by the controller as the effect resolves: "your choice of vigilance, lifelink, or haste", "your choice of double strike, protection from red, vigilance, or shadow".</summary>
/// <exampledoc>Butcher of the Horde</exampledoc>
/// <examplecapture>your choice of vigilance, lifelink, or haste</examplecapture>
[Dependent]
public class ChoiceOfBuffs : Glyph
{
    public override Nib[] Nibs => ["your choice of", Prop(Choices)];

    public ManyOf<ChoosableBuff> Choices { get; set; }
}
