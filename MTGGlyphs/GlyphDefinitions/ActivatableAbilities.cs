namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Abilities of one keyword or class, as something a player may activate: "equip abilities", "her loyalty abilities", "the loyalty abilities of urza", "loyalty abilities of jace planeswalkers you control".</summary>
/// <exampledoc>Urza, Planeswalker</exampledoc>
/// <examplecapture>the loyalty abilities of urza</examplecapture>
[Dependent]
public class ActivatableAbilities : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), Prop(Kind), "abilities", Prop(Holder)];

    [Optional]
    public OneOf<PersonalPossessive, DefiniteArticle?> Determiner { get; set; }
    public OneOf<Keyword?, AbilityClass?> Kind { get; set; }
    [Optional]
    public AbilityHolder Holder { get; set; }
}
