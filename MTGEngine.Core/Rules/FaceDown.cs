namespace MTGEngine.Core;

/// <summary>Turning a face-down permanent face up by paying its morph, megamorph or disguise cost: a special action, any time its controller has priority.</summary>
public sealed record TurnFaceUp(ObjectId Permanent) : LegalAction { public override string ToString() => $"turn {Permanent} face up"; }

public sealed partial class Game
{
    /// <summary>The alternative index that marks casting a card face down for {3}.</summary>
    const int FaceDownCast = -3;

    static readonly ImmutableHashSet<string> _faceDownKeywords = ["Morph", "Megamorph", "Disguise"];

    /// <summary>The cost to turn a card face up, if it has morph, megamorph or disguise.</summary>
    static KeywordInstance FaceUpKeyword(CardDefinition card) =>
        card.Abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).FirstOrDefault(x => _faceDownKeywords.Contains(CardDefinition.Normalize(x.Keyword)) && x.Cost is not null);

    /// <summary>A face-down object's characteristics: a nameless, colorless 2/2 creature with no abilities (disguise keeps ward {2}).</summary>
    public static CardDefinition FaceDownCard(CardDefinition card)
    {
        var disguised = FaceUpKeyword(card) is { } keyword && CardDefinition.Normalize(keyword.Keyword) == "Disguise";
        ImmutableList<Ability> abilities = disguised
            ? KeywordAbilities.Expand([new KeywordLine([new KeywordInstance("Ward", Cost: new PayMana(["{2}"]))])], false).ToImmutableList()
            : [];
        return new CardDefinition("", ManaCost.Free, [], ["Creature"], [], 2, 2, null, abilities, disguised ? ["Ward"] : [], [], card.IsToken);
    }

    /// <summary>Turns a permanent face up: for its cost when it's the special action, free when an effect does it. Megamorph adds a +1/+1 counter.</summary>
    public void TurnUp(ObjectId id, bool pay)
    {
        var obj = State.Object(id);
        if (obj is not { FaceDown: true })
            return;
        var keyword = FaceUpKeyword(obj.Card);
        if (pay)
        {
            if (keyword?.Cost is not { } cost)
                return;
            if (cost is PayMana mana && !Pay(obj.Controller, ManaCost.Parse(string.Concat(mana.Symbols))))
                return;
        }
        Emit(new TurnedFace(id, FaceDown: false));
        if (keyword is not null && CardDefinition.Normalize(keyword.Keyword) == "Megamorph")
            Emit(new CountersChanged(id, "+1/+1", 1));
    }

    /// <summary>The face-down permanents <paramref name="player"/> could turn face up now.</summary>
    IEnumerable<LegalAction> FaceUpActions(PlayerId player) =>
        State.Battlefield
            .Where(x => x.Controller == player && x.FaceDown && FaceUpKeyword(x.Card)?.Cost is PayMana mana && ManaPayment.CanPay(State, player, ManaCost.Parse(string.Concat(mana.Symbols))))
            .Select(x => new TurnFaceUp(x.Id));
}
