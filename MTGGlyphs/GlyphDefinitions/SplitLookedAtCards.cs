namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one of them into your hand and the rest on the bottom of your library in any order", "any number of them into your graveyard and the rest on top of your library in any order": splitting looked-at or revealed cards between two places, as scry, surveil and impulse effects do.</summary>
/// <exampledoc>Horizon Scholar</exampledoc>
/// <examplecapture>any number of them on the bottom of your library and the rest on top in any order</examplecapture>
[Dependent]
public class SplitLookedAtCards : Glyph
{
    public override Nib[] Nibs => [Prop(Cards), Prop(Chosen), "and the rest", Prop(Rest)];

    public ChosenLookedAtCards Cards { get; set; }
    public OneOf<LibraryPlacement, IntoZone> Chosen { get; set; }
    public OneOf<LibraryPlacement, IntoZone> Rest { get; set; }
}
