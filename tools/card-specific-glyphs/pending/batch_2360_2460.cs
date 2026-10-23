/// <summary>Card-specific effect of Norin the Wary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Norin the Wary</exampledoc>
[Dependent]
public class NorinTheWaryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile norin"];
}

/// <summary>Card-specific rules text of Open the Way, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Open the Way</exampledoc>
[TokenizationOrder(-1)]
public class OpenTheWayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the number of players in the game"];
}

/// <summary>Card-specific rules text of Overzealous Muscle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Overzealous Muscle</exampledoc>
[TokenizationOrder(-1)]
public class OverzealousMuscleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy a creature with indestructible"];
}

/// <summary>Card-specific rules text of Paradoxical Outcome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Paradoxical Outcome</exampledoc>
[TokenizationOrder(-1)]
public class ParadoxicalOutcomeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each card returned to your hand this way"];
}

/// <summary>Card-specific effect of Pendelhaven Elder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pendelhaven Elder</exampledoc>
[Dependent]
public class PendelhavenElderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each 1/1 creature you control gets +1/+2 until end of turn"];
}

/// <summary>Card-specific rules text of Planar Overlay, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planar Overlay</exampledoc>
[TokenizationOrder(-1)]
public class PlanarOverlayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player chooses a land they control of each basic land type"];
}

/// <summary>Card-specific rules text of Pompous Gadabout, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pompous Gadabout</exampledoc>
[TokenizationOrder(-1)]
public class PompousGadaboutRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't be blocked by creatures that don't have a name"];
}

/// <summary>Card-specific effect of Proft, Consulting Detective, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Proft, Consulting Detective</exampledoc>
[Dependent]
public class ProftConsultingDetectiveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw after you scry or surveil"];
}

/// <summary>Card-specific rules text of Psychotic Episode, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychotic Episode</exampledoc>
[TokenizationOrder(-1)]
public class PsychoticEpisodeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their hand and the top card of their library"];
}

/// <summary>Card-specific rules text of Raiding Party, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raiding Party</exampledoc>
[TokenizationOrder(-1)]
public class RaidingPartyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all plains that weren't chosen this way by any player"];
}

/// <summary>Card-specific effect of Rakdos Augermage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos Augermage</exampledoc>
[Dependent]
public class RakdosAugermage2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal your hand and discard a card of target opponent's choice"];
}

/// <summary>Card-specific rules text of Rally the Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rally the Horde</exampledoc>
[TokenizationOrder(-1)]
public class RallyTheHordeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the last card exiled isn't a land card, repeat this process"];
}

/// <summary>Card-specific rules text of Rampaging Geoderm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rampaging Geoderm</exampledoc>
[TokenizationOrder(-1)]
public class RampagingGeodermRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's attacking a battle, put a +1/+1 counter on it instead"];
}

/// <summary>Card-specific rules text of Rhystic Lightning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhystic Lightning</exampledoc>
[TokenizationOrder(-1)]
public class RhysticLightningCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they do,", Nib.This, "deals 2 damage to the permanent or player"];
}

/// <summary>Card-specific rules text of Rise of the Deathbringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rise of the Deathbringer</exampledoc>
[TokenizationOrder(-1)]
public class RiseOfTheDeathbringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose life equal to the number of cards drawn this way"];
}

/// <summary>Card-specific rules text of Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sacrifice</exampledoc>
[TokenizationOrder(-1)]
public class SacrificeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an amount of {b} equal to the", Nib.This, "d creature's mana value"];
}

/// <summary>Card-specific effect of Scourglass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scourglass</exampledoc>
[Dependent]
public class ScourglassEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all permanents except for artifacts and lands"];
}

/// <summary>Card-specific effect of Shadrix Silverquill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadrix Silverquill</exampledoc>
[Dependent]
public class ShadrixSilverquillEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose two"];
}

/// <summary>Card-specific rules text of Shield of Kaldra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shield of Kaldra</exampledoc>
[TokenizationOrder(-1)]
public class ShieldOfKaldraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipment named sword of kaldra,", Nib.This, ", and helm of kaldra have indestructible"];
}

/// <summary>Card-specific rules text of Silence the Believers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silence the Believers</exampledoc>
[TokenizationOrder(-1)]
public class SilenceTheBelieversRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile any number of target creatures and all auras attached to them"];
}

