namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of artifacts, creatures, or enchantments", "of cards in graveyards": the sources of barred abilities.</summary>
/// <exampledoc>Grand Abolisher</exampledoc>
/// <examplecapture>of artifacts, creatures, or enchantments</examplecapture>
[Dependent]
public class AbilitiesOfKinds : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Sources)];

    public OneOf<ManyOf<PlayableCardType>, CardsInZones> Sources { get; set; }
}
