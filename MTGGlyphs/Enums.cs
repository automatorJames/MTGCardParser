namespace MTGGlyphs.GlyphDefinitions;

public enum Quantity
{
    [RegexPattern("x")]
    X = -1,

    [RegexPattern("0", "zero", "none")]
    Zero = 0,

    [RegexPattern("1", "one", "a")]
    One = 1,

    [RegexPattern("2", "two")]
    Two = 2,

    [RegexPattern("3", "three")]
    Three = 3,

    [RegexPattern("4", "four")]
    Four = 4,

    [RegexPattern("5", "five")]
    Five = 5,

    [RegexPattern("6", "six")]
    Six = 6,

    [RegexPattern("7", "seven")]
    Seven = 7,

    [RegexPattern("8", "eight")]
    Eight = 8,

    [RegexPattern("9", "nine")]
    Nine = 9,

    [RegexPattern("10", "ten")]
    Ten = 10,

    [RegexPattern("11", "eleven")]
    Eleven = 11,

    [RegexPattern("12", "twelve")]
    Twelve = 12
}

public enum ManaColor
{
    Colorless,
    White,
    Blue,
    Black,
    Red,
    Green
}

public enum CardType
{
    Permanent,
    Artifact,
    Creature,
    Enchantment,
    Instant,
    Land,
    Planeswalker,
    Sorcery,
    Battle,
    Tribal,
}

public enum Keyword
{
    // Evergreen
    Deathtouch,
    Defender,
    FirstStrike,
    DoubleStrike,
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
    AuraSwap,
    Banding,
    BandsWithOther,
    BattleCry,
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
    CumulativeUpkeep,
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
    LevelUp,
    LivingWeapon,
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
    SplitSecond,
    Storm,
    Sunburst,
    Suspend,
    TotemArmor,
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
    FatefulHour,
    Grandeur,
    Hellbent,
    Heroic,
    Imprint,
    JoinForces,
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

public enum CounterType
{
    // Common counters
    [RegexPattern(@"\+1/\+1")] PlusOnePlusOne,
    [RegexPattern(@"-1/-1")] MinusOneMinusOne,
    Charge,
    Defense,
    Energy,
    Finality,
    Lore,
    Loyalty,
    Oil,
    Poison,
    Stun,
    Time,

    // Keyword counters
    Deathtouch,
    DoubleStrike,
    FirstStrike,
    Flying,
    Haste,
    Hexproof,
    Indestructible,
    Lifelink,
    Menace,
    Reach,
    Shadow,
    Trample,
    Vigilance,

    // Mechanic counters
    Age,
    [RegexPattern("crank!")] Crank,
    Divinity,
    Fade,
    Ki,
    Level,
    Rad,
    Shield,
    Spore,
    Ticket,

    // Cycle counters
    Brick,
    Depletion,
    Experience,
    Quest,
    Storage,
    Verse,

    // Other (multiple cards)
    Acorn,
    Aim,
    Blaze,
    Blood,
    Bounty,
    Coin,
    Collection,
    Corpse,
    Delay,
    Devotion,
    Doom,
    Dream,
    Egg,
    Eon,
    Fate,
    Feather,
    Fetch,
    Flame,
    Flood,
    Fungus,
    Fuse,
    Gold,
    Growth,
    Hatchling,
    Healing,
    Hit,
    Hour,
    Ice,
    Infection,
    Judgment,
    Landmark,
    Luck,
    Net,
    Omen,
    Page,
    Plague,
    Point,
    Pressure,
    Scream,
    Slime,
    Soul,
    Study,
    Tide,
    Velocity,
    Void,
    Wind,
    Wish,

