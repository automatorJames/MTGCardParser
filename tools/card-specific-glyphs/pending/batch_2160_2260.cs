/// <summary>Card-specific effect of Doubling Cube, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doubling Cube</exampledoc>
[Dependent]
public class DoublingCubeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double the amount of each type of unspent mana you have"];
}

/// <summary>Card-specific rules text of Dragon's Approach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragon's Approach</exampledoc>
[TokenizationOrder(-1)]
public class DragonsApproachRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile this spell and four cards named", Nib.This, "from your graveyard"];
}

/// <summary>Card-specific rules text of Dread Fugue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dread Fugue</exampledoc>
[TokenizationOrder(-1)]
public class DreadFugueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose a nonland card from it [with mana value 2 or less]"];
}

/// <summary>Card-specific rules text of Dream Salvage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Salvage</exampledoc>
[TokenizationOrder(-1)]
public class DreamSalvageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw cards equal to the number of cards target opponent discarded this turn"];
}

/// <summary>Card-specific rules text of Dromoka's Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dromoka's Command</exampledoc>
[TokenizationOrder(-1)]
public class DromokasCommandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• prevent all damage target instant or sorcery spell would deal this turn"];
}

/// <summary>Card-specific rules text of Embalmer's Tools, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Embalmer's Tools</exampledoc>
[TokenizationOrder(-1)]
public class EmbalmersToolsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activated abilities of creature cards in your graveyard cost {1} less to activate"];
}

/// <summary>Card-specific rules text of Emblazoned Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emblazoned Golem</exampledoc>
[TokenizationOrder(-1)]
public class EmblazonedGolemRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["no more than one mana of each color may be spent this way"];
}

/// <summary>Card-specific rules text of Epic Experiment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Epic Experiment</exampledoc>
[TokenizationOrder(-1)]
public class EpicExperimentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then put all cards exiled this way that weren't cast into your graveyard"];
}

/// <summary>Card-specific rules text of Escaped Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Escaped Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class EscapedShapeshifterCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the same is true for first strike, trample, and protection from any color"];
}

/// <summary>Card-specific rules text of Expel the Interlopers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Expel the Interlopers</exampledoc>
[TokenizationOrder(-1)]
public class ExpelTheInterlopersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures with power greater than or equal to the chosen number"];
}

/// <summary>Card-specific rules text of Falkenrath Gorger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falkenrath Gorger</exampledoc>
[TokenizationOrder(-1)]
public class FalkenrathGorgerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each vampire creature card you own that isn't on the battlefield has madness"];
}

/// <summary>Card-specific rules text of Fasting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fasting</exampledoc>
[TokenizationOrder(-1)]
public class FastingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would begin your draw step, you may skip that step instead"];
}

/// <summary>Card-specific rules text of Fatespinner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fatespinner</exampledoc>
[TokenizationOrder(-1)]
public class FatespinnerCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player skips each instance of the chosen step or phase this turn"];
}

/// <summary>Card-specific rules text of Festering Newt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Festering Newt</exampledoc>
[TokenizationOrder(-1)]
public class FesteringNewtRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature gets -4/-4 instead if you control a creature named bogbrew witch"];
}

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip a coin until you lose a flip or choose to stop flipping"];
}

/// <summary>Card-specific rules text of Firemind's Foresight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firemind's Foresight</exampledoc>
[TokenizationOrder(-1)]
public class FiremindsForesightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then repeat this process for instant cards with mana values 2 and 1"];
}

/// <summary>Card-specific rules text of Flash Foliage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flash Foliage</exampledoc>
[TokenizationOrder(-1)]
public class FlashFoliageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 green saproling creature token that's blocking target creature attacking you"];
}

/// <summary>Card-specific rules text of Furious Rise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Furious Rise</exampledoc>
[TokenizationOrder(-1)]
public class FuriousRiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play that card until you exile another card with this enchantment"];
}

/// <summary>Card-specific rules text of Gauntlets of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gauntlets of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GauntletsOfChaosCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if those permanents are exchanged this way, destroy all auras attached to them"];
}

/// <summary>Card-specific rules text of Getaway Glamer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Getaway Glamer</exampledoc>
[TokenizationOrder(-1)]
public class GetawayGlamerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["+ {2} — destroy target creature if no other creature has greater power"];
}

