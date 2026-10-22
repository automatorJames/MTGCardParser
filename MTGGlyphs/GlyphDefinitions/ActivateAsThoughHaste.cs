namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Letting creatures' {t} abilities be activated despite summoning sickness: "activate abilities of creatures you control as though those creatures had haste".</summary>
/// <exampledoc>Thousand-Year Elixir</exampledoc>
/// <examplecapture>activate abilities of creatures you control as though those creatures had haste</examplecapture>
[Dependent]
public class ActivateAsThoughHaste : Glyph, IPredicate
{
    public override Nib[] Nibs => ["activate abilities of", Prop(Creatures), "as though those creatures had haste"];

    public Permanents Creatures { get; set; }
}
