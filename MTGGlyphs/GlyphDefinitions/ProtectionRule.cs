namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Protection's reminder, as what the protected permanent can't be subject to: "can't be blocked, targeted, dealt damage, or enchanted by anything white", "… enchanted, or equipped by anything of that color".</summary>
/// <exampledoc>Black Knight</exampledoc>
/// <examplecapture>can't be blocked, targeted, dealt damage, or enchanted by anything white</examplecapture>
[Dependent]
public class ProtectionRule : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't be blocked, targeted, dealt damage,", Prop(Enchanted), "or", Alt("enchanted", "equipped"), "by anything", Prop(Quality)];

    [RegexPattern("enchanted,")]
    public bool Enchanted { get; set; }
    public OneOf<ManyOf<ManaColor>, ManaColor?, ThatQuality?> Quality { get; set; }
}
