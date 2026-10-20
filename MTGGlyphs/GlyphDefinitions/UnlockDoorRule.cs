namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Room cards' reminder: "as a sorcery, you may pay the mana cost of a locked door to unlock it".</summary>
/// <exampledoc>Dazzling Theater // Prop Room</exampledoc>
/// <examplecapture>as a sorcery, you may pay the mana cost of a locked door to unlock it</examplecapture>
public class UnlockDoorRule : Glyph
{
    public override Nib[] Nibs => ["as a sorcery, you may pay the mana cost of a locked door to unlock it"];
}
