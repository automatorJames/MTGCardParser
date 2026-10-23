namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana of a color picked earlier or by the player, with no count: "mana of that color" (inside "an amount of mana of that color").</summary>
/// <exampledoc>Nykthos, Shrine to Nyx</exampledoc>
/// <examplecapture>mana of that color</examplecapture>
[Dependent]
public class ManaOfChoice : Glyph
{
    public override Nib[] Nibs => ["mana of", Prop(Choice)];

    public ManaChoice Choice { get; set; }
}
