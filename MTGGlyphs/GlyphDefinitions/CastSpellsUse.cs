namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to cast spells: "cast a creature spell", "cast artifact spells", "cast a creature spell of the chosen type", "cast spells from your graveyard", a list of alternatives ("cast a dragon spell or an omen spell", "cast spells with mana value 5 or greater or spells with {x} in their mana costs"), or "to cast …" after "or".</summary>
/// <exampledoc>Ancient Ziggurat</exampledoc>
/// <examplecapture>cast a creature spell</examplecapture>
[Dependent]
public class CastSpellsUse : Glyph
{
    public override Nib[] Nibs => [Opt("to"), "cast", Prop(Spells), Prop(Narrowed)];

    public OneOf<IndefiniteSpell, PlayableCards, OneOf<ManyOf<IndefiniteSpell>, ManyOf<PlayableCards>>> Spells { get; set; }
    [Optional]
    public SpellNarrowing Narrowed { get; set; }
}
