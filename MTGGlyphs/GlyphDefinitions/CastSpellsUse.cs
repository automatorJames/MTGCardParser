namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to cast spells: "cast a creature spell", "cast artifact spells", "cast a creature spell of the chosen type", "cast spells from your graveyard", or "to cast …" after "or".</summary>
/// <exampledoc>Ancient Ziggurat</exampledoc>
/// <examplecapture>cast a creature spell</examplecapture>
[Dependent]
public class CastSpellsUse : Glyph
{
    public override Nib[] Nibs => [Opt("to"), "cast", Prop(Spells), Prop(Narrowed)];

    public OneOf<IndefiniteSpell, PlayableCards> Spells { get; set; }
    [Optional]
    public SpellNarrowing Narrowed { get; set; }
}
