namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Equip restricted to a kind of creature: "equip knight {1}", "equip legendary creature {3}", "equip creature token {1}", "equip planeswalker {1}" - attach only to that kind of permanent you control.</summary>
/// <exampledoc>Steelclaw Lance</exampledoc>
/// <examplecapture>equip knight {1}</examplecapture>
public class EquipKind : Glyph
{
    public override Nib[] Nibs => ["equip", Prop(Qualifiers), Prop(Kinds), Prop(Cost)];

    [Optional]
    public QualifierWords Qualifiers { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<PermanentKind> Kinds { get; set; }
    public ManaCost Cost { get; set; }
}
