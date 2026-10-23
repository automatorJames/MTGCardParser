namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rocket Launcher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rocket Launcher</exampledoc>
[TokenizationOrder(-1)]
public class RocketLauncherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if you've controlled this artifact continuously since the beginning of your most recent turn"];
}
