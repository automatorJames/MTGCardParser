namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"on the battlefield": anywhere on the battlefield, whoever controls it.</summary>
/// <exampledoc>Chain Reaction</exampledoc>
/// <examplecapture>on the battlefield</examplecapture>
[Dependent]
public class OnTheBattlefield : Glyph
{
    public override Nib[] Nibs => ["on the battlefield"];
}
