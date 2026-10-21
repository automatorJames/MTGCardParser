namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spells of a kind a player may cast, and from where: "a spell with mana value 3 or less from your hand", "an instant or sorcery spell from among them", "any number of spells from among them".</summary>
/// <exampledoc>Sram's Expertise</exampledoc>
/// <examplecapture>a spell with mana value 3 or less from your hand</examplecapture>
[Dependent]
public class CastableSpells : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Qualifier), Plural("spell"), Prop(With), Prop(From)];

    public Quantity Quantity { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, TargetQualifier> Qualifier { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    [Optional]
    public CastFrom? From { get; set; }
}
