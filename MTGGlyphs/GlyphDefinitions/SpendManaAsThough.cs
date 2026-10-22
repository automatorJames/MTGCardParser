namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana as though it were another kind, as a permission, optionally for one purpose: "spend mana as though it were mana of any color", "spend white mana as though it were red mana", "spend mana as though it were mana of any color to cast planeswalker spells", "… to activate those abilities".</summary>
/// <exampledoc>Sunglasses of Urza</exampledoc>
/// <examplecapture>spend white mana as though it were red mana</examplecapture>
[Dependent]
public class SpendManaAsThough : Glyph, IPredicate
{
    public override Nib[] Nibs => ["spend", Prop(Spent), "mana as though it were", Prop(As), Prop(Purpose)];

    [Optional]
    public ManaColor? Spent { get; set; }
    public OneOf<ManaOfAnyKind, ManaOfColor> As { get; set; }
    [Optional]
    public OneOf<SpendToCast, SpendToActivate> Purpose { get; set; }
}
