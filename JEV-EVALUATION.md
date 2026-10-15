# Jev: could a "System One" model make the grammar agent cheaper or faster?

Research notes from 2026-10-15. No action taken: this records what Jev is, where it could fit in the grammar agent's
loop and harness, and what it might be worth, for a later decision.

## What Jev is

Jev is TypeSafe AI's "System One" model (after Kahneman's fast, intuitive System 1, against the slow, deliberate
System 2 of a reasoning LLM). It launched on 2026-09-15, when TypeSafe came out of stealth with a $40M seed round led
by DCVC. Its founder, Diogo Almeida, previously worked at OpenAI.

It doesn't generate text. A call gives it context plus a fixed question, and it returns a typed answer with
probabilities. There are three question types, and every call is built from them:

- **Choice:** pick from options the caller supplies.
- **Score:** place something on an ordered scale.
- **Yes/no probability** (called "Noul" in some coverage).

It's pitched as sitting *in front of* an LLM, not replacing it: routine routing, classification and scoring go to
Jev, and only what needs open-ended reasoning or generation goes on to the LLM.

### Claims, and how far they're verified

| Claim | Source |
|---|---|
| 70-500 ms per response | Vendor |
| $0.042 per million input tokens, output free | Vendor |
| 20-200x faster, 40-400x cheaper than LLMs | Vendor benchmarks |
| Faster and cheaper than the LLMs tested, but not consistently more accurate than GPT-5.6 Terra (791 decisions) | Third-party benchmark |
| 5-18x faster with better accuracy, swapped in behind a command-safety classifier at Vercel | Third-party report |

Coverage at the time was mostly secondary blogs and Medium posts, so the figures are best checked against TypeSafe's
own launch post and the benchmark itself before relying on them.

## Where the agent's cost goes

Per-step figures from the agent pane, in early rounds: 227k-511k tokens and 1.5-3 minutes a step. Nearly all of it is
the agent rereading its growing context while it explores - `search_lines`, `show`, `tokenize`, `explain_mismatch` -
before it writes a glyph in C#. Two things bound what Jev could take over:

- **Writing glyphs is generative.** Drafting C#, and reasoning about a grammar's structure, stays with the LLM.
- **Scoring and tokenizing are already code.** `evaluate`, coverage and bits are deterministic, and need no model.

What's left for Jev are the bounded judgments the agent makes along the way, which it currently pays for in context.

## Candidate uses, best first

1. **Vocabulary membership sweeps.** For frequent unmatched words, ask of each vocabulary: "is *servo* a
   `CreatureType`?" (yes/no probability). The app's own scoring then confirms the proposed additions. One early step
   spent about 2 minutes and 451k tokens discovering that servo, scion, treasure and food were missing from
   `CreatureType` - a sweep would surface additions like those in bulk for well under a cent. The agent, or the app
   without any agent, evaluates and applies them. This is the most self-contained place to start: the evaluation
   score says whether it helped.
2. **Grouping residual text into families.** Label each unmatched span with the existing glyph it most resembles, or
   "new construction" (Choice), and attach the labels to what `unmatched_openings` and `residuals` report. The agent
   would start each step with targets already grouped, instead of discovering them through repeated `search_lines`
   and `show` calls - the part of a step that eats context. About 12.5k distinct spans at ~100 tokens each is ~1.2M
   tokens: about $0.05 with Jev, against dollars through an LLM, which would also add to the context it rereads on
   every call.
3. **Judging whether a step's new parse is better.** When a step takes words from another glyph, ask for each
   affected line which parse is more correct (Choice between the two). Today every such step stops for the person's
   approval. With Jev's probabilities, a policy like "auto-approve when at least 95% of affected lines prefer the new
   parse" would leave check-ins for the genuinely ambiguous cases. This saves the person's waiting time more than
   tokens.
4. **Choosing a model per step.** Easy target (a vocabulary member, a one-literal glyph) or hard (a new nested
   construction)? Run easy ones on Haiku, hard ones on Sonnet or Opus.
5. **The harness's own small decisions.** Did the agent stop to ask approval, check in, or get stuck? Does a journal
   entry still hold? Few calls, so little saved - the gain would be correctness over heuristics like the
   "awaiting approval" check the chat uses to decide whether to nudge the agent.

## How much it might save

Per decision, Jev's advantage is large (the vendor's 40-400x cheaper, and much faster). Per round, it's much smaller,
because the expensive and irreducible part is drafting glyphs from long context. Estimate: a **20-40% cut in tokens and
time per round**:

- 1 and 2 take most of the exploration out of easy steps;
- 3 cuts check-in stalls (time, more than tokens);
- 4 trims the cost of the cheap steps.

Working changes' per-step Time, Tokens and Cost columns, and the round totals, are a baseline to measure a trial
against.

## Things to weigh

- **A separate paid API.** The agent runs on a Claude Code subscription; Jev would be billed apart, at tiny amounts, but
  it means another account and key, and corpus text leaving the machine.
- **Accuracy on this corpus is unknown.** Treat every Jev answer as a proposal that the app's scoring confirms: the
  score still decides whether a change is made, and Jev only suggests what to try.
- **Where it plugs in.** It fits best inside the app - enriching tool results, or running sweeps between rounds - not
  as another tool the agent calls, which would put its results back into the context the agent rereads.

## Sources

- [TypeSafe Jev Explained: What This "System One" Model Changes for AI Agents](https://dymesty.com/blogs/articles/typesafe-jev-system-one-ai-model)
- [Jev (TypeSafe AI model) definition](https://www.sanity.io/glossary/jev-typesafe-ai-model)
- [What is Jev and can it really replace LLMs in your AI stack](https://levelup.gitconnected.com/what-is-jev-and-can-it-really-replace-llms-in-your-ai-stack-4d0b4e284ddd)
- [Jev AI model](https://kanerika.com/blogs/jev-ai-model/)
- [You're paying Claude Opus to answer yes or no. There's a model for that](https://pub.towardsai.net/youre-paying-claude-opus-to-answer-yes-or-no-there-s-a-model-for-that-cfa401d3ea71)
- [Jev and the return of the zero-shot classifier](https://www.glean.com/blog/jev-zero-shot-classifier)
- [Jev system one model: classifier, not LLM](https://innfactory.ai/en/blog/jev-system-one-model-classifier-not-llm/)
- [How to build a model router with Jev](https://www.mindstudio.ai/blog/how-to-build-model-router-with-jev)
- [What is Jev?](https://browserbase.com/blog/what-is-jev)
- [Jev, the anti-LLM, has arrived](https://www.stork.ai/blog/jev-the-anti-llm-has-arrived)
