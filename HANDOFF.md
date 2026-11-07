# Handoff: MTG card text → rules IR → engine

State as of 2026-11-07 (master `c7f4e000`). Numbers come from the full-corpus outputs in `C:/Users/James/source/forge-study/lower-full` (`report.txt`, `interpret-report.txt`, `castcheck.txt`, `rules.jsonl`).

## The three layers

The work splits into three layers. Each one consumes the previous layer's output. "Grammar / IR / Engine" is the right division, with two notes:
- The IR layer's work is the **lowering**: translating parse trees into typed IR records. The IR itself (`MTGRulesIR/Model`) is the interface between the grammar and the engine.
- Two concerns cut across the layers: **card data** (what text reaches the pipeline) and **semantic correctness** (whether "executable" cards actually do the right thing).

| Layer | Projects | Input → output | Metric | Status |
|---|---|---|---|---|
| **Grammar** | `Glyphotype` (generic parsing engine; must stay corpus-generic, with no MTG references), `MTGGlyphs` (the MTG grammar: 4,377 glyph types), `DocumentAnalysisInterface` (the app that hosts the grammar MCP server) | Oracle text → glyph parse trees | Lines fully parsed | **100%**, but 3,041 of the 4,377 glyph types are `ICardSpecific` literals (see below) |
| **IR (lowering)** | `MTGRulesIR` (model + `Lowering/`), `MTGRulesIR.Tests` | Parse trees → `CardRules` records (`rules.jsonl`), with `Opaque*` nodes wherever no lowering rule exists | Lines fully lowered and "engine-closed" (no opaque nodes, no open slots) | **90.1% of lines**, 83.2% of cards |
| **Engine** | `MTGEngine.Core`, `MTGEngine.Agents`, `MTGEngine.Tests` | IR → playable game (immutable `GameState`, event log, decisions as data) | Lines and cards whose every IR node the interpreter handles ("executable") | **85.1% of lines (40,728 / 47,865)**, **76.1% of cards (19,964 / 26,228)** |

The engine's percentage can never exceed the lowering's, because an `Opaque*` node is unexecutable by definition. So "the grammar says 100%" and "the engine says 76%" are both true. Most of the gap is lowering, not grammar.

### What "100% grammar coverage" hides

The grammar reached 100% in part by generating one literal glyph per leftover clause, using `tools/card-specific-glyphs/`. Examples are `TariffRule` and `KumenasAwakeningRule`, plus `<Card>Effect` for the text after a trigger or cost. These match a fixed clause as a whole and have no internal structure, so lowering cannot translate them, and each one lowers to an `Opaque*` node carrying the text and the glyph name. A decision on 2026-10-28 was to retire these with **hand-written IR records per card** (or an agent writing them), not with grammar work first. **No mechanism for hand-written IR records exists yet** (see step 2 below).

## Where the 6,264 non-executable cards are blocked

From `rules.jsonl`. A "literal" glyph is one that only a single card uses: 2,887 such glyphs, a close proxy for `ICardSpecific`.

| Blocked by | Cards | Layer that has to fix it |
|---|---|---|
| Opaque nodes from **literal glyphs only** | 2,284 | IR: hand-written IR records per card, or grammar: replace the literal with compositional glyphs |
| Opaque nodes from **general glyphs only** (410 glyphs with no lowering rule) | 1,843 | IR: lowering rules in `MTGRulesIR/Lowering` |
| Both kinds of opaque node | 256 | IR (both of the above) |
| No opaque nodes, but node kinds the interpreter doesn't handle | about 1,880 | Engine |
| Open (untyped) slots, "not engine-closed" | 23 | IR (`ZoneRef.Zone=?` 11 lines, `KeywordAction.Other` 3, …) |

Once a card stops being opaque, it may still hit an engine gap, so lowering gains won't turn into executable cards one-for-one.

### Engine-only blockers (no opaque nodes)

