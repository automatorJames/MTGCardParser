namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Trample's rule, from its reminder text: "this creature can deal excess combat damage to the player or planeswalker it's attacking".</summary>
/// <exampledoc>Ball Lightning</exampledoc>
/// <examplecapture>this creature can deal excess combat damage to the player or planeswalker it's attacking</examplecapture>
public class TrampleRule : Glyph
{
    public override Nib[] Nibs => ["this creature can deal excess combat damage to the player or planeswalker it's attacking"];
}
