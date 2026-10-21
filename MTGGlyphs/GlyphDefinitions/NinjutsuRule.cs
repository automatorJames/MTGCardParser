namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Ninjutsu's reminder text: "{1}{u}, return an unblocked attacker you control to hand: put this card onto the battlefield from your hand tapped and attacking". The cost repeats the keyword's.</summary>
/// <exampledoc>Ninja of the Deep Hours</exampledoc>
/// <examplecapture>{1}{u}, return an unblocked attacker you control to hand: put this card onto the battlefield from your hand tapped and attacking</examplecapture>
[Dependent]
public class NinjutsuRule : Glyph
{
    public override Nib[] Nibs => [Prop(Cost), ", return an unblocked attacker you control to hand: put this card onto the battlefield from your hand tapped and attacking"];

    public ManaCost Cost { get; set; }
}
