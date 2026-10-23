/// <summary>Card-specific rules text of Primitive Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primitive Justice</exampledoc>
[TokenizationOrder(-1)]
public class PrimitiveJusticeCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each additional {1}{r} you paid, destroy another target artifact"];
}

/// <summary>Card-specific rules text of Public Enemy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Public Enemy</exampledoc>
[TokenizationOrder(-1)]
public class PublicEnemyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all creatures attack enchanted creature's controller each combat if able"];
}

/// <summary>Card-specific rules text of Rasputin Dreamweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rasputin Dreamweaver</exampledoc>
[TokenizationOrder(-1)]
public class RasputinDreamweaverCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["rasputin can't have more than seven dream counters on it"];
}

/// <summary>Card-specific rules text of Rebirth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rebirth</exampledoc>
[TokenizationOrder(-1)]
public class RebirthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may ante the top card of their library"];
}

/// <summary>Card-specific rules text of Reversal of Fortune, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reversal of Fortune</exampledoc>
[TokenizationOrder(-1)]
public class ReversalOfFortuneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may copy an instant or sorcery card in it"];
}

/// <summary>Card-specific rules text of Rohgahh of Kher Keep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rohgahh of Kher Keep</exampledoc>
[TokenizationOrder(-1)]
public class RohgahhOfKherKeepCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control named kobolds of kher keep get +2/+2"];
}

/// <summary>Card-specific effect of SP//dr, Piloted by Peni, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>SP//dr, Piloted by Peni</exampledoc>
[Dependent]
public class SPDrPilotedByPeniEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature"];
}

/// <summary>Card-specific rules text of Search for Survivors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Search for Survivors</exampledoc>
[TokenizationOrder(-1)]
public class SearchForSurvivorsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["an opponent chooses a card at random in your graveyard"];
}

/// <summary>Card-specific rules text of Second Guess, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Second Guess</exampledoc>
[TokenizationOrder(-1)]
public class SecondGuessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell that's the second spell cast this turn"];
}

/// <summary>Card-specific rules text of Seraph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seraph</exampledoc>
[TokenizationOrder(-1)]
public class SeraphRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice the creature when you lose control of this creature"];
}

/// <summary>Card-specific rules text of Serum Snare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serum Snare</exampledoc>
[TokenizationOrder(-1)]
public class SerumSnareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that permanent had mana value 3 or less, proliferate"];
}

/// <summary>Card-specific rules text of Seven Dwarves, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seven Dwarves</exampledoc>
[TokenizationOrder(-1)]
public class SevenDwarvesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a deck can have up to seven cards named", Nib.This];
}

/// <summary>Card-specific rules text of Silverquill, the Disputant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silverquill, the Disputant</exampledoc>
[TokenizationOrder(-1)]
public class SilverquillTheDisputantCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instant and sorcery spell you cast has casualty 1"];
}

/// <summary>Card-specific rules text of Siren's Call, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Siren's Call</exampledoc>
[TokenizationOrder(-1)]
public class SirensCallCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures the active player controls attack this turn if able"];
}

/// <summary>Card-specific rules text of Solemnity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Solemnity</exampledoc>
[TokenizationOrder(-1)]
public class SolemnityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counters can't be put on artifacts, creatures, enchantments, or lands"];
}

/// <summary>Card-specific effect of Sorin, Lord of Innistrad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorin, Lord of Innistrad</exampledoc>
[Dependent]
public class SorinLordOfInnistradEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy up to three target creatures and/or other planeswalkers"];
}

/// <summary>Card-specific rules text of Sorrow's Path, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorrow's Path</exampledoc>
[TokenizationOrder(-1)]
public class SorrowsPathCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each one then blocks all creatures the other was blocking"];
}

/// <summary>Card-specific effect of Soulgorger Orgg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soulgorger Orgg</exampledoc>
[Dependent]
public class SoulgorgerOrggCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose all but 1 life"];
}

/// <summary>Card-specific rules text of Spectral Searchlight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spectral Searchlight</exampledoc>
[TokenizationOrder(-1)]
public class SpectralSearchlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player adds one mana of any color they choose"];
}

