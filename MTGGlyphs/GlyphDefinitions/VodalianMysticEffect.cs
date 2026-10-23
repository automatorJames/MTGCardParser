namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vodalian Mystic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vodalian Mystic</exampledoc>
[Dependent]
public class VodalianMysticEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target instant or sorcery spell becomes the color of your choice"];
}
