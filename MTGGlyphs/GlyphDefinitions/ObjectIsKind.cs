namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on what kind of permanent an object named earlier is or was: "that's another creature" (backup), "it's a creature", "it's an angel", "it's an artifact creature", "it was a human", "that creature was a human", "that permanent is a chandra planeswalker".</summary>
/// <exampledoc>Saiba Cryptomancer</exampledoc>
/// <examplecapture>that's another creature</examplecapture>
[Dependent]
public class ObjectIsKind : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Prop(Kind)];

    public OneOf<OneOf<ItIs, ThatIs>, ItWas, ThatObjectIsOrWas> Object { get; set; }
    public IndefinitePermanent Kind { get; set; }
}
