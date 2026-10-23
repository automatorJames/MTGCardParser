/// <summary>Card-specific rules text of Wolf Strike, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wolf Strike</exampledoc>
[TokenizationOrder(-1)]
public class WolfStrikeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature you control gets +2/+0 until end of turn if it's night"];
}

/// <summary>Card-specific rules text of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[TokenizationOrder(-1)]
public class WonderManHollywoodHeroRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each power-up ability of permanents you control can be activated an additional time"];
}

/// <summary>Card-specific rules text of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[TokenizationOrder(-1)]
public class WonderManHollywoodHero2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once? reduce the cost by his mana cost if he entered this turn"];
}

/// <summary>Card-specific rules text of Word of Blasting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Blasting</exampledoc>
[TokenizationOrder(-1)]
public class WordOfBlastingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to that wall's mana value to the wall's controller"];
}

/// <summary>Card-specific effect of World at War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>World at War</exampledoc>
[Dependent]
public class WorldAtWarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures that attacked this turn"];
}

/// <summary>Card-specific rules text of Xu-Ifit, Osteoharmonist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Xu-Ifit, Osteoharmonist</exampledoc>
[TokenizationOrder(-1)]
public class XuIfitOsteoharmonistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's a skeleton in addition to its other types and has no abilities"];
}

/// <summary>Card-specific effect of Yidaro, Wandering Monster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yidaro, Wandering Monster</exampledoc>
[Dependent]
public class YidaroWanderingMonsterCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle it into your library from your graveyard"];
}

/// <summary>Card-specific rules text of Zirda, the Dawnwaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zirda, the Dawnwaker</exampledoc>
[TokenizationOrder(-1)]
public class ZirdaTheDawnwakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each permanent card in your starting deck has an activated ability"];
}

/// <summary>Card-specific effect of Aang, Swift Savior // Aang and La, Ocean's Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aang, Swift Savior // Aang and La, Ocean's Fury</exampledoc>
[Dependent]
public class AangSwiftSaviorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["airbend up to one other target creature or spell"];
}

/// <summary>Card-specific rules text of Abandoned Sarcophagus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Abandoned Sarcophagus</exampledoc>
[TokenizationOrder(-1)]
public class AbandonedSarcophagusCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast spells that have a cycling ability from your graveyard"];
}

/// <summary>Card-specific rules text of Academic Probation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Academic Probation</exampledoc>
[TokenizationOrder(-1)]
public class AcademicProbationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["opponents can't cast spells with the chosen name until your next turn"];
}

/// <summary>Card-specific effect of Acidic Dagger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Acidic Dagger</exampledoc>
[Dependent]
public class AcidicDaggerCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this artifact"];
}

/// <summary>Card-specific rules text of Alchemist's Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alchemist's Gambit</exampledoc>
[TokenizationOrder(-1)]
public class AlchemistsGambitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["[at the beginning of that turn's end step, you lose the game"];
}

/// <summary>Card-specific effect of Alluring Siren, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alluring Siren</exampledoc>
[Dependent]
public class AlluringSirenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature an opponent controls attacks you this turn if able"];
}

/// <summary>Card-specific rules text of Ambush Commander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ambush Commander</exampledoc>
[TokenizationOrder(-1)]
public class AmbushCommanderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["forests you control are 1/1 green elf creatures that are still lands"];
}

/// <summary>Card-specific rules text of Angelic Arbiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angelic Arbiter</exampledoc>
[TokenizationOrder(-1)]
public class AngelicArbiterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who cast a spell this turn can't attack with creatures"];
}

/// <summary>Card-specific effect of Angelic Skirmisher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angelic Skirmisher</exampledoc>
[Dependent]
public class AngelicSkirmisherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose first strike, vigilance, or lifelink"];
}

/// <summary>Card-specific rules text of Angry Mob, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angry Mob</exampledoc>
[TokenizationOrder(-1)]
public class AngryMobRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during turns other than yours,", Nib.This, "'s power and toughness are each 2"];
}

/// <summary>Card-specific effect of Ant-Man's Army, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ant-Man's Army</exampledoc>
[Dependent]
public class AntMansArmyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a food token or a treasure token"];
}

/// <summary>Card-specific rules text of Backdraft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Backdraft</exampledoc>
[TokenizationOrder(-1)]
public class BackdraftCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a player who cast one or more sorcery spells this turn"];
}

