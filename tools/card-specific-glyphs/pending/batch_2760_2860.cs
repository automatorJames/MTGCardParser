/// <summary>Card-specific rules text of March of Burgeoning Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>March of Burgeoning Life</exampledoc>
[TokenizationOrder(-1)]
public class MarchOfBurgeoningLifeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target creature with mana value less than x"];
}

/// <summary>Card-specific rules text of Masked Gorgon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Masked Gorgon</exampledoc>
[TokenizationOrder(-1)]
public class MaskedGorgonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["green creatures and white creatures have protection from gorgons"];
}

/// <summary>Card-specific rules text of Memories Returning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memories Returning</exampledoc>
[TokenizationOrder(-1)]
public class MemoriesReturning2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they put one on the bottom of your library"];
}

/// <summary>Card-specific rules text of Metamorphic Alteration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Metamorphic Alteration</exampledoc>
[TokenizationOrder(-1)]
public class MetamorphicAlterationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature is a copy of the chosen creature"];
}

/// <summary>Card-specific effect of Meteor Crater, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meteor Crater</exampledoc>
[Dependent]
public class MeteorCraterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a color of a permanent you control"];
}

/// <summary>Card-specific rules text of Mirror Box, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Box</exampledoc>
[TokenizationOrder(-1)]
public class MirrorBoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply to permanents you control"];
}

/// <summary>Card-specific effect of Mystic Subdual, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mystic Subdual</exampledoc>
[Dependent]
public class MysticSubdualEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mutating onto the creature won't give it new abilities"];
}

/// <summary>Card-specific rules text of Natural Emergence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Natural Emergence</exampledoc>
[TokenizationOrder(-1)]
public class NaturalEmergenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lands you control are 2/2 creatures with first strike"];
}

/// <summary>Card-specific rules text of Nature's Revolt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nature's Revolt</exampledoc>
[TokenizationOrder(-1)]
public class NaturesRevoltRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands are 2/2 creatures that are still lands"];
}

/// <summary>Card-specific rules text of Nick Fury, Agent of S.H.I.E.L.D., held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nick Fury, Agent of S.H.I.E.L.D.</exampledoc>
[TokenizationOrder(-1)]
public class NickFuryAgentOfSHIELDRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a double-faced card, you may transform it"];
}

/// <summary>Card-specific effect of Noggle the Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noggle the Mind</exampledoc>
[Dependent]
public class NoggleTheMindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it loses all colors and all other creature types"];
}

/// <summary>Card-specific rules text of Ob Nixilis, the Adversary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ob Nixilis, the Adversary</exampledoc>
[TokenizationOrder(-1)]
public class ObNixilisTheAdversaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy isn't legendary and has starting loyalty x"];
}

/// <summary>Card-specific rules text of Oubliette, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oubliette</exampledoc>
[TokenizationOrder(-1)]
public class OublietteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap that creature as it phases in this way"];
}

/// <summary>Card-specific rules text of Path of Peril, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Path of Peril</exampledoc>
[TokenizationOrder(-1)]
public class PathOfPerilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures [with mana value 2 or less]"];
}

/// <summary>Card-specific effect of Pentarch Paladin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pentarch Paladin</exampledoc>
[Dependent]
public class PentarchPaladinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target permanent of the chosen color"];
}

/// <summary>Card-specific rules text of Peter Parker's Camera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Peter Parker's Camera</exampledoc>
[TokenizationOrder(-1)]
public class PeterParkersCameraCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact enters with three film counters on it"];
}

/// <summary>Card-specific effect of Phyrexian Furnace, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Furnace</exampledoc>
[Dependent]
public class PhyrexianFurnaceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the bottom card of target player's graveyard"];
}

/// <summary>Card-specific effect of Planetarium of Wan Shi Tong, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planetarium of Wan Shi Tong</exampledoc>
[Dependent]
public class PlanetariumOfWanShiTongEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the card after you scry or surveil"];
}

/// <summary>Card-specific rules text of Plargg and Nassari, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plargg and Nassari</exampledoc>
[TokenizationOrder(-1)]
public class PlarggAndNassariRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["an opponent chooses a nonland card exiled this way"];
}

