namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An action followed by another subject's action, joined by "and": "tap that creature and it doesn't untap during its controller's next untap step", "create three tapped 2/2 black zombie creature tokens and you gain 3 life", "tap all lands target player controls and that player loses all unspent mana".</summary>
/// <exampledoc>Kashi-Tribe Elite</exampledoc>
/// <examplecapture>tap that creature and it doesn't untap during its controller's next untap step</examplecapture>
public class ActionAndSubjectDoes : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph First { get; set; }
    public SubjectDoes Second { get; set; }
}
