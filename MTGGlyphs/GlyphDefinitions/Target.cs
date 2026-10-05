namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target creature", "target player".</summary>
[Dependent]
public class Target : Glyph
{
    public override Nib[] Nibs => ["target", Prop(TargetableEntity)];

    public TargetableEntity TargetableEntity { get; set; }
}

/// <summary>"any target": a creature, player or planeswalker.</summary>
[Dependent]
public class AnyTarget : Glyph;
