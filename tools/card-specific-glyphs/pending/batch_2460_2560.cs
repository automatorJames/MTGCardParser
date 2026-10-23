/// <summary>Card-specific rules text of Gaddock Teeg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gaddock Teeg</exampledoc>
[TokenizationOrder(-1)]
public class GaddockTeegRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["noncreature spells with {x} in their mana costs can't be cast"];
}

/// <summary>Card-specific effect of Garruk's Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garruk's Horde</exampledoc>
[Dependent]
public class GarruksHordeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["do this only any time you could cast that creature spell"];
}

/// <summary>Card-specific rules text of Getaway Barrel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Getaway Barrel</exampledoc>
[TokenizationOrder(-1)]
public class GetawayBarrelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a random creature card from among them onto the battlefield"];
}

/// <summary>Card-specific rules text of Ghostly Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghostly Flame</exampledoc>
[TokenizationOrder(-1)]
public class GhostlyFlameRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["black and/or red permanents and spells are colorless sources of damage"];
}

/// <summary>Card-specific rules text of Giant Ox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Ox</exampledoc>
[TokenizationOrder(-1)]
public class GiantOxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature crews vehicles using its toughness rather than its power"];
}

/// <summary>Card-specific rules text of Gilded Drake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilded Drake</exampledoc>
[TokenizationOrder(-1)]
public class GildedDrakeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't or can't make an exchange, sacrifice this creature"];
}

/// <summary>Card-specific rules text of Glyph of Reincarnation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glyph of Reincarnation</exampledoc>
[TokenizationOrder(-1)]
public class GlyphOfReincarnationCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that were blocked by target wall this turn"];
}

/// <summary>Card-specific effect of Golden Argosy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Golden Argosy</exampledoc>
[Dependent]
public class GoldenArgosyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile each creature that crewed it this turn"];
}

/// <summary>Card-specific rules text of Golden Ratio, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Golden Ratio</exampledoc>
[TokenizationOrder(-1)]
public class GoldenRatioRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each different power among creatures you control"];
}

/// <summary>Card-specific rules text of Green Sun's Twilight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Green Sun's Twilight</exampledoc>
[TokenizationOrder(-1)]
public class GreenSunsTwilightCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature card and/or a land card from among them"];
}

/// <summary>Card-specific rules text of Grimoire of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grimoire of the Dead</exampledoc>
[TokenizationOrder(-1)]
public class GrimoireOfTheDeadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're black zombies in addition to their other colors and types"];
}

/// <summary>Card-specific rules text of Hall of Echoes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hall of Echoes</exampledoc>
[TokenizationOrder(-1)]
public class HallOfEchoesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply to permanents you control this turn"];
}

/// <summary>Card-specific rules text of Halvar, God of Battle // Sword of the Realms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Halvar, God of Battle // Sword of the Realms</exampledoc>
[TokenizationOrder(-1)]
public class HalvarGodOfBattleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control that are enchanted or equipped have double strike"];
}

/// <summary>Card-specific effect of Hammerheim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hammerheim</exampledoc>
[Dependent]
public class HammerheimEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature loses all landwalk abilities until end of turn"];
}

/// <summary>Card-specific rules text of Harsh Mercy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harsh Mercy</exampledoc>
[TokenizationOrder(-1)]
public class HarshMercyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that aren't of a type chosen this way"];
}

/// <summary>Card-specific rules text of Hellkite Hatchling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hellkite Hatchling</exampledoc>
[TokenizationOrder(-1)]
public class HellkiteHatchlingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has flying and trample if it devoured a creature"];
}

/// <summary>Card-specific rules text of Hive Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hive Mind</exampledoc>
[TokenizationOrder(-1)]
public class HiveMindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each of those players may choose new targets for their copy"];
}

/// <summary>Card-specific rules text of Hivis of the Scale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hivis of the Scale</exampledoc>
[TokenizationOrder(-1)]
public class HivisOfTheScaleCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose not to untap hivis during your untap step"];
}

/// <summary>Card-specific rules text of Hunting Wilds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hunting Wilds</exampledoc>
[TokenizationOrder(-1)]
public class HuntingWildsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they become 3/3 green creatures with haste that are still lands"];
}

