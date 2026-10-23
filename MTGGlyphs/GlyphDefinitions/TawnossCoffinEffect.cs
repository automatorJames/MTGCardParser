namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tawnos's Coffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tawnos's Coffin</exampledoc>
/// <examplecapture>return that exiled card to the battlefield under its owner's control tapped with the noted number and kind of counters on it</examplecapture>
[Dependent]
public class TawnossCoffinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that exiled card to the battlefield under its owner's control tapped with the noted number and kind of counters on it"];
}
