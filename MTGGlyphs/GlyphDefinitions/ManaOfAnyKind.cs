namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"mana of any color", "mana of any type": mana treated as any kind.</summary>
/// <exampledoc>Chromatic Orrery</exampledoc>
/// <examplecapture>mana of any color</examplecapture>
[Dependent]
public class ManaOfAnyKind : Glyph
{
    public override Nib[] Nibs => ["mana of any", Prop(Kind)];

    public AnyManaKind Kind { get; set; }
}
