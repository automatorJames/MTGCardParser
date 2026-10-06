namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "tapped blue creatures they control": permanents of one type that a player controls. The card type is a referent,
/// so a later "those creatures" refers to them (see <see cref="ThatCard"/>).
/// </summary>
[Dependent]
public class PermanentsTheyControl : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural(Prop(CardType)), Prop(Controller), "control"];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }

    [Referent(GrammaticalNumber.Plural)]
    public CardType CardType { get; set; }

    public TheyPlayer Controller { get; set; }
}
