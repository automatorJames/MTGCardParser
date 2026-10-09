namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The aura resolution rule, from enchant's reminder text: "this card enters the battlefield attached to that creature".</summary>
/// <exampledoc>Fear</exampledoc>
/// <examplecapture>this card enters the battlefield attached to that creature</examplecapture>
public class AuraEntersAttached : Glyph
{
    public override Nib[] Nibs => ["this card enters the battlefield attached to", Prop(Enchanted)];

    public ThatCard Enchanted { get; set; }
}
