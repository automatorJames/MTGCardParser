namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Menace's reminder text, after the keyword ("this creature …") or after an ability granting it ("it …").</summary>
/// <exampledoc>Goblin Heelcutter</exampledoc>
/// <examplecapture>this creature can't be blocked except by two or more creatures</examplecapture>
public class MenaceRule : Glyph
{
    public override Nib[] Nibs => [Alt("this creature", "it"), "can't be blocked except by two or more creatures"];
}
