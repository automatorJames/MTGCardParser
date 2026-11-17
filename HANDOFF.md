# Handoff: MTG card text → rules IR → engine

State as of 2026-11-17 (master `0b56a19f`). Numbers come from the full-corpus outputs in `C:/Users/James/source/forge-study/lower-r15` (`report.txt`, `interpret-report.txt`, `rules.jsonl`). The last full castcheck is `lower-r15/castcheck.txt` (2026-11-17, after batch 51).

## The three layers

The work splits into three layers. Each one consumes the previous layer's output. "Grammar / IR / Engine" is the right division, with two notes:
- The IR layer's work is the **lowering**: translating parse trees into typed IR records. The IR itself (`MTGRulesIR/Model`) is the interface between the grammar and the engine.
- Two concerns cut across the layers: **card data** (what text reaches the pipeline) and **semantic correctness** (whether "executable" cards actually do the right thing).

| Layer | Projects | Input → output | Metric | Status |
|---|---|---|---|---|
| **Grammar** | `Glyphotype` (generic parsing engine; must stay corpus-generic, with no MTG references), `MTGGlyphs` (the MTG grammar: 4,377 glyph types), `DocumentAnalysisInterface` (the app that hosts the grammar MCP server) | Oracle text → glyph parse trees | Lines fully parsed | **100%**, but 3,041 of the 4,377 glyph types are `ICardSpecific` literals (see below) |
| **IR (lowering)** | `MTGRulesIR` (model, `Lowering/`, `Overrides/`), `MTGRulesIR.Tests` | Parse trees → `CardRules` records (`rules.jsonl`), with `Opaque*` nodes wherever no lowering rule or override exists | Lines fully lowered and "engine-closed" (no opaque nodes, no open slots) | **95.5% of lines (45,698 / 47,865)**, 92.1% of cards (24,169 / 26,228) |
| **Engine** | `MTGEngine.Core`, `MTGEngine.Agents`, `MTGEngine.Tests` | IR → playable game (immutable `GameState`, event log, decisions as data) | Lines and cards whose every IR node the interpreter handles ("executable") | **88.6% of lines (42,402 / 47,865)**, **81.5% of cards (21,384 / 26,228)** |

Progress since the 2026-11-07 handoff (`c7f4e000`): lines fully lowered 90.1% → 96.0%, cards fully lowered → 93.1%, cards executable 76.1% → 82.2% (lines 89.0%). The gain came from lowering rounds twelve to fifteen (general glyphs) and from 1,523 hand-written line overrides (literal glyphs) in 51 batches.

The engine's percentage can never exceed the lowering's, because an `Opaque*` node is unexecutable by definition. Of the engine-closed lines, 92.7% are executable; the rest hit engine gaps.

### What "100% grammar coverage" hides

The grammar reached 100% in part by generating one literal glyph per leftover clause, using `tools/card-specific-glyphs/`. Examples are `TariffRule` and `KumenasAwakeningRule`, plus `<Card>Effect` for the text after a trigger or cost. These match a fixed clause as a whole and have no internal structure, so lowering cannot translate them, and each one lowers to an `Opaque*` node carrying the text and the glyph name. The decision on 2026-10-28 was to retire these with **hand-written IR records per card**, not with grammar work first. That mechanism now exists (`MTGRulesIR/Lowering/LineOverrides.cs`, see "Hand-written line overrides" below) and 1,269 lines have been written.

## Where the remaining non-executable cards are blocked

From `interpret-report.txt` ("node kinds that are a card's only blocker", 351 kinds, flat tail) and `report.txt`.

