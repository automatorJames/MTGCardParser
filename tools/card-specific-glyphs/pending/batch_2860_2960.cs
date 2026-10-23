/// <summary>Card-specific rules text of Stangg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stangg</exampledoc>
[TokenizationOrder(-1)]
public class StanggCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that token when", Nib.This, "leaves the battlefield"];
}

/// <summary>Card-specific rules text of Stangg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stangg</exampledoc>
[TokenizationOrder(-1)]
public class StanggCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice", Nib.This, "when that token leaves the battlefield"];
}

/// <summary>Card-specific rules text of Summary Dismissal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Summary Dismissal</exampledoc>
[TokenizationOrder(-1)]
public class SummaryDismissalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all other spells and counter all abilities"];
}

/// <summary>Card-specific effect of Swarmborn Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swarmborn Giant</exampledoc>
[Dependent]
public class SwarmbornGiantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this creature"];
}

/// <summary>Card-specific effect of Teferi, Master of Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Master of Time</exampledoc>
[Dependent]
public class TeferiMasterOfTimeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["take two extra turns after this one"];
}

/// <summary>Card-specific rules text of Tempest Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tempest Efreet</exampledoc>
[TokenizationOrder(-1)]
public class TempestEfreetCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange ownership of the revealed card and", Nib.This];
}

/// <summary>Card-specific rules text of The Three Seasons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Three Seasons</exampledoc>
[TokenizationOrder(-1)]
public class TheThreeSeasonsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["their owners shuffle those cards into their libraries"];
}

/// <summary>Card-specific rules text of Thraben Exorcism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thraben Exorcism</exampledoc>
[TokenizationOrder(-1)]
public class ThrabenExorcismRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target spirit, creature with disturb, or enchantment"];
}

/// <summary>Card-specific effect of Time Stop, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Stop</exampledoc>
[Dependent]
public class TimeStopEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all spells and abilities, including this spell"];
}

/// <summary>Card-specific rules text of Tomik, Distinguished Advokist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tomik, Distinguished Advokist</exampledoc>
[TokenizationOrder(-1)]
public class TomikDistinguishedAdvokistCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't play land cards from graveyards"];
}

/// <summary>Card-specific rules text of Tromokratis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tromokratis</exampledoc>
[TokenizationOrder(-1)]
public class TromokratisCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "has hexproof unless it's attacking or blocking"];
}

/// <summary>Card-specific rules text of Unexpected Results, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unexpected Results</exampledoc>
[TokenizationOrder(-1)]
public class UnexpectedResultsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then reveal the top card"];
}

/// <summary>Card-specific rules text of Uvilda, Dean of Perfection // Nassari, Dean of Expression, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uvilda, Dean of Perfection // Nassari, Dean of Expression</exampledoc>
[TokenizationOrder(-1)]
public class UvildaDeanOfPerfectionCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it costs {4} less to cast this way"];
}

/// <summary>Card-specific rules text of Verdeloth the Ancient, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Verdeloth the Ancient</exampledoc>
[TokenizationOrder(-1)]
public class VerdelothTheAncientRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["saproling creatures and other treefolk creatures get +1/+1"];
}

/// <summary>Card-specific rules text of Ward of Bones, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ward of Bones</exampledoc>
[TokenizationOrder(-1)]
public class WardOfBones3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the same is true for artifacts and enchantments"];
}

/// <summary>Card-specific rules text of Word of Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Command</exampledoc>
[TokenizationOrder(-1)]
public class WordOfCommandCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you control that player until", Nib.This, "finishes resolving"];
}

/// <summary>Card-specific rules text of Worldfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worldfire</exampledoc>
[TokenizationOrder(-1)]
public class WorldfireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from all hands and graveyards"];
}

/// <summary>Card-specific rules text of Worms of the Earth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worms of the Earth</exampledoc>
[TokenizationOrder(-1)]
public class WormsOfTheEarthCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a player does either, destroy this enchantment"];
}

/// <summary>Card-specific rules text of Zur's Weirding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zur's Weirding</exampledoc>
[TokenizationOrder(-1)]
public class ZursWeirdingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then any other player may pay 2 life"];
}

