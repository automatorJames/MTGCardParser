namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Damping Sphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Damping Sphere</exampledoc>
[TokenizationOrder(-1)]
public class DampingSphereRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each spell a player casts costs {1} more to cast for each other spell that player has cast this turn"];
}
