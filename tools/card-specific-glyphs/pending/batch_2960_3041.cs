/// <summary>Card-specific effect of Luxior, Giada's Gift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luxior, Giada's Gift</exampledoc>
[Dependent]
public class LuxiorGiadasGiftCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["loyalty abilities can still be activated"];
}

/// <summary>Card-specific effect of Mavinda, Students' Advocate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mavinda, Students' Advocate</exampledoc>
[Dependent]
public class MavindaStudentsAdvocateCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you still pay the spell's costs"];
}

/// <summary>Card-specific rules text of Medomai the Ageless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Medomai the Ageless</exampledoc>
[TokenizationOrder(-1)]
public class MedomaiTheAgelessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["medomai can't attack during extra turns"];
}

/// <summary>Card-specific rules text of Melting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melting</exampledoc>
[TokenizationOrder(-1)]
public class MeltingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands are no longer snow"];
}

/// <summary>Card-specific rules text of Mindbreak Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindbreak Trap</exampledoc>
[TokenizationOrder(-1)]
public class MindbreakTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile any number of target spells"];
}

/// <summary>Card-specific effect of Moonmist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonmist</exampledoc>
[Dependent]
public class MoonmistCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only double-faced cards can be transformed"];
}

/// <summary>Card-specific rules text of Muraganda Petroglyphs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Muraganda Petroglyphs</exampledoc>
[TokenizationOrder(-1)]
public class MuragandaPetroglyphsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures with no abilities get +2/+2"];
}

/// <summary>Card-specific effect of North Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>North Star</exampledoc>
[Dependent]
public class NorthStarCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["additional costs are still paid normally"];
}

/// <summary>Card-specific effect of Platinum Emperion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Platinum Emperion</exampledoc>
[Dependent]
public class PlatinumEmperion2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't gain or lose life"];
}

/// <summary>Card-specific rules text of Spider-Punk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Punk</exampledoc>
[TokenizationOrder(-1)]
public class SpiderPunkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells and abilities can't be countered"];
}

/// <summary>Card-specific rules text of Spry and Mighty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spry and Mighty</exampledoc>
[TokenizationOrder(-1)]
public class SpryAndMightyCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose exactly two creatures you control"];
}

/// <summary>Card-specific rules text of Suncleanser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suncleanser</exampledoc>
[TokenizationOrder(-1)]
public class SuncleanserCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target opponent loses all counters"];
}

/// <summary>Card-specific rules text of Symbiote Spider-Man, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Symbiote Spider-Man</exampledoc>
[TokenizationOrder(-1)]
public class SymbioteSpiderManRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gains this card's other abilities"];
}

/// <summary>Card-specific rules text of The Immortal Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Immortal Sun</exampledoc>
[TokenizationOrder(-1)]
public class TheImmortalSunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't activate planeswalkers' loyalty abilities"];
}

/// <summary>Card-specific rules text of Torment of Hailfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Torment of Hailfire</exampledoc>
[TokenizationOrder(-1)]
public class TormentOfHailfireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat the following process x times"];
}

/// <summary>Card-specific rules text of Wall of Shards, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Shards</exampledoc>
[TokenizationOrder(-1)]
public class WallOfShardsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—an opponent gains 1 life"];
}

/// <summary>Card-specific rules text of Winds of Rath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winds of Rath</exampledoc>
[TokenizationOrder(-1)]
public class WindsOfRathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that aren't enchanted"];
}

/// <summary>Card-specific effect of Ageless Sentinels, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ageless Sentinels</exampledoc>
[Dependent]
public class AgelessSentinelsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's no longer a wall"];
}

/// <summary>Card-specific rules text of Biblioplex Tomekeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Biblioplex Tomekeeper</exampledoc>
[TokenizationOrder(-1)]
public class BiblioplexTomekeeperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target creature becomes unprepared"];
}

/// <summary>Card-specific effect of Castle Garenbrig, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Castle Garenbrig</exampledoc>
[Dependent]
public class CastleGarenbrigEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add six {g}"];
}

