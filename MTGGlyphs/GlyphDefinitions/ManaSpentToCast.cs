namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The mana spent on a spell, as converge counts its colors: "mana spent to cast this spell".</summary>
/// <exampledoc>Brilliant Spectrum</exampledoc>
/// <examplecapture>mana spent to cast this spell</examplecapture>
[Dependent]
public class ManaSpentToCast : Glyph
{
    public override Nib[] Nibs => ["mana spent to cast", Alt("this spell", "it")];
}
