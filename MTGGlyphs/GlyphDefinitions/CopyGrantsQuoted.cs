namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy that gains a quoted ability, possibly after keywords: `except it has haste and "at the beginning of the end step, sacrifice this token`, `except it has "{2}{u}{u}: return this creature to its owner's hand`.</summary>
/// <exampledoc>Heat Shimmer</exampledoc>
/// <examplecapture>except it has haste and "at the beginning of the end step, exile this token</examplecapture>
[Dependent]
public class CopyGrantsQuoted : Glyph
{
    public override Nib[] Nibs => ["except it", Prop(Grant)];

    public GrantsQuotedAbility Grant { get; set; }
}
