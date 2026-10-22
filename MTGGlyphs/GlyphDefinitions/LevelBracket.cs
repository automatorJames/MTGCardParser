namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A level up card's level bracket header: "level 1-2", "level 4+" - the levels at which the abilities and power/toughness listed under it apply.</summary>
/// <exampledoc>Brimstone Mage</exampledoc>
/// <examplecapture>level 1-2</examplecapture>
public class LevelBracket : Glyph
{
    public override Nib[] Nibs => ["level", Prop(Levels)];

    public CounterThreshold Levels { get; set; }
}
