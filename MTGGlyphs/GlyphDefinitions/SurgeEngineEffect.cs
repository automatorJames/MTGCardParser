namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Surge Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Surge Engine</exampledoc>
[Dependent]
public class SurgeEngineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature becomes blue and has base power and toughness 5/4"];
}
