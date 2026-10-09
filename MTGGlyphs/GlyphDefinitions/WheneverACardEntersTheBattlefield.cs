namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on any permanent of a type entering: "whenever a land enters the battlefield, …".</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>whenever a land enters the battlefield, {this} deals 2 damage to that land's controller</examplecapture>
public class WheneverACardEntersTheBattlefield : Glyph
{
    public override Nib[] Nibs => ["whenever a", Prop(Kind), "enters the battlefield,", Prop(Result)];

    public PermanentKind Kind { get; set; }
    public DynamicGlyph Result { get; set; }
}