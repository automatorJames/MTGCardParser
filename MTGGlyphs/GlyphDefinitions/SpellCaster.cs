namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you cast", "your opponents cast", "you cast from your graveyard": who casts the spells meant, and optionally from where.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>you cast</examplecapture>
[Dependent]
public class SpellCaster : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Pattern("casts?"), Prop(From)];

    public WhichPlayer Player { get; set; }
    [Optional]
    public CastFrom? From { get; set; }
}
