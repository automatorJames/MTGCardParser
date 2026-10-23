namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kill-Suit Cultist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kill-Suit Cultist</exampledoc>
[Dependent]
public class KillSuitCultistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time damage would be dealt to target creature this turn, destroy that creature instead"];
}