| Blocked by | Cards | Layer that has to fix it |
|---|---|---|
| Opaque nodes (`OpaqueAbility` 890, `OpaqueEffect` 366, `OpaqueCondition` 75, `OpaqueSelector` 67, `OpaquePredicate` 33, `OpaqueQuantity` 22) | about 1,450 | IR: overrides for literal glyphs (1,078 lines on 1,040 cards are blocked only by literals, nearly all of them needing new vocabulary), lowering rules for the rest |
| `Transform` 171, `Keyword.Daybound` 30, `Keyword.Disturb` 25, `KeywordAction.Flip` 15 | about 240 | Engine: double-faced cards (data gap, see below) |
| Everything else (`Term[StackAbilityKind]` 31, `ManaRestriction.PoolMana` 30, `TriggeredAbility[NamedEvent]` 27, `EffectLine.OnPermanent[…]` statics 25+24+17, `Replacement[WouldHappen.DealDamage]` 24, `Union.Empty` 23, `KeywordAction.Venture` 20, `Qualifier[PlayerIdentity]` 20, `Keyword.Banding` 19, `MayPlayFrom` 18, `AdditionalPhase` 17, `KeywordAction.Empower` 17, `Compared` 16, `PutCounters.Kind` 15, …) | about 1,700 | Engine tail |
| Open (untyped) slots, "not engine-closed" | 16 lines | IR (4 slot kinds, `report.txt` lists them) |

The general-glyph side is nearly exhausted: `report.txt` now ranks 1,864 opaque glyph types but the largest blocks only 12 lines (`SelfAbility`, `PayToEndEffect`, `ChangeText` 11, `ShuffleHandIntoLibrary` 7, then 6 and below). Most of the remaining opaque lines are literal glyphs, and most of those are the hard ones that two alphabetical passes skipped (see below).

### Engine-only blockers (no opaque nodes)

