namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An ability a permanent enters with, as kicker rewards give: "with flying", "with "pay 3 life: regenerate this creature"" (the closing quote falls after the sentence's period).</summary>
/// <exampledoc>Anavolver</exampledoc>
/// <examplecapture>with "pay 3 life: regenerate this creature</examplecapture>
[Dependent]
public class EntersWithAbility : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Ability)];

    public OneOf<QuotedAbility, ManyOf<Keyword>, Keyword?> Ability { get; set; }
}
