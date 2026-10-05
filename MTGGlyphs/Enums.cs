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

[OptionalPlural]
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
    Conspire,
    Convoke,
    CumulativeUpkeep,
    Cycling,
    Delve,
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
    Morph,
    Multikicker,
    Ninjutsu,
    Offering,
    Overload,
    Persist,
    Phasing,
    Poisonous,
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
    Transmute,
    Typecycling,
    Undying,
    Unearth,
    Unleash,
    Vanishing,
    Wither,

    // Discontinued
    Landhome,
    Substance
}

public enum KeywordAction
{
    Attach,
    Clash,
    Counter,
    Detain,
    Exile,
    Fateseal,
    Fight,
    Monstrosity,
    Populate,
    Proliferate,
    Regenerate,
    Sacrifice,
    Scry,
    Tap,
    Transform,
    Untap,

    // Discontinued
    Bury
}

public enum AbilityWord
{
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
    Threshold
}

/// <summary>Named counters. Power/toughness counters ("+1/+1", "+1/+0") are a <see cref="PowerToughnessMod"/> instead (see <see cref="Counters"/>).</summary>
public enum CounterType
{
    // Common counters
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
    Ally,
    Angel,
    Antelope,
    Ape,
    Archer,
    Archon,
    Artificer,
    Assassin,
    [RegexPattern("assembly-worker")] AssemblyWorker,
    Atog,
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
    Bringer,
    Brushwagg,
    Camel,
    Carrier,
    Cat,
    Centaur,
    Cephalid,
    Chimera,
    Cleric,
    Cockatrice,
    Construct,
    Crab,
    Crocodile,
    Cyclops,
    Dauthi,
    Demon,
    Devil,
    Dinosaur,
    Djinn,
    Dog,
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
    Eye,
    Faerie,
    Ferret,
    Fish,
    Flagbearer,
    Fox,
    Frog,
    Fungus,
    Gargoyle,
    Giant,
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
    Human,
    Hydra,
    Hyena,
    Illusion,
    Imp,
    Incarnation,
    Insect,
    Jackal,
    Jellyfish,
    Juggernaut,
    Kavu,
    Kirin,
    Kithkin,
    Knight,
    Kobold,
    Kor,
    Kraken,
    Lamia,
    Lammasu,
    Leech,
    Leviathan,
    Lhurgoyf,
    Licid,
    Lizard,
    Manticore,
    Masticore,
    Mercenary,
    Merfolk,
    Metathran,
    Minion,
    Minotaur,
    Mole,
    Monger,
    Mongoose,
    Monk,
    Monkey,
    Moonfolk,
    Mutant,
    Myr,
    Mystic,
    Naga,
    Nautilus,
    Nephilim,
    Nightmare,
    Nightstalker,
    Ninja,
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
    Plant,
    Praetor,
    Processor,
    Rabbit,
    Ranger,
    Rat,
    Rebel,
    Rhino,
    Rigger,
    Rogue,
    Sable,
    Salamander,
    Samurai,
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
    Siren,
    Skeleton,
    Slith,
    Sliver,
    Slug,
    Snake,
    Soldier,
    Soltari,
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
    Thalakos,
    Thopter,
    Thrull,
    Treefolk,
    Trilobite,
    Troll,
    Turtle,
    Unicorn,
    Vampire,
    Vedalken,
    Viashino,
    Volver,
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

    [RegexPattern("gain(s)?")]
    Gain,

    [RegexPattern("lose(s)?")]
    Lose,
}

public enum WhichPlayer
{
    You,

    [RegexPattern("each opponent")]
    EachOpponent,

    [RegexPattern("an opponent")]
    AnyOpponent
}

public enum PlayerIdentity
{
    Player,
    Opponent
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

[OptionalPlural]
public enum LandType
{
    Desert,
    Forest,
    Gate,
    Island,
    Lair,
    Locus,
    Mine,
    Mountain,
    Plains,
    [RegexPattern("power-plant")] PowerPlant,
    Swamp,
    Tower,
    [RegexPattern("urza's")] Urzas
}

public enum ArtifactType
{
    Clue,
    Equipment,
    Fortification,
    Vehicle
}

public enum EnchantmentType
{
    Aura,
    Cartouche,
    Curse,
    Shrine
}

public enum SpellType
{
    Arcane,
    Trap
}

public enum PlaneswalkerType
{
    Ajani,
    Angrath,
    Arlinn,
    Ashiok,
    Bolas,
    Chandra,
    Domri,
    Dovin,
    Elspeth,
    Garruk,
    Gideon,
    Huatli,
    Jace,
    Kaito,
    Karn,
    Kiora,
    Koth,
    Liliana,
    Nahiri,
    Narset,
    Nissa,
    Nixilis,
    Ral,
    Saheeli,
    Samut,
    Sarkhan,
    Sorin,
    Tamiyo,
    Tezzeret,
    Tibalt,
    Ugin,
    Venser,
    Vraska,
    Xenagos
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
