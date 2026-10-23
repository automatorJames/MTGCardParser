namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Callous Oppressor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Callous Oppressor</exampledoc>
[Dependent]
public class CallousOppressorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target creature that isn't of the chosen type for as long as this creature remains tapped"];
}
