/// <summary>Card-specific rules text of Teferi, Timeless Voyager, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Timeless Voyager</exampledoc>
[TokenizationOrder(-1)]
public class TeferiTimelessVoyagerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until the end of your next turn, they can't phase in"];
}

/// <summary>Card-specific rules text of Thanos, the Mad Titan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thanos, the Mad Titan</exampledoc>
[TokenizationOrder(-1)]
public class ThanosTheMadTitanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each other creature with mana value of the chosen quality"];
}

/// <summary>Card-specific rules text of The Eternal Wanderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Eternal Wanderer</exampledoc>
[TokenizationOrder(-1)]
public class TheEternalWandererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player sacrifices all creatures they control not chosen this way"];
}

/// <summary>Card-specific rules text of Thought Distortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Distortion</exampledoc>
[TokenizationOrder(-1)]
public class ThoughtDistortionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all noncreature, nonland cards from that player's hand and graveyard"];
}

/// <summary>Card-specific rules text of Timmerian Fiends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timmerian Fiends</exampledoc>
[TokenizationOrder(-1)]
public class TimmerianFiendsCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player doesn't, exchange ownership of that artifact and", Nib.This];
}

/// <summary>Card-specific effect of Ultima, Origin of Oblivion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ultima, Origin of Oblivion</exampledoc>
[Dependent]
public class UltimaOriginOfOblivionCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an additional {c}"];
}

/// <summary>Card-specific effect of Underworld Breach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Underworld Breach</exampledoc>
[Dependent]
public class UnderworldBreachCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast cards from your graveyard for their escape cost"];
}

/// <summary>Card-specific rules text of Unexpected Fangs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unexpected Fangs</exampledoc>
[TokenizationOrder(-1)]
public class UnexpectedFangsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter and a lifelink counter on target creature"];
}

/// <summary>Card-specific rules text of Varchild's War-Riders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Varchild's War-Riders</exampledoc>
[TokenizationOrder(-1)]
public class VarchildsWarRidersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—have an opponent create a 1/1 red survivor creature token"];
}

/// <summary>Card-specific rules text of Venser's Diffusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Venser's Diffusion</exampledoc>
[TokenizationOrder(-1)]
public class VensersDiffusionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target nonland permanent or suspended card to its owner's hand"];
}

/// <summary>Card-specific effect of Vivien, Arkbow Ranger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vivien, Arkbow Ranger</exampledoc>
[Dependent]
public class VivienArkbowRangerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute two +1/+1 counters among up to two target creatures"];
}

/// <summary>Card-specific rules text of Vizier of the Menagerie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizier of the Menagerie</exampledoc>
[TokenizationOrder(-1)]
public class VizierOfTheMenagerieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can spend mana of any type to cast creature spells"];
}

/// <summary>Card-specific rules text of Volrath's Dungeon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Dungeon</exampledoc>
[TokenizationOrder(-1)]
public class VolrathsDungeonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only during their turn"];
}

/// <summary>Card-specific rules text of Walker of the Wastes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Walker of the Wastes</exampledoc>
[TokenizationOrder(-1)]
public class WalkerOfTheWastesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +1/+1 for each land you control named wastes"];
}

/// <summary>Card-specific effect of Wall of Corpses, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Corpses</exampledoc>
[Dependent]
public class WallOfCorpsesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target creature this creature is blocking"];
}

/// <summary>Card-specific effect of Wan Shi Tong, Librarian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wan Shi Tong, Librarian</exampledoc>
[Dependent]
public class WanShiTongLibrarianCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put x +1/+1 counters on him"];
}

/// <summary>Card-specific rules text of Ward of Bones, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ward of Bones</exampledoc>
[TokenizationOrder(-1)]
public class WardOfBones2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who controls more lands than you can't play lands"];
}

/// <summary>Card-specific rules text of Winter Blast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winter Blast</exampledoc>
[TokenizationOrder(-1)]
public class WinterBlastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 2 damage to each of those creatures with flying"];
}

/// <summary>Card-specific rules text of Yare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yare</exampledoc>
[TokenizationOrder(-1)]
public class YareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can block up to two additional creatures this turn"];
}

/// <summary>Card-specific rules text of Zurgo and Ojutai, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zurgo and Ojutai</exampledoc>
[TokenizationOrder(-1)]
public class ZurgoAndOjutaiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return one of those dragons to its owner's hand"];
}

