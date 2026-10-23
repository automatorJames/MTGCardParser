namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exchanging a player's life total with a creature's power or toughness: "exchange your life total with this creature's toughness", "exchange target opponent's life total with this creature's toughness".</summary>
/// <exampledoc>Tree of Redemption</exampledoc>
/// <examplecapture>exchange your life total with this creature's toughness</examplecapture>
public class ExchangeLifeTotalWithStat : Glyph, IPredicate
{
    public override Nib[] Nibs => ["exchange", Prop(Player), "life total with", Prop(Stat)];

    public Owner Player { get; set; }
    public CharacteristicOf Stat { get; set; }
}
