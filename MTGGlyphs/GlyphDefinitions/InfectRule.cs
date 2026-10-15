namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Infect's reminder text.</summary>
/// <exampledoc>Blackcleave Goblin</exampledoc>
/// <examplecapture>this creature deals damage to creatures in the form of -1/-1 counters and to players in the form of poison counters</examplecapture>
public class InfectRule : Glyph
{
    public override Nib[] Nibs => ["this creature deals damage to creatures in the form of -1/-1 counters and to players in the form of poison counters"];
}