/// <summary>Card-specific rules text of Agency Coroner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agency Coroner</exampledoc>
[TokenizationOrder(-1)]
public class AgencyCoronerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the sacrificed creature was suspected, draw two cards instead"];
}

/// <summary>Card-specific rules text of Akroma, Angel of Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Akroma, Angel of Fury</exampledoc>
[TokenizationOrder(-1)]
public class AkromaAngelOfFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["turn her face up any time for her morph cost"];
}

/// <summary>Card-specific rules text of Alchemist's Retrieval, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alchemist's Retrieval</exampledoc>
[TokenizationOrder(-1)]
public class AlchemistsRetrievalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target nonland permanent [you control] to its owner's hand"];
}

/// <summary>Card-specific effect of Alluring Suitor // Deadly Dancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alluring Suitor // Deadly Dancer</exampledoc>
[Dependent]
public class AlluringSuitorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["transform this creature"];
}

/// <summary>Card-specific effect of Anafenza, Unyielding Lineage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anafenza, Unyielding Lineage</exampledoc>
[Dependent]
public class AnafenzaUnyieldingLineageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["anafenza endures 2"];
}

/// <summary>Card-specific effect of Animation Module, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Animation Module</exampledoc>
[Dependent]
public class AnimationModuleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a counter on target permanent or player"];
}

/// <summary>Card-specific rules text of Animation Module, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Animation Module</exampledoc>
[TokenizationOrder(-1)]
public class AnimationModule2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["give that permanent or player another counter of that kind"];
}

/// <summary>Card-specific effect of Ashiok, Nightmare Weaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Nightmare Weaver</exampledoc>
[Dependent]
public class AshiokNightmareWeaverCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from all opponents' hands and graveyards"];
}

/// <summary>Card-specific effect of Assembly-Worker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Assembly-Worker</exampledoc>
[Dependent]
public class AssemblyWorkerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target", Nib.This, "creature gets +1/+1 until end of turn"];
}

/// <summary>Card-specific rules text of Aura Graft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Graft</exampledoc>
[TokenizationOrder(-1)]
public class AuraGraftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target aura that's attached to a permanent"];
}

/// <summary>Card-specific effect of Avatar Aang // Aang, Master of Elements, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar Aang // Aang, Master of Elements</exampledoc>
[Dependent]
public class AvatarAang2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["earthbend, firebend, or airbend, draw a card"];
}

/// <summary>Card-specific rules text of Blinding Beam, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blinding Beam</exampledoc>
[TokenizationOrder(-1)]
public class BlindingBeamRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• creatures don't untap during target player's next untap step"];
}

/// <summary>Card-specific rules text of Breaking Wave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breaking Wave</exampledoc>
[TokenizationOrder(-1)]
public class BreakingWaveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["simultaneously untap all tapped creatures and tap all untapped creatures"];
}

/// <summary>Card-specific rules text of Case of the Gateway Express, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Case of the Gateway Express</exampledoc>
[TokenizationOrder(-1)]
public class CaseOfTheGatewayExpressRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["to solve — three or more creatures attacked this turn"];
}

/// <summary>Card-specific rules text of Chain Lightning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chain Lightning</exampledoc>
[TokenizationOrder(-1)]
public class ChainLightningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player or that permanent's controller may pay {r}{r}"];
}

/// <summary>Card-specific effect of Coils of the Medusa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coils of the Medusa</exampledoc>
[Dependent]
public class CoilsOfTheMedusaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all non-wall creatures blocking enchanted creature"];
}

/// <summary>Card-specific rules text of Conduit of Worlds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conduit of Worlds</exampledoc>
[TokenizationOrder(-1)]
public class ConduitOfWorldsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you can't cast additional spells this turn"];
}

/// <summary>Card-specific rules text of Consuming Ashes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Consuming Ashes</exampledoc>
[TokenizationOrder(-1)]
public class ConsumingAshesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it had mana value 3 or less, surveil 2"];
}

/// <summary>Card-specific effect of Corrupted Grafstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Corrupted Grafstone</exampledoc>
[Dependent]
public class CorruptedGrafstoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a color of a card in your graveyard"];
}

/// <summary>Card-specific rules text of Cultural Exchange, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cultural Exchange</exampledoc>
[TokenizationOrder(-1)]
public class CulturalExchangeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose the same number of creatures another target player controls"];
}