/// <summary>Card-specific rules text of Bladehold War-Whip, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bladehold War-Whip</exampledoc>
[TokenizationOrder(-1)]
public class BladeholdWarWhipRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equip abilities you activate of other equipment cost {1} less to activate"];
}

/// <summary>Card-specific rules text of Blossombind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blossombind</exampledoc>
[TokenizationOrder(-1)]
public class BlossombindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature can't become untapped and can't have counters put on it"];
}

/// <summary>Card-specific rules text of Brilliant Ultimatum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brilliant Ultimatum</exampledoc>
[TokenizationOrder(-1)]
public class BrilliantUltimatumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play lands and cast spells from one of those piles"];
}

/// <summary>Card-specific rules text of Captive Audience, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Captive Audience</exampledoc>
[TokenizationOrder(-1)]
public class CaptiveAudienceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment enters under the control of an opponent of your choice"];
}

/// <summary>Card-specific rules text of Celestial Dawn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Celestial Dawn</exampledoc>
[TokenizationOrder(-1)]
public class CelestialDawnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may spend other mana only as though it were colorless mana"];
}

/// <summary>Card-specific effect of Champion of Stray Souls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Champion of Stray Souls</exampledoc>
[Dependent]
public class ChampionOfStraySoulsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your graveyard on top of your library"];
}

/// <summary>Card-specific effect of Charging War Boar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Charging War Boar</exampledoc>
[Dependent]
public class ChargingWarBoarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can deal excess damage to the player or planeswalker it's attacking"];
}

/// <summary>Card-specific rules text of Charmed Pendant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Charmed Pendant</exampledoc>
[TokenizationOrder(-1)]
public class CharmedPendantCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if the card's mana cost is {3}{u}{u}{b}, you add {u}{u}{b}"];
}

/// <summary>Card-specific rules text of Circling Vultures, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Circling Vultures</exampledoc>
[TokenizationOrder(-1)]
public class CirclingVulturesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may discard this card any time you could cast an instant"];
}

/// <summary>Card-specific rules text of Common Cause, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Common Cause</exampledoc>
[TokenizationOrder(-1)]
public class CommonCauseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nonartifact creatures get +2/+2 as long as they all share a color"];
}

/// <summary>Card-specific rules text of Consuming Tide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Consuming Tide</exampledoc>
[TokenizationOrder(-1)]
public class ConsumingTideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all nonland permanents not chosen this way to their owners' hands"];
}

/// <summary>Card-specific rules text of Coveted Falcon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coveted Falcon</exampledoc>
[TokenizationOrder(-1)]
public class CovetedFalconCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each one they gained control of this way"];
}

/// <summary>Card-specific effect of Crazed Firecat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crazed Firecat</exampledoc>
[Dependent]
public class CrazedFirecatEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip a coin until you lose a flip"];
}

/// <summary>Card-specific rules text of Crazed Firecat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crazed Firecat</exampledoc>
[TokenizationOrder(-1)]
public class CrazedFirecat2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on this creature for each flip you won"];
}

/// <summary>Card-specific effect of Critical Hit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Critical Hit</exampledoc>
[Dependent]
public class CriticalHitCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a natural 20 is a roll that displays 20 on the die"];
}

/// <summary>Card-specific rules text of Defense Grid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Defense Grid</exampledoc>
[TokenizationOrder(-1)]
public class DefenseGridRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each spell costs {3} more to cast except during its controller's turn"];
}

/// <summary>Card-specific rules text of Demonlord Belzenlok, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonlord Belzenlok</exampledoc>
[TokenizationOrder(-1)]
public class DemonlordBelzenlokCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the card's mana value is 4 or greater, repeat this process"];
}

/// <summary>Card-specific rules text of Detention Vortex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Detention Vortex</exampledoc>
[TokenizationOrder(-1)]
public class DetentionVortexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only your opponents may activate this ability and only as a sorcery"];
}

/// <summary>Card-specific rules text of Domri, Chaos Bringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Domri, Chaos Bringer</exampledoc>
[TokenizationOrder(-1)]
public class DomriChaosBringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on a creature spell, it gains riot"];
}

/// <summary>Card-specific rules text of Dream Devourer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Devourer</exampledoc>
[TokenizationOrder(-1)]
public class DreamDevourerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its foretell cost is equal to its mana cost reduced by {2}"];
}

