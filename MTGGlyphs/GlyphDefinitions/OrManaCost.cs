namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative mana payment after a first one: the "or {w}" of "cumulative upkeep {g} or {w}".</summary>
/// <exampledoc>Arctic Nishoba</exampledoc>
/// <examplecapture>or {w}</examplecapture>
[Dependent]
public class OrManaCost : Glyph
{
    public override Nib[] Nibs => ["or", Prop(Mana)];

    public ManaCost Mana { get; set; }
}
