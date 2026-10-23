namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Icingdeath, Frost Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Icingdeath, Frost Tyrant</exampledoc>
/// <examplecapture>create icingdeath, frost tongue, a legendary white equipment artifact token with "equipped creature gets +2/+0," "whenever equipped creature attacks, tap target creature defending player controls," and equip {2}</examplecapture>
[Dependent]
public class IcingdeathFrostTyrantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create icingdeath, frost tongue, a legendary white equipment artifact token with \"equipped creature gets +2/+0,\" \"whenever equipped creature attacks, tap target creature defending player controls,\" and equip {2}"];
}
