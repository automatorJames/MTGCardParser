namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"without summoning sickness": a creature that has been under its controller's control since their most recent turn began (enlist, crew-like costs).</summary>
/// <exampledoc>Hexbane Tortoise</exampledoc>
/// <examplecapture>without summoning sickness</examplecapture>
[Dependent]
public class WithoutSummoningSickness : Glyph
{
    public override Nib[] Nibs => ["without summoning sickness"];
}
