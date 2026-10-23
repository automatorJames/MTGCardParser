namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Relic Robber, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Relic Robber</exampledoc>
/// <examplecapture>that player creates a 0/1 colorless goblin construct artifact creature token with "this token can't block" and "at the beginning of your upkeep, this token deals 1 damage to you</examplecapture>
[Dependent]
public class RelicRobberEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player creates a 0/1 colorless goblin construct artifact creature token with \"this token can't block\" and \"at the beginning of your upkeep, this token deals 1 damage to you"];
}
