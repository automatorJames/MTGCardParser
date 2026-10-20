namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Indestructible's reminder text, for whatever has it: "damage and effects that say "destroy" don't destroy it", "effects that say "destroy" don't destroy this artifact", "… don't destroy them".</summary>
/// <exampledoc>Darksteel Colossus</exampledoc>
/// <examplecapture>damage and effects that say "destroy" don't destroy it</examplecapture>
public class IndestructibleRule : Glyph
{
    public override Nib[] Nibs => [Prop(Damage), "effects that say \"destroy\" don't destroy", Prop(Object)];

    [RegexPattern("damage and")]
    public bool Damage { get; set; }
    public OneOf<ThisCreature, It, Them> Object { get; set; }
}
