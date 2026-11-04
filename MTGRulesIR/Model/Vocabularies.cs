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
    public static readonly IReadOnlyDictionary<string, Type[]> Slots = new Dictionary<string, Type[]>
    {
        ["TokenSpec.Types"] = [typeof(G.CardType)],
        ["TokenSpec.Colors"] = [typeof(G.ManaColor)],
        ["TokenSpec.Subtypes"] = [typeof(G.CreatureType), typeof(G.LandType), typeof(G.PermanentSubtype), typeof(G.PlaneswalkerType)],
        ["TokenSpec.Predefined"] = [typeof(G.PredefinedToken), typeof(G.RoleKind)],
        ["Animate.Types"] = [typeof(G.CardType)],
        ["Animate.Colors"] = [typeof(G.ManaColor)],
        ["Animate.Subtypes"] = [typeof(G.CreatureType), typeof(G.LandType), typeof(G.PermanentSubtype), typeof(G.PlaneswalkerType)],
        ["Animate.Still"] = [typeof(G.CardType)],
        ["StillOfKind.Kinds"] = [typeof(G.CardType), typeof(G.CreatureType)],
        ["Devotion.Colors"] = [typeof(G.ManaColor)],
        ["KeywordInstance.Keyword"] = [typeof(G.Keyword)],
        ["HasKeywords.Keywords"] = [typeof(G.Keyword)],
        ["CanAsThough.Keyword"] = [typeof(G.Keyword)],
        ["CanAsThough.Action"] = [typeof(G.CombatAction)],
        ["Timing.Keyword"] = [typeof(G.Keyword)],
        ["Timing.Kind"] = [typeof(TimingKind)],
        ["ZoneRef.Zone"] = [typeof(Zone)],
        ["ZoneRef.Position"] = [typeof(Position)],
        ["ZoneRef.Order"] = [typeof(G.LibraryOrder)],
        ["ZoneRef.State"] = [typeof(G.ExileStatus)],
        ["LabeledAbility.Word"] = [typeof(G.AbilityWord)],
        ["ObjectEvent.Events"] = [typeof(G.PermanentEvent), typeof(ObjectEventKind), typeof(G.PastPermanentEvent), typeof(G.DepartureEvent), typeof(G.ObjectTurnEvent), typeof(G.EnteringWay), typeof(G.AsPermanentEvent), typeof(G.ThisWayAction), typeof(G.CardZoneEvent)],
        ["ObjectEvent.Only"] = [typeof(G.EventOccurrence), typeof(G.TurnOwner), typeof(G.TurnSpan), typeof(EventSpan)],
        ["ObjectEvent.By"] = [typeof(G.TargetingSource)],
        ["PlayerEvent.Event"] = [typeof(G.PlayerEventKind)],
        ["PlayerEvent.Timing"] = [typeof(G.PlayerEventTiming)],
        ["NamedEvent.Kind"] = [typeof(G.TurnEvent), typeof(EventKind)],
        ["PlayerAction.Action"] = [typeof(G.PastPlayerAction), typeof(G.CardVerb), typeof(G.RevealOrExile), typeof(G.PlayerThisWayAction)],
        ["CombatEncounterEvent.Encounter"] = [typeof(G.CombatEncounterKind)],
        ["Happened.Span"] = [typeof(G.TurnSpan), typeof(EventSpan)],
        ["ThisTurn.Period"] = [typeof(G.TurnPeriod)],
        ["MustCombat.Period"] = [typeof(G.CombatPeriod)],
        ["MustCombat.Actions"] = [typeof(G.CombatAction)],
        ["CombatRestriction.Cant"] = [typeof(G.CombatAction)],
        ["CombatRestriction.Recipient"] = [typeof(G.AttackRecipient)],
        ["CombatRestriction.Modifiers"] = [typeof(CombatModifier)],
        ["InCombatWith.Relation"] = [typeof(CombatRelation)],
        ["ChooseEffect.Kinds"] = [typeof(G.Choice)],
        ["ChooseEffect.Except"] = [typeof(G.CardType), typeof(G.CreatureType)],
        ["ChooseModes.Count"] = [typeof(G.ModeCount)],
        ["PhaseRef.Phase"] = [typeof(G.Phase)],
        ["PhaseRef.Part"] = [typeof(G.PhasePart)],
        ["Special.Kind"] = [typeof(G.Quantity), typeof(SpecialAmount)],
        ["CantBeTargeted.By"] = [typeof(TargetingBy)],
        ["ManaChoice.Choice"] = [typeof(G.ManaChoice), typeof(ManaChoiceKind)],
        ["AddMana.Amount"] = [typeof(G.AddedManaAmount)],
        ["ManaRestriction.Rule"] = [typeof(G.ManaSpendingRule)],
        ["OfChosen.Characteristic"] = [typeof(G.ChosenAspect)],
        ["SearchLibrary.Zones"] = [typeof(G.SearchedZones)],
        ["Controlled.Relation"] = [typeof(G.ControlRelation), typeof(Possession)],
        ["Controls.Connective"] = [typeof(G.ConditionConnective)],
        ["Controls.Tense"] = [typeof(G.ControlTense)],
        ["DistinctCount.Aspect"] = [typeof(G.CountedAspect)],
        ["EntersBattlefield.States"] = [typeof(G.EntryState), typeof(G.EnteringWay), typeof(G.PermanentDesignation)],
        ["ChangeZone.States"] = [typeof(ZoneEntryState)],
        ["Exile.Manner"] = [typeof(G.RemovalManner), typeof(G.FaceDirection), typeof(Manner), typeof(ExileManner)],
        ["Destroy.Manner"] = [typeof(G.RemovalManner)],
        ["CharacteristicBound.Characteristics"] = [typeof(G.Characteristic)],
        ["CharacteristicOf.Characteristic"] = [typeof(G.Characteristic)],
        ["SetStats.Which"] = [typeof(G.PowerAndOrToughness)],
        ["CounterSpec.Prefix"] = [typeof(G.CountPrefix)],
        ["CounterSpec.Bound"] = [typeof(G.CountComparison)],
        ["CompareControl.Bound"] = [typeof(G.CountBound)],
        ["Compared.Comparison"] = [typeof(G.MoreOrFewer)],
        ["PlayCard.From"] = [typeof(G.NonBattlefieldZone)],
        ["CastBy.From"] = [typeof(G.CastFrom)],
        ["CastBy.Relation"] = [typeof(G.SpellRelation)],
        ["Targeting.Count"] = [typeof(G.SpellTargetCount)],
        ["WasCastWith.How"] = [typeof(G.OptionalCostVerb)],
        ["OutcomeCondition.Result"] = [typeof(G.OutcomeResult)],
        ["Arithmetic.Operator"] = [typeof(G.ArithmeticOperator)],
        ["Scaled.Rounding"] = [typeof(G.Rounding)],
        ["Scaled.Factor"] = [typeof(G.ScaleFactor)],
        ["Adjusted.Factor"] = [typeof(G.AmountFactor)],
        ["AlternativeCost.Replaces"] = [typeof(G.ReplacedOwnCost)],
        ["HandSize.Verb"] = [typeof(G.HaveVerb)],
        ["HasCountersCondition.Tense"] = [typeof(Possession)],
        ["ObjectIs.Tense"] = [typeof(G.Assertion)],
        ["BecomesAspect.Verb"] = [typeof(G.TypeCopula), typeof(G.Assertion), typeof(G.BecomeVerb)],
        ["Playable.Verb"] = [typeof(G.PlayVerb), typeof(UseVerb)],
        ["RevealUntil.Verb"] = [typeof(G.RevealOrExile)],
        ["PreventDamage.Kind"] = [typeof(G.DamageKind)],
        ["CostModification.Action"] = [typeof(G.CostedAction)],
        ["CostModification.Change"] = [typeof(G.CostChange)],
        ["QualifiedKind.Kind"] = [typeof(G.CardType), typeof(G.CreatureType), typeof(G.PlayerIdentity)],
        ["GameOutcome.Result"] = [typeof(G.GameResult)],
        ["BecomeCopy.Verb"] = [typeof(G.CopyingVerb)],
        ["CanBlock.Count"] = [typeof(G.BlockCount)],
        ["CanBlock.Period"] = [typeof(G.BlockPeriod)],
        ["CantPlay.Verb"] = [typeof(G.PlayVerb)],
        ["DealDamage.Divided"] = [typeof(G.DamageDivision)],
        ["DealDamage.Kind"] = [typeof(G.DamageKind)],
        ["WasAffectedThisWay.Action"] = [typeof(G.ThisWayAction)],
        ["Extreme.Characteristic"] = [typeof(G.Characteristic)],
        ["Extreme.Which"] = [typeof(ExtremeKind)],
        ["Remains.State"] = [typeof(G.RemainingState)],
        ["Skip.Event"] = [typeof(G.SkippedEvent)],
        ["ManaSpent.Color"] = [typeof(G.ManaColor)],
        ["Distinct.Characteristic"] = [typeof(G.Characteristic)],
        ["SharesCharacteristic.Characteristic"] = [typeof(G.Characteristic)],
        ["CharacteristicCompared.Characteristic"] = [typeof(G.Characteristic)],
        ["CharacteristicCompared.OtherCharacteristic"] = [typeof(G.Characteristic)],
    };

    static readonly System.Text.RegularExpressions.Regex ManaSymbol = new(@"^\{(\d+|[wubrgcsxe]|[wubrg2]/[wubrgp]|[wubrg]/[wubrg]/p)\}$");
    static readonly System.Text.RegularExpressions.Regex CounterName = new(@"^([+-]\d+/[+-]\d+|[a-z][a-z' -]*)$");

    /// <summary>
    /// String slots whose values follow a pattern rather than belonging to a finite vocabulary: mana symbols, and the
    /// names of counters, which any card may coin.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, System.Text.RegularExpressions.Regex> Patterns = new Dictionary<string, System.Text.RegularExpressions.Regex>
    {
        ["PayMana.Symbols"] = ManaSymbol,
        ["ManaAmount.Symbols"] = ManaSymbol,
        ["ManaChoice.Among"] = ManaSymbol,
        ["CounterSpec.Kind"] = CounterName,
        ["Qualifier.Quality[CounterKind]"] = CounterName,
    };


    /// <summary>The enum named <paramref name="vocabulary"/>, among the grammar's and the IR's; null when there is none.</summary>
    public static Type Find(string vocabulary) =>
        vocabulary is null ? null : _byName.GetValueOrDefault(vocabulary);

    /// <summary>Every enum of the grammar and the IR: the vocabularies a slot could be closed against.</summary>
    public static IEnumerable<Type> All => _byName.Values;

    static readonly IReadOnlyDictionary<string, Type> _byName =
        typeof(MTGGlyphs.GlyphDefinitions.CardType).Assembly.GetTypes().Concat(typeof(Vocabularies).Assembly.GetTypes())
            .Where(x => x.IsEnum)
            .GroupBy(x => x.Name)
            .ToDictionary(x => x.Key, x => x.First());
}

