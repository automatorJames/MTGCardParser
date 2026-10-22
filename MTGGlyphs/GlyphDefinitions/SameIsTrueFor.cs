namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Extending the previous sentence to more things: other keywords ("the same is true for fear, first strike, double strike, …") or spells and cards off the battlefield ("the same is true for creature spells you control and creature cards you own that aren't on the battlefield").</summary>
/// <exampledoc>Odric, Lunarch Marshal</exampledoc>
/// <examplecapture>the same is true for flying, deathtouch, double strike, haste, hexproof, indestructible, lifelink, menace, reach, skulk, trample, and vigilance</examplecapture>
public class SameIsTrueFor : Glyph
{
    public override Nib[] Nibs => ["the same is true for", Prop(Extended)];

    public OneOf<ManyOf<Keyword>, SpellsAndCardsOffBattlefield> Extended { get; set; }
}
