namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Airbend's reminder, its second sentence: "while it's exiled, its owner may cast it for {2} rather than its mana cost" (or "while each one is exiled" for several).</summary>
/// <exampledoc>Airbending Lesson</exampledoc>
/// <examplecapture>while it's exiled, its owner may cast it for {2} rather than its mana cost</examplecapture>
public class AirbendCastRule : Glyph
{
    public override Nib[] Nibs => [Alt("while it's exiled", "while each one is exiled"), ", its owner may cast it for", Prop(Cost), "rather than its mana cost"];

    public ManaCost Cost { get; set; }
}
