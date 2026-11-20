using Dapper;
using Microsoft.Data.SqlClient;

namespace MTGGlyphs.Data;

/// <summary>The card database's cards, as a corpus whose volumes are its sets, numbered by set sequence.</summary>
public class CardDataGetter : IDocumentRepository
{
    readonly string _sqlConnString;
    readonly bool _includeEmptyDocuments;
    readonly Lazy<int[]> _volumes;
    readonly Lazy<IReadOnlyDictionary<int, string>> _volumeNames;

    public CardDataGetter(GlobalSettings settings)
    {
        _sqlConnString = settings.SqlConnString;
        _includeEmptyDocuments = settings.IncludeEmptyDocuments;
        _volumes = new(ReadVolumes, LazyThreadSafetyMode.PublicationOnly);
        _volumeNames = new(ReadVolumeNames, LazyThreadSafetyMode.PublicationOnly);
    }

    public Task<List<IDocument>> GetDocumentsAsync() =>
        QueryAsync(null, null);

    public Task<List<IDocument>> GetDocumentsAsync(int minVolume, int maxVolume) =>
        QueryAsync("SetSequence between @MinVolume and @MaxVolume", new { MinVolume = minVolume, MaxVolume = maxVolume });

    public Task<List<IDocument>> GetDocumentsAsync(params int[] volumes) =>
        QueryAsync("SetSequence in @Volumes", new { Volumes = volumes });

    /// <summary>The set sequences of the sets with cards in them - read once, since sets don't change while the app runs.</summary>
    public int[] GetVolumes() => _volumes.Value;

    /// <summary>The names of the sets with cards in them, by set sequence - read once, as <see cref="GetVolumes"/> is.</summary>
    public IReadOnlyDictionary<int, string> GetVolumeNames() => _volumeNames.Value;

    IReadOnlyDictionary<int, string> ReadVolumeNames()
    {
        using var conn = new SqlConnection(_sqlConnString);

        return conn.Query<(int Volume, string Name)>("select distinct c.SetSequence, s.name from Card c join [Set] s on s.code = c.SetCode" + Where(null))
            .ToDictionary(x => x.Volume, x => x.Name);
    }

    async Task<List<IDocument>> QueryAsync(string volumeCondition, object parameters)
    {
        using var conn = new SqlConnection(_sqlConnString);
        var result = (await conn.QueryAsync<Card>("select * from Card" + Where(volumeCondition), parameters)).ToList();
        AttachBackFaces(conn, result);

        return result.Cast<IDocument>().ToList();
    }

    /// <summary>
    /// A two-faced card's back face, from the printings table (<c>CardAll</c>, every printing of every face under the
    /// card's full "Front // Back" name): the face whose characteristics aren't the front's, with the text of its most
    /// recent printing. Cards whose faces can't be told apart that way (both faces alike, or more than two) get none.
    /// </summary>
    static void AttachBackFaces(SqlConnection conn, List<Card> cards)
    {
        var twoFaced = cards.Where(x => x.Name?.Contains(" // ") == true).ToList();
        if (twoFaced.Count == 0)
            return;
        var printings = conn.Query<Printing>("select Name, Text, ManaCost, Types, Supertypes, Subtypes, Keywords, Power, Toughness, Loyalty, Colors, MultiverseId from CardAll where Name like '% // %'")
            .ToLookup(x => x.Name);

        foreach (var card in twoFaced)
        {
            var names = card.Name.Split(" // ");
            if (names.Length != 2 || names[0] == names[1])
                continue;
            var front = Printing.KeyOf(card.ManaCost, card.Types, card.Supertypes, card.Subtypes, card.Power, card.Toughness, card.Loyalty);
            var others = printings[card.Name].Where(x => x.Key != front).GroupBy(x => x.Key).ToList();
            if (others.Count != 1)
                continue;
            // The newest printing, and among imports of one printing the fuller keyword list (the later import) and the shorter text (today's wording).
            var back = others[0].OrderByDescending(x => x.MultiverseId ?? -1).ThenByDescending(x => x.Keywords?.Length ?? 0).ThenBy(x => x.Text?.Length ?? 0).First();
            card.BackFace = new Card
            {
                CardId = card.CardId,
                Name = names[1],
                Text = back.Text,
                ManaCost = back.ManaCost,
                Types = back.Types,
                Supertypes = back.Supertypes,
                Subtypes = back.Subtypes,
                Keywords = back.Keywords,
                Power = back.Power,
                Toughness = back.Toughness,
                Loyalty = back.Loyalty,
                Colors = back.Colors,
                SetCode = card.SetCode,
                SetSequence = card.SetSequence,
            };
        }
    }

    /// <summary>One printing of one face, as the printings table has it.</summary>
    class Printing
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string ManaCost { get; set; }
        public string Types { get; set; }
        public string Supertypes { get; set; }
        public string Subtypes { get; set; }
        public string Keywords { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string Loyalty { get; set; }
        public string Colors { get; set; }
        public int? MultiverseId { get; set; }

        /// <summary>The characteristics that tell one face from the other.</summary>
        public string Key => KeyOf(ManaCost, Types, Supertypes, Subtypes, Power, Toughness, Loyalty);

        public static string KeyOf(params string[] parts) => string.Join("|", parts.Select(x => x ?? ""));
    }

    int[] ReadVolumes()
    {
        using var conn = new SqlConnection(_sqlConnString);

        return conn.Query<int>("select distinct SetSequence from Card" + Where(null) + " order by SetSequence").ToArray();
    }

    string Where(string volumeCondition)
    {
        var conditions = new List<string>();

        if (volumeCondition is not null)
            conditions.Add(volumeCondition);

        if (!_includeEmptyDocuments)
            conditions.Add("Text is not null");

        return conditions.Count > 0
            ? " where " + string.Join(" and ", conditions)
            : string.Empty;
    }
}
