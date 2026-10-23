namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rush of Vitality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rush of Vitality</exampledoc>
/// <examplecapture>damage dealt by that creature also causes its controller to gain that much life, and it can't be destroyed by damage or effects that say "destroy</examplecapture>
[Dependent]
public class RushOfVitalityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage dealt by that creature also causes its controller to gain that much life, and it can't be destroyed by damage or effects that say \"destroy"];
}