/// <summary>Card-specific rules text of Spelltwine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spelltwine</exampledoc>
[TokenizationOrder(-1)]
public class SpelltwineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast the copies if able without paying their mana costs"];
}

/// <summary>Card-specific rules text of Spider-Verse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Verse</exampledoc>
[TokenizationOrder(-1)]
public class SpiderVerseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the copy is a permanent spell, it gains haste"];
}

/// <summary>Card-specific rules text of Stand or Fall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stand or Fall</exampledoc>
[TokenizationOrder(-1)]
public class StandOrFallCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only creatures in the chosen piles can block this turn"];
}

/// <summary>Card-specific rules text of Strategic Betrayal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strategic Betrayal</exampledoc>
[TokenizationOrder(-1)]
public class StrategicBetrayalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent exiles a creature they control and their graveyard"];
}

/// <summary>Card-specific effect of Strionic Resonator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strionic Resonator</exampledoc>
[Dependent]
public class StrionicResonatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a triggered ability uses the words \"when,\" \"whenever,\" or \"at"];
}

/// <summary>Card-specific rules text of Telekinesis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Telekinesis</exampledoc>
[TokenizationOrder(-1)]
public class TelekinesisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it doesn't untap during its controller's next two untap steps"];
}

/// <summary>Card-specific effect of Temporal Aperture, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temporal Aperture</exampledoc>
[Dependent]
public class TemporalApertureCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then reveal the top card"];
}

/// <summary>Card-specific rules text of Thassa's Intervention, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thassa's Intervention</exampledoc>
[TokenizationOrder(-1)]
public class ThassasInterventionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• counter target spell unless its controller pays twice {x}"];
}

/// <summary>Card-specific rules text of The Deck of Many Things, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Deck of Many Things</exampledoc>
[TokenizationOrder(-1)]
public class TheDeckOfManyThingsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the result is 0 or less, discard your hand"];
}

/// <summary>Card-specific effect of The Goose Mother, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Goose Mother</exampledoc>
[Dependent]
public class TheGooseMotherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create half x food tokens, rounded up"];
}

/// <summary>Card-specific rules text of Thermal Flux, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thermal Flux</exampledoc>
[TokenizationOrder(-1)]
public class ThermalFluxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target nonsnow permanent becomes snow until end of turn"];
}

/// <summary>Card-specific rules text of Thermal Flux, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thermal Flux</exampledoc>
[TokenizationOrder(-1)]
public class ThermalFlux2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target snow permanent isn't snow until end of turn"];
}

/// <summary>Card-specific effect of Torrent Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Torrent Elemental</exampledoc>
[Dependent]
public class TorrentElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from exile onto the battlefield tapped"];
}

/// <summary>Card-specific rules text of Turn the Earth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Turn the Earth</exampledoc>
[TokenizationOrder(-1)]
public class TurnTheEarthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the owners of those cards shuffle them into their libraries"];
}

/// <summary>Card-specific effect of Unyaro Griffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unyaro Griffin</exampledoc>
[Dependent]
public class UnyaroGriffinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target red instant or sorcery spell"];
}

/// <summary>Card-specific rules text of Void Winnower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void Winnower</exampledoc>
[TokenizationOrder(-1)]
public class VoidWinnowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't block with creatures with even mana values"];
}

/// <summary>Card-specific rules text of Wash Away, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wash Away</exampledoc>
[TokenizationOrder(-1)]
public class WashAwayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell [that wasn't cast from its owner's hand]"];
}

/// <summary>Card-specific rules text of Watcher in the Web, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Watcher in the Web</exampledoc>
[TokenizationOrder(-1)]
public class WatcherInTheWebRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can block an additional seven creatures each combat"];
}

/// <summary>Card-specific effect of Whip Vine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whip Vine</exampledoc>
[Dependent]
public class WhipVineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap target creature with flying blocked by this creature"];
}

/// <summary>Card-specific rules text of Winged Portent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winged Portent</exampledoc>
[TokenizationOrder(-1)]
public class WingedPortentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each creature you control [with flying]"];
}

/// <summary>Card-specific rules text of Wirefly Hive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wirefly Hive</exampledoc>
[TokenizationOrder(-1)]
public class WireflyHiveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, destroy all permanents named wirefly"];
}

