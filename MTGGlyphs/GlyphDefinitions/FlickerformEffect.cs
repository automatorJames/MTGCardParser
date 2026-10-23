namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flickerform, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flickerform</exampledoc>
[Dependent]
public class FlickerformEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile enchanted creature and all auras attached to it"];
}
