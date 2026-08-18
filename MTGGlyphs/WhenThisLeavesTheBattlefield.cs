namespace MTGGlyphs;

public class WhenThisLeavesTheBattlefield : Glyph
{
    public override Nib[] Nibs => ["when {this} leaves the battlefield,", Prop(Result)];

    public DynamicGlyph Result { get; set; }
}