namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ghostly Keybearer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghostly Keybearer</exampledoc>
[Dependent]
public class GhostlyKeybearerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["unlock a locked door of up to one target room you control"];
}
