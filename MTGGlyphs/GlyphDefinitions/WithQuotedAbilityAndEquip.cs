namespace MTGGlyphs.GlyphDefinitions;

/// <summary>`with "equipped creature gets +1/+0" and equip {2}`: an equipment token described with a closed quoted ability and then its equip cost.</summary>
/// <exampledoc>Iron Hills Blacksmith</exampledoc>
/// <examplecapture>with "equipped creature gets +1/+0" and equip {2}</examplecapture>
[Dependent]
public class WithQuotedAbilityAndEquip : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["with \"", Prop(Ability), "\" and equip ", Prop(Cost)];

    public DynamicGlyph Ability { get; set; }
    public ManaCost Cost { get; set; }
}