/// <summary>Card-specific rules text of Imaginary Threats, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imaginary Threats</exampledoc>
[TokenizationOrder(-1)]
public class ImaginaryThreatsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during that player's next untap step, creatures they control don't untap"];
}

/// <summary>Card-specific rules text of Imperiosaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imperiosaur</exampledoc>
[TokenizationOrder(-1)]
public class ImperiosaurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only mana produced by basic lands to cast this spell"];
}

/// <summary>Card-specific rules text of Indomitable Creativity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Indomitable Creativity</exampledoc>
[TokenizationOrder(-1)]
public class IndomitableCreativityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players put the exiled cards onto the battlefield, then shuffle"];
}

/// <summary>Card-specific rules text of Interdict, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Interdict</exampledoc>
[TokenizationOrder(-1)]
public class InterdictRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability from an artifact, creature, enchantment, or land"];
}

/// <summary>Card-specific effect of Jace, Reality Sculptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, Reality Sculptor</exampledoc>
[Dependent]
public class JaceRealitySculptorCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all but the bottom card of each opponent's library"];
}

/// <summary>Card-specific rules text of Jeweled Amulet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeweled Amulet</exampledoc>
[TokenizationOrder(-1)]
public class JeweledAmuletCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["note the type of mana spent to pay this activation cost"];
}

/// <summary>Card-specific effect of Jhoira's Timebug, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jhoira's Timebug</exampledoc>
[Dependent]
public class JhoirasTimebugCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target permanent you control or suspended card you own"];
}

/// <summary>Card-specific rules text of Kasmina, Enigma Sage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kasmina, Enigma Sage</exampledoc>
[TokenizationOrder(-1)]
public class KasminaEnigmaSageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other planeswalker you control has the loyalty abilities of kasmina"];
}

/// <summary>Card-specific rules text of Koll, the Forgemaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koll, the Forgemaster</exampledoc>
[TokenizationOrder(-1)]
public class KollTheForgemasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creature tokens you control that are enchanted or equipped get +1/+1"];
}

/// <summary>Card-specific effect of Larval Scoutlander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Larval Scoutlander</exampledoc>
[Dependent]
public class LarvalScoutlanderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may sacrifice a land or lander"];
}

/// <summary>Card-specific effect of Leyline Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leyline Tyrant</exampledoc>
[Dependent]
public class LeylineTyrantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay any amount of {r}"];
}

/// <summary>Card-specific rules text of Liar's Pendulum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liar's Pendulum</exampledoc>
[TokenizationOrder(-1)]
public class LiarsPendulum2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do and your opponent guessed wrong, draw a card"];
}

/// <summary>Card-specific rules text of Light Up the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light Up the Night</exampledoc>
[TokenizationOrder(-1)]
public class LightUpTheNightCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast this spell this way, x can't be 0"];
}

/// <summary>Card-specific rules text of Liliana of the Veil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana of the Veil</exampledoc>
[TokenizationOrder(-1)]
public class LilianaOfTheVeilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player sacrifices all permanents in the pile of their choice"];
}

/// <summary>Card-specific rules text of Lo and Li, Twin Tutors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lo and Li, Twin Tutors</exampledoc>
[TokenizationOrder(-1)]
public class LoAndLiTwinTutorsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["noble creatures you control and lesson spells you control have lifelink"];
}

/// <summary>Card-specific effect of Mageta the Lion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mageta the Lion</exampledoc>
[Dependent]
public class MagetaTheLionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures except for mageta"];
}

/// <summary>Card-specific effect of Mannichi, the Fevered Dream, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mannichi, the Fevered Dream</exampledoc>
[Dependent]
public class MannichiTheFeveredDreamEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["switch each creature's power and toughness until end of turn"];
}

/// <summary>Card-specific effect of Marina Vendrell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Marina Vendrell</exampledoc>
[Dependent]
public class MarinaVendrellEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lock or unlock a door of target room you control"];
}

/// <summary>Card-specific rules text of Masako the Humorless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Masako the Humorless</exampledoc>
[TokenizationOrder(-1)]
public class MasakoTheHumorlessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tapped creatures you control can block as though they were untapped"];
}

/// <summary>Card-specific rules text of Meeting of the Five, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meeting of the Five</exampledoc>
[TokenizationOrder(-1)]
public class MeetingOfTheFive2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast spells with exactly three colors"];
}