/// <summary>Card-specific rules text of A Killer Among Us, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>A Killer Among Us</exampledoc>
[TokenizationOrder(-1)]
public class AKillerAmongUsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then secretly choose human, merfolk, or goblin"];
}

/// <summary>Card-specific effect of Aerial Caravan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aerial Caravan</exampledoc>
[Dependent]
public class AerialCaravanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the card as you exile it"];
}

/// <summary>Card-specific rules text of Airtight Alibi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Airtight Alibi</exampledoc>
[TokenizationOrder(-1)]
public class AirtightAlibi2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's suspected, it's no longer suspected"];
}

/// <summary>Card-specific rules text of Alchemist's Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alchemist's Gambit</exampledoc>
[TokenizationOrder(-1)]
public class AlchemistsGambit2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during that turn, damage can't be prevented"];
}

/// <summary>Card-specific effect of Aligned Hedron Network, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aligned Hedron Network</exampledoc>
[Dependent]
public class AlignedHedronNetworkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those creatures return under their owners' control"];
}

/// <summary>Card-specific rules text of Artificial Evolution, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Artificial Evolution</exampledoc>
[TokenizationOrder(-1)]
public class ArtificialEvolutionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the new creature type can't be wall"];
}

/// <summary>Card-specific rules text of Balancing Act, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balancing Act</exampledoc>
[TokenizationOrder(-1)]
public class BalancingActCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player discards cards the same way"];
}

/// <summary>Card-specific rules text of Bend or Break, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bend or Break</exampledoc>
[TokenizationOrder(-1)]
public class BendOrBreakCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all lands in the chosen piles"];
}

/// <summary>Card-specific rules text of Bend or Break, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bend or Break</exampledoc>
[TokenizationOrder(-1)]
public class BendOrBreakCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all lands in the other piles"];
}

/// <summary>Card-specific rules text of Blow Your House Down, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blow Your House Down</exampledoc>
[TokenizationOrder(-1)]
public class BlowYourHouseDownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy any of them that are walls"];
}

/// <summary>Card-specific rules text of Bound by Moonsilver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bound by Moonsilver</exampledoc>
[TokenizationOrder(-1)]
public class BoundByMoonsilverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature can't attack, block, or transform"];
}

/// <summary>Card-specific rules text of Cultural Exchange, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cultural Exchange</exampledoc>
[TokenizationOrder(-1)]
public class CulturalExchange2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players exchange control of those creatures"];
}

/// <summary>Card-specific rules text of Darkpact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darkpact</exampledoc>
[TokenizationOrder(-1)]
public class Darkpact2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you own target card in the ante"];
}

/// <summary>Card-specific rules text of Debt to the Deathless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Debt to the Deathless</exampledoc>
[TokenizationOrder(-1)]
public class DebtToTheDeathlessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent loses two times x life"];
}

/// <summary>Card-specific rules text of Etrata, the Silencer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Etrata, the Silencer</exampledoc>
[TokenizationOrder(-1)]
public class EtrataTheSilencerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["etrata's owner shuffles etrata into their library"];
}

/// <summary>Card-specific rules text of Falling Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falling Star</exampledoc>
[TokenizationOrder(-1)]
public class FallingStarCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all creatures dealt damage by", Nib.This];
}

/// <summary>Card-specific rules text of Feint, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Feint</exampledoc>
[TokenizationOrder(-1)]
public class FeintCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all creatures blocking target attacking creature"];
}

/// <summary>Card-specific effect of Fight with Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fight with Fire</exampledoc>
[Dependent]
public class FightWithFireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those targets can include players and planeswalkers"];
}

/// <summary>Card-specific rules text of Game of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Game of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GameOfChaosCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double the life stakes with each flip"];
}

/// <summary>Card-specific rules text of Glissa, Herald of Predation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glissa, Herald of Predation</exampledoc>
[TokenizationOrder(-1)]
public class GlissaHeraldOfPredationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• transform all incubator tokens you control"];
}

/// <summary>Card-specific rules text of God-Eternal Kefnet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>God-Eternal Kefnet</exampledoc>
[TokenizationOrder(-1)]
public class GodEternalKefnetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that copy costs {2} less to cast"];
}

