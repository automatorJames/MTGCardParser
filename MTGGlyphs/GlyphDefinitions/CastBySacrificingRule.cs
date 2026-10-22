namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Emerge's reminder: casting a spell by sacrificing a creature and paying the keyword's cost reduced by that creature's mana value: "you may cast this spell by sacrificing a creature and paying the emerge cost reduced by that creature's mana value".</summary>
/// <exampledoc>Elder Deep-Fiend</exampledoc>
/// <examplecapture>you may cast this spell by sacrificing a creature and paying the emerge cost reduced by that creature's mana value</examplecapture>
public class CastBySacrificingRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this spell by sacrificing", Prop(Sacrificed), "and paying the", Prop(Keyword), "cost reduced by that creature's mana value"];

    public IndefinitePermanent Sacrificed { get; set; }
    public Keyword Keyword { get; set; }
}
