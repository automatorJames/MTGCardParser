namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total some margin above their starting life total, as a condition: "you have at least 7 life more than your starting life total" (the Angel of Destiny / Righteous Valkyrie threshold).</summary>
/// <exampledoc>Righteous Valkyrie</exampledoc>
/// <examplecapture>you have at least 7 life more than your starting life total</examplecapture>
[Dependent]
public class LifeAboveStarting : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Verb), "at least", Prop(Margin), "life more than", Prop(Starting)];

    public WhichPlayer Player { get; set; }
    public HaveVerb Verb { get; set; }
    public Quantity Margin { get; set; }
    public StartingLifeTotal Starting { get; set; }
}
