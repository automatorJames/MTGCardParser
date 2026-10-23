namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eriette, the Beguiler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eriette, the Beguiler</exampledoc>
/// <examplecapture>gain control of that permanent for as long as that aura is attached to it</examplecapture>
[Dependent]
public class ErietteTheBeguilerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of that permanent for as long as that aura is attached to it"];
}