/// <summary>Card-specific rules text of Drill Too Deep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drill Too Deep</exampledoc>
[TokenizationOrder(-1)]
public class DrillTooDeepRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• put five charge counters on target spacecraft or planet you control"];
}

/// <summary>Card-specific effect of Dwarven Sea Clan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwarven Sea Clan</exampledoc>
[Dependent]
public class DwarvenSeaClanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target attacking or blocking creature whose controller controls an island"];
}

/// <summary>Card-specific rules text of Elesh Norn, Mother of Machines, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elesh Norn, Mother of Machines</exampledoc>
[TokenizationOrder(-1)]
public class EleshNornMotherOfMachinesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanents entering don't cause abilities of permanents your opponents control to trigger"];
}

/// <summary>Card-specific rules text of Emrakul, the Aeons Torn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emrakul, the Aeons Torn</exampledoc>
[TokenizationOrder(-1)]
public class EmrakulTheAeonsTornRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flying, protection from spells that are one or more colors, annihilator 6"];
}

/// <summary>Card-specific rules text of Enduring Angel // Angelic Enforcer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enduring Angel // Angelic Enforcer</exampledoc>
[TokenizationOrder(-1)]
public class EnduringAngelCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if this creature didn't transform this way, you lose the game"];
}

/// <summary>Card-specific rules text of Energybending, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Energybending</exampledoc>
[TokenizationOrder(-1)]
public class EnergybendingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lands you control gain all basic land types until end of turn"];
}

/// <summary>Card-specific rules text of Enhanced Surveillance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enhanced Surveillance</exampledoc>
[TokenizationOrder(-1)]
public class EnhancedSurveillanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at an additional two cards each time you surveil"];
}

/// <summary>Card-specific effect of Ertai's Familiar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Familiar</exampledoc>
[Dependent]
public class ErtaisFamiliarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mill three cards"];
}

/// <summary>Card-specific effect of Esoteric Duplicator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Esoteric Duplicator</exampledoc>
[Dependent]
public class EsotericDuplicatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {2}"];
}

/// <summary>Card-specific rules text of Farideh, Devil's Chosen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Farideh, Devil's Chosen</exampledoc>
[TokenizationOrder(-1)]
public class FaridehDevilsChosenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of those results was 10 or higher, draw a card"];
}

/// <summary>Card-specific rules text of Fear, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fear</exampledoc>
[TokenizationOrder(-1)]
public class FearRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has", Nib.This];
}

/// <summary>Card-specific rules text of Fight or Flight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fight or Flight</exampledoc>
[TokenizationOrder(-1)]
public class FightOrFlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only creatures in the pile of their choice can attack this turn"];
}

/// <summary>Card-specific rules text of Finale of Promise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Finale of Promise</exampledoc>
[TokenizationOrder(-1)]
public class FinaleOfPromiseCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 10 or more, copy each of those spells twice"];
}

/// <summary>Card-specific effect of Flashback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flashback</exampledoc>
[Dependent]
public class FlashbackCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast that card from your graveyard for its", Nib.This, "cost"];
}

/// <summary>Card-specific rules text of Flay, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flay</exampledoc>
[TokenizationOrder(-1)]
public class FlayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player discards another card at random unless they pay {1}"];
}

/// <summary>Card-specific rules text of Fork, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fork</exampledoc>
[TokenizationOrder(-1)]
public class ForkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target instant or sorcery spell, except that the copy is red"];
}

/// <summary>Card-specific rules text of Geistlight Snare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Geistlight Snare</exampledoc>
[TokenizationOrder(-1)]
public class GeistlightSnareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it also costs {1} less to cast if you control an enchantment"];
}

/// <summary>Card-specific rules text of Gideon Blackblade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon Blackblade</exampledoc>
[TokenizationOrder(-1)]
public class GideonBlackbladeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt to", Nib.This, "during your turn"];
}

/// <summary>Card-specific rules text of Glen Elendra's Answer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glen Elendra's Answer</exampledoc>
[TokenizationOrder(-1)]
public class GlenElendrasAnswerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter all spells your opponents control and all abilities your opponents control"];
}

/// <summary>Card-specific rules text of Goblin Game, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Game</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGameCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player loses life equal to the number of items they revealed"];
}

