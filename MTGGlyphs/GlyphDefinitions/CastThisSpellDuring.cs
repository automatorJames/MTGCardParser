namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When this spell was cast, as a condition (addendum): "you cast this spell during your main phase".</summary>
/// <exampledoc>Arrester's Admonition</exampledoc>
/// <examplecapture>you cast this spell during your main phase</examplecapture>
[Dependent]
public class CastThisSpellDuring : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "cast this spell", Prop(During)];

    public WhichPlayer Player { get; set; }
    public DuringStep During { get; set; }
}
