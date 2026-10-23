namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that target this creature", "that target you", "that target a merfolk you control", "that target a creature or planeswalker you control", "that target charix": spells picked out by what they target (a legend by its short name last).</summary>
/// <exampledoc>Icefall Regent</exampledoc>
/// <examplecapture>that target this creature</examplecapture>
[Dependent]
public class ThatTarget : Glyph
{
    public override Nib[] Nibs => ["that target", Prop(Target)];

    public OneOf<PermanentPhrase, WhichPlayer?, OneOf<IndefinitePermanent, ShortName>> Target { get; set; }
}
