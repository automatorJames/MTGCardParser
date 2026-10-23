namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on the mana paid for a spell or ability, for hybrid "if {w} was spent to cast this spell" cards and Alara/Shadowmoor checks: "{u} was spent to cast it", "{r} was spent to cast this spell", "mana from a treasure was spent to activate this ability". Counted amounts are <see cref="ManaSpentCondition"/>.</summary>
/// <exampledoc>Ogre Savant</exampledoc>
/// <examplecapture>{u} was spent to cast it</examplecapture>
[Dependent]
public class ManaWasSpent : Glyph
{
    public override Nib[] Nibs => [Prop(Mana), "was spent", Prop(Use)];

    public OneOf<ManaCost, ManaFromSource> Mana { get; set; }
    public OneOf<SpellPaidFor, AbilityPaidFor> Use { get; set; }
}
