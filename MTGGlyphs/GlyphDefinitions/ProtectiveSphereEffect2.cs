namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Protective Sphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Protective Sphere</exampledoc>
/// <examplecapture>prevent all damage that would be dealt to you this turn by a source of your choice that shares a color with the mana spent on this activation cost</examplecapture>
[Dependent]
public class ProtectiveSphereEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt to you this turn by a source of your choice that shares a color with the mana spent on this activation cost"];
}