/// <summary>Where in an ordered zone: "the top card of your library", "second from the top".</summary>
public enum Position { Top, Bottom, TopOrBottom, Second, Third, SecondFromTheTopOrBottom }

/// <summary>Objects an earlier effect on the line acted on: "the exiled card" as "exiled this way".</summary>
public enum AffectedThisWay { ExiledThisWay, RevealedThisWay, DestroyedThisWay, DiscardedThisWay, SacrificedThisWay, TappedThisWay, PutIntoAGraveyardThisWay, ReturnedThisWay, MilledThisWay }

/// <summary>Where an exiled card goes instead of its usual destination: "exile it instead of putting it anywhere else".</summary>
public enum ExileManner { InsteadOfAnywhereElse }

/// <summary>How objects came to be the ones meant: found by a search, left over from a choice, acted on earlier.</summary>
public enum Provenance { Found, TheRest, Chosen }

/// <summary>A qualifier setting objects apart from others on the line: "another", "named {this}", "or tied".</summary>
public enum Distinction { Other, NamedThis, OrTied }

/// <summary>How a choice is made: "at random".</summary>
public enum Manner { AtRandom }

/// <summary>Damage the objects were dealt: "dealt damage this turn", "dealt damage this way".</summary>
public enum DamageHistory { DealtDamageThisTurn, DealtDamageThisWay }

