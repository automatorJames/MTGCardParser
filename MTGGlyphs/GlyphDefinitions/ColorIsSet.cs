namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability setting objects' colors: "{this} is all colors", "{this} is colorless", "all creatures are black", "all slivers are colorless", "nonland permanents you control are white", "each nonland permanent you control is all colors", "enchanted creature is red".</summary>
/// <exampledoc>Darkest Hour</exampledoc>
/// <examplecapture>all creatures are black</examplecapture>
public class ColorIsSet : Glyph
{
    public override Nib[] Nibs => [Prop(Objects), Prop(Verb), Prop(Colors)];

    public OneOf<This, ThisCreature, EnchantedPermanent, AllPermanents, EachControlledPermanent, Permanents> Objects { get; set; }
    public ColorCopula Verb { get; set; }
    public OneOf<ManyOf<ManaColor>, ManaColor?, SetColor?> Colors { get; set; }
}