/// <summary>Card-specific rules text of Cavernous Maw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cavernous Maw</exampledoc>
[TokenizationOrder(-1)]
public class CavernousMawCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's still a cave land"];
}

/// <summary>Card-specific effect of Chaoslace, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaoslace</exampledoc>
[Dependent]
public class ChaoslaceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its mana symbols remain unchanged"];
}

/// <summary>Card-specific rules text of Confusion in the Ranks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Confusion in the Ranks</exampledoc>
[TokenizationOrder(-1)]
public class ConfusionInTheRanksCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange control of those permanents"];
}

/// <summary>Card-specific rules text of Domineer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Domineer</exampledoc>
[TokenizationOrder(-1)]
public class DomineerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you control enchanted artifact creature"];
}

/// <summary>Card-specific effect of Dwindle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwindle</exampledoc>
[Dependent]
public class DwindleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the attacking creature remains blocked"];
}

/// <summary>Card-specific effect of Ersatz Gnomes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ersatz Gnomes</exampledoc>
[Dependent]
public class ErsatzGnomesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target spell becomes colorless"];
}

/// <summary>Card-specific rules text of Exile, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exile</exampledoc>
[TokenizationOrder(-1)]
public class ExileRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "target nonwhite attacking creature"];
}

/// <summary>Card-specific rules text of Gonti's Machinations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gonti's Machinations</exampledoc>
[TokenizationOrder(-1)]
public class GontisMachinationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage causes loss of life"];
}

/// <summary>Card-specific rules text of Harness Infinity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harness Infinity</exampledoc>
[TokenizationOrder(-1)]
public class HarnessInfinityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange your hand and graveyard"];
}

/// <summary>Card-specific rules text of Mathemagics, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mathemagics</exampledoc>
[TokenizationOrder(-1)]
public class MathemagicsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player draws 2ˣ cards"];
}

/// <summary>Card-specific rules text of Menacing Ogre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Menacing Ogre</exampledoc>
[TokenizationOrder(-1)]
public class MenacingOgreCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then those numbers are revealed"];
}

/// <summary>Card-specific rules text of Mindblaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindblaze</exampledoc>
[TokenizationOrder(-1)]
public class MindblazeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their library"];
}

/// <summary>Card-specific rules text of Mirror Gallery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Gallery</exampledoc>
[TokenizationOrder(-1)]
public class MirrorGalleryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply"];
}

/// <summary>Card-specific rules text of Morality Shift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Morality Shift</exampledoc>
[TokenizationOrder(-1)]
public class MoralityShiftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange your graveyard and library"];
}

/// <summary>Card-specific rules text of Pain's Reward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain's Reward</exampledoc>
[TokenizationOrder(-1)]
public class PainsRewardCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may bid life"];
}

/// <summary>Card-specific rules text of Phyrexian Portal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Portal</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianPortalCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile one of those piles"];
}

/// <summary>Card-specific rules text of Platinum Emperion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Platinum Emperion</exampledoc>
[TokenizationOrder(-1)]
public class PlatinumEmperion3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your life total can't change"];
}

/// <summary>Card-specific effect of Protective Sphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Protective Sphere</exampledoc>
[Dependent]
public class ProtectiveSphereCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["colorless mana prevents no damage"];
}

/// <summary>Card-specific rules text of Search for Survivors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Search for Survivors</exampledoc>
[TokenizationOrder(-1)]
public class SearchForSurvivors2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reorder your graveyard at random"];
}

/// <summary>Card-specific rules text of Shoot the Sheriff, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shoot the Sheriff</exampledoc>
[TokenizationOrder(-1)]
public class ShootTheSheriffRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["everyone else is fair game"];
}

/// <summary>Card-specific rules text of Sigarda's Splendor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sigarda's Splendor</exampledoc>
[TokenizationOrder(-1)]
public class SigardasSplendorCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then note your life total"];
}

