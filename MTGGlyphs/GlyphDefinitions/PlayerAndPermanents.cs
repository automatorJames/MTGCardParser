namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you and permanents you control", "you and/or other creatures you control": a player together with a group of permanents, as protected by prevention and redirection effects.</summary>
/// <exampledoc>Endure</exampledoc>
/// <examplecapture>you and permanents you control</examplecapture>
[Dependent]
public class PlayerAndPermanents : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Alt("and", "and/or"), Prop(Group)];

    public WhichPlayer Player { get; set; }
    public OneOf<OtherPermanents, Permanents> Group { get; set; }
}
