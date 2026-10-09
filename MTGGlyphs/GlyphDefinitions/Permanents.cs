namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "lands", "nonblack creatures", "tapped blue creatures they control": permanents of one type, optionally narrowed
/// and optionally by who controls them. The card type is a referent, so a later "they" or "those creatures" refers to
/// them (see <see cref="ThatCard"/>).
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>tapped blue creatures they control</examplecapture>
[Dependent]
public class Permanents : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural(Prop(CardType)), Prop(Controller)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }

    [Referent]
    [Plural]
    public CardType CardType { get; set; }

    [Optional]
    public ControlledBy Controller { get; set; }
}
