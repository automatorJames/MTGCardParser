namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by an opponent", "by you": who cast a spell.</summary>
/// <exampledoc>Soul Partition</exampledoc>
/// <examplecapture>by an opponent</examplecapture>
[Dependent]
public class SpellCastBy : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Caster)];

    public WhichPlayer Caster { get; set; }
}
