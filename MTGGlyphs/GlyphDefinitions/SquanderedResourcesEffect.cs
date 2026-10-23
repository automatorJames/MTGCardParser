namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Squandered Resources, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squandered Resources</exampledoc>
[Dependent]
public class SquanderedResourcesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add one mana of any type the sacrificed land could produce"];
}