/// <summary>Card-specific rules text of Platinum Emperion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Platinum Emperion</exampledoc>
[TokenizationOrder(-1)]
public class PlatinumEmperionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't pay any amount of life except 0"];
}

/// <summary>Card-specific rules text of Power Artifact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Power Artifact</exampledoc>
[TokenizationOrder(-1)]
public class PowerArtifactRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted artifact's activated abilities cost {2} less to activate"];
}

/// <summary>Card-specific rules text of Pull from Eternity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pull from Eternity</exampledoc>
[TokenizationOrder(-1)]
public class PullFromEternityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put target face-up exiled card into its owner's graveyard"];
}

/// <summary>Card-specific rules text of Reality Shift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reality Shift</exampledoc>
[TokenizationOrder(-1)]
public class RealityShiftCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller manifests the top card of their library"];
}

/// <summary>Card-specific rules text of Reflecting Mirror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reflecting Mirror</exampledoc>
[TokenizationOrder(-1)]
public class ReflectingMirrorCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x is twice the mana value of that spell"];
}

/// <summary>Card-specific effect of Replenish, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Replenish</exampledoc>
[Dependent]
public class ReplenishEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras with nothing to enchant remain in your graveyard"];
}

/// <summary>Card-specific rules text of Restore Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Restore Balance</exampledoc>
[TokenizationOrder(-1)]
public class RestoreBalanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players sacrifice creatures and discard cards the same way"];
}

/// <summary>Card-specific effect of Reverse the Sands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reverse the Sands</exampledoc>
[Dependent]
public class ReverseTheSandsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each of those players gets one life total back"];
}

/// <summary>Card-specific rules text of Rhystic Scrying, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhystic Scrying</exampledoc>
[TokenizationOrder(-1)]
public class RhysticScryingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if any player pays {2}, discard three cards"];
}

/// <summary>Card-specific effect of Riftsweeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riftsweeper</exampledoc>
[Dependent]
public class RiftsweeperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target face-up exiled card"];
}

/// <summary>Card-specific rules text of Rivals' Duel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rivals' Duel</exampledoc>
[TokenizationOrder(-1)]
public class RivalsDuelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose two target creatures that share no creature types"];
}

/// <summary>Card-specific rules text of Rosheen Meanderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rosheen Meanderer</exampledoc>
[TokenizationOrder(-1)]
public class RosheenMeandererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only on costs that contain {x}"];
}

/// <summary>Card-specific rules text of Ruin Raider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ruin Raider</exampledoc>
[TokenizationOrder(-1)]
public class RuinRaiderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose life equal to the card's mana value"];
}

/// <summary>Card-specific effect of Sarkhan, the Dragonspeaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarkhan, the Dragonspeaker</exampledoc>
[Dependent]
public class SarkhanTheDragonspeakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["he doesn't lose loyalty while he's not a planeswalker"];
}

/// <summary>Card-specific rules text of Scrambleverse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrambleverse</exampledoc>
[TokenizationOrder(-1)]
public class Scrambleverse2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each nonland permanent, choose a player at random"];
}

/// <summary>Card-specific effect of Serum Powder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serum Powder</exampledoc>
[Dependent]
public class SerumPowderCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can do this in addition to taking mulligans"];
}

/// <summary>Card-specific rules text of Sideswipe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sideswipe</exampledoc>
[TokenizationOrder(-1)]
public class SideswipeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may change any targets of target arcane spell"];
}

/// <summary>Card-specific rules text of Songcrafter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Songcrafter Mage</exampledoc>
[TokenizationOrder(-1)]
public class SongcrafterMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its harmonize cost is equal to its mana cost"];
}

/// <summary>Card-specific rules text of Spell Contortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spell Contortion</exampledoc>
[TokenizationOrder(-1)]
public class SpellContortionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each time", Nib.This, "was kicked"];
}

/// <summary>Card-specific rules text of Spider-Sense, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Sense</exampledoc>
[TokenizationOrder(-1)]
public class SpiderSenseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target instant spell, sorcery spell, or triggered ability"];
}

/// <summary>Card-specific rules text of Spider-Verse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Verse</exampledoc>
[TokenizationOrder(-1)]
public class SpiderVerse2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply to spiders you control"];
}