/// <summary>Card-specific rules text of Merseine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Merseine</exampledoc>
[TokenizationOrder(-1)]
public class MerseineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only the controller of the enchanted creature may activate this ability"];
}

/// <summary>Card-specific rules text of Minion of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minion of Leshrac</exampledoc>
[TokenizationOrder(-1)]
public class MinionOfLeshracRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature deals damage to you this way, tap it"];
}

/// <summary>Card-specific rules text of Mogg Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mogg Assassin</exampledoc>
[TokenizationOrder(-1)]
public class MoggAssassinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, destroy the creature your opponent chose"];
}

/// <summary>Card-specific effect of Momentum Breaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Momentum Breaker</exampledoc>
[Dependent]
public class MomentumBreakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to your speed"];
}

/// <summary>Card-specific effect of Moraug, Fury of Akoum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moraug, Fury of Akoum</exampledoc>
[Dependent]
public class MoraugFuryOfAkoumCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all creatures you control"];
}

/// <summary>Card-specific rules text of Multiple Choice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Multiple Choice</exampledoc>
[TokenizationOrder(-1)]
public class MultipleChoiceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 4 or more, do all of the above"];
}

/// <summary>Card-specific rules text of Mungha Wurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mungha Wurm</exampledoc>
[TokenizationOrder(-1)]
public class MunghaWurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't untap more than one land during your untap step"];
}

/// <summary>Card-specific rules text of Murderous Spoils, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Murderous Spoils</exampledoc>
[TokenizationOrder(-1)]
public class MurderousSpoilsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain control of all equipment that were attached to it"];
}

/// <summary>Card-specific rules text of Nacatl War-Pride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nacatl War-Pride</exampledoc>
[TokenizationOrder(-1)]
public class NacatlWarPrideCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature must be blocked by exactly one creature if able"];
}

/// <summary>Card-specific rules text of Not of This World, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Not of This World</exampledoc>
[TokenizationOrder(-1)]
public class NotOfThisWorldCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell or ability that targets a permanent you control"];
}

/// <summary>Card-specific rules text of Odds // Ends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Odds // Ends</exampledoc>
[TokenizationOrder(-1)]
public class OddsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it comes up heads, counter target instant or sorcery spell"];
}

/// <summary>Card-specific effect of Old Hob, Alleycat Blues, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Old Hob, Alleycat Blues</exampledoc>
[Dependent]
public class OldHobAlleycatBluesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target attacking creature token gains indestructible until end of turn"];
}

/// <summary>Card-specific effect of One with the Stars, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>One with the Stars</exampledoc>
[Dependent]
public class OneWithTheStarsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it still has its abilities, but it's no longer a creature"];
}

/// <summary>Card-specific rules text of Palladia-Mors, the Ruiner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Palladia-Mors, the Ruiner</exampledoc>
[TokenizationOrder(-1)]
public class PalladiaMorsTheRuinerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["palladia-mors has hexproof as long as it hasn't dealt damage yet"];
}

/// <summary>Card-specific rules text of Parker Luck, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parker Luck</exampledoc>
[TokenizationOrder(-1)]
public class ParkerLuckCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then they each put the card they revealed into their hand"];
}

/// <summary>Card-specific rules text of Phthisis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phthisis</exampledoc>
[TokenizationOrder(-1)]
public class PhthisisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller loses life equal to its power plus its toughness"];
}

/// <summary>Card-specific rules text of Phyrexian Grimoire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Grimoire</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianGrimoireCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that card and put the other one into your hand"];
}

/// <summary>Card-specific rules text of Phyrexian Purge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Purge</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianPurgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs 3 life more to cast for each target"];
}

/// <summary>Card-specific effect of Phyrexian Unlife, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Unlife</exampledoc>
[Dependent]
public class PhyrexianUnlifeCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage is dealt to you in the form of poison counters"];
}

/// <summary>Card-specific effect of Pit of Offerings, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pit of Offerings</exampledoc>
[Dependent]
public class PitOfOfferingsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add one mana of any of the exiled cards' colors"];
}

/// <summary>Card-specific rules text of Plague of Vermin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plague of Vermin</exampledoc>
[TokenizationOrder(-1)]
public class PlagueOfVerminRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player may pay any amount of life"];
}

