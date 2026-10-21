namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Rewriting a spell's text so it affects every object instead of a target - overload's reminder and its kin: "change "target" in its text to "each", "change its text by replacing all instances of "target" with "each".</summary>
/// <exampledoc>Cyclonic Rift</exampledoc>
/// <examplecapture>change "target" in its text to "each</examplecapture>
public class ChangeTargetToEach : Glyph, IPredicate
{
    public override Nib[] Nibs => ["change", Prop(Wording)];

    public TargetToEachWording Wording { get; set; }
}
