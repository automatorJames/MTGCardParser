namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guard Dogs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guard Dogs</exampledoc>
[TokenizationOrder(-1)]
public class GuardDogsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage target creature would deal this turn if it shares a color with that permanent"];
}
