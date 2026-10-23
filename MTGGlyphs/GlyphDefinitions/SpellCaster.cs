namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you cast", "your opponents cast", "you cast from your graveyard", "you control": who casts (or controls) the spells meant, and optionally from where.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>you cast</examplecapture>
[Dependent]
public class SpellCaster : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Relation), Prop(From)];

    public WhichPlayer Player { get; set; }
    public SpellRelation Relation { get; set; }
    [Optional]
    public CastFrom? From { get; set; }
}
