namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having been in combat: "{this} attacked or blocked this combat" - a condition checked after combat.</summary>
/// <exampledoc>Clockwork Avian</exampledoc>
/// <examplecapture>{this} attacked or blocked this combat</examplecapture>
[Dependent]
public class AttackedOrBlocked : Glyph
{
    public override Nib[] Nibs => [Prop(Combatant), "attacked or blocked", Prop(Period)];

    public PermanentPhrase Combatant { get; set; }
    public ThisPeriod Period { get; set; }
}