/// <summary>Card-specific effect of Godsire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Godsire</exampledoc>
[Dependent]
public class GodsireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create an 8/8 beast creature token that's red, green, and white"];
}

/// <summary>Card-specific rules text of Grisly Sigil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grisly Sigil</exampledoc>
[TokenizationOrder(-1)]
public class GrislySigilCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise,", Nib.This, "deals 1 damage to it and you gain 1 life"];
}

/// <summary>Card-specific rules text of Guild Feud, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guild Feud</exampledoc>
[TokenizationOrder(-1)]
public class GuildFeudCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you do the same with the top three cards of your library"];
}

/// <summary>Card-specific rules text of Gyruda, Doom of Depths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gyruda, Doom of Depths</exampledoc>
[TokenizationOrder(-1)]
public class GyrudaDoomOfDepthsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — your starting deck contains only cards with even mana values"];
}

/// <summary>Card-specific rules text of Haktos the Unscarred, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haktos the Unscarred</exampledoc>
[TokenizationOrder(-1)]
public class HaktosTheUnscarredRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["haktos has protection from each mana value other than the chosen number"];
}

/// <summary>Card-specific rules text of Hall of the Bandit Lord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hall of the Bandit Lord</exampledoc>
[TokenizationOrder(-1)]
public class HallOfTheBanditLordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on a creature spell, it gains haste"];
}

/// <summary>Card-specific rules text of Hallowed Respite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hallowed Respite</exampledoc>
[TokenizationOrder(-1)]
public class HallowedRespiteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it entered under your control, put a +1/+1 counter on it"];
}

/// <summary>Card-specific rules text of Haunting Echoes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haunting Echoes</exampledoc>
[TokenizationOrder(-1)]
public class HauntingEchoesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from target player's graveyard other than basic land cards"];
}

/// <summary>Card-specific rules text of Herald of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Leshrac</exampledoc>
[TokenizationOrder(-1)]
public class HeraldOfLeshracCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +1/+1 for each land you control but don't own"];
}

/// <summary>Card-specific rules text of Hex Magic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hex Magic</exampledoc>
[TokenizationOrder(-1)]
public class HexMagicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all the cards from your hand, then draw that many cards"];
}

/// <summary>Card-specific effect of High Perfect Morcant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>High Perfect Morcant</exampledoc>
[Dependent]
public class HighPerfectMorcantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent blights 1"];
}

/// <summary>Card-specific rules text of Hulk, Gamma Goliath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hulk, Gamma Goliath</exampledoc>
[TokenizationOrder(-1)]
public class HulkGammaGoliathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["power-up abilities of other creatures you control cost {3} less to activate"];
}

/// <summary>Card-specific effect of Ichormoon Gauntlet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichormoon Gauntlet</exampledoc>
[Dependent]
public class IchormoonGauntletEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a counter on target permanent"];
}

/// <summary>Card-specific rules text of Illusionary Terrain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionary Terrain</exampledoc>
[TokenizationOrder(-1)]
public class IllusionaryTerrainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["basic lands of the first chosen type are the second chosen type"];
}

/// <summary>Card-specific rules text of Insurrection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Insurrection</exampledoc>
[TokenizationOrder(-1)]
public class InsurrectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures and gain control of them until end of turn"];
}

/// <summary>Card-specific rules text of Into the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Into the Night</exampledoc>
[TokenizationOrder(-1)]
public class IntoTheNightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard any number of cards, then draw that many cards plus one"];
}

/// <summary>Card-specific rules text of Invoke Calamity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invoke Calamity</exampledoc>
[TokenizationOrder(-1)]
public class InvokeCalamityCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if those spells would be put into your graveyard, exile them instead"];
}

/// <summary>Card-specific rules text of Kaervek, the Punisher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaervek, the Punisher</exampledoc>
[TokenizationOrder(-1)]
public class KaervekThePunisherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copies of permanent spells become tokens"];
}

/// <summary>Card-specific rules text of Karakyk Guardian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karakyk Guardian</exampledoc>
[TokenizationOrder(-1)]
public class KarakykGuardianRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has hexproof as long as it hasn't dealt damage yet"];
}

/// <summary>Card-specific rules text of Karlov Watchdog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karlov Watchdog</exampledoc>
[TokenizationOrder(-1)]
public class KarlovWatchdogRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanents your opponents control can't be turned face up during your turn"];
}

