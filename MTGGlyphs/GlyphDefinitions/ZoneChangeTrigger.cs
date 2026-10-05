namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A triggered ability on an object changing zones: "when {this} enters the battlefield, ...", "whenever a creature dies, ...".</summary>
public class ZoneChangeTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Subject), Prop(ZoneChange), ",", Prop(Effect)];

    public OneOf<This, IndefiniteObject> Subject { get; set; }
    public ZoneChange ZoneChange { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}

public enum ZoneChange
{
    EntersTheBattlefield,
    LeavesTheBattlefield,
    Dies,
    IsPutIntoAGraveyardFromTheBattlefield,
}
