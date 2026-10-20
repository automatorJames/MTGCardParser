namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on the mana paid for a spell, for hybrid "if {w} was spent to cast this spell" cards and Alara/Shadowmoor checks: "{u} was spent to cast it", "{r} was spent to cast this spell". The mana is a ManaCost; what was cast is it or this spell.</summary>
/// <exampledoc>Ogre Savant</exampledoc>
/// <examplecapture>{u} was spent to cast it</examplecapture>
[Dependent]
public class ManaWasSpent : Glyph
{
    public override Nib[] Nibs => [Prop(Mana), "was spent to cast", Prop(Spell)];

    public ManaCost Mana { get; set; }
    public OneOf<It, ThisObject> Spell { get; set; }
}
