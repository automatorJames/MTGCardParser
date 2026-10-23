namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sworn Defender, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sworn Defender</exampledoc>
/// <examplecapture>this creature's power becomes the toughness of target creature blocking or being blocked by this creature minus 1 until end of turn, and its toughness becomes 1 plus the power of that creature until end of turn</examplecapture>
[Dependent]
public class SwornDefenderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's power becomes the toughness of target creature blocking or being blocked by this creature minus 1 until end of turn, and its toughness becomes 1 plus the power of that creature until end of turn"];
}