/// <summary>Card-specific effect of Witchbane Orb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Witchbane Orb</exampledoc>
[Dependent]
public class WitchbaneOrbCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all curses attached to you"];
}

/// <summary>Card-specific rules text of Aether Gust, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aether Gust</exampledoc>
[TokenizationOrder(-1)]
public class AetherGustRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target spell or permanent that's red or green"];
}

/// <summary>Card-specific rules text of Ancient Adamantoise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ancient Adamantoise</exampledoc>
[TokenizationOrder(-1)]
public class AncientAdamantoiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage isn't removed from this creature during cleanup steps"];
}

/// <summary>Card-specific effect of Arcum's Weathervane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arcum's Weathervane</exampledoc>
[Dependent]
public class ArcumsWeathervaneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target snow land is no longer snow"];
}

/// <summary>Card-specific rules text of Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balance</exampledoc>
[TokenizationOrder(-1)]
public class BalanceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players discard cards and sacrifice creatures the same way"];
}

/// <summary>Card-specific rules text of Blaze of Glory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blaze of Glory</exampledoc>
[TokenizationOrder(-1)]
public class BlazeOfGloryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it blocks each attacking creature this turn if able"];
}

/// <summary>Card-specific rules text of Bumi, Unleashed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bumi, Unleashed</exampledoc>
[TokenizationOrder(-1)]
public class BumiUnleashedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only land creatures can attack during that combat phase"];
}

/// <summary>Card-specific effect of Call In a Professional, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call In a Professional</exampledoc>
[Dependent]
public class CallInAProfessionalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shield counters don't prevent this damage as they're removed"];
}

/// <summary>Card-specific rules text of Caterwauling Boggart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Caterwauling Boggart</exampledoc>
[TokenizationOrder(-1)]
public class CaterwaulingBoggartRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["goblins you control and elementals you control have menace"];
}

/// <summary>Card-specific effect of Champion of the Weird, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Champion of the Weird</exampledoc>
[Dependent]
public class ChampionOfTheWeirdEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent blights 2"];
}

/// <summary>Card-specific effect of Clash of the Eikons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clash of the Eikons</exampledoc>
[Dependent]
public class ClashOfTheEikonsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["removing lore counters doesn't cause chapter abilities to trigger"];
}

/// <summary>Card-specific rules text of Clockspinning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clockspinning</exampledoc>
[TokenizationOrder(-1)]
public class ClockspinningCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a counter on target permanent or suspended card"];
}

/// <summary>Card-specific effect of Cowardice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cowardice</exampledoc>
[Dependent]
public class CowardiceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it won't be affected by the spell or ability"];
}

/// <summary>Card-specific rules text of Crystalline Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crystalline Giant</exampledoc>
[TokenizationOrder(-1)]
public class CrystallineGiantCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a counter of that kind on this creature"];
}

/// <summary>Card-specific effect of Curator of Sun's Creation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curator of Sun's Creation</exampledoc>
[Dependent]
public class CuratorOfSunsCreationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discover again for the same value"];
}

/// <summary>Card-specific rules text of Darksteel Forge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darksteel Forge</exampledoc>
[TokenizationOrder(-1)]
public class DarksteelForgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifact creatures with indestructible can't be destroyed by damage"];
}

/// <summary>Card-specific rules text of Demonic Attorney, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonic Attorney</exampledoc>
[TokenizationOrder(-1)]
public class DemonicAttorneyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player antes the top card of their library"];
}

/// <summary>Card-specific rules text of Desperate Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desperate Gambit</exampledoc>
[TokenizationOrder(-1)]
public class DesperateGambitCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a source you control and flip a coin"];
}

/// <summary>Card-specific rules text of Disrupting Shoal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Disrupting Shoal</exampledoc>
[TokenizationOrder(-1)]
public class DisruptingShoalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell if its mana value is x"];
}

/// <summary>Card-specific rules text of Djinn Illuminatus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Djinn Illuminatus</exampledoc>
[TokenizationOrder(-1)]
public class DjinnIlluminatusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instant and sorcery spell you cast has replicate"];
}