    // Other (single card)
    Aegis,
    Arrow,
    Arrowhead,
    Awakening,
    Bait,
    Blessing,
    Blight,
    Bloodline,
    Bloodstain,
    Book,
    Bore,
    Brain,
    Bribery,
    Burden,
    Cage,
    Carrion,
    Chip,
    Chorus,
    Contested,
    Credit,
    Croak,
    Crystal,
    Component,
    Corruption,
    Cube,
    Currency,
    Death,
    Descent,
    Despair,
    Discovery,
    Dread,
    Duty,
    Echo,
    Elixir,
    Ember,
    Enlightened,
    Eruption,
    Everything,
    Eyeball,
    Eyestalk,
    Feeding,
    Fellowship,
    Filibuster,
    Foreshadow,
    Funk,
    Fury,
    Gem,
    Ghostform,
    Globe,
    Glyph,
    Hack,
    Harmony,
    Hatching,
    Hone,
    Hoofprint,
    Hope,
    Hourglass,
    Hunger,
    Husk,
    Impostor,
    Incarnation,
    Incubation,
    Influence,
    Ingenuity,
    Intel,
    Intervention,
    Isolation,
    Invitation,
    Javelin,
    Kick,
    Knickknack,
    Knowledge,
    Loot,
    Magnet,
    Manifestation,
    Mannequin,
    Matrix,
    Memory,
    Midway,
    Mine,
    Mining,
    Mire,
    Music,
    Muster,
    Necrodermis,
    Nest,
    Night,
    Ore,
    Pain,
    Palliation,
    Paralyzation,
    Pause,
    Petal,
    Petrification,
    Phylactery,
    Phyresis,
    Pin,
    Plot,
    Polyp,
    [RegexPattern("Pop!")] Pop,
    Possession,
    Prey,
    Pupa,
    Rejection,
    Reprieve,
    Rev,
    Revival,
    Ribbon,
    Ritual,
    Rope,
    Rust,
    Scroll,
    Shell,
    Shoe,
    Shred,
    Skewer,
    Silver,
    Sleep,
    Sleight,
    Slumber,
    Soot,
    Spark,
    Spite,
    Stash,
    Story,
    Strife,
    Supply,
    Suspect,
    Takeover,
    Task,
    Theft,
    [RegexPattern("third-degree-burn")] ThirdDegreeBurn,
    Tower,
    Training,
    Trap,
    Treasure,
    Unity,
    Unlock,
    Valor,
    Vitality,
    Vortex,
    Vow,
    Voyage,
    Wage,
    Winch,

    // Test card counters
    Art,
    BasePower,
    BaseToughness,
    Day,
    Glass,
    Hole,
    Manabond,
    Milk,
    Primeval,
    Rebuilding,
    Release,
    Resonance,
    Shy,
    Stroopwafel,
    Token
}

public enum CreatureType
{
    Advisor,
    Aetherborn,
    Ajani,
    Ally,
    Angel,
    Angrath,
    Antelope,
    Ape,
    Arcane,
    Archer,
    Archon,
    Arlinn,
    Artificer,
    Ashiok,
    Assassin,
    [RegexPattern("assembly-worker")] AssemblyWorker,
    Atog,
    Aura,
    Aurochs,
    Avatar,
    Badger,
    Barbarian,
    Bard,
    Basilisk,
    Bat,
    Bear,
    Beast,
    Beeble,
    Berserker,
    Bird,
    Boar,
    Bolas,
    Bringer,
    Brushwagg,
    Camel,
    Carrier,
    Cartouche,
    Cat,
    Centaur,
    Cephalid,
    Chandra,
    Chimera,
    Cleric,
    Clue,
    Cockatrice,
    Construct,
    Crab,
    Crocodile,
    Curse,
    Cyclops,
    Dauthi,
    Demon,
    Desert,
    Devil,
    Dinosaur,
    Djinn,
    Dog,
    Domri,
    Dovin,
    Dragon,
    Drake,
    Dreadnought,
    Drone,
    Druid,
    Dryad,
    Dwarf,
    Efreet,
    Egg,
    Elder,
    Eldrazi,
    Elemental,
    Elephant,
    Elf,
    Elk,
    Elspeth,
    Equipment,
    Eye,
    Faerie,
    Ferret,
    Fish,
    Flagbearer,
    Forest,
    Fortification,
    Fox,
    Frog,
    Fungus,
    Gargoyle,
    Garruk,
    Gate,
    Giant,
    Gideon,
    Gnome,
    Goat,
    Goblin,
    God,
    Golem,
    Gorgon,
    Gremlin,
    Griffin,
    Hag,
    Harpy,
    Hellion,
    Hippo,
    Hippogriff,
    Homarid,
    Homunculus,
    Horror,
    Horse,
    Huatli,
    Human,
    Hydra,
    Hyena,
    Illusion,
    Imp,
    Incarnation,
    Insect,
    Island,
    Jace,
    Jackal,
    Jellyfish,
    Juggernaut,
    Kaito,
    Karn,
    Kavu,
    Kiora,
    Kirin,
    Kithkin,
    Knight,
    Kobold,
    Kor,
    Koth,
    Kraken,
    Lair,
    Lamia,
    Lammasu,
    Leech,
    Leviathan,
    Lhurgoyf,
    Licid,
    Liliana,
    Lizard,
    Locus,
    Manticore,
    Masticore,
    Mercenary,
    Merfolk,
    Metathran,
    Mine,
    Minion,
    Minotaur,
    Mole,
    Monger,
    Mongoose,
    Monk,
    Monkey,
    Moonfolk,
    Mountain,
    Mutant,
    Myr,
    Mystic,
    Naga,
    Nahiri,
    Narset,
    Nautilus,
    Nephilim,
    Nightmare,
    Nightstalker,
    Ninja,
    Nissa,
    Nixilis,
    Noble,
    Noggle,
    Nomad,
    Nymph,
    Octopus,
    Ogre,
    Ooze,
    Orc,
    Orgg,
    Ouphe,
    Ox,
    Oyster,
    Pangolin,
    Pegasus,
    Pest,
    Phelddagrif,
    Phoenix,
    Phyrexian,
    Pilot,
    Pirate,
    Plains,
    Plant,
    [RegexPattern("power-plant")] PowerPlant,
    Praetor,
    Processor,
    Rabbit,
    Ral,
    Ranger,
    Rat,
    Rebel,
    Rhino,
    Rigger,
    Rogue,
    Sable,
    Saheeli,
    Salamander,
    Samurai,
    Samut,
    Sarkhan,
    Satyr,
    Scarecrow,
    Scorpion,
    Scout,
    Serpent,
    Shade,
    Shaman,
    Shapeshifter,
    Shark,
    Sheep,
    Shrine,
    Siren,
    Skeleton,
    Slith,
    Sliver,
    Slug,
    Snake,
    Soldier,
    Soltari,
    Sorin,
    Spawn,
    Specter,
    Spellshaper,
    Sphinx,
    Spider,
    Spike,
    Spirit,
    Sponge,
    Squid,
    Squirrel,
    Starfish,
    Surrakar,
    Swamp,
    Tamiyo,
    Tezzeret,
    Thalakos,
    Thopter,
    Thrull,
    Tibalt,
    Tower,
    Trap,
    Treefolk,
    Trilobite,
    Troll,
    Turtle,
    Ugin,
    Unicorn,
    [RegexPattern("urza's")] Urzas,
    Vampire,
    Vedalken,
    Vehicle,
    Venser,
    Viashino,
    Volver,
    Vraska,
    Wall,
    Warlock,
    Warrior,
    Weird,
    Werewolf,
    Whale,
    Wizard,
    Wolf,
    Wolverine,
    Wombat,
    Worm,
    Wraith,
    Wurm,
    Xenagos,
    Yeti,
    Zombie,
    Zubera
}

[OptionalPlural]
public enum GainOrLose
{
    Lose,
    Gain
}

public enum PermanentVerb
{
    [RegexPattern("get(s)?")]
    Get,

