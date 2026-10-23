namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Commandeer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Commandeer</exampledoc>
[Dependent]
public class CommandeerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that spell is an artifact, enchantment, or planeswalker, the permanent enters under your control"];
}
