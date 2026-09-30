---
name: grammar
description: Work on the Glyphotype grammar through the glyphotype MCP server (the running DocumentAnalysisInterface app) - drafting, evaluating and applying glyphs step by step, checking in as the app's settings say. Use when the user types /grammar or asks to work on or improve the grammar.
argument-hint: "[instructions, e.g. from scratch | focus on triggers]"
---

Call the glyphotype `start_session` tool with `instructions` set to: $ARGUMENTS

Then follow the brief it returns. If the glyphotype tools aren't available, the app isn't running or connected: tell the user to start DocumentAnalysisInterface (the DocumentAnalysisInterface launch profile) and reconnect it from `/mcp`.
