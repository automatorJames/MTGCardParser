namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flummoxed Cyclops, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flummoxed Cyclops</exampledoc>
[Dependent]
public class FlummoxedCyclopsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't block this combat"];
}
