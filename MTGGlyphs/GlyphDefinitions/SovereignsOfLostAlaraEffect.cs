namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sovereigns of Lost Alara, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sovereigns of Lost Alara</exampledoc>
/// <examplecapture>you may search your library for an aura card that could enchant that creature, put it onto the battlefield attached to that creature, then shuffle</examplecapture>
[Dependent]
public class SovereignsOfLostAlaraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for an aura card that could enchant that creature, put it onto the battlefield attached to that creature, then shuffle"];
}
