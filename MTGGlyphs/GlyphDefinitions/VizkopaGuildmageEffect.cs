namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vizkopa Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizkopa Guildmage</exampledoc>
[Dependent]
public class VizkopaGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you gain life this turn, each opponent loses that much life"];
}
