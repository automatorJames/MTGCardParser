namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana from a kind of permanent having paid for a spell, as a condition: "mana from a treasure was spent to cast it", "mana from a treasure was spent to cast this spell".</summary>
/// <exampledoc>Smaug, Wicked Worm</exampledoc>
/// <examplecapture>mana from a treasure was spent to cast it</examplecapture>
[Dependent]
public class ManaFromSourceSpent : Glyph
{
    public override Nib[] Nibs => ["mana from", Pattern("an?"), Prop(Source), "was spent to cast", Prop(Spell)];

    public PermanentKind Source { get; set; }
    public OneOf<It, ThisSpell, ThatStackObject> Spell { get; set; }
}
