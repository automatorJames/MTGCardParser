namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Preventing half the damage a replacement condition just named: "prevent half that damage, rounded up".</summary>
/// <exampledoc>Gisela, Blade of Goldnight</exampledoc>
/// <examplecapture>prevent half that damage, rounded up</examplecapture>
[Dependent]
public class PreventHalfThatDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => ["prevent", Prop(Half)];

    public HalfThatDamage Half { get; set; }
}