/// <summary>Card-specific rules text of Gleam of Authority, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gleam of Authority</exampledoc>
[TokenizationOrder(-1)]
public class GleamOfAuthorityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +1/+1 for each +1/+1 counter on other creatures you control"];
}

/// <summary>Card-specific rules text of Goblin Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Assassin</exampledoc>
[TokenizationOrder(-1)]
public class GoblinAssassinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player whose coin comes up tails sacrifices a creature of their choice"];
}

/// <summary>Card-specific rules text of Goblin Game, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Game</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGameCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player hides at least one item, then all players reveal them simultaneously"];
}

/// <summary>Card-specific rules text of Haakon, Stromgald Scourge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haakon, Stromgald Scourge</exampledoc>
[TokenizationOrder(-1)]
public class HaakonStromgaldScourgeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard, but not from anywhere else"];
}

/// <summary>Card-specific rules text of Head Games, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Head Games</exampledoc>
[TokenizationOrder(-1)]
public class HeadGamesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent puts the cards from their hand on top of their library"];
}

/// <summary>Card-specific rules text of Ice Cauldron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ice Cauldron</exampledoc>
[TokenizationOrder(-1)]
public class IceCauldronCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast the last card exiled with this artifact"];
}

/// <summary>Card-specific effect of Infectious Rage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infectious Rage</exampledoc>
[Dependent]
public class InfectiousRageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature at random this aura can enchant"];
}

/// <summary>Card-specific effect of Iron Spider, Stark Upgrade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Iron Spider, Stark Upgrade</exampledoc>
[Dependent]
public class IronSpiderStarkUpgradeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}

/// <summary>Card-specific effect of Izzet Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Izzet Guildmage</exampledoc>
[Dependent]
public class IzzetGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target instant spell you control with mana value 2 or less"];
}

/// <summary>Card-specific effect of Izzet Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Izzet Guildmage</exampledoc>
[Dependent]
public class IzzetGuildmage2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target sorcery spell you control with mana value 2 or less"];
}

/// <summary>Card-specific effect of Jace, Cunning Castaway, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, Cunning Castaway</exampledoc>
[Dependent]
public class JaceCunningCastawayEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create two tokens that are copies of jace, except they're not legendary"];
}

/// <summary>Card-specific rules text of Jaded Response, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jaded Response</exampledoc>
[TokenizationOrder(-1)]
public class JadedResponseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell if it shares a color with a creature you control"];
}

/// <summary>Card-specific effect of Jandor's Ring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jandor's Ring</exampledoc>
[Dependent]
public class JandorsRingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}

/// <summary>Card-specific effect of Jinxed Choker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jinxed Choker</exampledoc>
[Dependent]
public class JinxedChokerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a charge counter on this artifact or remove one from it"];
}

/// <summary>Card-specific effect of Kher Keep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kher Keep</exampledoc>
[Dependent]
public class KherKeepEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 0/1 red kobold creature token named kobolds of", Nib.This];
}

/// <summary>Card-specific rules text of Klothys, God of Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Klothys, God of Destiny</exampledoc>
[TokenizationOrder(-1)]
public class KlothysGodOfDestinyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, you gain 2 life and klothys deals 2 damage to each opponent"];
}

/// <summary>Card-specific rules text of Kogla and Yidaro, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kogla and Yidaro</exampledoc>
[TokenizationOrder(-1)]
public class KoglaAndYidaroRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle this card into your library from your graveyard, then draw a card"];
}

/// <summary>Card-specific rules text of Krark's Thumb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Krark's Thumb</exampledoc>
[TokenizationOrder(-1)]
public class KrarksThumbRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would flip a coin, instead flip two coins and ignore one"];
}

/// <summary>Card-specific effect of Leafkin Avenger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leafkin Avenger</exampledoc>
[Dependent]
public class LeafkinAvengerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add {g} for each creature with power 4 or greater you control"];
}

/// <summary>Card-specific rules text of Liar's Pendulum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liar's Pendulum</exampledoc>
[TokenizationOrder(-1)]
public class LiarsPendulumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent guesses whether a card with that name is in your hand"];
}

/// <summary>Card-specific effect of Liliana of the Dark Realms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana of the Dark Realms</exampledoc>
[Dependent]
public class LilianaOfTheDarkRealmsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"swamps you control have '{t}: add {b}{b}{b}{b}"];
}