/// <summary>Card-specific rules text of Darkpact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darkpact</exampledoc>
[TokenizationOrder(-1)]
public class DarkpactRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange that card with the top card of your library"];
}

/// <summary>Card-specific rules text of Dimir Charm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dimir Charm</exampledoc>
[TokenizationOrder(-1)]
public class DimirCharmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one back and the rest into that player's graveyard"];
}

/// <summary>Card-specific rules text of Do or Die, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Do or Die</exampledoc>
[TokenizationOrder(-1)]
public class DoOrDieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures in the pile of that player's choice"];
}

/// <summary>Card-specific rules text of Druidic Satchel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Druidic Satchel</exampledoc>
[TokenizationOrder(-1)]
public class DruidicSatchelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a noncreature, nonland card, you gain 2 life"];
}

/// <summary>Card-specific effect of Duty Beyond Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Duty Beyond Death</exampledoc>
[Dependent]
public class DutyBeyondDeathEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy those creatures"];
}

/// <summary>Card-specific rules text of Eliminate the Impossible, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eliminate the Impossible</exampledoc>
[TokenizationOrder(-1)]
public class EliminateTheImpossibleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of them are suspected, they're no longer suspected"];
}

/// <summary>Card-specific rules text of Elixir of Immortality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elixir of Immortality</exampledoc>
[TokenizationOrder(-1)]
public class ElixirOfImmortalityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle this artifact and your graveyard into their owner's library"];
}

/// <summary>Card-specific rules text of Elspeth Conquers Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elspeth Conquers Death</exampledoc>
[TokenizationOrder(-1)]
public class ElspethConquersDeathCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter or a loyalty counter on it"];
}

/// <summary>Card-specific effect of Elspeth Tirel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elspeth Tirel</exampledoc>
[Dependent]
public class ElspethTirelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all other permanents except for lands and tokens"];
}

/// <summary>Card-specific effect of Errant, Street Artist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Errant, Street Artist</exampledoc>
[Dependent]
public class ErrantStreetArtistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target spell you control that wasn't cast"];
}

/// <summary>Card-specific effect of Ertai's Familiar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Familiar</exampledoc>
[Dependent]
public class ErtaisFamiliar2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next upkeep, this creature can't phase out"];
}

/// <summary>Card-specific rules text of Everlasting Torment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Everlasting Torment</exampledoc>
[TokenizationOrder(-1)]
public class EverlastingTormentCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all damage is dealt as though its source had wither"];
}

/// <summary>Card-specific rules text of Experimental Frenzy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Experimental Frenzy</exampledoc>
[TokenizationOrder(-1)]
public class ExperimentalFrenzyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't play lands or cast spells from your hand"];
}

/// <summary>Card-specific rules text of Extinction Event, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Extinction Event</exampledoc>
[TokenizationOrder(-1)]
public class ExtinctionEventRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile each creature with mana value of the chosen quality"];
}

/// <summary>Card-specific rules text of Falling Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falling Star</exampledoc>
[TokenizationOrder(-1)]
public class FallingStarCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 3 damage to each creature it lands on"];
}

/// <summary>Card-specific rules text of Fiery Annihilation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Annihilation</exampledoc>
[TokenizationOrder(-1)]
public class FieryAnnihilationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile up to one target equipment attached to that creature"];
}

/// <summary>Card-specific effect of Flickerform, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flickerform</exampledoc>
[Dependent]
public class FlickerformEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile enchanted creature and all auras attached to it"];
}

/// <summary>Card-specific rules text of Gix's Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gix's Command</exampledoc>
[TokenizationOrder(-1)]
public class GixsCommandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• put two +1/+1 counters on up to one creature"];
}

/// <summary>Card-specific rules text of Gogo, Master of Mimicry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gogo, Master of Mimicry</exampledoc>
[TokenizationOrder(-1)]
public class GogoMasterOfMimicryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability can't be copied and x can't be 0"];
}

/// <summary>Card-specific rules text of Guardian Beast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guardian Beast</exampledoc>
[TokenizationOrder(-1)]
public class GuardianBeastCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect doesn't remove auras already attached to those artifacts"];
}

/// <summary>Card-specific rules text of Haktos the Unscarred, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haktos the Unscarred</exampledoc>
[TokenizationOrder(-1)]
public class HaktosTheUnscarred2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as haktos enters, choose 2, 3, or 4 at random"];
}