/// <summary>Card-specific rules text of Splitting the Powerstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Splitting the Powerstone</exampledoc>
[TokenizationOrder(-1)]
public class SplittingThePowerstoneCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the sacrificed artifact was legendary, draw a card"];
}

/// <summary>Card-specific rules text of Swift Silence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swift Silence</exampledoc>
[TokenizationOrder(-1)]
public class SwiftSilenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each spell countered this way"];
}

/// <summary>Card-specific effect of Sword of Kaldra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of Kaldra</exampledoc>
[Dependent]
public class SwordOfKaldraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile it only if it's still on the battlefield"];
}

/// <summary>Card-specific rules text of Tale's End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tale's End</exampledoc>
[TokenizationOrder(-1)]
public class TalesEndRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability, triggered ability, or legendary spell"];
}

/// <summary>Card-specific rules text of Tazri, Stalwart Survivor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tazri, Stalwart Survivor</exampledoc>
[TokenizationOrder(-1)]
public class TazriStalwartSurvivorCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if this creature has another activated ability"];
}

/// <summary>Card-specific rules text of The Death of Gwen Stacy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Death of Gwen Stacy</exampledoc>
[TokenizationOrder(-1)]
public class TheDeathOfGwenStacyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["iii — exile any number of target players' graveyards"];
}

/// <summary>Card-specific effect of The Mightstone and Weakstone // Urza, Planeswalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mightstone and Weakstone // Urza, Planeswalker</exampledoc>
[Dependent]
public class TheMightstoneAndWeakstoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one —"];
}

/// <summary>Card-specific effect of The Soul Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Soul Stone</exampledoc>
[Dependent]
public class TheSoulStoneCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["harness", Nib.This];
}

/// <summary>Card-specific rules text of The Unagi of Kyoshi Island, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Unagi of Kyoshi Island</exampledoc>
[TokenizationOrder(-1)]
public class TheUnagiOfKyoshiIslandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they can tap their artifacts and creatures to help"];
}

/// <summary>Card-specific rules text of Time Stretch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Stretch</exampledoc>
[TokenizationOrder(-1)]
public class TimeStretchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player takes two extra turns after this one"];
}

/// <summary>Card-specific effect of Titanbones, Towering Heart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titanbones, Towering Heart</exampledoc>
[Dependent]
public class TitanbonesToweringHeartEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 3 life"];
}

/// <summary>Card-specific effect of Toph, the First Metalbender, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toph, the First Metalbender</exampledoc>
[Dependent]
public class TophTheFirstMetalbenderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they don't gain the ability to {t} for mana"];
}

/// <summary>Card-specific rules text of Tovolar, Dire Overlord // Tovolar, the Midnight Scourge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tovolar, Dire Overlord // Tovolar, the Midnight Scourge</exampledoc>
[TokenizationOrder(-1)]
public class TovolarDireOverlordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then transform any number of human werewolves you control"];
}

/// <summary>Card-specific effect of Trap Runner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trap Runner</exampledoc>
[Dependent]
public class TrapRunnerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability works on creatures that can't be blocked"];
}

/// <summary>Card-specific rules text of Umbra Mystic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Umbra Mystic</exampledoc>
[TokenizationOrder(-1)]
public class UmbraMysticCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras attached to permanents you control have umbra armor"];
}

/// <summary>Card-specific rules text of Unstable Glyphbridge // Sandswirl Wanderglyph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unstable Glyphbridge // Sandswirl Wanderglyph</exampledoc>
[TokenizationOrder(-1)]
public class UnstableGlyphbridgeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all creatures except creatures chosen this way"];
}

/// <summary>Card-specific effect of Urza, Powerstone Prodigy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza, Powerstone Prodigy</exampledoc>
[Dependent]
public class UrzaPowerstoneProdigyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the token is an artifact with \"{t}: add {c}"];
}

/// <summary>Card-specific rules text of Visions, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Visions</exampledoc>
[TokenizationOrder(-1)]
public class VisionsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may then have that player shuffle that library"];
}

/// <summary>Card-specific rules text of Void Winnower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void Winnower</exampledoc>
[TokenizationOrder(-1)]
public class VoidWinnower2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't cast spells with even mana values"];
}

