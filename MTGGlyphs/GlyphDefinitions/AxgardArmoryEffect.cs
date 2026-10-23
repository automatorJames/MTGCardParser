namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Axgard Armory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Axgard Armory</exampledoc>
/// <examplecapture>search your library for an aura card and/or an equipment card, reveal them, put them into your hand, then shuffle</examplecapture>
[Dependent]
public class AxgardArmoryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for an aura card and/or an equipment card, reveal them, put them into your hand, then shuffle"];
}