/// <summary>Card-specific rules text of Halls of Mist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Halls of Mist</exampledoc>
[TokenizationOrder(-1)]
public class HallsOfMistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures that attacked during their controller's last turn can't attack"];
}

/// <summary>Card-specific rules text of Hama Pashar, Ruin Seeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hama Pashar, Ruin Seeker</exampledoc>
[TokenizationOrder(-1)]
public class HamaPasharRuinSeekerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["room abilities of dungeons you own trigger an additional time"];
}

/// <summary>Card-specific rules text of Hazoret, Godseeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hazoret, Godseeker</exampledoc>
[TokenizationOrder(-1)]
public class HazoretGodseekerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["hazoret can't attack or block unless you have max speed"];
}

/// <summary>Card-specific rules text of Ichormoon Gauntlet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichormoon Gauntlet</exampledoc>
[TokenizationOrder(-1)]
public class IchormoonGauntlet2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put an additional counter of that kind on that permanent"];
}

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
[TokenizationOrder(-1)]
public class IllicitAuctionCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may bid life for control of target creature"];
}

/// <summary>Card-specific rules text of Illuna, Apex of Wishes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illuna, Apex of Wishes</exampledoc>
[TokenizationOrder(-1)]
public class IllunaApexOfWishesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put that card onto the battlefield or into your hand"];
}

/// <summary>Card-specific rules text of Innocent Bystander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Innocent Bystander</exampledoc>
[TokenizationOrder(-1)]
public class InnocentBystanderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature is dealt 3 or more damage, investigate"];
}

/// <summary>Card-specific rules text of Invasion Plans, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion Plans</exampledoc>
[TokenizationOrder(-1)]
public class InvasionPlansRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the attacking player chooses how each creature blocks each combat"];
}

/// <summary>Card-specific rules text of Invasion of Pyrulea // Gargantuan Slabhorn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Pyrulea // Gargantuan Slabhorn</exampledoc>
[TokenizationOrder(-1)]
public class InvasionOfPyruleaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a land or double-faced card, draw a card"];
}

/// <summary>Card-specific rules text of Jegantha, the Wellspring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jegantha, the Wellspring</exampledoc>
[TokenizationOrder(-1)]
public class JeganthaTheWellspringCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this mana can't be spent to pay generic mana costs"];
}

/// <summary>Card-specific rules text of Kormus Bell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kormus Bell</exampledoc>
[TokenizationOrder(-1)]
public class KormusBellRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all swamps are 1/1 black creatures that are still lands"];
}

/// <summary>Card-specific rules text of Light Up the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light Up the Night</exampledoc>
[TokenizationOrder(-1)]
public class LightUpTheNightCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flashback—{3}{r}, remove x loyalty counters from among planeswalkers you control"];
}

/// <summary>Card-specific rules text of Lovestruck Beast // Heart's Desire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lovestruck Beast // Heart's Desire</exampledoc>
[TokenizationOrder(-1)]
public class LovestruckBeastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't attack unless you control a 1/1 creature"];
}

/// <summary>Card-specific rules text of Lunar Rejection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lunar Rejection</exampledoc>
[TokenizationOrder(-1)]
public class LunarRejectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target [wolf or werewolf] creature to its owner's hand"];
}

/// <summary>Card-specific rules text of Maddening Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maddening Imp</exampledoc>
[TokenizationOrder(-1)]
public class MaddeningImpCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only during an opponent's turn and only before combat"];
}

/// <summary>Card-specific rules text of Mages' Contest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mages' Contest</exampledoc>
[TokenizationOrder(-1)]
public class MagesContestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the high bidder loses life equal to the high bid"];
}

/// <summary>Card-specific effect of Matzalantli, the Great Door // The Core, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Matzalantli, the Great Door // The Core</exampledoc>
[Dependent]
public class MatzalantliTheGreatDoorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifact, battle, creature, enchantment, land, and planeswalker are permanent types"];
}

/// <summary>Card-specific rules text of Memories Returning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memories Returning</exampledoc>
[TokenizationOrder(-1)]
public class MemoriesReturningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then they put one on the bottom of your library"];
}

/// <summary>Card-specific effect of Menacing Ogre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Menacing Ogre</exampledoc>
[Dependent]
public class MenacingOgreCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player secretly chooses a number"];
}

