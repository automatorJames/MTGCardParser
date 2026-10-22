namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Trample's reminder text: "[it / this creature / a creature with trample / each of those creatures] can deal excess combat damage to the player or planeswalker it's attacking". The subject is whichever creature the trample is on.</summary>
/// <exampledoc>Thundering Giant</exampledoc>
/// <examplecapture>it can deal excess combat damage to the player or planeswalker it's attacking</examplecapture>
public class TrampleItRule : Glyph
{
    public override Nib[] Nibs => [Prop(Tramplers), "can deal excess combat damage to the player or planeswalker it's attacking"];

    public Subject Tramplers { get; set; }
}
