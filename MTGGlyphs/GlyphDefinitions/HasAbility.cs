namespace MTGGlyphs.GlyphDefinitions;

public class HasAbility : Glyph
{
    public override Nib[] Nibs => ["has \"", Prop(Ability), "\""];
    public DynamicGlyph Ability { get; set; }
}