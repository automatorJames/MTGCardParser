namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"mana of any type can be spent": the passive form of the any-mana rider.</summary>
/// <exampledoc>Gonti, Lord of Luxury</exampledoc>
/// <examplecapture>mana of any type can be spent</examplecapture>
[Dependent]
public class ManaCanBeSpent : Glyph
{
    public override Nib[] Nibs => ["mana of any", Prop(Kind), "can be spent"];

    public AnyManaKind Kind { get; set; }
}
