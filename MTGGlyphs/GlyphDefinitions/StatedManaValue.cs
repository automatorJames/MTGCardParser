namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder of a hybrid-mana card's fixed mana value: "this card's mana value is 6" - follows the "{2/u} can be paid with any two mana" reminder.</summary>
/// <exampledoc>Flame Javelin</exampledoc>
/// <examplecapture>this card's mana value is 6</examplecapture>
public class StatedManaValue : Glyph
{
    public override Nib[] Nibs => ["this card's mana value is", Prop(Value)];

    public int Value { get; set; }
}
