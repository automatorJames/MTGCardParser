namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Expend's reminder rule: "you expend 4 as you spend your fourth total mana to cast spells during a turn". Held by ReminderText.</summary>
/// <exampledoc>Bakersbane Duo</exampledoc>
/// <examplecapture>you expend 4 as you spend your fourth total mana to cast spells during a turn</examplecapture>
[Dependent]
public class ExpendRule : Glyph
{
    public override Nib[] Nibs => [Prop(Event), "as you spend your", Prop(Nth), "total mana to cast spells during a turn"];

    public ExpendEvent Event { get; set; }
    public SpellOrdinal Nth { get; set; }
}