These come from the "only blocker" section of `interpret-report.txt` (274 node kinds). The tail is flat, so most items unblock 5–30 cards each.
- **Double-faced cards: about 250 cards.**
  - Blockers: `Transform` (144 cards), `Keyword.Daybound` (29), `Keyword.Disturb` (23), `KeywordAction.Flip` (15), plus 14 day/night glyphs on the lowering side.
  - **Data gap:** a double-faced card's `CardRules` holds only its front face's lines. For example, *Aberrant Researcher // Perfected Form* has no Perfected Form lines in `rules.jsonl`. The corpus has 757 cards with `A // B` names. Back-face text must reach the lowering, and the engine needs a face model, before any of this works. This is the largest single engine item.
- **Mana:** `ManaRestriction.PoolMana` (28), `SpendManaAsThough` (9), `ManaDoesntEmpty` (9).
- **Stack:** `Term[StackAbilityKind]` (24): counter or copy target activated or triggered ability.
- **Keywords and keyword actions, 8–19 cards each:** Banding, Venture, Empower, Fuse, Conspire, Emerge, Hideaway, Living Weapon, Mobilize, Blight, Endure, Recruit, Afterlife, Amplify, Battle Cry, Escalate, Firebending, Mutate (30 lines), Gift, Ascend, Amass, Exert, …
- **Other:**
  - `Union.Empty` (20)
  - `EffectLine.OnPermanent[...]` statics the engine doesn't apply yet (Sequence 24, UnlessPaid 17, CastAsThoughFlash 13, Lasting 9)
  - `PlayerEvent.FullyUnlocksARoom` (14)
  - `PutCounters.Kind` (14)
  - `StaticAbility[CostModification]` (13)
  - `TriggeredAbility[NamedEvent]` (15)
  - `AdditionalPhase` (10)
  - `Repeat` ("repeat this process", 23 lines)
  - "prepared" (`Term[PermanentDesignation.Prepared]`, 62 lines)

## What it takes to reach 100% executable

In order of value per unit of effort:

1. **Lowering rules for the 410 general glyphs (about 1,850 cards, plus some of the 256 mixed).**
   - The work is pure C# in `MTGRulesIR/Lowering`: add a `Rules.RoundTwelve.cs` (rounds two to eleven exist), with tests in `MTGRulesIR.Tests`.
   - Top glyphs touch 12–16 cards each: `IsCreatureWithBaseStats`, `AssignsDamageByToughness`, `DurationScopedTrigger`, `BecomesCreatureWithQuotedAbility`, `NamedLegendaryToken`, `CraftWith`, `CastThisSpellDuring`, `PayToEndEffect`, `MaximumHandSize`, `TimesKicked`, …
   - `report.txt` ranks them all.
   - No grammar change is needed unless a glyph doesn't expose the typed slots the rule needs.
   - Where a rule introduces a new IR shape, add engine support for it too.
2. **Hand-written IR for the literal glyphs (about 2,300 cards, one record each).**
   - Build the mechanism first: an overrides file (for example, jsonl keyed by card name and line index or text) that `CorpusLowering` merges in place of the opaque node, validated by the same closure check.
   - Then have agents write the records in batches from the clause text, using the existing IR vocabulary.
   - Many literal clauses are really instances of general patterns. When a batch shows a repeating shape, write a general glyph plus a lowering rule instead; that needs the grammar MCP server.
3. **Double-faced cards (about 250 cards).** Ingest the back-face text, give `CardDefinition` and `GameObject` a face model, then implement Transform, Daybound/Nightbound with day and night, Disturb and Flip.
4. **The engine tail (about 1,600 cards).** Work down the "only blocker" list in `interpret-report.txt`.
5. **Semantic correctness, alongside all of the above.** "Executable" only means every node has a handler. The latest castcheck:
   - 18,889 executable nonland cards: 18,452 cast, 0 offered but not cast, 0 threw.
   - **175 resolved with no effect** (each one usually a wrong reference or a condition that never holds).
   - **437 never offered** (some legitimately; review them).
   - Known lowering bugs:
     - *Wormfang Crab* exiles itself: the grammar binds "it" to the card.
     - "Target creature's owner puts it on their choice of the top or bottom of their library" (Dire Downdraft, Run Behind, Uneasy Partings) loses its target.
   - Grammar binding errors are repaired after lowering in `MTGRulesIR/Lowering/SelfReferenceRepair.cs`, a reflection-based IR rewriter, so extend that file.

