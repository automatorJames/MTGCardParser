namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The first reminder sentence of convoke and improvise, which let permanents help pay: "your creatures can help cast this spell", "your artifacts can help cast this spell", and, where the keyword is granted to spells, "your creatures can help cast those spells". Replaces the fixed ConvokeHelp/ImproviseHelp sentences.</summary>
/// <exampledoc>Autochthon Wurm</exampledoc>
/// <examplecapture>your creatures can help cast this spell</examplecapture>
public class HelpCastReminder : Glyph
{
    public override Nib[] Nibs => ["your", Prop(Helpers), "can help cast", Prop(Spells)];

    public HelpingPermanents Helpers { get; set; }
    public HelpedSpells Spells { get; set; }
}
