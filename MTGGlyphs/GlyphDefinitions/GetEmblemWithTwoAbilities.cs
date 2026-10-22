namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An emblem with two quoted abilities: `you get an emblem with "you have no maximum hand size" and "whenever a card is put into your graveyard from anywhere, …"`.</summary>
/// <exampledoc>Sarkhan, the Dragonspeaker</exampledoc>
/// <examplecapture>get an emblem with "at the beginning of your draw step, draw two additional cards" and "at the beginning of your end step, discard your hand</examplecapture>
public class GetEmblemWithTwoAbilities : Glyph, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Pattern("gets?"), " an emblem with \"", Prop(First), "\" and \"", Prop(Second)];

    public DynamicGlyph First { get; set; }
    public DynamicGlyph Second { get; set; }
}
