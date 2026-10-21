namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder rule on delayed returns: "return it only if it's on the battlefield" / "… only if it survived combat". Held by ReminderText.</summary>
/// <exampledoc>Phantom Whelp</exampledoc>
/// <examplecapture>return it only if it's on the battlefield</examplecapture>
[Dependent]
public class ReturnOnlyIfStillThere : Glyph
{
    public override Nib[] Nibs => ["return it only if", Alt("it's on the battlefield", "it survived combat")];
}
