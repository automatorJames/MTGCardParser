namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"blocking or blocked by this creature", "blocked by it", "attacking you": a qualifier relating a creature to another creature or a player in combat - which it blocks, is blocked by, or attacks.</summary>
/// <exampledoc>Engulfing Slagwurm</exampledoc>
/// <examplecapture>blocking or blocked by {this}</examplecapture>
[Dependent]
public class CombatRelation : Glyph
{
    public override Nib[] Nibs => [Alt("blocking or blocked by", "blocking or being blocked by", "blocking", "blocked by", "attacking"), Prop(Other)];

    public OneOf<ThisCreature, This, OneOf<It, WhichPlayer?>> Other { get; set; }
}
