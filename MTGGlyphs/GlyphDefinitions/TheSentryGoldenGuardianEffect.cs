namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Sentry, Golden Guardian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Sentry, Golden Guardian</exampledoc>
/// <examplecapture>target opponent creates the void, a legendary 5/5 black horror villain creature token with flying, indestructible, and "the void attacks each combat if able</examplecapture>
[Dependent]
public class TheSentryGoldenGuardianEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent creates the void, a legendary 5/5 black horror villain creature token with flying, indestructible, and \"the void attacks each combat if able"];
}
