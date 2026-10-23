namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dream Fighter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Fighter</exampledoc>
[Dependent]
public class DreamFighterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature and that creature phase out"];
}
