namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tyrant of Discord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tyrant of Discord</exampledoc>
[Dependent]
public class TyrantOfDiscordEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent chooses a permanent they control at random and sacrifices it"];
}
