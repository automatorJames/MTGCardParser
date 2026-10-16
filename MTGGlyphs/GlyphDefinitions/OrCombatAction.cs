namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The second action of "can't attack or block": "or block".</summary>
/// <exampledoc>Bound in Silence</exampledoc>
/// <examplecapture>or block</examplecapture>
[Dependent]
public class OrCombatAction : Glyph
{
    public override Nib[] Nibs => ["or", Prop(Action)];

    public CombatAction Action { get; set; }
}
