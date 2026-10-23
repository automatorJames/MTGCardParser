namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mark of Eviction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mark of Eviction</exampledoc>
[Dependent]
public class MarkOfEvictionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return enchanted creature and all auras attached to that creature to their owners' hands"];
}
