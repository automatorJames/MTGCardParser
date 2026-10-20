namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The stun counter's reminder text, restating its replacement rule.</summary>
/// <exampledoc>Rowdy Snowballers</exampledoc>
/// <examplecapture>if a permanent with a stun counter would become untapped, remove one from it instead</examplecapture>
public class StunCounterRule : Glyph
{
    public override Nib[] Nibs => ["if a permanent with a stun counter would become untapped, remove one from it instead"];
}