/// <summary>Card-specific rules text of Spectra Ward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spectra Ward</exampledoc>
[TokenizationOrder(-1)]
public class SpectraWardCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect doesn't remove auras"];
}

/// <summary>Card-specific rules text of The Enigma Jewel // Locus of Enlightenment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Enigma Jewel // Locus of Enlightenment</exampledoc>
[TokenizationOrder(-1)]
public class TheEnigmaJewelCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the enigma jewel enters tapped"];
}

/// <summary>Card-specific effect of Tom, Bert, and William, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tom, Bert, and William</exampledoc>
[Dependent]
public class TomBertAndWilliam2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're no longer a creature"];
}

/// <summary>Card-specific rules text of Trailblazer's Boots, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trailblazer's Boots</exampledoc>
[TokenizationOrder(-1)]
public class TrailblazersBootsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature has nonbasic landwalk"];
}

/// <summary>Card-specific rules text of Vaevictis Asmadi, the Dire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vaevictis Asmadi, the Dire</exampledoc>
[TokenizationOrder(-1)]
public class VaevictisAsmadiTheDireCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players sacrifice those permanents"];
}

/// <summary>Card-specific rules text of Weatherlight Compleated, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Weatherlight Compleated</exampledoc>
[TokenizationOrder(-1)]
public class WeatherlightCompleatedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't, scry 1"];
}

/// <summary>Card-specific effect of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[Dependent]
public class WonderManHollywoodHero3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate each power-up ability only"];
}

/// <summary>Card-specific rules text of Delina, Wild Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Delina, Wild Mage</exampledoc>
[TokenizationOrder(-1)]
public class DelinaWildMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may roll again"];
}

/// <summary>Card-specific effect of Discerning Financier, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Discerning Financier</exampledoc>
[Dependent]
public class DiscerningFinancierEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose another player"];
}

/// <summary>Card-specific rules text of Fierce Retribution, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fierce Retribution</exampledoc>
[TokenizationOrder(-1)]
public class FierceRetributionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target [attacking] creature"];
}

/// <summary>Card-specific rules text of Intuition, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Intuition</exampledoc>
[TokenizationOrder(-1)]
public class IntuitionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent chooses one"];
}

/// <summary>Card-specific effect of Invasion of Tarkir // Defiant Thundermaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Tarkir // Defiant Thundermaw</exampledoc>
[Dependent]
public class InvasionOfTarkirEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can be 0"];
}

/// <summary>Card-specific rules text of Ivy, Gleeful Spellthief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ivy, Gleeful Spellthief</exampledoc>
[TokenizationOrder(-1)]
public class IvyGleefulSpellthiefCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy targets ivy"];
}

/// <summary>Card-specific effect of Jeweled Bird, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeweled Bird</exampledoc>
[Dependent]
public class JeweledBirdCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ante this artifact"];
}

/// <summary>Card-specific rules text of Karplusan Minotaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karplusan Minotaur</exampledoc>
[TokenizationOrder(-1)]
public class KarplusanMinotaurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—flip a coin"];
}

/// <summary>Card-specific rules text of Koth of the Hammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koth of the Hammer</exampledoc>
[TokenizationOrder(-1)]
public class KothOfTheHammerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["'\""];
}

/// <summary>Card-specific rules text of Snowblind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Snowblind</exampledoc>
[TokenizationOrder(-1)]
public class SnowblindCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets -x/-y"];
}

/// <summary>Card-specific rules text of Songcrafter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Songcrafter Mage</exampledoc>
[TokenizationOrder(-1)]
public class SongcrafterMage2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then exile the spell"];
}

/// <summary>Card-specific rules text of Swift Silence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swift Silence</exampledoc>
[TokenizationOrder(-1)]
public class SwiftSilence2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter all other spells"];
}