/// <summary>Card-specific rules text of Menacing Ogre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Menacing Ogre</exampledoc>
[TokenizationOrder(-1)]
public class MenacingOgreCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player with the highest number loses that much life"];
}

/// <summary>Card-specific effect of Minimus Containment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minimus Containment</exampledoc>
[Dependent]
public class MinimusContainmentCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was a creature, it's no longer a creature"];
}

/// <summary>Card-specific rules text of Mishra's Domination, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra's Domination</exampledoc>
[TokenizationOrder(-1)]
public class MishrasDominationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you control enchanted creature, it gets +2/+2"];
}

/// <summary>Card-specific rules text of Mishra's War Machine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra's War Machine</exampledoc>
[TokenizationOrder(-1)]
public class MishrasWarMachineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it deals damage to you this way, tap it"];
}

/// <summary>Card-specific effect of Mishra, Tamer of Mak Fawa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra, Tamer of Mak Fawa</exampledoc>
[Dependent]
public class MishraTamerOfMakFawaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return the card from your graveyard to the battlefield"];
}

/// <summary>Card-specific rules text of Mister Negative, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mister Negative</exampledoc>
[TokenizationOrder(-1)]
public class MisterNegativeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lost life this way, draw that many cards"];
}

/// <summary>Card-specific rules text of Mogg Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mogg Assassin</exampledoc>
[TokenizationOrder(-1)]
public class MoggAssassin2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the flip, destroy the creature you chose"];
}

/// <summary>Card-specific rules text of Mutinous Massacre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mutinous Massacre</exampledoc>
[TokenizationOrder(-1)]
public class MutinousMassacreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each creature with mana value of the chosen quality"];
}

/// <summary>Card-specific rules text of Myr Superion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Myr Superion</exampledoc>
[TokenizationOrder(-1)]
public class MyrSuperionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only mana produced by creatures to cast this spell"];
}

/// <summary>Card-specific rules text of Nettling Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nettling Imp</exampledoc>
[TokenizationOrder(-1)]
public class NettlingImpCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only during an opponent's turn, before attackers are declared"];
}

/// <summary>Card-specific rules text of Nezumi Graverobber // Nighteyes the Desecrator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nezumi Graverobber // Nighteyes the Desecrator</exampledoc>
[TokenizationOrder(-1)]
public class NezumiGraverobberRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if no cards are in that graveyard, flip this creature"];
}

/// <summary>Card-specific rules text of Omnath, Locus of All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omnath, Locus of All</exampledoc>
[TokenizationOrder(-1)]
public class OmnathLocusOfAllCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't reveal it, put it into your hand"];
}

/// <summary>Card-specific rules text of Omnath, Locus of Mana, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omnath, Locus of Mana</exampledoc>
[TokenizationOrder(-1)]
public class OmnathLocusOfManaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["omnath gets +1/+1 for each unspent green mana you have"];
}

/// <summary>Card-specific effect of Outlaws' Merriment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outlaws' Merriment</exampledoc>
[Dependent]
public class OutlawsMerrimentCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one at random"];
}

/// <summary>Card-specific rules text of Outlaws' Merriment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outlaws' Merriment</exampledoc>
[TokenizationOrder(-1)]
public class OutlawsMerrimentCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a red and white creature token with those characteristics"];
}

/// <summary>Card-specific rules text of Oviya, Automech Artisan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oviya, Automech Artisan</exampledoc>
[TokenizationOrder(-1)]
public class OviyaAutomechArtisanCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature that's attacking one of your opponents has trample"];
}

/// <summary>Card-specific rules text of Pain's Reward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain's Reward</exampledoc>
[TokenizationOrder(-1)]
public class PainsRewardCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you start the bidding with a bid of any number"];
}

/// <summary>Card-specific effect of Perplexing Chimera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Perplexing Chimera</exampledoc>
[Dependent]
public class PerplexingChimeraCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the spell becomes a permanent, you control that permanent"];
}

/// <summary>Card-specific rules text of Petra Sphinx, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Petra Sphinx</exampledoc>
[TokenizationOrder(-1)]
public class PetraSphinxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't, the player puts it into their graveyard"];
}

/// <summary>Card-specific rules text of Planewide Celebration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planewide Celebration</exampledoc>
[TokenizationOrder(-1)]
public class PlanewideCelebrationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• create a 2/2 citizen creature token that's all colors"];
}
