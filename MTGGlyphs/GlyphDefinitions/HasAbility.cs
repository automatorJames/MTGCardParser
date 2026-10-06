namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Having a quoted ability: 'has "at the beginning of your upkeep, …"'.</summary>
/// <exampledoc>Farmstead</exampledoc>
/// <examplecapture>has "at the beginning of your upkeep, you may pay {w}{w}. if you do, you gain 1 life."</examplecapture>
public class HasAbility : Glyph
{
    public override Nib[] Nibs => ["has \"", Prop(Ability), "\""];
    public DynamicGlyph Ability { get; set; }
}