/// <summary>Card-specific rules text of Tezzeret, Cruel Machinist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tezzeret, Cruel Machinist</exampledoc>
[TokenizationOrder(-1)]
public class TezzeretCruelMachinistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're 5/5 artifact creatures"];
}

/// <summary>Card-specific rules text of The Celestus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Celestus</exampledoc>
[TokenizationOrder(-1)]
public class TheCelestusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, it becomes night"];
}

/// <summary>Card-specific effect of The Mind Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mind Stone</exampledoc>
[Dependent]
public class TheMindStoneCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["harness", Nib.This];
}

/// <summary>Card-specific rules text of The Serpent Society, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Serpent Society</exampledoc>
[TokenizationOrder(-1)]
public class TheSerpentSocietyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ward—get five poison counters"];
}

/// <summary>Card-specific rules text of Transmogrant's Crown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transmogrant's Crown</exampledoc>
[TokenizationOrder(-1)]
public class TransmograntsCrownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equip {2} or {b}"];
}

/// <summary>Card-specific rules text of Braid of Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Braid of Fire</exampledoc>
[TokenizationOrder(-1)]
public class BraidOfFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—add {r}"];
}

/// <summary>Card-specific rules text of Chandra, Heart of Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Heart of Fire</exampledoc>
[TokenizationOrder(-1)]
public class ChandraHeartOfFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add six {r}"];
}

/// <summary>Card-specific rules text of Infinite Coursework, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infinite Coursework</exampledoc>
[TokenizationOrder(-1)]
public class InfiniteCourseworkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it becomes unprepared"];
}

/// <summary>Card-specific rules text of Inspired Idea, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inspired Idea</exampledoc>
[TokenizationOrder(-1)]
public class InspiredIdeaCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["]"];
}

/// <summary>Card-specific rules text of Irencrag Feat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Irencrag Feat</exampledoc>
[TokenizationOrder(-1)]
public class IrencragFeat2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add seven {r}"];
}

/// <summary>Card-specific effect of Ixidron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ixidron</exampledoc>
[Dependent]
public class IxidronEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're 2/2 creatures"];
}

/// <summary>Card-specific rules text of Moonmist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonmist</exampledoc>
[TokenizationOrder(-1)]
public class MoonmistCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["transform all humans"];
}

/// <summary>Card-specific rules text of Regenerate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Regenerate</exampledoc>
[TokenizationOrder(-1)]
public class RegenerateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "target creature"];
}

/// <summary>Card-specific rules text of Stormscale Scion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stormscale Scion</exampledoc>
[TokenizationOrder(-1)]
public class StormscaleScionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copies become tokens"];
}

/// <summary>Card-specific rules text of Swallowed by Leviathan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swallowed by Leviathan</exampledoc>
[TokenizationOrder(-1)]
public class SwallowedByLeviathanCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target spell"];
}

/// <summary>Card-specific rules text of Tom, Bert, and William, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tom, Bert, and William</exampledoc>
[TokenizationOrder(-1)]
public class TomBertAndWilliam3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're an artifact"];
}

/// <summary>Card-specific rules text of U.S.Agent, John Walker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>U.S.Agent, John Walker</exampledoc>
[TokenizationOrder(-1)]
public class USAgentJohnWalkerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when u"];
}

/// <summary>Card-specific rules text of U.S.Agent, John Walker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>U.S.Agent, John Walker</exampledoc>
[TokenizationOrder(-1)]
public class USAgentJohnWalkerCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["s"];
}

/// <summary>Card-specific rules text of Desert Nomads, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desert Nomads</exampledoc>
[TokenizationOrder(-1)]
public class DesertNomadsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["desertwalk"];
}

/// <summary>Card-specific rules text of Manifest Dread, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Manifest Dread</exampledoc>
[TokenizationOrder(-1)]
public class ManifestDreadCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This];
}

/// <summary>Card-specific rules text of U.S.Agent, John Walker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>U.S.Agent, John Walker</exampledoc>
[TokenizationOrder(-1)]
public class USAgentJohnWalkerCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["agent"];
}
