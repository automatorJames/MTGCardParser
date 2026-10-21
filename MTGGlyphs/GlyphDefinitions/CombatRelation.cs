namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"blocking or blocked by this creature", "blocked by it": a qualifier relating a creature to another in combat - which creature it blocks or is blocked by.</summary>
/// <exampledoc>Engulfing Slagwurm</exampledoc>
/// <examplecapture>blocking or blocked by {this}</examplecapture>
[Dependent]
public class CombatRelation : Glyph
{
    public override Nib[] Nibs => [Alt("blocking or blocked by", "blocking or being blocked by", "blocking", "blocked by"), Prop(Other)];

    public OneOf<ThisCreature, This, It> Other { get; set; }
}