    [RegexPattern("have", "has")]
    Have,

    [RegexPattern("deal(s)?")]
    Deal,

    [RegexPattern("gain(s)?")]
    Gain,

    [RegexPattern("lose(es)?")]
    Lose,

    [RegexPattern("it's")]
    Is,
}

public enum WhichPlayer
{
    You,

    [RegexPattern("each opponent")]
    EachOpponent,

    [RegexPattern("each opponent")]
    AnyOpponent
}

public enum PlayerIdentity
{
    Player,
    Opponent
}

public enum CardPlace
{
    Graveyard,
    Hand,
}

public enum PlusMinus
{
    [RegexPattern(@"\+")]
    Plus,

    [RegexPattern("-")]
    Minus,
}

public enum VariableName
{
    X,
    Y
}

public enum LandType
{
    [RegexPattern("forest(s)?")]
    Forest,

    [RegexPattern("island(s)?")]
    Island,

    [RegexPattern("mountains(s)?")]
    Mountain,

    [RegexPattern("plains")]
    Plains,

    [RegexPattern("swamp(s)?")]
    Swamp
}

public enum TemporalDisposition
{
    At,
    During,
    Until
}

public enum PhasePart
{
    Beginning,
    End
}

public enum Whose
{
    [RegexPattern("(an|your) opponent's")]
    Opponent,

    [RegexPattern("each player's")]
    EachPlayer,

    Your,

    [RegexPattern("a")]
    Any,

    TheNext
}

public enum Phase
{
    Upkeep,
    DrawStep,
    MainPhase,
    CombatPhase,
    CombatStep,
    DeclareAttackersStep,
    DeclareBlockersStep,
    DamageStep,
    EndStep,
    EndOfTurn
}

public enum NonBattlefieldZone
{
    [RegexPattern("exile(d)?")]
    Exile,

    Graveyard,
    Hand,
    Library,

    [RegexPattern("you own outside the game")]
    Sideboard,

    [RegexPattern("spell")]
    Stack
}

public enum PowerAndOrToughness
{
    Power,
    Toughness,
    PowerAndToughness
}

public enum EquivalentToMeasurement
{
    ItsManaValue,
}

public enum Assertion
{
    Is,

    [RegexPattern("isn't")]
    Isnt
}

public enum Who
{
    [RegexPattern("that creature's controller")]
    ThatCreaturesController
}