/// <summary>Card-specific rules text of Luminesce, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luminesce</exampledoc>
[TokenizationOrder(-1)]
public class LuminesceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that black sources and red sources would deal this turn"];
}

/// <summary>Card-specific rules text of Lutri, the Spellchaser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lutri, the Spellchaser</exampledoc>
[TokenizationOrder(-1)]
public class LutriTheSpellchaserRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each nonland card in your starting deck has a different name"];
}

/// <summary>Card-specific rules text of Meeting of the Five, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meeting of the Five</exampledoc>
[TokenizationOrder(-1)]
public class MeetingOfTheFiveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast spells with exactly three colors from among them this turn"];
}

/// <summary>Card-specific rules text of Might of the Meek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Might of the Meek</exampledoc>
[TokenizationOrder(-1)]
public class MightOfTheMeekRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it also gets +1/+0 until end of turn if you control a mouse"];
}

/// <summary>Card-specific effect of Mirror of Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror of Fate</exampledoc>
[Dependent]
public class MirrorOfFateCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose up to seven face-up exiled cards you own"];
}

/// <summary>Card-specific rules text of Mistform Ultimus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistform Ultimus</exampledoc>
[TokenizationOrder(-1)]
public class MistformUltimusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "is every creature type (even if this card isn't on the battlefield)"];
}

/// <summary>Card-specific rules text of Ms. Marvel, Kamala Khan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ms. Marvel, Kamala Khan</exampledoc>
[TokenizationOrder(-1)]
public class MsMarvelKamalaKhanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["marvel's base power is equal to the number of cards in your hand"];
}

/// <summary>Card-specific effect of Muldrotha, the Gravetide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Muldrotha, the Gravetide</exampledoc>
[Dependent]
public class MuldrothaTheGravetideCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card has multiple permanent types, choose one as you play it"];
}

/// <summary>Card-specific rules text of Nicol Bolas, Dragon-God, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicol Bolas, Dragon-God</exampledoc>
[TokenizationOrder(-1)]
public class NicolBolasDragonGodCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nicol bolas has all loyalty abilities of all other planeswalkers on the battlefield"];
}

/// <summary>Card-specific rules text of Nicol Bolas, Dragon-God, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicol Bolas, Dragon-God</exampledoc>
[TokenizationOrder(-1)]
public class NicolBolasDragonGodCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent exiles a card from their hand or a permanent they control"];
}

/// <summary>Card-specific rules text of Niko Defies Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niko Defies Destiny</exampledoc>
[TokenizationOrder(-1)]
public class NikoDefiesDestinyCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to foretell cards or cast spells that have foretell"];
}

/// <summary>Card-specific effect of Nissa, Genesis Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nissa, Genesis Mage</exampledoc>
[Dependent]
public class NissaGenesisMageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap up to two target creatures and up to two target lands"];
}

/// <summary>Card-specific effect of Novijen Sages, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Novijen Sages</exampledoc>
[Dependent]
public class NovijenSagesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}

/// <summary>Card-specific effect of Nullstone Gargoyle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nullstone Gargoyle</exampledoc>
[Dependent]
public class NullstoneGargoyleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter that spell"];
}

/// <summary>Card-specific rules text of Ochre Jelly, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ochre Jelly</exampledoc>
[TokenizationOrder(-1)]
public class OchreJellyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the token enters with half that many +1/+1 counters on it, rounded down"];
}

/// <summary>Card-specific rules text of Panglacial Wurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Panglacial Wurm</exampledoc>
[TokenizationOrder(-1)]
public class PanglacialWurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["while you're searching your library, you may cast this card from your library"];
}

/// <summary>Card-specific rules text of Parallectric Feedback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parallectric Feedback</exampledoc>
[TokenizationOrder(-1)]
public class ParallectricFeedbackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to target spell's controller equal to that spell's mana value"];
}

/// <summary>Card-specific rules text of Pollen Remedy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pollen Remedy</exampledoc>
[TokenizationOrder(-1)]
public class PollenRemedyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was kicked, prevent the next 6 damage this way instead"];
}

/// <summary>Card-specific rules text of Rakdos Augermage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos Augermage</exampledoc>
[TokenizationOrder(-1)]
public class RakdosAugermageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player reveals their hand and discards a card of your choice"];
}

