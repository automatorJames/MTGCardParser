namespace MTGGlyphs.GlyphDefinitions;

/// <summary>`with "this token can't block`, `with "{t}: target creature you control gets +1/+0 until end of turn`: an object described as having a quoted ability. The closing quote falls after the period.</summary>
/// <exampledoc>Voracious Vermin</exampledoc>
/// <examplecapture>with "this token can't block</examplecapture>
[Dependent]
public class WithQuotedAbility : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["with \"", Prop(Ability)];

    public DynamicGlyph Ability { get; set; }
}
