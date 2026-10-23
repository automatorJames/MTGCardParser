namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Book of Vile Darkness, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Book of Vile Darkness</exampledoc>
/// <examplecapture>create vecna, a legendary 8/8 black zombie god creature token with indestructible and all triggered abilities of the exiled cards</examplecapture>
[Dependent]
public class TheBookOfVileDarknessEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create vecna, a legendary 8/8 black zombie god creature token with indestructible and all triggered abilities of the exiled cards"];
}