/// <summary>Card-specific rules text of Inquisitive Glimmer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inquisitive Glimmer</exampledoc>
[TokenizationOrder(-1)]
public class InquisitiveGlimmerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["unlock costs you pay cost {1} less"];
}

/// <summary>Card-specific rules text of Judith, Carnage Connoisseur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Judith, Carnage Connoisseur</exampledoc>
[TokenizationOrder(-1)]
public class JudithCarnageConnoisseurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• that spell gains deathtouch and lifelink"];
}

/// <summary>Card-specific effect of Jungle Patrol, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jungle Patrol</exampledoc>
[Dependent]
public class JunglePatrolEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add {r}"];
}

/// <summary>Card-specific rules text of Laquatus's Disdain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Laquatus's Disdain</exampledoc>
[TokenizationOrder(-1)]
public class LaquatussDisdainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell cast from a graveyard"];
}

/// <summary>Card-specific rules text of Lightstall Inquisitor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lightstall Inquisitor</exampledoc>
[TokenizationOrder(-1)]
public class LightstallInquisitorCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land played this way enters tapped"];
}

/// <summary>Card-specific rules text of Mages' Contest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mages' Contest</exampledoc>
[TokenizationOrder(-1)]
public class MagesContest4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you and target spell's controller bid life"];
}

/// <summary>Card-specific rules text of Magistrate's Veto, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magistrate's Veto</exampledoc>
[TokenizationOrder(-1)]
public class MagistratesVetoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["white creatures and blue creatures can't block"];
}

/// <summary>Card-specific rules text of Maskwood Nexus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maskwood Nexus</exampledoc>
[TokenizationOrder(-1)]
public class MaskwoodNexusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control are every creature type"];
}

/// <summary>Card-specific rules text of Mavinda, Students' Advocate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mavinda, Students' Advocate</exampledoc>
[TokenizationOrder(-1)]
public class MavindaStudentsAdvocateCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["timing rules for the spell still apply"];
}

/// <summary>Card-specific rules text of Memories Returning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memories Returning</exampledoc>
[TokenizationOrder(-1)]
public class MemoriesReturning3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then you put one into your hand"];
}

/// <summary>Card-specific rules text of Mjölnir, Hammer of Thor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mjölnir, Hammer of Thor</exampledoc>
[TokenizationOrder(-1)]
public class MjolnirHammerOfThorCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double all damage equipped creature would deal"];
}

/// <summary>Card-specific rules text of Mornsong Aria, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mornsong Aria</exampledoc>
[TokenizationOrder(-1)]
public class MornsongAriaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't draw cards or gain life"];
}

/// <summary>Card-specific rules text of Mystic Subdual, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mystic Subdual</exampledoc>
[TokenizationOrder(-1)]
public class MysticSubdual2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can gain abilities in other ways"];
}

/// <summary>Card-specific effect of Neglected Heirloom // Ashmouth Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Neglected Heirloom // Ashmouth Blade</exampledoc>
[Dependent]
public class NeglectedHeirloomEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["transform this equipment"];
}

/// <summary>Card-specific rules text of Nowhere to Run, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nowhere to Run</exampledoc>
[TokenizationOrder(-1)]
public class NowhereToRunCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ward abilities of those creatures don't trigger"];
}

/// <summary>Card-specific effect of Oubliette, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oubliette</exampledoc>
[Dependent]
public class Oubliette2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras and equipment phase out with it"];
}

/// <summary>Card-specific rules text of Poison the Cup, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Poison the Cup</exampledoc>
[TokenizationOrder(-1)]
public class PoisonTheCupRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was foretold, scry 2"];
}

/// <summary>Card-specific rules text of Psychotic Episode, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychotic Episode</exampledoc>
[TokenizationOrder(-1)]
public class PsychoticEpisode2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose a card revealed this way"];
}

/// <summary>Card-specific effect of Qarsi Deceiver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Qarsi Deceiver</exampledoc>
[Dependent]
public class QarsiDeceiverCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a megamorph cost is a morph cost"];
}

