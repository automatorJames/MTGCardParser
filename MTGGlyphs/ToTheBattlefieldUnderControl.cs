namespace MTGGlyphs;

public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => ["(on)?to the battlefield under", Prop(Whose), "control", Prop(AndAttachThisToIt)];

    public Whose Whose { get; set; }

    [RegexPattern("and attach {this} to it")]
    public bool AndAttachThisToIt { get; set; }
}