/// <summary>Card-specific rules text of Pledge of Loyalty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pledge of Loyalty</exampledoc>
[TokenizationOrder(-1)]
public class PledgeOfLoyaltyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has protection from each color among permanents you control"];
}

/// <summary>Card-specific effect of Pontiff of Blight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pontiff of Blight</exampledoc>
[Dependent]
public class PontiffOfBlightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature has multiple instances of extort, each triggers separately"];
}

/// <summary>Card-specific rules text of Portent Tracker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Portent Tracker</exampledoc>
[TokenizationOrder(-1)]
public class PortentTrackerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent protects it, remove a defense counter from it"];
}

/// <summary>Card-specific rules text of Precipitous Drop, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Precipitous Drop</exampledoc>
[TokenizationOrder(-1)]
public class PrecipitousDropRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets -5/-5 instead as long as you've completed a dungeon"];
}

/// <summary>Card-specific rules text of Predator's Rapport, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Predator's Rapport</exampledoc>
[TokenizationOrder(-1)]
public class PredatorsRapportRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to that creature's power plus its toughness"];
}

/// <summary>Card-specific rules text of Premature Burial, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Premature Burial</exampledoc>
[TokenizationOrder(-1)]
public class PrematureBurialRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target nonblack creature that entered since your last turn ended"];
}

/// <summary>Card-specific rules text of Psychic Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Battle</exampledoc>
[TokenizationOrder(-1)]
public class PsychicBattleCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["changing targets this way doesn't trigger abilities of permanents named", Nib.This];
}

/// <summary>Card-specific effect of Psychic Possession, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Possession</exampledoc>
[Dependent]
public class PsychicPossessionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may draw a card"];
}

/// <summary>Card-specific rules text of Purity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Purity</exampledoc>
[TokenizationOrder(-1)]
public class PurityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if noncombat damage would be dealt to you, prevent that damage"];
}

/// <summary>Card-specific effect of Quicken, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicken</exampledoc>
[Dependent]
public class QuickenCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can be cast any time you could cast an instant"];
}

/// <summary>Card-specific effect of Quintorius, Loremaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quintorius, Loremaster</exampledoc>
[Dependent]
public class QuintoriusLoremasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target card exiled with quintorius"];
}

/// <summary>Card-specific rules text of Rakdos, Lord of Riots, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos, Lord of Riots</exampledoc>
[TokenizationOrder(-1)]
public class RakdosLordOfRiotsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't cast rakdos unless an opponent lost life this turn"];
}

/// <summary>Card-specific rules text of Revealing Wind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Revealing Wind</exampledoc>
[TokenizationOrder(-1)]
public class RevealingWindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at each face-down creature that's attacking or blocking"];
}

/// <summary>Card-specific effect of Riptide Mangler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riptide Mangler</exampledoc>
[Dependent]
public class RiptideManglerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's base power becomes equal to target creature's power"];
}

/// <summary>Card-specific effect of Rix Maadi Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rix Maadi Guildmage</exampledoc>
[Dependent]
public class RixMaadiGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player who lost life this turn loses 1 life"];
}

/// <summary>Card-specific rules text of Rock Jockey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rock Jockey</exampledoc>
[TokenizationOrder(-1)]
public class RockJockeyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't cast", Nib.This, "if you've played a land this turn"];
}

/// <summary>Card-specific rules text of Rock Jockey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rock Jockey</exampledoc>
[TokenizationOrder(-1)]
public class RockJockey2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't play lands if this creature was cast this turn"];
}

/// <summary>Card-specific rules text of Runic Repetition, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runic Repetition</exampledoc>
[TokenizationOrder(-1)]
public class RunicRepetitionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target exiled card with flashback you own to your hand"];
}

/// <summary>Card-specific rules text of Sacrifice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sacrifice</exampledoc>
[TokenizationOrder(-1)]
public class Sacrifice2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell,", Nib.This, "a creature"];
}

/// <summary>Card-specific rules text of Samut, the Driving Force, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samut, the Driving Force</exampledoc>
[TokenizationOrder(-1)]
public class SamutTheDrivingForceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["other creatures you control get +x/+0, where x is your speed"];
}