/// <summary>Card-specific rules text of Siren's Call, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Siren's Call</exampledoc>
[TokenizationOrder(-1)]
public class SirensCallCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast this spell only during an opponent's turn, before attackers are declared"];
}

/// <summary>Card-specific effect of Skeleton Scavengers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skeleton Scavengers</exampledoc>
[Dependent]
public class SkeletonScavengersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["regenerate this creature"];
}

/// <summary>Card-specific effect of Skyship Weatherlight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skyship Weatherlight</exampledoc>
[Dependent]
public class SkyshipWeatherlightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a card at random that was exiled with", Nib.This];
}

/// <summary>Card-specific rules text of Soul Immolation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Immolation</exampledoc>
[TokenizationOrder(-1)]
public class SoulImmolationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the greatest toughness among creatures you control"];
}

/// <summary>Card-specific rules text of Soul Parry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Parry</exampledoc>
[TokenizationOrder(-1)]
public class SoulParryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage one or two target creatures would deal this turn"];
}

/// <summary>Card-specific rules text of Sparring Dummy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sparring Dummy</exampledoc>
[TokenizationOrder(-1)]
public class SparringDummyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 2 life if a lesson card is milled this way"];
}

/// <summary>Card-specific effect of Spatial Binding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spatial Binding</exampledoc>
[Dependent]
public class SpatialBindingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next upkeep, target permanent can't phase out"];
}

/// <summary>Card-specific effect of Spell Blast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spell Blast</exampledoc>
[Dependent]
public class SpellBlastEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if that spell's mana cost is {3}{u}{u}, x is 5"];
}

/// <summary>Card-specific rules text of Spider-Woman, Secret Agent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Woman, Secret Agent</exampledoc>
[TokenizationOrder(-1)]
public class SpiderWomanSecretAgentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can't become untapped for as long as you control spider-woman"];
}

/// <summary>Card-specific rules text of Split the Party, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Split the Party</exampledoc>
[TokenizationOrder(-1)]
public class SplitThePartyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return half the creatures they control to their owner's hand, rounded up"];
}

/// <summary>Card-specific rules text of Squee's Revenge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squee's Revenge</exampledoc>
[TokenizationOrder(-1)]
public class SqueesRevengeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win all the flips, draw two cards for each flip"];
}

/// <summary>Card-specific rules text of Storage Matrix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storage Matrix</exampledoc>
[TokenizationOrder(-1)]
public class StorageMatrixCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player can untap only permanents of the chosen type this step"];
}

/// <summary>Card-specific rules text of Storm the Seedcore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storm the Seedcore</exampledoc>
[TokenizationOrder(-1)]
public class StormTheSeedcoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute four +1/+1 counters among up to four target creatures you control"];
}

/// <summary>Card-specific rules text of Stormscale Anarch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stormscale Anarch</exampledoc>
[TokenizationOrder(-1)]
public class StormscaleAnarchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the discarded card was multicolored, this creature deals 4 damage instead"];
}

/// <summary>Card-specific rules text of Stormwild Capridor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stormwild Capridor</exampledoc>
[TokenizationOrder(-1)]
public class StormwildCapridorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if noncombat damage would be dealt to this creature, prevent that damage"];
}

/// <summary>Card-specific rules text of Struggle for Sanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Struggle for Sanity</exampledoc>
[TokenizationOrder(-1)]
public class StruggleForSanityCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until all cards in that hand have been exiled"];
}

/// <summary>Card-specific effect of Subterranean Schooner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Subterranean Schooner</exampledoc>
[Dependent]
public class SubterraneanSchoonerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature that crewed it this turn explores"];
}

/// <summary>Card-specific effect of Surge Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Surge Engine</exampledoc>
[Dependent]
public class SurgeEngineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature becomes blue and has base power and toughness 5/4"];
}

/// <summary>Card-specific rules text of Sword of Wealth and Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of Wealth and Power</exampledoc>
[TokenizationOrder(-1)]
public class SwordOfWealthAndPowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +2/+2 and has protection from instants and from sorceries"];
}

/// <summary>Card-specific rules text of Sylvan Library, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sylvan Library</exampledoc>
[TokenizationOrder(-1)]
public class SylvanLibraryCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, choose two cards in your hand drawn this turn"];
}

/// <summary>Card-specific rules text of Tannuk, Steadfast Second, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tannuk, Steadfast Second</exampledoc>
[TokenizationOrder(-1)]
public class TannukSteadfastSecondRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifact cards and red creature cards in your hand have warp {2}{r}"];
}

