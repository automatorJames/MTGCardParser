namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Keys to the House, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keys to the House</exampledoc>
[Dependent]
public class KeysToTheHouseEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lock or unlock a door of target room you control"];
}
