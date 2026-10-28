namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A subtype as card text names it, mostly creature types ("elf", "assembly-worker", "ally"/"allies") but also planeswalker ("ajani"), artifact ("clue", "equipment"), enchantment ("aura", "curse"), land ("desert", "urza's") and spell ("arcane") subtypes; in token descriptions, "becomes a …" and "is also a …" effects, and "protection from goblins".</summary>
public enum CreatureType
{
    Advisor,

    Aetherborn,

    [RegexPattern("ally", "allies")]
    Ally,

    Angel,

    Angrath,

    Antelope,

    Ape,

    Arcane,

    Archer,

    Archon,

    Arlinn,

    Army,

    Artificer,

    Assassin,

    [RegexPattern("assembly-worker")]
    AssemblyWorker,

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

    Bringer,

    Brushwagg,

    Camel,

    Carrier,

    Cartouche,

    Cat,

    Centaur,

    Cephalid,

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

    Dragon,

    Drake,

    Dreadnought,

    Drone,

    Druid,

    Dryad,

    [RegexPattern("dwarf", "dwarves")]
    Dwarf,

    Efreet,

    Egg,

    Elder,

    Eldrazi,

    Elemental,

    Elephant,

    [RegexPattern("elf", "elves")]
    Elf,

    Elk,

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

    [RegexPattern("fungus", "fungi")]
    Fungus,

    Gargoyle,

    Gate,

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

    [RegexPattern("harpy", "harpies")]
    Harpy,

    Hellion,

    Hippo,

    Hippogriff,

    Homarid,

    [RegexPattern("homunculus", "homunculi")]
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

    Island,

    Jackal,

    Jellyfish,

    Juggernaut,

    Kavu,

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

    Lizard,

    Locus,

    Manticore,

    Masticore,

    [RegexPattern("mercenary", "mercenaries")]
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

    Moogle,

    Moonfolk,

    Mountain,

    Mutant,

    Myr,

    Mystic,

    Naga,

    Narset,

    Nautilus,

    Nephilim,

    Nightmare,

    Nightstalker,

    Ninja,

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

    [RegexPattern("ox", "oxen")]
    Ox,

    Oyster,

    Pangolin,

    [RegexPattern("pegasus", "pegasi")]
    Pegasus,

    Pest,

    Phelddagrif,

    Phoenix,

    Phyrexian,

    Pilot,

    Pirate,

    Plains,

    Plant,

    [RegexPattern("power-plant")]
    PowerPlant,

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

    Shrine,

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

    [RegexPattern("sphinx", "sphinxes")]
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

    Symbiote,

    Thalakos,

    Thopter,

    Thrull,

    Tower,

    Trap,

    Treefolk,

    Trilobite,

    Troll,

    Turtle,

    Unicorn,

    [RegexPattern("urza's")]
    Urzas,

    Vampire,

    Vedalken,

    Vehicle,

    Viashino,

    Volver,

    Wall,

    Warlock,

    Warrior,

    Weird,

    [RegexPattern("werewolf", "werewolves")]
    Werewolf,

    Whale,

    Wizard,

    [RegexPattern("wolf", "wolves")]
    Wolf,

    Wolverine,

    Wombat,

    Worm,

    Wraith,

    Wurm,

    Xenagos,

    Yeti,

    Zombie,

    Zubera,

    Saproling,

    Graveborn,

    Citizen,

    Serf,

    Camarid,

    Deserter,

    Survivor,

    Splinter,

    Tetravite,

    Caribou,

    Orb,

    Sand,

    Servo,

    Scion,

    Treasure,

    Food,

    Alien,

    Armadillo,

    Astartes,

    Azra,

    Balloon,

    Beaver,

    Beholder,

    Blinkmoth,

    Capybara,

    [RegexPattern("child", "children")]
    Child,

    Clown,

    Coward,

    Coyote,

    [RegexPattern("c'tan")]
    CTan,

    Custodes,

    Cyberman,

    Dalek,

    Demigod,

    Detective,

    Doctor,

    Drix,

    Employee,

    Fractal,

    Gamer,

    Germ,

    Gith,

    Glimmer,

    Gnoll,

    Guest,

    Halfling,

    Hamster,

    Inkling,

    Inquisitor,

    Llama,

    Mite,

    Mount,

    [RegexPattern("mouse", "mice")]
    Mouse,

    Necron,

    Otter,

    Peasant,

    Pentavite,

    Performer,

    Pincher,

    Porcupine,

    Possum,

    Primarch,

    Prism,

    Raccoon,

    Reflection,

    Robot,

    Scientist,

    Sculpture,

    Skunk,

    Sloth,

    Snail,

    Synth,

    Tentacle,

    Tiefling,

    Toy,

    Triskelavite,

    Tyranid,

    Varmint,

    Walrus,

    Weasel,

    [RegexPattern("hero", "heroes")]
    Hero,

    Villain,

    Lesson,

    Omen,

    Cave,

    Spacecraft,

    Blood,

    Map,

    Powerstone,

    Incubator,

    Junk,

    Gold,

    Sorcerer,

    Yanling,

    Teyo,

    Daretti,

    Estrid,

}