/// <summary>Card-specific rules text of Ravager Wurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ravager Wurm</exampledoc>
[TokenizationOrder(-1)]
public class RavagerWurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• destroy target land with an activated ability that isn't a mana ability"];
}

/// <summary>Card-specific rules text of Refuse // Cooperate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Refuse // Cooperate</exampledoc>
[TokenizationOrder(-1)]
public class RefuseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["refuse deals damage to target spell's controller equal to that spell's mana value"];
}

/// <summary>Card-specific effect of Renegade Doppelganger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Renegade Doppelganger</exampledoc>
[Dependent]
public class RenegadeDoppelgangerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it does, it loses this ability for the rest of the turn"];
}

/// <summary>Card-specific effect of Risona, Asari Commander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Risona, Asari Commander</exampledoc>
[Dependent]
public class RisonaAsariCommanderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["remove an indestructible counter from risona"];
}

/// <summary>Card-specific rules text of Rootwater Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rootwater Shaman</exampledoc>
[TokenizationOrder(-1)]
public class RootwaterShamanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast aura spells with enchant creature as though they had flash"];
}

/// <summary>Card-specific effect of Runed Halo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runed Halo</exampledoc>
[Dependent]
public class RunedHaloEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't be targeted, dealt damage, or enchanted by anything with that name"];
}

/// <summary>Card-specific rules text of Ruric Thar, Magecrusher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ruric Thar, Magecrusher</exampledoc>
[TokenizationOrder(-1)]
public class RuricTharMagecrusherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ruric thar has hexproof as long as they haven't dealt combat damage yet"];
}

/// <summary>Card-specific rules text of Sanctum Lurker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanctum Lurker</exampledoc>
[TokenizationOrder(-1)]
public class SanctumLurkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["planeswalkers you control aren't put into their owners' graveyards for having 0 loyalty"];
}

/// <summary>Card-specific effect of Sarpadian Empires, Vol. VII, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarpadian Empires, Vol. VII</exampledoc>
[Dependent]
public class SarpadianEmpiresVolVIICard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 creature token of the chosen color and type"];
}

/// <summary>Card-specific rules text of Scrambleverse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrambleverse</exampledoc>
[TokenizationOrder(-1)]
public class ScrambleverseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player gains control of each permanent for which they were chosen"];
}

/// <summary>Card-specific rules text of Scroll Rack, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scroll Rack</exampledoc>
[TokenizationOrder(-1)]
public class ScrollRackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put that many cards from the top of your library into your hand"];
}

/// <summary>Card-specific rules text of Sea Gate Restoration // Sea Gate, Reborn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sea Gate Restoration // Sea Gate, Reborn</exampledoc>
[TokenizationOrder(-1)]
public class SeaGateRestorationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw cards equal to the number of cards in your hand plus one"];
}

/// <summary>Card-specific rules text of Sen Triplets, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sen Triplets</exampledoc>
[TokenizationOrder(-1)]
public class SenTripletsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play lands and cast spells from that player's hand this turn"];
}

/// <summary>Card-specific rules text of Shahrazad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shahrazad</exampledoc>
[TokenizationOrder(-1)]
public class ShahrazadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who doesn't win the subgame loses half their life, rounded up"];
}

/// <summary>Card-specific rules text of Shaman's Trance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shaman's Trance</exampledoc>
[TokenizationOrder(-1)]
public class ShamansTranceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["other players can't play lands or cast spells from their graveyards this turn"];
}

/// <summary>Card-specific rules text of Shanna, Purifying Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shanna, Purifying Blade</exampledoc>
[TokenizationOrder(-1)]
public class ShannaPurifyingBladeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the amount of life you gained this turn"];
}

/// <summary>Card-specific effect of Shelkin Brownie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shelkin Brownie</exampledoc>
[Dependent]
public class ShelkinBrownieEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature loses all \"bands with other\" abilities until end of turn"];
}

/// <summary>Card-specific rules text of Slayer's Cleaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slayer's Cleaver</exampledoc>
[TokenizationOrder(-1)]
public class SlayersCleaverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +3/+1 and must be blocked by an eldrazi if able"];
}

/// <summary>Card-specific rules text of Swat Away, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swat Away</exampledoc>
[TokenizationOrder(-1)]
public class SwatAwayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {2} less to cast if a creature is attacking you"];
}

/// <summary>Card-specific effect of Tamanoa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tamanoa</exampledoc>
[Dependent]
public class TamanoaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain that much life"];
}