/// <summary>Card-specific effect of Tannuk, Steadfast Second, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tannuk, Steadfast Second</exampledoc>
[Dependent]
public class TannukSteadfastSecond2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a card from your hand for its warp cost"];
}

/// <summary>Card-specific rules text of Tawnos's Coffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tawnos's Coffin</exampledoc>
[TokenizationOrder(-1)]
public class TawnossCoffinCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["note the number and kind of counters that were on that creature"];
}

/// <summary>Card-specific rules text of Teferi, Temporal Pilgrim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Temporal Pilgrim</exampledoc>
[TokenizationOrder(-1)]
public class TeferiTemporalPilgrimRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then they shuffle each nonland permanent they control into its owner's library"];
}

/// <summary>Card-specific rules text of Tempest Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tempest Efreet</exampledoc>
[TokenizationOrder(-1)]
public class TempestEfreetCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this change in ownership is permanent"];
}

/// <summary>Card-specific rules text of Tezzeret, Cruel Captain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tezzeret, Cruel Captain</exampledoc>
[TokenizationOrder(-1)]
public class TezzeretCruelCaptainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's not a creature, it becomes a 0/0 robot artifact creature"];
}

/// <summary>Card-specific rules text of The Biblioplex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Biblioplex</exampledoc>
[TokenizationOrder(-1)]
public class TheBiblioplexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if you have exactly zero or seven cards in hand"];
}

/// <summary>Card-specific rules text of The Phasing of Zhalfir, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Phasing of Zhalfir</exampledoc>
[TokenizationOrder(-1)]
public class ThePhasingOfZhalfirRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can't phase in for as long as you control this saga"];
}

/// <summary>Card-specific rules text of The Tarrasque, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Tarrasque</exampledoc>
[TokenizationOrder(-1)]
public class TheTarrasqueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "has haste and ward {10} as long as it was cast"];
}

/// <summary>Card-specific rules text of Thieves' Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thieves' Auction</exampledoc>
[TokenizationOrder(-1)]
public class ThievesAuctionCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until all cards exiled this way have been chosen"];
}

/// <summary>Card-specific rules text of Thran Portal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thran Portal</exampledoc>
[TokenizationOrder(-1)]
public class ThranPortalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mana abilities of this land cost an additional 1 life to activate"];
}

/// <summary>Card-specific effect of Time Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Elemental</exampledoc>
[Dependent]
public class TimeElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target permanent that isn't enchanted to its owner's hand"];
}

/// <summary>Card-specific effect of Toluz, Clever Conductor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toluz, Clever Conductor</exampledoc>
[Dependent]
public class ToluzCleverConductorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile them from your graveyard"];
}

/// <summary>Card-specific rules text of Toxrill, the Corrosive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toxrill, the Corrosive</exampledoc>
[TokenizationOrder(-1)]
public class ToxrillTheCorrosiveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you don't control get -1/-1 for each slime counter on them"];
}

/// <summary>Card-specific rules text of Transcendent Archaic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transcendent Archaic</exampledoc>
[TokenizationOrder(-1)]
public class TranscendentArchaicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you draw one or more cards this way, discard two cards"];
}

/// <summary>Card-specific rules text of Transmute Artifact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transmute Artifact</exampledoc>
[TokenizationOrder(-1)]
public class TransmuteArtifactRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's greater, you may pay {x}, where x is the difference"];
}

/// <summary>Card-specific effect of Triad of Fates, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triad of Fates</exampledoc>
[Dependent]
public class TriadOfFatesCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target creature that has a fate counter on it"];
}

/// <summary>Card-specific rules text of Tromokratis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tromokratis</exampledoc>
[TokenizationOrder(-1)]
public class TromokratisCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "can't be blocked unless all creatures defending player controls block it"];
}

/// <summary>Card-specific rules text of Unified Will, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unified Will</exampledoc>
[TokenizationOrder(-1)]
public class UnifiedWillRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell if you control more creatures than that spell's controller"];
}

/// <summary>Card-specific effect of Valki, God of Lies // Tibalt, Cosmic Impostor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Valki, God of Lies // Tibalt, Cosmic Impostor</exampledoc>
[Dependent]
public class ValkiGodOfLiesCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature card exiled with valki with mana value x"];
}

/// <summary>Card-specific effect of Vodalian Mystic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vodalian Mystic</exampledoc>
[Dependent]
public class VodalianMysticEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target instant or sorcery spell becomes the color of your choice"];
}

