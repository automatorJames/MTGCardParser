namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ageless Sentinels, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ageless Sentinels</exampledoc>
[Dependent]
public class AgelessSentinelsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's no longer a wall"];
}
