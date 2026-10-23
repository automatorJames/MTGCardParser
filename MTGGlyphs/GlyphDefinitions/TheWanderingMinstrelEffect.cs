namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Wandering Minstrel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Wandering Minstrel</exampledoc>
/// <examplecapture>if you control five or more towns, create a 2/2 elemental creature token that's all colors</examplecapture>
[Dependent]
public class TheWanderingMinstrelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control five or more towns, create a 2/2 elemental creature token that's all colors"];
}
