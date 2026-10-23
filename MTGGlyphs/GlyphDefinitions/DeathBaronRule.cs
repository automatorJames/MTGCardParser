namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Death Baron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Death Baron</exampledoc>
[TokenizationOrder(-1)]
public class DeathBaronRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["skeletons you control and other zombies you control get +1/+1 and have deathtouch"];
}
