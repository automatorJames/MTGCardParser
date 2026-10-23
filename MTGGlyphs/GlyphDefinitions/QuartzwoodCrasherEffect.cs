namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quartzwood Crasher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quartzwood Crasher</exampledoc>
/// <examplecapture>create an x/x green dinosaur beast creature token with trample, where x is the amount of damage those creatures dealt to that player</examplecapture>
[Dependent]
public class QuartzwoodCrasherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create an x/x green dinosaur beast creature token with trample, where x is the amount of damage those creatures dealt to that player"];
}
