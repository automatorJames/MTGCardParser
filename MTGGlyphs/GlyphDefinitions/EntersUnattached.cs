namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Bestow-style attached tail: "this card enters the battlefield unattached and stays on the battlefield if the creature leaves".</summary>
/// <exampledoc>Gift of Immortality</exampledoc>
/// <examplecapture>this card enters the battlefield unattached and stays on the battlefield if the creature leaves</examplecapture>
public class EntersUnattached : Glyph
{
    public override Nib[] Nibs => ["this card enters the battlefield unattached and stays on the battlefield if the creature leaves"];
}
