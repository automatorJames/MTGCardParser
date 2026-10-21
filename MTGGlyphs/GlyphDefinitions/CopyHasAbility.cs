namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Abilities a copy gains: "except it has haste", "except it has this ability" (the copying ability itself, so the copy can copy again).</summary>
/// <exampledoc>Cryptoplasm</exampledoc>
/// <examplecapture>except it has this ability</examplecapture>
[Dependent]
public class CopyHasAbility : Glyph
{
    public override Nib[] Nibs => ["except it has", Prop(Ability)];

    public OneOf<ThisAbility, ManyOf<Keyword>, Keyword?> Ability { get; set; }
}
