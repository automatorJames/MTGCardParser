namespace MTGPlexer.Static;

public static class Lists
{
    public static readonly HashSet<string> Keywords =
    [
        // Evergreen
        "deathtouch", "defender", "first strike", "double strike", "enchant", "equip", "flash", "flying",
        "haste", "hexproof", "indestructible", "intimidate", "landwalk", "lifelink", "protection", "reach",
        "shroud", "trample", "vigilance",
    
        // Keyword Actions
        "attach", "counter", "exile", "fight", "regenerate", "sacrifice", "tap", "untap",
    
        // Expansion Keywords
        "absorb", "affinity", "amplify", "annihilator", "aura swap", "banding", "bands with other", "battle cry",
        "bestow", "bloodthirst", "bushido", "buyback", "cascade", "champion", "changeling", "cipher", "clash",
        "conspire", "convoke", "cumulative upkeep", "cycling", "delve", "detain", "devour", "dredge", "echo",
        "entwine", "epic", "evolve", "evoke", "exalted", "extort", "fading", "fateseal", "fear", "flanking",
        "flashback", "flip", "forecast", "fortify", "frenzy", "graft", "gravestorm", "haunt", "hideaway",
        "horsemanship", "infect", "kicker", "level up", "living weapon", "madness", "miracle", "modular",
        "monstrosity", "morph", "multikicker", "ninjutsu", "offering", "overload", "persist", "phasing",
        "poisonous", "populate", "proliferate", "provoke", "prowl", "rampage", "rebound", "recover", "reinforce",
        "replicate", "retrace", "ripple", "scavenge", "scry", "shadow", "soulbond", "soulshift", "splice",
        "split second", "storm", "sunburst", "suspend", "totem armor", "transfigure", "transform", "transmute",
        "typecycling", "undying", "unearth", "unleash", "vanishing", "wither",
    
        // Ability Words
        "battalion", "bloodrush", "channel", "chroma", "domain", "fateful hour", "grandeur", "hellbent",
        "heroic", "imprint", "join forces", "kinship", "landfall", "metalcraft", "morbid", "radiance",
        "sweep", "threshold",
    
        // Discontinued
        "bury", "landhome", "substance"
    ];

    public static readonly List<string> CardTypes = 
    [
        "artifact",
        "creature",
        "enchantment",
        "instant",
        "land",
        "planeswalker",
        "sorcery",
        "battle",
        "tribal"
    ];

    public static readonly HashSet<string> Subtypes =
    [
        "advisor", "aetherborn", "ajani", "ally", "angel", "angrath", "antelope", "ape", "arcane",
        "archer", "archon", "arlinn", "artificer", "ashiok", "assassin", "assembly-worker", "atog",
        "aura", "aurochs", "avatar", "badger", "barbarian", "bard", "basilisk", "bat", "bear", "beast",
        "beeble", "berserker", "bird", "boar", "bolas", "bringer", "brushwagg", "camel", "carrier",
        "cartouche", "cat", "centaur", "cephalid", "chandra", "chimera", "cleric", "clue", "cockatrice",
        "construct", "crab", "crocodile", "curse", "cyclops", "dauthi", "demon", "desert", "devil",
        "dinosaur", "djinn", "dog", "domri", "dovin", "dragon", "drake", "dreadnought", "drone", "druid",
        "dryad", "dwarf", "efreet", "egg", "elder", "eldrazi", "elemental", "elephant", "elf", "elk",
        "elspeth", "equipment", "eye", "faerie", "ferret", "fish", "flagbearer", "forest", "fortification",
        "fox", "frog", "fungus", "gargoyle", "garruk", "gate", "giant", "gideon", "gnome", "goat",
        "goblin", "god", "golem", "gorgon", "gremlin", "griffin", "hag", "harpy", "hellion", "hippo",
        "hippogriff", "homarid", "homunculus", "horror", "horse", "huatli", "human", "hydra", "hyena",
        "illusion", "imp", "incarnation", "insect", "island", "jace", "jackal", "jellyfish", "juggernaut",
        "kaito", "karn", "kavu", "kiora", "kirin", "kithkin", "knight", "kobold", "kor", "koth", "kraken",
        "lair", "lamia", "lammasu", "leech", "leviathan", "lhurgoyf", "licid", "liliana", "lizard",
        "locus", "manticore", "masticore", "mercenary", "merfolk", "metathran", "mine", "minion",
        "minotaur", "mole", "monger", "mongoose", "monk", "monkey", "moonfolk", "mountain", "mutant",
        "myr", "mystic", "naga", "nahiri", "narset", "nautilus", "nephilim", "nightmare", "nightstalker",
        "ninja", "nissa", "nixilis", "noble", "noggle", "nomad", "nymph", "octopus", "ogre", "ooze",
        "orc", "orgg", "ouphe", "ox", "oyster", "pangolin", "pegasus", "pest", "phelddagrif", "phoenix",
        "phyrexian", "pilot", "pirate", "plains", "plant", "power-plant", "praetor", "processor", "rabbit",
        "ral", "ranger", "rat", "rebel", "rhino", "rigger", "rogue", "sable", "saheeli", "salamander",
        "samurai", "samut", "sarkhan", "satyr", "scarecrow", "scorpion", "scout", "serpent", "shade",
        "shaman", "shapeshifter", "shark", "sheep", "shrine", "siren", "skeleton", "slith", "sliver",
        "slug", "snake", "soldier", "soltari", "sorin", "spawn", "specter", "spellshaper", "sphinx",
        "spider", "spike", "spirit", "sponge", "squid", "squirrel", "starfish", "surrakar", "swamp",
        "tamiyo", "tezzeret", "thalakos", "thopter", "thrull", "tibalt", "tower", "trap", "treefolk",
        "trilobite", "troll", "turtle", "ugin", "unicorn", "urza’s", "vampire", "vedalken", "vehicle",
        "venser", "viashino", "volver", "vraska", "wall", "warlock", "warrior", "weird", "werewolf",
        "whale", "wizard", "wolf", "wolverine", "wombat", "worm", "wraith", "wurm", "xenagos", "yeti",
        "zombie", "zubera"
    ];

    public static readonly List<string> Colors =
    [
        "white",
        "blue",
        "black",
        "red",
        "green"
    ];

    public static readonly List<string> GamePhases =
    [
        "upkeep",
        "draw step",
        "main phase",
        "combat phase",
        "combat step",
        "declare attackers step",
        "declare blockers step",
        "damage step",
        "end step",
        "end of turn"
    ];

    public static readonly List<string> PlayerIdentifiers =
    [
        "your opponent's",
        "each opponent's",
        "an opponent's",
        "the active player's",
        "the defending player's",
        "the attacking player's",
        "each player's",
        "your"
    ];
}

