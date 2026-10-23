namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaza, Roil Chaser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaza, Roil Chaser</exampledoc>
/// <examplecapture>the next instant or sorcery spell you cast this turn costs {x} less to cast, where x is the number of wizards you control as this ability resolves</examplecapture>
[Dependent]
public class KazaRoilChaserEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next instant or sorcery spell you cast this turn costs {x} less to cast, where x is the number of wizards you control as this ability resolves"];
}