/// <summary>Card-specific effect of Tatsumasa, the Dragon's Fang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsumasa, the Dragon's Fang</exampledoc>
[Dependent]
public class TatsumasaTheDragonsFangEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 5/5 blue dragon spirit creature token with flying"];
}

/// <summary>Card-specific rules text of Tatsumasa, the Dragon's Fang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsumasa, the Dragon's Fang</exampledoc>
[TokenizationOrder(-1)]
public class TatsumasaTheDragonsFang2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return tatsumasa to the battlefield under its owner's control when that token dies"];
}

/// <summary>Card-specific rules text of Tide of War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tide of War</exampledoc>
[TokenizationOrder(-1)]
public class TideOfWarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the flip, each blocking creature is sacrificed by its controller"];
}

/// <summary>Card-specific rules text of Tide of War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tide of War</exampledoc>
[TokenizationOrder(-1)]
public class TideOfWar2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, each blocked creature is sacrificed by its controller"];
}

/// <summary>Card-specific rules text of Time and Tide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time and Tide</exampledoc>
[TokenizationOrder(-1)]
public class TimeAndTideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["simultaneously, all phased-out creatures phase in and all creatures with phasing phase out"];
}

/// <summary>Card-specific effect of Tishana's Tidebinder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tishana's Tidebinder</exampledoc>
[Dependent]
public class TishanasTidebinderCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter up to one target activated or triggered ability"];
}

/// <summary>Card-specific rules text of Titania's Song, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titania's Song</exampledoc>
[TokenizationOrder(-1)]
public class TitaniasSongRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this enchantment leaves the battlefield, this effect continues until end of turn"];
}

/// <summary>Card-specific effect of Tom, Bert, and William, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tom, Bert, and William</exampledoc>
[Dependent]
public class TomBertAndWilliamEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they were a creature, return them to the battlefield"];
}

/// <summary>Card-specific effect of Tome of Legends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tome of Legends</exampledoc>
[Dependent]
public class TomeOfLegendsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a page counter on this artifact"];
}

/// <summary>Card-specific rules text of Trial // Error, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trial // Error</exampledoc>
[TokenizationOrder(-1)]
public class TrialRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all creatures blocking or blocked by target creature to their owner's hand"];
}

/// <summary>Card-specific effect of Tundra Kavu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tundra Kavu</exampledoc>
[Dependent]
public class TundraKavuEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target land becomes a plains or an island until end of turn"];
}

/// <summary>Card-specific rules text of Umori, the Collector, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Umori, the Collector</exampledoc>
[TokenizationOrder(-1)]
public class UmoriTheCollectorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each nonland card in your starting deck shares a card type"];
}

/// <summary>Card-specific rules text of Vision Quest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vision Quest</exampledoc>
[TokenizationOrder(-1)]
public class VisionQuestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 4 or greater, it gains haste until end of turn"];
}

/// <summary>Card-specific effect of Vizkopa Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizkopa Guildmage</exampledoc>
[Dependent]
public class VizkopaGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you gain life this turn, each opponent loses that much life"];
}

/// <summary>Card-specific effect of Volrath's Laboratory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Laboratory</exampledoc>
[Dependent]
public class VolrathsLaboratoryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 creature token of the chosen color and type"];
}

/// <summary>Card-specific rules text of War Historian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>War Historian</exampledoc>
[TokenizationOrder(-1)]
public class WarHistorianRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has indestructible as long as it attacked a battle this turn"];
}

/// <summary>Card-specific rules text of Wheel and Deal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wheel and Deal</exampledoc>
[TokenizationOrder(-1)]
public class WheelAndDealRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any number of target opponents each discard their hands, then draw seven cards"];
}

/// <summary>Card-specific rules text of Whims of the Fates, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whims of the Fates</exampledoc>
[TokenizationOrder(-1)]
public class WhimsOfTheFatesCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player separates all permanents they control into three piles"];
}

/// <summary>Card-specific effect of Wicked Akuba, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wicked Akuba</exampledoc>
[Dependent]
public class WickedAkubaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player dealt damage by this creature this turn loses 1 life"];
}

/// <summary>Card-specific rules text of Wish, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wish</exampledoc>
[TokenizationOrder(-1)]
public class WishRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play a card you own from outside the game this turn"];
}