/// <summary>The game's zones.</summary>
public enum Zone { Battlefield, Hand, Library, Graveyard, Exile, Stack, Command }

/// <summary>How two amounts compare.</summary>
public enum Comparison { Equal, Less, LessOrEqual, Greater, GreaterOrEqual }

/// <summary>Kinds of object the lowering names itself, beside the grammar's card types: "card", "spell", "counter", "any target".</summary>
public enum ObjectKind { Card, Spell, Any, Counter, Token, Permanent, Player, CardName, ManaSpent, Source, Ability, Cost, Door }

/// <summary>Events the lowering names beyond the turn's: the last counter being removed.</summary>
public enum EventKind { LastCounterRemoved }

/// <summary>Ways mana may be chosen beyond the grammar's: any combination of colors.</summary>
public enum ManaChoiceKind { AnyCombination }

/// <summary>What mana may be spent to do, beyond playing and casting.</summary>
public enum UseVerb { Activate, PayKeywordCost, TurnFaceUp, UnlockDoor }

/// <summary>A span an event is looked for in beyond the turn: "this way", by an earlier effect on the line.</summary>
public enum EventSpan { ThisWay, ThisCombat, LastTurn }

/// <summary>Which end of a range: the greatest or the least.</summary>
public enum ExtremeKind { Greatest, Least }

