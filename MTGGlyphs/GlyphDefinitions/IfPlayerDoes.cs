namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"if the player does, …": what follows a choice another player made.</summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>if the player does, untap those creatures</examplecapture>
public class IfPlayerDoes : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Player), "does,", Prop(Outcome)];

    public ThatPlayer Player { get; set; }
    public DynamicGlyph Outcome { get; set; }
}
