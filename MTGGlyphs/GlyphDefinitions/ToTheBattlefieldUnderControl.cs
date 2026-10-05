namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => [Alt("onto", "to"), "the battlefield under", Prop(Whose), "control", Prop(AndAttachThisToIt)];

    public Whose Whose { get; set; }

    [RegexPattern("and attach {this} to it")]
    public bool AndAttachThisToIt { get; set; }
}