/// <summary>Amounts named rather than counted, beyond the grammar's quantity words: "those" cards named earlier.</summary>
public enum SpecialAmount { Those, ThatMuch }

/// <summary>When an activated ability may be activated.</summary>
public enum TimingKind { SorcerySpeed, InstantSpeed, OnceEachTurn, Once, During, Before, If }

/// <summary>Events of an object the lowering names beyond the grammar's permanent events: being cast, becoming a target.</summary>
public enum ObjectEventKind { Cast, BecomesTarget, EntersOrTransforms }

/// <summary>What something can't be the target of, or can't be done to it: countered, copied, targeted by spells or abilities.</summary>
public enum TargetingBy { Countered, Copied, SpellsOrAbilities, Spells, Abilities }

/// <summary>Modifiers of a combat restriction: "except by …", "alone".</summary>
public enum CombatModifier { Except, Alone }

/// <summary>How creatures stand to one another in combat.</summary>
public enum CombatRelation { BlockingOrBlockedBy, Blocking, BlockedBy, Attacking }

/// <summary>States a permanent may enter the battlefield in beyond tapped: transformed, attacking, face down.</summary>
public enum ZoneEntryState { Transformed, Attacking, FaceDown }

/// <summary>Having something, now or earlier: "has", "had".</summary>
public enum Possession { Has, Had }

/// <summary>Two or more joined: "and", "or", "and/or".</summary>
public enum Connective { And, Or, AndOr }

/// <summary>A characteristic a player chooses: "a color of your choice", "the creature type of your choice".</summary>
public enum ChosenCharacteristic { ColorOfYourChoice, BasicLandTypeOfYourChoice, CreatureTypeOfYourChoice, ColorOrColorsOfYourChoice }

/// <summary>Words about a card that are neither type nor color: "nonbasic", "token".</summary>
public enum CardAspectWord { Nonbasic, Token }
