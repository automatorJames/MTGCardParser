namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and attach {this} to it": an aura attaching itself to the card it just put onto the battlefield.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>and attach {this} to it</examplecapture>
[Dependent]
public class AttachThisToIt : Glyph
{
    public override Nib[] Nibs => ["and attach", Nib.This, "to it"];
}
