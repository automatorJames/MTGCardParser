namespace MTGRulesIR.Model;

/// <summary>
/// The vocabularies the lowering defines itself, for words the grammar doesn't carry as a vocabulary member: a
/// <see cref="Qualifier"/> or <see cref="Term"/> names one of these (or a grammar vocabulary) as its vocabulary, and
/// the census checks that every value is a member. The IR's leaf vocabulary is the grammar's enums plus these.
/// </summary>
public static class Vocabularies
{
    /// <summary>
    /// String slots whose values all belong to one vocabulary, by <c>Record.Property</c>: what the census checks them
    /// against. A slot missing here either holds free text or is a vocabulary still to be declared.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Type> Slots = new Dictionary<string, Type>
    {
        ["TokenSpec.Types"] = typeof(MTGGlyphs.GlyphDefinitions.CardType),
        ["TokenSpec.Colors"] = typeof(MTGGlyphs.GlyphDefinitions.ManaColor),
        ["Animate.Types"] = typeof(MTGGlyphs.GlyphDefinitions.CardType),
        ["Animate.Colors"] = typeof(MTGGlyphs.GlyphDefinitions.ManaColor),
        ["Devotion.Colors"] = typeof(MTGGlyphs.GlyphDefinitions.ManaColor),
        ["KeywordInstance.Keyword"] = typeof(MTGGlyphs.GlyphDefinitions.Keyword),
        ["HasKeywords.Keywords"] = typeof(MTGGlyphs.GlyphDefinitions.Keyword),
        ["ZoneRef.Zone"] = typeof(Zone),
    };

    /// <summary>The enum named <paramref name="vocabulary"/>, among the grammar's and the IR's; null when there is none.</summary>
    public static Type Find(string vocabulary) =>
        vocabulary is null ? null : _byName.GetValueOrDefault(vocabulary);

    static readonly IReadOnlyDictionary<string, Type> _byName =
        typeof(MTGGlyphs.GlyphDefinitions.CardType).Assembly.GetTypes().Concat(typeof(Vocabularies).Assembly.GetTypes())
            .Where(x => x.IsEnum)
            .GroupBy(x => x.Name)
            .ToDictionary(x => x.Key, x => x.First());
}

/// <summary>Where in an ordered zone: "the top card of your library".</summary>
public enum Position { Top, Bottom }

/// <summary>How objects came to be the ones meant: found by a search, left over from a choice, acted on earlier.</summary>
public enum Provenance { Found, TheRest }

/// <summary>A qualifier setting objects apart from others on the line: "another", "named {this}", "or tied".</summary>
public enum Distinction { Other, NamedThis, OrTied }

/// <summary>How a choice is made: "at random".</summary>
public enum Manner { AtRandom }

/// <summary>Damage the objects were dealt: "dealt damage this turn", "dealt damage this way".</summary>
public enum DamageHistory { DealtDamageThisTurn, DealtDamageThisWay }

/// <summary>The game's zones.</summary>
public enum Zone { Battlefield, Hand, Library, Graveyard, Exile, Stack, Command }