A more honest headline metric would be worth adding next to "executable": "casts and resolves with an effect" from castcheck.

## Tooling

Build `ConsoleUtility` first. The binaries are in `ConsoleUtility/bin/Debug/net10.0`, and full-corpus outputs go to `C:/Users/James/source/forge-study/lower-full`.

| Command | What it does | Time |
|---|---|---|
| `dotnet ConsoleUtility.dll lower <dir> every` | Re-lowers the corpus and writes `rules.jsonl`, `report.txt` (opaque glyphs ranked) and `census.txt` | about 3 min |
| `dotnet ConsoleUtility.dll interpret <rules.jsonl> <dir>` | Writes `interpret-report.txt`: executable %, unhandled node kinds, and sole blockers by card | about 1 min |
| `dotnet ConsoleUtility.dll simulate <rules.jsonl> N seed` | Random two-color games of every executable card; reports exceptions and run-time skips. `SIMULATE_DUMP=<dir>` writes a failing game's log | about 0.6 s/game; keep N ≤ 300 under the 10-minute command limit |
| `dotnet ConsoleUtility.dll castcheck <rules.jsonl> <out.txt>` | Casts every executable card on a rich board (`ConsoleUtility/CastCheck.cs`) | about 25 min; **locks the build output** |
| `dotnet ConsoleUtility.dll lowertext "text"` | One line's IR (slow: builds the grammar each time) | |
| `tools/ir_samples.py <rules.jsonl> <Kind> [n] [field=value]`, `tools/ir_tally.py` | Sample or tally IR node shapes (run with Python312, `-I -X utf8`) | |
| `tools/add_test_cards.py <rules.jsonl> <names...>` | Adds real lowered cards to the engine test fixture `MTGEngine.Tests/Cards/cards.jsonl` | |

- **Tests:** lowering tests take about 2 min, and engine tests are under `MTGEngine.Tests`. Together there are about 370 tests.
- **Engine tests:** write them against real fixture cards, not just hand-built IR. Real cards keep exposing lowering-level semantic gaps.
- **Grammar server:** the grammar MCP server is the DocumentAnalysisInterface app. When the MCP connection is down but the app is running, POST JSON-RPC to `localhost:5253/mcp`; it was reachable on 2026-11-07.
- **Verification budget:** run one full castcheck per session, not one per change.

## Engine map (`MTGEngine.Core`)

- `Interpretation/`: `Interpreter.cs` (effects), `Resolver.cs` (selectors, references such as "that player" and "it"), `Filters.cs`, `Quantities.cs`, `KeywordActions.cs`, `Keywords.cs` (which keywords count as carried out).
- `Rules/`: `Game.cs`, `Casting.cs`, `Combat.cs`, `Triggers.cs`, `Characteristics.cs` (layers, cached per immutable state), `ManaPayment.cs`, `Replacements.cs`, `GraveyardCasts.cs`, `SpellKeywords.cs`, `Phasing.cs`, `FaceDown.cs`, `Copies.cs`, …
- `Model/`: `CardDefinition.cs`, `GameState.cs`, `KeywordAbilities.cs` (keyword → ability expansions), `Modal.cs`, `Levels.cs`.
- `Report/InterpretReport.cs`: the support sets behind the executable %. **Keep it in step with the engine**, so the report and the engine can't disagree.
- Design rules:
  - Every change is a `GameEvent` applied by the reducer.
  - `Game.Skip` records IR the engine can't act on instead of throwing.
  - Decisions go through `IDecisionMaker`.
  - Randomness is seeded.

## Conventions

- Commit directly to `master` and push after each finished unit. Don't hard-wrap commit messages.
- Stage only the paths you touched; never `git add -A`. `.codex/` and `.agents/` stay untracked.
- No MTG references in `Glyphotype` code or comments.
- Shell editing pitfalls:
  - `\b` and `\n` inside bash or python heredocs get mangled. Write edit scripts to a file, or use a real editor tool.
  - PowerShell `Get-Content`/`Set-Content` corrupts non-ASCII.
- The Forge clone at `C:/Users/James/source/forge-study` is a design reference only (GPL); don't copy code from it.