/// <summary>Card-specific effect of Sandstone Deadfall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sandstone Deadfall</exampledoc>
[Dependent]
public class SandstoneDeadfallEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target attacking creature"];
}

/// <summary>Card-specific rules text of Sarkhan the Mad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarkhan the Mad</exampledoc>
[TokenizationOrder(-1)]
public class SarkhanTheMadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sarkhan deals damage to himself equal to that card's mana value"];
}

/// <summary>Card-specific rules text of Security Rhox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Security Rhox</exampledoc>
[TokenizationOrder(-1)]
public class SecurityRhoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only mana produced by treasures to cast it this way"];
}

/// <summary>Card-specific rules text of Selfless Exorcist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Selfless Exorcist</exampledoc>
[TokenizationOrder(-1)]
public class SelflessExorcistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that card deals damage equal to its power to this creature"];
}

/// <summary>Card-specific rules text of Serene Sunset, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serene Sunset</exampledoc>
[TokenizationOrder(-1)]
public class SereneSunsetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage x target creatures would deal this turn"];
}

/// <summary>Card-specific rules text of Shahrazad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shahrazad</exampledoc>
[TokenizationOrder(-1)]
public class Shahrazad2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players play a magic subgame, using their libraries as their decks"];
}

/// <summary>Card-specific effect of Silent Gravestone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silent Gravestone</exampledoc>
[Dependent]
public class SilentGravestoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile this artifact and all cards from all graveyards"];
}

/// <summary>Card-specific rules text of Skrelv, Defector Mite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skrelv, Defector Mite</exampledoc>
[TokenizationOrder(-1)]
public class SkrelvDefectorMiteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can't be blocked by creatures of that color this turn"];
}

/// <summary>Card-specific rules text of Snowblind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Snowblind</exampledoc>
[TokenizationOrder(-1)]
public class SnowblindCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, x is the number of snow lands its controller controls"];
}

/// <summary>Card-specific effect of Spellweaver Volute, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spellweaver Volute</exampledoc>
[Dependent]
public class SpellweaverVoluteCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy the enchanted instant card"];
}

/// <summary>Card-specific effect of Sphinx of the Chimes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sphinx of the Chimes</exampledoc>
[Dependent]
public class SphinxOfTheChimesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw four cards"];
}

/// <summary>Card-specific rules text of Squee, the Immortal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squee, the Immortal</exampledoc>
[TokenizationOrder(-1)]
public class SqueeTheImmortalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard or from exile"];
}

/// <summary>Card-specific rules text of Starnheim Unleashed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Starnheim Unleashed</exampledoc>
[TokenizationOrder(-1)]
public class StarnheimUnleashedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was foretold, create x of those tokens instead"];
}

/// <summary>Card-specific effect of Startled Awake // Persistent Nightmare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Startled Awake // Persistent Nightmare</exampledoc>
[Dependent]
public class StartledAwakeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your graveyard onto the battlefield transformed"];
}

/// <summary>Card-specific rules text of Stature, Size Shifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stature, Size Shifter</exampledoc>
[TokenizationOrder(-1)]
public class StatureSizeShifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["stature can't be blocked if her power is 1 or less"];
}

/// <summary>Card-specific rules text of Steam Vines, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Steam Vines</exampledoc>
[TokenizationOrder(-1)]
public class SteamVinesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player attaches this aura to a land of their choice"];
}

/// <summary>Card-specific effect of Sublime Archangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sublime Archangel</exampledoc>
[Dependent]
public class SublimeArchangelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature has multiple instances of exalted, each triggers separately"];
}

/// <summary>Card-specific rules text of Suspend Aggression, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suspend Aggression</exampledoc>
[TokenizationOrder(-1)]
public class SuspendAggressionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target nonland permanent and the top card of your library"];
}

/// <summary>Card-specific rules text of Teferi's Response, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi's Response</exampledoc>
[TokenizationOrder(-1)]
public class TeferisResponseCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a permanent's ability is countered this way, destroy that permanent"];
}

/// <summary>Card-specific rules text of Teferi, Mage of Zhalfir, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Mage of Zhalfir</exampledoc>
[TokenizationOrder(-1)]
public class TeferiMageOfZhalfirRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creature cards you own that aren't on the battlefield have flash"];
}