/// <summary>Card-specific rules text of Keen-Eared Sentry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keen-Eared Sentry</exampledoc>
[TokenizationOrder(-1)]
public class KeenEaredSentryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent can't venture into the dungeon more than once each turn"];
}

/// <summary>Card-specific effect of Knight of the Holy Nimbus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Knight of the Holy Nimbus</exampledoc>
[Dependent]
public class KnightOfTheHolyNimbusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap it, remove it from combat, and heal all damage on it"];
}

/// <summary>Card-specific rules text of Koh, the Face Stealer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koh, the Face Stealer</exampledoc>
[TokenizationOrder(-1)]
public class KohTheFaceStealerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["koh has all activated and triggered abilities of the last chosen card"];
}

/// <summary>Card-specific rules text of Kumena's Awakening, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kumena's Awakening</exampledoc>
[TokenizationOrder(-1)]
public class KumenasAwakeningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have the city's blessing, instead only you draw a card"];
}

/// <summary>Card-specific rules text of Laccolith Rig, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Laccolith Rig</exampledoc>
[TokenizationOrder(-1)]
public class LaccolithRigRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, the first creature assigns no combat damage this turn"];
}

/// <summary>Card-specific rules text of Lavabrink Venturer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lavabrink Venturer</exampledoc>
[TokenizationOrder(-1)]
public class LavabrinkVenturerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has protection from each mana value of the chosen quality"];
}

/// <summary>Card-specific rules text of Liliana's Defeat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana's Defeat</exampledoc>
[TokenizationOrder(-1)]
public class LilianasDefeatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that permanent was a liliana planeswalker, her controller loses 3 life"];
}

/// <summary>Card-specific rules text of Lithomantic Barrage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lithomantic Barrage</exampledoc>
[TokenizationOrder(-1)]
public class LithomanticBarrageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 5 damage instead if that target is white and/or blue"];
}

/// <summary>Card-specific rules text of Living Terrain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Terrain</exampledoc>
[TokenizationOrder(-1)]
public class LivingTerrainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted land is a 5/6 green treefolk creature that's still a land"];
}

/// <summary>Card-specific effect of Maddening Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maddening Imp</exampledoc>
[Dependent]
public class MaddeningImpCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["non-wall creatures the active player controls attack this turn if able"];
}

/// <summary>Card-specific effect of Magnetic Theft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magnetic Theft</exampledoc>
[Dependent]
public class MagneticTheftEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["control of the equipment doesn't change"];
}

/// <summary>Card-specific rules text of Mana Clash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mana Clash</exampledoc>
[TokenizationOrder(-1)]
public class ManaClashCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 1 damage to each player whose coin comes up tails"];
}

/// <summary>Card-specific rules text of Master of Predicaments, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Predicaments</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfPredicamentsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player guesses whether the card's mana value is greater than 4"];
}

/// <summary>Card-specific effect of Meletis Charlatan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meletis Charlatan</exampledoc>
[Dependent]
public class MeletisCharlatanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the controller of target instant or sorcery spell copies it"];
}

/// <summary>Card-specific rules text of Mirror Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Golem</exampledoc>
[TokenizationOrder(-1)]
public class MirrorGolemRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has protection from each of the exiled card's card types"];
}

/// <summary>Card-specific rules text of Mycosynth Lattice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mycosynth Lattice</exampledoc>
[TokenizationOrder(-1)]
public class MycosynthLatticeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all cards that aren't on the battlefield, spells, and permanents are colorless"];
}

/// <summary>Card-specific rules text of Nahiri, Forged in Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nahiri, Forged in Fury</exampledoc>
[TokenizationOrder(-1)]
public class NahiriForgedInFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast equipment spells this way without paying their mana costs"];
}

/// <summary>Card-specific rules text of Nebuchadnezzar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nebuchadnezzar</exampledoc>
[TokenizationOrder(-1)]
public class NebuchadnezzarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player discards all cards with that name revealed this way"];
}

/// <summary>Card-specific rules text of Niko, Light of Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niko, Light of Hope</exampledoc>
[TokenizationOrder(-1)]
public class NikoLightOfHopeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shards you control become copies of it until the next end step"];
}

/// <summary>Card-specific rules text of Noble Quarry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noble Quarry</exampledoc>
[TokenizationOrder(-1)]
public class NobleQuarryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all creatures able to block this creature or enchanted creature do so"];
}
