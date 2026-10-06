namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player gaining or losing an amount of life: "you gain 2 life".</summary>
/// <exampledoc>Onulet</exampledoc>
/// <examplecapture>you gain 2 life</examplecapture>
public class LifeChangeQuantity : Glyph
{
    public override Nib[] Nibs => [Prop(WhichPlayer), Prop(LifeVerb), Prop(Quantity), "life"];

    public WhichPlayer WhichPlayer { get; set; }
    public LifeVerb LifeVerb { get; set; }
    public Quantity Quantity { get; set; }
}

public enum LifeVerb
{
    Gain,
    Lose
}