/// <summary>Card-specific rules text of Dreadlight Monstrosity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dreadlight Monstrosity</exampledoc>
[TokenizationOrder(-1)]
public class DreadlightMonstrosityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if you own a card in exile"];
}

/// <summary>Card-specific rules text of Dubious Challenge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dubious Challenge</exampledoc>
[TokenizationOrder(-1)]
public class DubiousChallengeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the rest onto the battlefield under your control"];
}

/// <summary>Card-specific effect of Edgar's Awakening, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Edgar's Awakening</exampledoc>
[Dependent]
public class EdgarsAwakeningEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {b}"];
}

/// <summary>Card-specific rules text of Elemental Resonance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elemental Resonance</exampledoc>
[TokenizationOrder(-1)]
public class ElementalResonanceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a mana symbol has multiple colors, choose one"];
}

/// <summary>Card-specific effect of Embodiment of Agonies, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Embodiment of Agonies</exampledoc>
[Dependent]
public class EmbodimentOfAgoniesCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, {2}{b} and {1}{b}{b} are different mana costs"];
}

/// <summary>Card-specific rules text of Emrakul, the Promised End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emrakul, the Promised End</exampledoc>
[TokenizationOrder(-1)]
public class EmrakulThePromisedEndRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["after that turn, that player takes an extra turn"];
}

/// <summary>Card-specific rules text of End Hostilities, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>End Hostilities</exampledoc>
[TokenizationOrder(-1)]
public class EndHostilitiesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures and all permanents attached to creatures"];
}

/// <summary>Card-specific rules text of Eye of Yawgmoth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eye of Yawgmoth</exampledoc>
[TokenizationOrder(-1)]
public class EyeOfYawgmothRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one into your hand and exile the rest"];
}

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitCard5Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose a flip,", Nib.This, "has no effect"];
}

/// <summary>Card-specific effect of Fire Nation Attacks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fire Nation Attacks</exampledoc>
[Dependent]
public class FireNationAttacksEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature with firebending 1 attacks, add {r}"];
}

/// <summary>Card-specific rules text of Flash of Defiance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flash of Defiance</exampledoc>
[TokenizationOrder(-1)]
public class FlashOfDefianceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["green creatures and white creatures can't block this turn"];
}

/// <summary>Card-specific rules text of Flashback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flashback</exampledoc>
[TokenizationOrder(-1)]
public class FlashbackCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the", Nib.This, "cost is equal to its mana cost"];
}

/// <summary>Card-specific rules text of Foraging Wickermaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Foraging Wickermaw</exampledoc>
[TokenizationOrder(-1)]
public class ForagingWickermawRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature becomes that color until end of turn"];
}

/// <summary>Card-specific rules text of Frantic Scapegoat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frantic Scapegoat</exampledoc>
[TokenizationOrder(-1)]
public class FranticScapegoatCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, this creature is no longer suspected"];
}

/// <summary>Card-specific rules text of Frozen in Ice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frozen in Ice</exampledoc>
[TokenizationOrder(-1)]
public class FrozenInIceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature loses all abilities and can't become untapped"];
}

/// <summary>Card-specific rules text of Fulgent Distraction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fulgent Distraction</exampledoc>
[TokenizationOrder(-1)]
public class FulgentDistractionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap those creatures, then unattach all equipment from them"];
}

/// <summary>Card-specific effect of Gideon, the Oathsworn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon, the Oathsworn</exampledoc>
[Dependent]
public class GideonTheOathswornEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["he can't attack if he was cast this turn"];
}

/// <summary>Card-specific effect of Gideon, the Oathsworn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon, the Oathsworn</exampledoc>
[Dependent]
public class GideonTheOathsworn2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile gideon and each creature your opponents control"];
}

/// <summary>Card-specific rules text of Gilded Drake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilded Drake</exampledoc>
[TokenizationOrder(-1)]
public class GildedDrake2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability still resolves if its target becomes illegal"];
}

/// <summary>Card-specific rules text of Grifter's Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grifter's Blade</exampledoc>
[TokenizationOrder(-1)]
public class GriftersBladeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, it enters attached to that creature"];
}

