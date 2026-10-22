namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect that happens unless a player pays first, the payment leading: "unless any player pays {2}, search your library for a card, …", "unless target player pays {3}, that player loses 5 life and you gain 5 life", "at the beginning of your upkeep, unless you pay {b}{b}{b}, tap this creature …".</summary>
/// <exampledoc>Rhystic Tutor</exampledoc>
/// <examplecapture>unless any player pays {2}, search your library for a card, put that card into your hand, then shuffle</examplecapture>
public class UnlessPaidFirst : Glyph
{
    public override Nib[] Nibs => [Prop(Payment), ",", Prop(Effect)];

    public OptionalPayCost Payment { get; set; }
    public DynamicGlyph Effect { get; set; }
}
