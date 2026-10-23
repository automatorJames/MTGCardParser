namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chandra's Incinerator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Incinerator</exampledoc>
/// <examplecapture>this creature deals that much damage to target creature or planeswalker that player controls</examplecapture>
[Dependent]
public class ChandrasIncineratorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals that much damage to target creature or planeswalker that player controls"];
}
