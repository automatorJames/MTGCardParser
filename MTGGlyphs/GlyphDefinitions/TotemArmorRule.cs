namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Totem armor's reminder text.</summary>
/// <exampledoc>Hyena Umbra</exampledoc>
/// <examplecapture>if enchanted creature would be destroyed, instead remove all damage from it and destroy this aura</examplecapture>
public class TotemArmorRule : Glyph
{
    public override Nib[] Nibs => ["if enchanted creature would be destroyed, instead remove all damage from it and destroy this aura"];
}
