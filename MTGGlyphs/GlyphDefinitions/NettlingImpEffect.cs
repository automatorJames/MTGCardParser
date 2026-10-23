namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nettling Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nettling Imp</exampledoc>
/// <examplecapture>choose target non-wall creature the active player has controlled continuously since the beginning of the turn</examplecapture>
[Dependent]
public class NettlingImpEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target non-wall creature the active player has controlled continuously since the beginning of the turn"];
}