/// <summary>Card-specific rules text of Void, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void</exampledoc>
[TokenizationOrder(-1)]
public class VoidCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all artifacts and creatures with mana value equal to that number"];
}

/// <summary>Card-specific effect of Wakeroot Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wakeroot Elemental</exampledoc>
[Dependent]
public class WakerootElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect lasts as long as that land remains on the battlefield"];
}

/// <summary>Card-specific rules text of Ward of Bones, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ward of Bones</exampledoc>
[TokenizationOrder(-1)]
public class WardOfBonesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who controls more creatures than you can't cast creature spells"];
}

/// <summary>Card-specific rules text of Well of Knowledge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Well of Knowledge</exampledoc>
[TokenizationOrder(-1)]
public class WellOfKnowledgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only during their draw step"];
}

/// <summary>Card-specific rules text of Winter's Chill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winter's Chill</exampledoc>
[TokenizationOrder(-1)]
public class WintersChillCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the number of snow lands you control"];
}

/// <summary>Card-specific rules text of Wort, the Raidmother, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wort, the Raidmother</exampledoc>
[TokenizationOrder(-1)]
public class WortTheRaidmotherCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each red or green instant or sorcery spell you cast has conspire"];
}

/// <summary>Card-specific effect of Zendikar Resurgent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zendikar Resurgent</exampledoc>
[Dependent]
public class ZendikarResurgentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the types of mana are white, blue, black, red, green, and colorless"];
}

/// <summary>Card-specific effect of Absolving Lammasu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Absolving Lammasu</exampledoc>
[Dependent]
public class AbsolvingLammasuEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all suspected creatures are no longer suspected"];
}

/// <summary>Card-specific effect of Ajani, Sleeper Agent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ajani, Sleeper Agent</exampledoc>
[Dependent]
public class AjaniSleeperAgentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute three +1/+1 counters among up to three target creatures"];
}

/// <summary>Card-specific rules text of Alhammarret, High Arbiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alhammarret, High Arbiter</exampledoc>
[TokenizationOrder(-1)]
public class AlhammarretHighArbiterCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose the name of a nonland card revealed this way"];
}

/// <summary>Card-specific effect of All-Out Assault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All-Out Assault</exampledoc>
[Dependent]
public class AllOutAssaultEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap each creature you control"];
}

/// <summary>Card-specific rules text of Amalia Benavides Aguirre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Amalia Benavides Aguirre</exampledoc>
[TokenizationOrder(-1)]
public class AmaliaBenavidesAguirreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all other creatures if its power is exactly 20"];
}

/// <summary>Card-specific rules text of Angrath, Minotaur Pirate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angrath, Minotaur Pirate</exampledoc>
[TokenizationOrder(-1)]
public class AngrathMinotaurPirateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["angrath deals damage to that player equal to their total power"];
}

/// <summary>Card-specific effect of Anzrag, the Quake-Mole, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anzrag, the Quake-Mole</exampledoc>
[Dependent]
public class AnzragTheQuakeMoleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["anzrag must be blocked each combat this turn if able"];
}

/// <summary>Card-specific rules text of Ashling's Prerogative, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashling's Prerogative</exampledoc>
[TokenizationOrder(-1)]
public class AshlingsPrerogativeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature with mana value of the chosen quality has haste"];
}

/// <summary>Card-specific rules text of Ashling's Prerogative, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashling's Prerogative</exampledoc>
[TokenizationOrder(-1)]
public class AshlingsPrerogative2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature without mana value of the chosen quality enters tapped"];
}

/// <summary>Card-specific rules text of Auratouched Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Auratouched Mage</exampledoc>
[TokenizationOrder(-1)]
public class AuratouchedMageCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, reveal the aura card and put it into your hand"];
}

/// <summary>Card-specific rules text of Aurelia's Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aurelia's Fury</exampledoc>
[TokenizationOrder(-1)]
public class AureliasFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players dealt damage this way can't cast noncreature spells this turn"];
}

/// <summary>Card-specific rules text of Avatar Aang // Aang, Master of Elements, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar Aang // Aang, Master of Elements</exampledoc>
[TokenizationOrder(-1)]
public class AvatarAangRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if you've done all four this turn, transform avatar aang"];
}

