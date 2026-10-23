namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rasputin Dreamweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rasputin Dreamweaver</exampledoc>
[Dependent]
public class RasputinDreamweaver2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if rasputin started the turn untapped, put a dream counter on it"];
}
