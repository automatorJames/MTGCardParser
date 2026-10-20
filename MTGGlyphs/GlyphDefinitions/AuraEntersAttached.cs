namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The aura resolution rule, from enchant's reminder text: "this card enters attached to that creature" ("enters the battlefield" in older wording).</summary>
/// <exampledoc>Arcane Teachings</exampledoc>
/// <examplecapture>this card enters attached to that creature</examplecapture>
public class AuraEntersAttached : Glyph
{
    public override Nib[] Nibs => ["this card", Alt("enters", "enters the battlefield"), "attached to", Prop(Enchanted)];

    public ThatCard Enchanted { get; set; }
}
