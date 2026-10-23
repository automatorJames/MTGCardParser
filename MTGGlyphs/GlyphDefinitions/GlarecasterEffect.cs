namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Glarecaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glarecaster</exampledoc>
[Dependent]
public class GlarecasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time damage would be dealt to this creature and/or you this turn, that damage is dealt to any target instead"];
}
