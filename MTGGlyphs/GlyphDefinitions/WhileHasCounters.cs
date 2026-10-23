namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"while it has a +1/+1 counter on it": a state that must hold as an event happens.</summary>
/// <exampledoc>Undergrowth Champion</exampledoc>
/// <examplecapture>while it has a +1/+1 counter on it</examplecapture>
[Dependent]
public class WhileHasCounters : Glyph
{
    public override Nib[] Nibs => ["while", Prop(State)];

    public HasCounters State { get; set; }
}