/// <summary>Card-specific rules text of Raiding Schemes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raiding Schemes</exampledoc>
[TokenizationOrder(-1)]
public class RaidingSchemesCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each noncreature spell you cast has conspire"];
}

/// <summary>Card-specific rules text of Retether, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Retether</exampledoc>
[TokenizationOrder(-1)]
public class RetetherCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only creatures can be enchanted this way"];
}

/// <summary>Card-specific rules text of Reverse the Sands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reverse the Sands</exampledoc>
[TokenizationOrder(-1)]
public class ReverseTheSands2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["redistribute any number of players' life totals"];
}

/// <summary>Card-specific rules text of Riddles in the Dark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riddles in the Dark</exampledoc>
[TokenizationOrder(-1)]
public class RiddlesInTheDarkCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["an opponent chooses one of the piles"];
}

/// <summary>Card-specific rules text of Roar of Jukai, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roar of Jukai</exampledoc>
[TokenizationOrder(-1)]
public class RoarOfJukaiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["splice onto arcane—an opponent gains 5 life"];
}

/// <summary>Card-specific rules text of Rover Blades, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rover Blades</exampledoc>
[TokenizationOrder(-1)]
public class RoverBladesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures can't be attached to other permanents"];
}

/// <summary>Card-specific rules text of Savor the Moment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Savor the Moment</exampledoc>
[TokenizationOrder(-1)]
public class SavorTheMomentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["skip the untap step of that turn"];
}

/// <summary>Card-specific effect of Sedris, the Traitor King, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sedris, the Traitor King</exampledoc>
[Dependent]
public class SedrisTheTraitorKingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return the card to the battlefield"];
}

/// <summary>Card-specific rules text of Shadrix Silverquill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadrix Silverquill</exampledoc>
[TokenizationOrder(-1)]
public class ShadrixSilverquill2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each mode must target a different player"];
}

/// <summary>Card-specific effect of Shapeshifter's Marrow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shapeshifter's Marrow</exampledoc>
[Dependent]
public class ShapeshiftersMarrowCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it does, it loses this ability"];
}

/// <summary>Card-specific rules text of Shark Shredder, Killer Clone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shark Shredder, Killer Clone</exampledoc>
[TokenizationOrder(-1)]
public class SharkShredderKillerCloneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it enters tapped and attacking that player"];
}

/// <summary>Card-specific rules text of Silver Wyvern, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silver Wyvern</exampledoc>
[TokenizationOrder(-1)]
public class SilverWyvernRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the new target must be a creature"];
}

/// <summary>Card-specific rules text of Spinerock Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinerock Tyrant</exampledoc>
[TokenizationOrder(-1)]
public class SpinerockTyrantCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, those spells gain wither"];
}

/// <summary>Card-specific rules text of Stronghold Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stronghold Gambit</exampledoc>
[TokenizationOrder(-1)]
public class StrongholdGambitCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player reveals their chosen card"];
}

/// <summary>Card-specific effect of Su-Chi Cave Guard, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Su-Chi Cave Guard</exampledoc>
[Dependent]
public class SuChiCaveGuardEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add eight {c}"];
}

/// <summary>Card-specific rules text of Syr Gwyn, Hero of Ashvale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Gwyn, Hero of Ashvale</exampledoc>
[TokenizationOrder(-1)]
public class SyrGwynHeroOfAshvaleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipment you control have equip knight {0}"];
}

/// <summary>Card-specific rules text of The Eagles Are Coming!, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Eagles Are Coming!</exampledoc>
[TokenizationOrder(-1)]
public class TheEaglesAreComingCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return each chosen creature to your hand"];
}

/// <summary>Card-specific rules text of Tibalt's Trickery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tibalt's Trickery</exampledoc>
[TokenizationOrder(-1)]
public class TibaltsTrickeryCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose 1, 2, or 3 at random"];
}

/// <summary>Card-specific effect of Two-Headed Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Two-Headed Giant</exampledoc>
[Dependent]
public class TwoHeadedGiantCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip two coins"];
}

/// <summary>Card-specific effect of Uldaros Theorix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uldaros Theorix</exampledoc>
[Dependent]
public class UldarosTheorixCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanent spells cast this way become tokens"];
}

