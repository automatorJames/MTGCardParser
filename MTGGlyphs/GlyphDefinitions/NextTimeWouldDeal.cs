namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A one-shot replacement on the next damage a source would deal: "the next time a red source of your choice would deal damage to you this turn, prevent that damage", "the next time target creature would deal damage this turn, prevent that damage", "the next time this creature would deal combat damage to an opponent this turn, it deals that damage to target creature instead". The source, whether only combat damage, what it would be dealt to (if limited), and the replacement, resolved as any effect.</summary>
/// <exampledoc>Circle of Protection: Red</exampledoc>
/// <examplecapture>the next time a red source of your choice would deal damage to you this turn, prevent that damage</examplecapture>
public class NextTimeWouldDeal : Glyph
{
    public override Nib[] Nibs => ["the next time", Prop(Source), "would deal", Prop(Kind), "damage", Prop(Protected), Alt("this turn,", "this combat,"), Prop(Replacement)];

    public OneOf<ChosenSource, IndefinitePermanent, PermanentPhrase> Source { get; set; }
    [Optional]
    public DamageKind? Kind { get; set; }
    [Optional]
    public ProtectedRecipient Protected { get; set; }
    public DynamicGlyph Replacement { get; set; }
}
