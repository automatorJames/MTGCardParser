namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pyxis of Pandemonium, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyxis of Pandemonium</exampledoc>
/// <examplecapture>each player turns face up all cards they own exiled with this artifact, then puts all permanent cards among them onto the battlefield</examplecapture>
[Dependent]
public class PyxisOfPandemoniumEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player turns face up all cards they own exiled with this artifact, then puts all permanent cards among them onto the battlefield"];
}
