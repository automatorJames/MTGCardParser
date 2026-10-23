namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Steamflogger Boss, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Steamflogger Boss</exampledoc>
[TokenizationOrder(-1)]
public class SteamfloggerBossRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a rigger you control would assemble a contraption, it assembles two contraptions instead"];
}
