namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Companion's reminder: "if this card is your chosen companion, you may put it into your hand from outside the game for {3} as a sorcery".</summary>
/// <exampledoc>Gyruda, Doom of Depths</exampledoc>
/// <examplecapture>if this card is your chosen companion, you may put it into your hand from outside the game for {3} as a sorcery</examplecapture>
public class CompanionRule : Glyph
{
    public override Nib[] Nibs => ["if this card is your chosen companion, you may put it into your hand from outside the game for", Prop(Cost), "as a sorcery"];

    public ManaCost Cost { get; set; }
}
