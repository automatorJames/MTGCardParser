namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An action multiplied by a count after it: "draw a card for each creature you control", "add {g} for each creature you control", "target player draws a card for each basic land type among lands they control". The action is one predicate, with or without a subject, so "for each" scopes over one action rather than a whole "…, then …" sequence; and this isn't a predicate itself, so glyphs that model their own "for each" keep it. Tried after other glyphs.</summary>
/// <exampledoc>Gaea's Cradle</exampledoc>
/// <examplecapture>add {g} for each creature you control</examplecapture>
[TokenizationOrder(-1)]
public class EffectForEach : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Effect), Prop(PerEach)];

    [Optional]
    public Subject Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Effect { get; set; }
    public ForEach PerEach { get; set; }
}
