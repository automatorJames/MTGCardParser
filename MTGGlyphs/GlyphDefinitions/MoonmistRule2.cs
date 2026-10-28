namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moonmist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonmist</exampledoc>
[TokenizationOrder(-1)]
public class MoonmistRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage that would be dealt this turn by creatures other than werewolves and wolves"];
}