/// <summary>Card-specific rules text of Gruul Spellbreaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gruul Spellbreaker</exampledoc>
[TokenizationOrder(-1)]
public class GruulSpellbreakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, you and this creature have hexproof"];
}

/// <summary>Card-specific rules text of Hallow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hallow</exampledoc>
[TokenizationOrder(-1)]
public class HallowRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage target spell would deal this turn"];
}

/// <summary>Card-specific rules text of Hazoret's Undying Fury, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hazoret's Undying Fury</exampledoc>
[TokenizationOrder(-1)]
public class HazoretsUndyingFuryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle your library, then exile the top four cards"];
}

/// <summary>Card-specific rules text of Herald of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Leshrac</exampledoc>
[TokenizationOrder(-1)]
public class HeraldOfLeshracCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—gain control of a land you don't control"];
}

/// <summary>Card-specific rules text of Highway Robbery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Highway Robbery</exampledoc>
[TokenizationOrder(-1)]
public class HighwayRobberyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may discard a card or sacrifice a land"];
}

/// <summary>Card-specific effect of Homura, Human Ascendant // Homura's Essence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Homura, Human Ascendant // Homura's Essence</exampledoc>
[Dependent]
public class HomuraHumanAscendantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return it to the battlefield flipped"];
}

/// <summary>Card-specific rules text of Identity Crisis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Identity Crisis</exampledoc>
[TokenizationOrder(-1)]
public class IdentityCrisisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from target player's hand and graveyard"];
}

/// <summary>Card-specific rules text of Illicit Auction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illicit Auction</exampledoc>
[TokenizationOrder(-1)]
public class IllicitAuctionCard5Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you start the bidding with a bid of 0"];
}

/// <summary>Card-specific rules text of Incriminate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Incriminate</exampledoc>
[TokenizationOrder(-1)]
public class IncriminateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player sacrifices one of them of their choice"];
}

/// <summary>Card-specific rules text of Interdict, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Interdict</exampledoc>
[TokenizationOrder(-1)]
public class Interdict2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that permanent's activated abilities can't be activated this turn"];
}

/// <summary>Card-specific rules text of Irencrag Feat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Irencrag Feat</exampledoc>
[TokenizationOrder(-1)]
public class IrencragFeatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can cast only one more spell this turn"];
}

/// <summary>Card-specific effect of Ivy, Gleeful Spellthief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ivy, Gleeful Spellthief</exampledoc>
[Dependent]
public class IvyGleefulSpellthiefCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a copy of an aura spell becomes a token"];
}

/// <summary>Card-specific rules text of Kindred Judgment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kindred Judgment</exampledoc>
[TokenizationOrder(-1)]
public class KindredJudgmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that aren't of the chosen type"];
}

/// <summary>Card-specific rules text of Krovikan Vampire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Krovikan Vampire</exampledoc>
[TokenizationOrder(-1)]
public class KrovikanVampireCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice it when you lose control of this creature"];
}

/// <summary>Card-specific rules text of Living Lands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Lands</exampledoc>
[TokenizationOrder(-1)]
public class LivingLandsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all forests are 1/1 creatures that are still lands"];
}

/// <summary>Card-specific rules text of Living Plane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Plane</exampledoc>
[TokenizationOrder(-1)]
public class LivingPlaneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands are 1/1 creatures that are still lands"];
}

/// <summary>Card-specific rules text of Louisoix's Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Louisoix's Sacrifice</exampledoc>
[TokenizationOrder(-1)]
public class LouisoixsSacrificeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability, triggered ability, or noncreature spell"];
}

/// <summary>Card-specific effect of Luke Cage, Power Man, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luke Cage, Power Man</exampledoc>
[Dependent]
public class LukeCagePowerManEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy him"];
}

/// <summary>Card-specific rules text of Lyzolda, the Blood Witch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lyzolda, the Blood Witch</exampledoc>
[TokenizationOrder(-1)]
public class LyzoldaTheBloodWitchCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card if the sacrificed creature was black"];
}

/// <summary>Card-specific rules text of Mages' Contest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mages' Contest</exampledoc>
[TokenizationOrder(-1)]
public class MagesContest2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you start the bidding with a bid of 1"];
}
