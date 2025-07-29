# MTGCardParser

A modern, extensible parser and analyzer for Magic: The Gathering (MTG) card text, built with C# and Blazor.  
This project provides a robust backend for tokenizing and analyzing card rules text, and a rich interactive web interface for visualizing the parsing process, variable extraction, and card logic decomposition.

---
![Preview](CardAnalysisInterface/SampleImages/sample.gif)
---

## Features

- **Tokenization & Parsing:**  
  Breaks down MTG card text into a hierarchical structure of tokens, supporting complex nested rules and abilities.

- **Attribute-Driven Analysis:**  
  Uses C# attributes to control parsing, collapsing, and display logic (e.g., punctuation handling, ignored tokens, etc.).

- **Interactive Web UI:**  
  - Visualizes the parse tree and variable extraction.
  - Highlights and color-codes different token types.
  - Allows inspection of how each part of the card text is interpreted.

- **Extensible Token System:**  
  Easily add new token types or parsing rules by extending the backend.

- **Sentence-Aware Rendering:**  
  Automatically capitalizes the first word of each sentence in the UI, and handles punctuation and spacing according to MTG conventions.
---

## Usage

- **Download the SQL data here: https://github.com/automatorJames/MTGCardParser/releases/download/SqlData/CardSql.zip, or find an updated source (widely available online)
- **Explore the parse tree:**
  - Hover or click on spans to see token details.
  - Collapsed tokens (like punctuation) are rendered inline according to parsing rules.
  - Sentences are automatically capitalized for readability.
- **Inspect variables and extracted properties** for each card.

---

## Customization & Extensibility

- **Add new token types:**  
  Implement new classes in `MTGPlexer/TokenUnits/` and register them in `TokenClassRegistry`.
- **Control analysis behavior:**  
  Use attributes like `[CollapseInAnalysis]`, `[IgnoreInAnalysis]`, etc., to fine-tune parsing and rendering.
- **UI customization:**  
  Modify or extend Razor components in `CardAnalysisInterface/Components/`.

## Acknowledgments

- Inspired by the complexity and beauty of Magic: The Gathering rules text.
- Built with [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) and .NET.
