namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Protective Sphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Protective Sphere</exampledoc>
[Dependent]
public class ProtectiveSphereEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["colorless mana prevents no damage"];
}
