namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Living weapon's reminder text.</summary>
/// <exampledoc>Batterskull</exampledoc>
/// <examplecapture>when this equipment enters, create a 0/0 black phyrexian germ creature token, then attach this to it</examplecapture>
public class LivingWeaponRule : Glyph
{
    public override Nib[] Nibs => ["when this equipment", Alt("enters", "enters the battlefield"), ", create a 0/0 black phyrexian germ creature token, then attach this to it"];
}
