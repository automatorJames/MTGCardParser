namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Titania, Voice of Gaea // Titania, Gaea Incarnate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titania, Voice of Gaea // Titania, Gaea Incarnate</exampledoc>
/// <examplecapture>if there are four or more land cards in your graveyard and you both own and control titania, voice of gaea and a land named argoth, sanctum of nature, exile them, then meld them into titania, gaea incarnate</examplecapture>
[Dependent]
public class TitaniaVoiceOfGaeaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are four or more land cards in your graveyard and you both own and control titania, voice of gaea and a land named argoth, sanctum of nature, exile them, then meld them into titania, gaea incarnate"];
}