/// <summary>Card-specific rules text of Uphill Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uphill Battle</exampledoc>
[TokenizationOrder(-1)]
public class UphillBattleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures played by your opponents enter tapped"];
}

/// <summary>Card-specific rules text of Urza's Ruinous Blast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza's Ruinous Blast</exampledoc>
[TokenizationOrder(-1)]
public class UrzasRuinousBlastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all nonland permanents that aren't legendary"];
}

/// <summary>Card-specific rules text of Vannifar, Evolved Enigma, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vannifar, Evolved Enigma</exampledoc>
[TokenizationOrder(-1)]
public class VannifarEvolvedEnigmaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• cloak a card from your hand"];
}

/// <summary>Card-specific rules text of Wan Shi Tong, Librarian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wan Shi Tong, Librarian</exampledoc>
[TokenizationOrder(-1)]
public class WanShiTongLibrarianCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then draw half x cards, rounded down"];
}

/// <summary>Card-specific rules text of Word of Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Command</exampledoc>
[TokenizationOrder(-1)]
public class WordOfCommandCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player plays that card if able"];
}

/// <summary>Card-specific effect of Aura Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Thief</exampledoc>
[Dependent]
public class AuraThiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you don't get to move auras"];
}

/// <summary>Card-specific rules text of Beamsplitter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beamsplitter Mage</exampledoc>
[TokenizationOrder(-1)]
public class BeamsplitterMageCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy targets the chosen creature"];
}

/// <summary>Card-specific rules text of Brainspoil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brainspoil</exampledoc>
[TokenizationOrder(-1)]
public class BrainspoilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target creature that isn't enchanted"];
}

/// <summary>Card-specific effect of Cavalry Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cavalry Master</exampledoc>
[Dependent]
public class CavalryMasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instance of flanking triggers separately"];
}

/// <summary>Card-specific rules text of Commandeer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Commandeer</exampledoc>
[TokenizationOrder(-1)]
public class CommandeerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target noncreature spell"];
}

/// <summary>Card-specific effect of Creeping Dread, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Creeping Dread</exampledoc>
[Dependent]
public class CreepingDreadCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players reveal the discarded cards simultaneously"];
}

/// <summary>Card-specific rules text of Emblazoned Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emblazoned Golem</exampledoc>
[TokenizationOrder(-1)]
public class EmblazonedGolem2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only colored mana on x"];
}

/// <summary>Card-specific effect of Evermind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evermind</exampledoc>
[Dependent]
public class EvermindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nonexistent mana costs can't be paid"];
}

/// <summary>Card-specific rules text of Garruk's Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garruk's Horde</exampledoc>
[TokenizationOrder(-1)]
public class GarruksHorde2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you still pay the spell's costs"];
}

/// <summary>Card-specific rules text of Grimoire Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grimoire Thief</exampledoc>
[TokenizationOrder(-1)]
public class GrimoireThiefRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter all spells with those names"];
}

/// <summary>Card-specific effect of Grip of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grip of Chaos</exampledoc>
[Dependent]
public class GripOfChaosCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["select from among all legal targets"];
}

/// <summary>Card-specific rules text of Guided Passage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guided Passage</exampledoc>
[TokenizationOrder(-1)]
public class GuidedPassageCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the cards in your library"];
}

/// <summary>Card-specific rules text of Immerwolf, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Immerwolf</exampledoc>
[TokenizationOrder(-1)]
public class ImmerwolfRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["non-human werewolves you control can't transform"];
}

/// <summary>Card-specific effect of Intimidation Campaign, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Intimidation Campaign</exampledoc>
[Dependent]
public class IntimidationCampaignEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it returns only from the battlefield"];
}

/// <summary>Card-specific rules text of Leeches, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leeches</exampledoc>
[TokenizationOrder(-1)]
public class LeechesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player loses all poison counters"];
}

/// <summary>Card-specific effect of Linvala, Shield of Sea Gate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Linvala, Shield of Sea Gate</exampledoc>
[Dependent]
public class LinvalaShieldOfSeaGateCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose hexproof or indestructible"];
}