- **Double-faced cards: about 240 cards.** A double-faced card's `CardRules` holds only its front face's lines (for example *Aberrant Researcher // Perfected Form* has no Perfected Form lines in `rules.jsonl`). The corpus has 757 cards with `A // B` names. Back-face text must reach the lowering, and the engine needs a face model, before `Transform`, Daybound/Nightbound, Disturb and Flip can work. This is the largest single engine item and nothing has been done on it yet.
- **Mana:** `ManaRestriction.PoolMana` (30), `ManaDoesntEmpty` (10), `SpendManaAsThough`.
- **Stack:** `Term[StackAbilityKind]` (31): counter or copy target activated or triggered ability.
- **Statics the engine doesn't apply yet:** `EffectLine.OnPermanent[Sequence]` 25, `[Optional.CastAsThoughFlash]` 24, `[UnlessPaid]` 17, `[Lasting]` 10, `[Skip]` 9.
- **Keywords and keyword actions, 8–20 cards each:** Venture, Banding, Empower, Flip, Fuse, Conspire, Emerge, Hideaway, Living Weapon, Mobilize, Blight, Endure, Recruit, Afterlife, Amplify, Battle Cry, Escalate, Firebending, Mutate, Gift, Ascend, Amass, Exert, Harmonize, …
- **Other:** `Replacement[WouldHappen.DealDamage]` 24, `Union.Empty` 23, `Qualifier[PlayerIdentity]` 20, `MayPlayFrom` 18, `AdditionalPhase` 17, `Compared` 16, `PutCounters.Kind` 15, `TriggeredAbility[PlayerAction]` 10, `CantBeTargeted.ByQualifiers` 9, `OfChosen.OfTheCreatureTypeOfYourChoice` 10, `PlayerSelector.Chosen` 10, `Repeat` ("repeat this process"), "prepared" (`Term[PermanentDesignation.Prepared]`).

## Hand-written line overrides (the literal-glyph mechanism)

- **Files:** `MTGRulesIR/Overrides/line-overrides-1.jsonl` … `line-overrides-51.jsonl`, one JSON object per line, `//` lines are comments. The csproj copies them beside the binaries (`CopyToOutputDirectory`), so **ConsoleUtility must be rebuilt after any edit to an override file**, including deleting a bad entry; otherwise the old copy in `bin/Debug/net10.0/Overrides` is what loads.
- **Record:** `{"Card":…,"Text":…,"Abilities":[…],"Note":…}`. `LineOverrides` keys on the lowercased card name plus the whitespace-normalized line text, and `CorpusLowering` swaps the override's abilities in for that line's, so the closure check and the engine see the same records a rule would have produced. `Abilities` is the polymorphic IR JSON (`$kind` discriminators) exactly as `rules.jsonl` writes it. `Note` records which literal glyph(s) the override retires and any approximation made.
- **Batches 1–10** were written by the previous session (with rounds twelve to fifteen); **batches 11–40** walked the alphabet (Hopeful Initiate through Zurgo and Ojutai) taking the lines that fit the existing vocabulary; **batches 41–51** made a second, harder pass over the whole alphabet (Aang through Ziatora's Envoy), taking everything the existing vocabulary could express, with approximations flagged in `Note`. 1,078 literal-blocked lines on 1,040 cards remain: A 41, B 43, C 96, D 67, E 47, F 48, G 63, H 33, I 38, J 10, K 37, L 24, M 69, N 28, O 27, P 43, Q 9, R 37, S 129, T 107, U 16, V 20, W 31, X 1, Y 5, Z 9. A third pass with the same vocabulary would yield little (under 10 writable lines per 100); the leftovers need model and engine additions first (see "What it takes", item 3). The batch generators for 43–51 are `.agents/gen43.py` … `gen51.py` on top of the shared helper module `.agents/irh.py` (small Python builders for selectors, filters, triggers, costs); writing a batch as a generator script is faster and less error-prone than hand-typing JSON, and `irh.write()` emits the `.jsonl`.
- **Commands:**
  - `dotnet ConsoleUtility.dll overrides-todo <rules.jsonl> <out.jsonl> [count] [skip]` loads every override (JSON or discriminator errors surface here as an exception), reports how many lines are still blocked only by literals, and writes skeleton records.
  - `python -I -X utf8 .agents/todo_list.py <rules.jsonl> MTGRulesIR/Overrides "<after card name>" <count>` lists the still-unoverridden literal-blocked lines alphabetically after a card, with their glyph names, without needing the DLL.
  - `python -I -X utf8 .agents/corpus_examples.py <rules.jsonl> <maxchars> "<phrase>"…` prints, for each phrase, the first fully lowered corpus line containing it with its IR JSON. This is how to discover the IR shape for a clause before writing it.
  - `python -I -X utf8 .agents/check_overrides.py <rules.jsonl> MTGRulesIR/Overrides/line-overrides-N.jsonl | grep -v '^OK'` after a `lower` run: prints `BAD <card> | … | None` when the override's `Text` doesn't match any corpus line (fix the text), or `BAD … (True, False, [], ['X.Y'])` when the line lowered but the engine left slot `X.Y` open (drop the entry or change the shape).
  - (`.agents/` is untracked; copy the three scripts if a fresh clone needs them. `opaque_tally.py` and the `edit_round*.py` files are one-offs from earlier rounds.)
- **Per-batch loop** (one background shell chain, about 5 minutes): write `line-overrides-N.jsonl` → JSON-parse it with a two-line Python check → `dotnet build ConsoleUtility/ConsoleUtility.csproj` → from `ConsoleUtility/bin/Debug/net10.0`: `overrides-todo … 1`, `lower C:/Users/James/source/forge-study/lower-r15 every`, `interpret …/lower-r15/rules.jsonl …/lower-r15` → `check_overrides.py` → fix any BAD, rebuild, rerun → commit and push that one file ("N more hand-written line overrides (First card through Last card); all lower closed"). Never rebuild while a background `lower` is running. Draft batch N+1 while batch N validates. The `lower` step rewrites `lower-r15/rules.jsonl` in place, so `corpus_examples.py` and `check_overrides.py` must not read it while a chain is running (they crash mid-write); `todo_list.py` reads the frozen `lower-r14` and is always safe.
- **Shapes that bit, so the next author doesn't repeat them:** `$kind` discriminators and enum fields are validated by the JSON loader (there is no `Unattach` effect, no `ThoseTokens` selector, `Related.Relation` is an enum: Among, On, Is, OtherThan, AbilityOf, DealtDamageBy, AttachedTo, ExiledWith, …; `AddMana.Options` are `ManaAmount`/`ManaChoice`); string fields such as event names, `Special.Kind`, `AmountThisWay.Kind`, `PlayerEvent.Event` and qualifier vocabularies are free text and lower fine even when invented, so keep them to names the corpus already uses where one exists. `CopySpell.Except` is left open by the engine (use a following `BecomesAspect` step instead). `CreateTokens` has no creating-player field, `DoesntUntap` has no count, `MustBeBlocked` has no "by", `PlayerSelector` has no filter or "up to", `CastFrom` has no negation ("from anywhere other than your hand" is only expressible as a `WasCast … Negated` condition, not as a trigger filter), `Comparison` has no NotEqual, `Scaled.Factor` stops at a fifth, `DistinctCount.Aspect` has no "Powers" or counter kinds, `ObjectEvent` knows `EntersOrTransforms` but not `Transforms`/`PhasesOut`/`BecomesCrewed`, and the engine has no `Loyalty` characteristic or `Commander` qualifier; lines that need these were skipped or approximated with a `Note`. Useful shapes confirmed in batches 43–51: `TotalOf(Of, Characteristic)` for "total power of", `UnlessPaid{Effect, Payment{Cost}}` (wrap in `LetVariable` to define `{x}`), `CompoundCost{Parts}` for two-part alternative costs, `MayPlayFrom.Among` for "from among those cards", `PlayCard.ByPaying` for "by discarding a card rather than paying", `Lasting{Delayed{…}}` for "whenever … this turn", `Satisfying{Happened{ObjectEvent[Attacked]}}` for "that attacked this turn", `Duration While(Controls You Self)` for "for as long as you control this", `KeywordAction{Manifest, Objects}`, `TokenSpec.Abilities`/`Legendary`/`Name` for named legendary tokens with abilities, `ManaSpent{Color}` for "if {r} was spent", `AmountThisWay.CountersRemoved`. "Instead" clauses conditioned on something checkable were written as `Conditional` if/else; "when you do" reflexive triggers as `WhenDone`; "for as long as this creature remains on the battlefield" as `Duration: While(Did(Remains …))`.

## What it takes to reach 100% executable

In order of value per unit of effort now:

1. **Double-faced cards (about 240 cards, nothing started).** Ingest the back-face text, give `CardDefinition` and `GameObject` a face model, then implement Transform, Daybound/Nightbound with day and night, Disturb and Flip. `SetDayNight`/`DayNight` nodes already exist in the IR.
2. **The engine tail (about 1,700 cards).** Work down the "only blocker" list in `interpret-report.txt`; the largest items are listed above and each unblocks 15–30 cards.
3. **More line overrides (about 1,078 lines on 1,040 cards).** The two alphabetical passes are complete; what is left is listed by `todo_list.py … "" 5000`. Nearly all of it needs vocabulary the IR lacks: parity ("odd/even mana value"), "the legend rule", piles, chosen-type negation, per-player creators, "can't be sacrificed", damage-dealt-this-turn quantities, players filtered by a condition ("each opponent who …"), randomly chosen objects as cost targets. When a shape repeats, add it to the model and the engine rather than approximating.
4. **Lowering rules for general glyphs.** Rounds two to fifteen exist in `MTGRulesIR/Lowering/Rules.Round*.cs`; what is left is a flat tail (12 lines or fewer per glyph), so only worth it where a glyph is the sole blocker of a card that an override can't reach.
5. **Semantic correctness, alongside all of the above.** "Executable" only means every node has a handler. The castcheck of 2026-11-17 (after all 51 override batches) had 20,709 executable nonland cards: 20,212 cast (97.6%), 2 skipped something as they resolved, 214 resolved with no effect, 497 never offered, 0 threw; the 2026-11-07 run had 18,889 / 18,452 / 175 / 437, so the override cards cast at the same rate as the rest. Of the 41 cards newly "resolved with no effect", the ones checked were conditions the castcheck board doesn't meet (no auras in the graveyard for Retether, no vehicles for Reckless Crew, no spells on the stack for Glen Elendra's Answer, two creatures of one controller chosen for Switcheroo, and extra turns, which the checker never counted as an effect: Time Warp is in both lists). Rite of Replication (general lowering, a `TokenCopy` of a target creature) is the one worth a look. Approximations flagged in override `Note` fields are the first place to look when an override card resolves with no effect. Known lowering bugs: *Wormfang Crab* exiles itself (the grammar binds "it" to the card); "target creature's owner puts it on their choice of the top or bottom of their library" loses its target. Grammar binding errors are repaired after lowering in `MTGRulesIR/Lowering/SelfReferenceRepair.cs`, so extend that file.

A more honest headline metric would be worth adding next to "executable": "casts and resolves with an effect" from castcheck.

## Tooling

Build `ConsoleUtility` first. The binaries are in `ConsoleUtility/bin/Debug/net10.0`. Full-corpus outputs: `lower-r14` is the frozen reference the helper scripts read (its `rules.jsonl` still carries the opaque nodes, which is what `todo_list.py` and `corpus_examples.py` need); `lower-r15` is the working output that each batch overwrites.

| Command | What it does | Time |
|---|---|---|
| `dotnet ConsoleUtility.dll lower <dir> every` | Re-lowers the corpus and writes `rules.jsonl`, `report.txt` (opaque glyphs ranked) and `census.txt`; prints the `Cards:`/`Lines:` summary | about 3 min |
| `dotnet ConsoleUtility.dll interpret <rules.jsonl> <dir>` | Writes `interpret-report.txt`: executable %, unhandled node kinds, and sole blockers by card | about 1 min |
| `dotnet ConsoleUtility.dll overrides-todo <rules.jsonl> <out.jsonl> [count] [skip]` | Loads all overrides, counts literal-blocked lines, writes skeletons | seconds |
| `dotnet ConsoleUtility.dll simulate <rules.jsonl> N seed` | Random two-color games of every executable card; reports exceptions and run-time skips. `SIMULATE_DUMP=<dir>` writes a failing game's log | about 0.6 s/game; keep N ≤ 300 under the 10-minute command limit |
| `dotnet ConsoleUtility.dll castcheck <rules.jsonl> <out.txt>` | Casts every executable card on a rich board (`ConsoleUtility/CastCheck.cs`) | about 25 min; **locks the build output** |
| `dotnet ConsoleUtility.dll lowertext "text"` | One line's IR (slow: builds the grammar each time) | |
| `tools/ir_samples.py <rules.jsonl> <Kind> [n] [field=value]`, `tools/ir_tally.py` | Sample or tally IR node shapes (run with Python312, `-I -X utf8`) | |
| `tools/add_test_cards.py <rules.jsonl> <names...>` | Adds real lowered cards to the engine test fixture `MTGEngine.Tests/Cards/cards.jsonl` | |

- **Tests:** lowering tests take about 2 min, and engine tests are under `MTGEngine.Tests`. Together there are about 370 tests.
- **Engine tests:** write them against real fixture cards, not just hand-built IR. Real cards keep exposing lowering-level semantic gaps.
- **Grammar server:** the grammar MCP server is the DocumentAnalysisInterface app. When the MCP connection is down but the app is running, POST JSON-RPC to `localhost:5253/mcp`. It was not running during the override sessions and was not needed: override work touches no grammar.
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
  - `\b` and `\n` inside bash or python heredocs get mangled. Write edit scripts to a file, or use a real editor tool. New override files are easiest written whole with an editor tool and fixed with `sed -i '/^{"Card":"Name"/d'` or a small Python script.
  - PowerShell `Get-Content`/`Set-Content` corrupts non-ASCII.
- The Forge clone at `C:/Users/James/source/forge-study` is a design reference only (GPL); don't copy code from it.
