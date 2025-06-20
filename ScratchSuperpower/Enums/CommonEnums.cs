namespace MTGCardParser.Enums;

public enum Quantity
{
    [RegPat("none", "zero", "0")]
    Zero = 0,

    [RegPat("one", "1")]
    One = 1,

    [RegPat("two", "2")]
    Two = 2,

    [RegPat("three", "3")]
    Three = 3,

    [RegPat("four", "4")]
    Four = 4,

    [RegPat("five", "5")]
    Five = 5,

    [RegPat("six", "6")]
    Six = 6,

    [RegPat("seven", "7")]
    Seven = 7,

    [RegPat("eight", "8")]
    Eight = 8,

    [RegPat("nine", "9")]
    Nine = 9,

    [RegPat("ten", "10")]
    Ten = 10
}

public enum CardType
{
    Artifact,
    Creature,
    Enchantment,
    Instant,
    Land,
    Planeswalker,
    Sorcery,
    Battle,
    Tribal
}

public enum Keyword
{
    // Evergreen
    Deathtouch,
    Defender,
    [RegPat("first strike")] FirstStrike,
    [RegPat("double strike")] DoubleStrike,
    Enchant,
    Equip,
    Flash,
    Flying,
    Haste,
    Hexproof,
    Indestructible,
    Intimidate,
    Landwalk,
    Lifelink,
    Protection,
    Reach,
    Shroud,
    Trample,
    Vigilance,

    // Keyword Actions
    Attach,
    Counter,
    Exile,
    Fight,
    Regenerate,
    Sacrifice,
    Tap,
    Untap,

    // Expansion Keywords
    Absorb,
    Affinity,
    Amplify,
    Annihilator,
    [RegPat("aura swap")] AuraSwap,
    Banding,
    [RegPat("bands with other")] BandsWithOther,
    [RegPat("battle cry")] BattleCry,
    Bestow,
    Bloodthirst,
    Bushido,
    Buyback,
    Cascade,
    Champion,
    Changeling,
    Cipher,
    Clash,
    Conspire,
    Convoke,
    [RegPat("cumulative upkeep")] CumulativeUpkeep,
    Cycling,
    Delve,
    Detain,
    Devour,
    Dredge,
    Echo,
    Entwine,
    Epic,
    Evolve,
    Evoke,
    Exalted,
    Extort,
    Fading,
    Fateseal,
    Fear,
    Flanking,
    Flashback,
    Flip,
    Forecast,
    Fortify,
    Frenzy,
    Graft,
    Gravestorm,
    Haunt,
    Hideaway,
    Horsemanship,
    Infect,
    Kicker,
    [RegPat("level up")] LevelUp,
    [RegPat("living weapon")] LivingWeapon,
    Madness,
    Miracle,
    Modular,
    Monstrosity,
    Morph,
    Multikicker,
    Ninjutsu,
    Offering,
    Overload,
    Persist,
    Phasing,
    Poisonous,
    Populate,
    Proliferate,
    Provoke,
    Prowl,
    Rampage,
    Rebound,
    Recover,
    Reinforce,
    Replicate,
    Retrace,
    Ripple,
    Scavenge,
    Scry,
    Shadow,
    Soulbond,
    Soulshift,
    Splice,
    [RegPat("split second")] SplitSecond,
    Storm,
    Sunburst,
    Suspend,
    [RegPat("totem armor")] TotemArmor,
    Transfigure,
    Transform,
    Transmute,
    Typecycling,
    Undying,
    Unearth,
    Unleash,
    Vanishing,
    Wither,

    // Ability Words
    Battalion,
    Bloodrush,
    Channel,
    Chroma,
    Domain,
    [RegPat("fateful hour")] FatefulHour,
    Grandeur,
    Hellbent,
    Heroic,
    Imprint,
    [RegPat("join forces")] JoinForces,
    Kinship,
    Landfall,
    Metalcraft,
    Morbid,
    Radiance,
    Sweep,
    Threshold,

    // Discontinued
    Bury,
    Landhome,
    Substance
}