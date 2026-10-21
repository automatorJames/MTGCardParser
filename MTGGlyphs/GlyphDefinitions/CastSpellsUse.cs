namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to cast spells: "cast a creature spell", "cast artifact spells".</summary>
/// <exampledoc>Ancient Ziggurat</exampledoc>
/// <examplecapture>cast a creature spell</examplecapture>
[Dependent]
public class CastSpellsUse : Glyph
{
    public override Nib[] Nibs => ["cast", Prop(Spells)];

    public OneOf<IndefiniteSpell, PlayableCards> Spells { get; set; }
}
