namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Bloodthirst's reminder text, with the bloodthirst amount as a slot.</summary>
/// <exampledoc>Bloodlord of Vaasgoth</exampledoc>
/// <examplecapture>if an opponent was dealt damage this turn, this creature enters with three +1/+1 counters on it</examplecapture>
public class BloodthirstRule : Glyph
{
    public override Nib[] Nibs => ["if an opponent was dealt damage this turn, this creature", Alt("enters", "enters the battlefield"), "with", Prop(Counters), Plural("+1/+1 counter"), "on it"];

    public Quantity Counters { get; set; }
}
