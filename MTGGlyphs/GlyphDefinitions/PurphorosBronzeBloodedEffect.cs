namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Purphoros, Bronze-Blooded, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Purphoros, Bronze-Blooded</exampledoc>
[Dependent]
public class PurphorosBronzeBloodedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a red creature card or an artifact creature card from your hand onto the battlefield"];
}
