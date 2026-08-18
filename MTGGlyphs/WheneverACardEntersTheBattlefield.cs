namespace MTGGlyphs;

[IsolateForTesting]
public class WheneverACardEntersTheBattlefield : Glyph
{
    public override Nib[] Nibs => ["whenever a", Prop(CardOrCreatureType), "enters the battlefield,", Prop(Result)];

    public OneOf<CardType?, CreatureType?> CardOrCreatureType { get; set; }
    public DynamicGlyph Result { get; set; }
}
