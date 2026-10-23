namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A sentence defining the variable X used in the ability before it: "x is the number of cards in an opponent's hand", "x is the mana value of the exiled card". The amount is a game-state <see cref="Amount"/>.</summary>
/// <exampledoc>Bargaining Table</exampledoc>
/// <examplecapture>x is the number of cards in an opponent's hand</examplecapture>
public class VariableDefinition : Glyph
{
    public override Nib[] Nibs => ["x is", Prop(Definition)];

    public Amount Definition { get; set; }
}
