namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The stun counter's reminder text, restating its replacement rule: the general form ("a permanent with a stun counter … remove one") and the per-card form ("if it would become untapped, remove a stun counter from it").</summary>
/// <exampledoc>Rowdy Snowballers</exampledoc>
/// <examplecapture>if a permanent with a stun counter would become untapped, remove one from it instead</examplecapture>
public class StunCounterRule : Glyph
{
    public override Nib[] Nibs => [Alt("if a permanent with a stun counter would become untapped, remove one from it instead", "if it would become untapped, remove a stun counter from it instead")];
}