/// <summary>Card-specific rules text of Avatar's Wrath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar's Wrath</exampledoc>
[TokenizationOrder(-1)]
public class AvatarsWrathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose up to one target creature, then airbend all other creatures"];
}

/// <summary>Card-specific rules text of Awakening of Vitu-Ghazi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Awakening of Vitu-Ghazi</exampledoc>
[TokenizationOrder(-1)]
public class AwakeningOfVituGhaziRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it becomes a legendary 0/0 elemental creature with haste named vitu-ghazi"];
}

/// <summary>Card-specific effect of Bria, Riptide Rogue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bria, Riptide Rogue</exampledoc>
[Dependent]
public class BriaRiptideRogueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature has multiple instances of prowess, each triggers separately"];
}

/// <summary>Card-specific rules text of Chain of Plasma, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chain of Plasma</exampledoc>
[TokenizationOrder(-1)]
public class ChainOfPlasmaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player or that permanent's controller may discard a card"];
}

/// <summary>Card-specific rules text of Covenant of Minds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Covenant of Minds</exampledoc>
[TokenizationOrder(-1)]
public class CovenantOfMindsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent may choose to put those cards into your hand"];
}

/// <summary>Card-specific rules text of Cut Short, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cut Short</exampledoc>
[TokenizationOrder(-1)]
public class CutShortRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target planeswalker that was activated this turn or tapped creature"];
}

/// <summary>Card-specific rules text of Dark Matter Manipulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dark Matter Manipulator</exampledoc>
[TokenizationOrder(-1)]
public class DarkMatterManipulatorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +2/+0 for every seven cards in your graveyard"];
}

/// <summary>Card-specific rules text of Debt of Loyalty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Debt of Loyalty</exampledoc>
[TokenizationOrder(-1)]
public class DebtOfLoyaltyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain control of that creature if it regenerates this way"];
}

/// <summary>Card-specific rules text of Drain Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drain Power</exampledoc>
[TokenizationOrder(-1)]
public class DrainPowerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player activates a mana ability of each land they control"];
}

/// <summary>Card-specific rules text of Elvish Healer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elvish Healer</exampledoc>
[TokenizationOrder(-1)]
public class ElvishHealerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a green creature, prevent the next 2 damage instead"];
}

/// <summary>Card-specific rules text of Empty-Shrine Kannushi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Empty-Shrine Kannushi</exampledoc>
[TokenizationOrder(-1)]
public class EmptyShrineKannushiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has protection from each color among permanents you control"];
}

/// <summary>Card-specific effect of Erayo, Soratami Ascendant // Erayo's Essence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Erayo, Soratami Ascendant // Erayo's Essence</exampledoc>
[Dependent]
public class ErayoSoratamiAscendantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip erayo"];
}

/// <summary>Card-specific rules text of Ertai's Meddling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Meddling</exampledoc>
[TokenizationOrder(-1)]
public class ErtaisMeddlingCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target spell's controller exiles it with x delay counters on it"];
}

/// <summary>Card-specific rules text of Etched Host Doombringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Etched Host Doombringer</exampledoc>
[TokenizationOrder(-1)]
public class EtchedHostDoombringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent protects it, remove three defense counters from it"];
}

/// <summary>Card-specific effect of Exiled Doomsayer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exiled Doomsayer</exampledoc>
[Dependent]
public class ExiledDoomsayerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this doesn't affect the cost to cast creature spells face down"];
}

/// <summary>Card-specific effect of Falkenrath Gorger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falkenrath Gorger</exampledoc>
[Dependent]
public class FalkenrathGorger2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you discard a card with madness, discard it into exile"];
}

/// <summary>Card-specific rules text of False Orders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>False Orders</exampledoc>
[TokenizationOrder(-1)]
public class FalseOrders2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have it block an attacking creature of your choice"];
}

/// <summary>Card-specific rules text of Fblthp, Lost on the Range, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fblthp, Lost on the Range</exampledoc>
[TokenizationOrder(-1)]
public class FblthpLostOnTheRangeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may plot nonland cards from the top of your library"];
}

/// <summary>Card-specific rules text of Flash of Insight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flash of Insight</exampledoc>
[TokenizationOrder(-1)]
public class FlashOfInsightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't exile", Nib.This, "to pay for its own flashback cost"];
}

/// <summary>Card-specific effect of Forsaken Monument, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forsaken Monument</exampledoc>
[Dependent]
public class ForsakenMonumentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an additional {c}"];
}
