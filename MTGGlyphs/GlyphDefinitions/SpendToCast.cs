namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to cast planeswalker spells", "to cast that spell": what spent mana pays for.</summary>
/// <exampledoc>Oath of Nissa</exampledoc>
/// <examplecapture>to cast planeswalker spells</examplecapture>
[Dependent]
public class SpendToCast : Glyph
{
    public override Nib[] Nibs => ["to cast", Prop(Spells)];

    public OneOf<ThatStackObject, CastObject, SpellsCast> Spells { get; set; }
}