/// <summary>Card-specific rules text of Zoyowa Lava-Tongue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zoyowa Lava-Tongue</exampledoc>
[TokenizationOrder(-1)]
public class ZoyowaLavaTongueCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["zoyowa deals 3 damage to each opponent who didn't"];
}

/// <summary>Card-specific rules text of Absolute Virtue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Absolute Virtue</exampledoc>
[TokenizationOrder(-1)]
public class AbsoluteVirtueCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you have protection from each of your opponents"];
}

/// <summary>Card-specific rules text of Airtight Alibi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Airtight Alibi</exampledoc>
[TokenizationOrder(-1)]
public class AirtightAlibiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +2/+2 and can't become suspected"];
}

/// <summary>Card-specific effect of Arcum's Weathervane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arcum's Weathervane</exampledoc>
[Dependent]
public class ArcumsWeathervane2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target nonsnow basic land becomes snow"];
}

/// <summary>Card-specific rules text of Aura Graft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Graft</exampledoc>
[TokenizationOrder(-1)]
public class AuraGraft2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["attach it to another permanent it can enchant"];
}

/// <summary>Card-specific rules text of Blood Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blood Sun</exampledoc>
[TokenizationOrder(-1)]
public class BloodSunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands lose all abilities except mana abilities"];
}

/// <summary>Card-specific rules text of Booby Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Booby Trap</exampledoc>
[TokenizationOrder(-1)]
public class BoobyTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the chosen player reveals each card they draw"];
}

/// <summary>Card-specific effect of Breeches, the Blastmaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breeches, the Blastmaker</exampledoc>
[Dependent]
public class BreechesTheBlastmakerCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that spell"];
}

/// <summary>Card-specific effect of Codie, Ravenous Codex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Codie, Ravenous Codex</exampledoc>
[Dependent]
public class CodieRavenousCodexEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy it"];
}

/// <summary>Card-specific rules text of Daybreak Coronet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Daybreak Coronet</exampledoc>
[TokenizationOrder(-1)]
public class DaybreakCoronetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchant creature with another aura attached to it"];
}

/// <summary>Card-specific rules text of Deadly Complication, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deadly Complication</exampledoc>
[TokenizationOrder(-1)]
public class DeadlyComplicationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have it become no longer suspected"];
}

/// <summary>Card-specific rules text of Deification, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deification</exampledoc>
[TokenizationOrder(-1)]
public class DeificationCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this enchantment enters, choose a planeswalker type"];
}

/// <summary>Card-specific rules text of Doc Aurlock, Grizzled Genius, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doc Aurlock, Grizzled Genius</exampledoc>
[TokenizationOrder(-1)]
public class DocAurlockGrizzledGeniusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["plotting cards from your hand costs {2} less"];
}

/// <summary>Card-specific rules text of Ethereal Ambush, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ethereal Ambush</exampledoc>
[TokenizationOrder(-1)]
public class EtherealAmbushRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["manifest the top two cards of your library"];
}

/// <summary>Card-specific rules text of Fblthp, Lost on the Range, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fblthp, Lost on the Range</exampledoc>
[TokenizationOrder(-1)]
public class FblthpLostOnTheRange2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the top card of your library has plot"];
}

/// <summary>Card-specific effect of Hakim, Loreweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hakim, Loreweaver</exampledoc>
[Dependent]
public class HakimLoreweaverEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all auras attached to hakim"];
}

/// <summary>Card-specific effect of Indestructibility, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Indestructibility</exampledoc>
[Dependent]
public class IndestructibilityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["effects that say \"destroy\" don't destroy that permanent"];
}

/// <summary>Card-specific effect of Jace, the Perfected Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, the Perfected Mind</exampledoc>
[Dependent]
public class JaceThePerfectedMindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player mills three times x cards"];
}

/// <summary>Card-specific rules text of Juxtapose, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Juxtapose</exampledoc>
[TokenizationOrder(-1)]
public class JuxtaposeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then exchange control of artifacts the same way"];
}

/// <summary>Card-specific rules text of Kang the Conqueror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kang the Conqueror</exampledoc>
[TokenizationOrder(-1)]
public class KangTheConquerorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during that turn, power-up abilities can't be activated"];
}

