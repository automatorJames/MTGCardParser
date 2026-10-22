namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Prototype's reminder: "you may cast this spell with different mana cost, color, and size" (its second sentence is KeepsAbilitiesAndTypes).</summary>
/// <exampledoc>Steel Seraph</exampledoc>
/// <examplecapture>you may cast this spell with different mana cost, color, and size</examplecapture>
public class CastWithDifferentCharacteristicsRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this spell with different mana cost, color, and size"];
}
