namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Moonshadow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonshadow</exampledoc>
/// <examplecapture>remove a -1/-1 counter from this creature</examplecapture>
[Dependent]
public class MoonshadowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["remove a -1/-1 counter from this creature"];
}