/// <summary>Card-specific rules text of Lantern Flare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lantern Flare</exampledoc>
[TokenizationOrder(-1)]
public class LanternFlareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["[x is the number of creatures you control"];
}

/// <summary>Card-specific rules text of Mages' Contest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mages' Contest</exampledoc>
[TokenizationOrder(-1)]
public class MagesContest3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the bidding, counter that spell"];
}

/// <summary>Card-specific effect of Melira, the Living Cure, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melira, the Living Cure</exampledoc>
[Dependent]
public class MeliraTheLivingCureCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose another target creature or artifact"];
}

/// <summary>Card-specific rules text of Mistmeadow Skulk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistmeadow Skulk</exampledoc>
[TokenizationOrder(-1)]
public class MistmeadowSkulkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lifelink, protection from mana value 3 or greater"];
}

/// <summary>Card-specific effect of Mordenkainen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mordenkainen</exampledoc>
[Dependent]
public class MordenkainenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange your hand and library, then shuffle"];
}

/// <summary>Card-specific rules text of Mysterio, Master of Illusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mysterio, Master of Illusion</exampledoc>
[TokenizationOrder(-1)]
public class MysterioMasterOfIllusionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile those tokens when mysterio leaves the battlefield"];
}

/// <summary>Card-specific rules text of No Witnesses, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>No Witnesses</exampledoc>
[TokenizationOrder(-1)]
public class NoWitnessesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who controls the most creatures investigates"];
}

/// <summary>Card-specific effect of Open the Vaults, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Open the Vaults</exampledoc>
[Dependent]
public class OpenTheVaultsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras with nothing to enchant remain in graveyards"];
}

/// <summary>Card-specific rules text of Parasitic Grasp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parasitic Grasp</exampledoc>
[TokenizationOrder(-1)]
public class ParasiticGraspRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 3 damage to target [human] creature"];
}

/// <summary>Card-specific rules text of Personal Incarnation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Personal Incarnation</exampledoc>
[TokenizationOrder(-1)]
public class PersonalIncarnationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only this creature's owner may activate this ability"];
}

/// <summary>Card-specific rules text of Plague of Vermin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plague of Vermin</exampledoc>
[TokenizationOrder(-1)]
public class PlagueOfVermin2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until no one pays life"];
}

/// <summary>Card-specific rules text of Preferred Selection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Preferred Selection</exampledoc>
[TokenizationOrder(-1)]
public class PreferredSelectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may sacrifice this enchantment and pay {2}{g}{g}"];
}

/// <summary>Card-specific rules text of Prophecy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Prophecy</exampledoc>
[TokenizationOrder(-1)]
public class ProphecyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the top card of target opponent's library"];
}

/// <summary>Card-specific rules text of Rakdos, the Showstopper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos, the Showstopper</exampledoc>
[TokenizationOrder(-1)]
public class RakdosTheShowstopperCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each creature whose coin comes up tails"];
}

/// <summary>Card-specific effect of Ral Zarek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ral Zarek</exampledoc>
[Dependent]
public class RalZarekCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip five coins"];
}

/// <summary>Card-specific rules text of Rite of Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rite of Ruin</exampledoc>
[TokenizationOrder(-1)]
public class RiteOfRuinCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose an order for artifacts, creatures, and lands"];
}

/// <summary>Card-specific rules text of Shimmer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shimmer</exampledoc>
[TokenizationOrder(-1)]
public class ShimmerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land of the chosen type has phasing"];
}

/// <summary>Card-specific rules text of Sigarda's Splendor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sigarda's Splendor</exampledoc>
[TokenizationOrder(-1)]
public class SigardasSplendorCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this enchantment enters, note your life total"];
}

/// <summary>Card-specific rules text of Skull Rend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skull Rend</exampledoc>
[TokenizationOrder(-1)]
public class SkullRendRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players each discard two cards at random"];
}

/// <summary>Card-specific rules text of Soul Burn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Burn</exampledoc>
[TokenizationOrder(-1)]
public class SoulBurnCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only black and/or red mana on x"];
}

/// <summary>Card-specific rules text of Spikeshell Harrier, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spikeshell Harrier</exampledoc>
[TokenizationOrder(-1)]
public class SpikeshellHarrierCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect can't reduce their speed below 1"];
}
