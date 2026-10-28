namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The creature another met in combat, after "blocked"/"was blocked by": "it", "that creature", "this creature", or any one of a kind ("a zombie", "a blue creature"). Written without the shared permanent descriptors, which would loop back through this one.</summary>
/// <exampledoc>Gaze of the Gorgon</exampledoc>
/// <examplecapture>it</examplecapture>
[Dependent]
public class CombatPartner : Glyph
{
    public override Nib[] Nibs => [Prop(Partner)];

    public OneOf<It, ThatCard, ThisCreature, AnyOneOfKind> Partner { get; set; }
}
