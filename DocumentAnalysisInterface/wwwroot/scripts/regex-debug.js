// regex-debug.js
//
// Regex-match troubleshooting entry point on the Corpus Captures page: watches for a left-click-drag
// text selection inside a document line's <pre class="nested-underline-container">, snaps the selection
// to word boundaries on mouseup, and floats a "Debug regex" button diagonally above the selection's end.
// Clicking the button hands the corrected text to Blazor (CorpusCapturesPage.OpenDebugRegexDialog).
//
// The highlight itself is the *native* browser selection, restyled via ::selection CSS (color/opacity
// come from RegexDebugPresentation in C#, surfaced here as the --regex-debug-selection-bg custom
// property) plus a text-shadow faux-bold once armed. Deliberately so: keeping the native selection means
// Ctrl+C copies the corrected text, clicking anywhere else dismisses the highlight, and nothing here ever
// mutates Blazor-rendered DOM (a real font-weight bold would require wrapping the selected text in new
// elements, which both fights Blazor's diffing and reflows the underline layout).

let regexDebug = {
    dotNetRef: null,
    button: null,
    range: null,     // the word-boundary-corrected selection range, kept for repositioning on scroll
    text: null,
    armedPre: null,
    mouseUpHandler: null,
    selectionChangeHandler: null,
    scrollHandler: null,
};

const regexDebugPreSelector = 'pre.nested-underline-container';
const regexDebugArmedClass = 'regex-debug-armed';
const regexDebugButtonClass = 'regex-debug-button';

// Decoration-only elements inside a line's <pre> whose text is NOT part of the line's semantic text —
// currently the echo count badges. Their text nodes sit between the words in DOM order, so any
// textContent-offset math that didn't skip them would splice stray digits into the extracted segment
// (e.g. "and6 toughness").
const regexDebugExcludedTextSelector = '.echo-badge';

function initRegexDebugSelection(dotNetRef, options) {
    disposeRegexDebugSelection();
    regexDebug.dotNetRef = dotNetRef;

    // The highlight color/transparency are configured in C# (RegexDebugPresentation); combine them into
    // the one rgba() custom property the ::selection rule in site.css reads.
    document.documentElement.style.setProperty(
        '--regex-debug-selection-bg',
        regexDebugHexToRgba(options.selectionHighlightColorHex, options.selectionHighlightOpacity));

    regexDebug.mouseUpHandler = onRegexDebugMouseUp;
    regexDebug.selectionChangeHandler = onRegexDebugSelectionChange;
    regexDebug.scrollHandler = onRegexDebugScroll;

    document.addEventListener('mouseup', regexDebug.mouseUpHandler);
    document.addEventListener('selectionchange', regexDebug.selectionChangeHandler);
    window.addEventListener('scroll', regexDebug.scrollHandler, true);
}

function disposeRegexDebugSelection() {
    if (regexDebug.mouseUpHandler) document.removeEventListener('mouseup', regexDebug.mouseUpHandler);
    if (regexDebug.selectionChangeHandler) document.removeEventListener('selectionchange', regexDebug.selectionChangeHandler);
    if (regexDebug.scrollHandler) window.removeEventListener('scroll', regexDebug.scrollHandler, true);

    clearRegexDebugSelection();

    if (regexDebug.button) {
        regexDebug.button.remove();
        regexDebug.button = null;
    }

    regexDebug.dotNetRef = null;
    regexDebug.mouseUpHandler = null;
    regexDebug.selectionChangeHandler = null;
    regexDebug.scrollHandler = null;
}

function clearRegexDebugSelection() {
    if (regexDebug.armedPre) regexDebug.armedPre.classList.remove(regexDebugArmedClass);
    if (regexDebug.button) regexDebug.button.style.display = 'none';
    regexDebug.armedPre = null;
    regexDebug.range = null;
    regexDebug.text = null;
}

function onRegexDebugMouseUp(event) {
    if (event.button !== 0) return;
    if (event.target.closest && event.target.closest('.' + regexDebugButtonClass)) return;

    // Defer one tick: the selection object isn't final until after the mouseup dispatch completes.
    setTimeout(() => processRegexDebugSelection(), 0);
}

function processRegexDebugSelection() {
    const selection = window.getSelection();

    if (!selection || selection.rangeCount === 0 || selection.isCollapsed) {
        clearRegexDebugSelection();
        return;
    }

    const range = selection.getRangeAt(0);
    const startPre = regexDebugClosestPre(range.startContainer);
    const endPre = regexDebugClosestPre(range.endContainer);

    // Only a selection contained within a single document line's <pre> qualifies.
    if (!startPre || startPre !== endPre) {
        clearRegexDebugSelection();
        return;
    }

    const pre = startPre;
    const model = regexDebugTextModel(pre);
    const fullText = model.text;
    let start = regexDebugModelOffset(model, range.startContainer, range.startOffset);
    let end = regexDebugModelOffset(model, range.endContainer, range.endOffset);

    // Correct to word boundaries: trim any selected whitespace inward, then grow both ends outward to
    // cover whole words (a selection starting/ending mid-word means the whole word).
    while (start < end && regexDebugIsWs(fullText[start])) start++;
    while (end > start && regexDebugIsWs(fullText[end - 1])) end--;

    if (start >= end) {
        clearRegexDebugSelection();
        return;
    }

    while (start > 0 && !regexDebugIsWs(fullText[start - 1])) start--;
    while (end < fullText.length && !regexDebugIsWs(fullText[end])) end++;

    const corrected = regexDebugRangeFromOffsets(model, start, end);
    if (!corrected) {
        clearRegexDebugSelection();
        return;
    }

    // Re-apply the corrected bounds as the live selection (this is what the user copies with Ctrl+C).
    selection.removeAllRanges();
    selection.addRange(corrected);

    clearRegexDebugSelection();
    regexDebug.range = corrected;
    regexDebug.text = fullText.slice(start, end).replace(/\s+/g, ' ').trim();
    regexDebug.armedPre = pre;
    pre.classList.add(regexDebugArmedClass);
    positionRegexDebugButton();
}

function onRegexDebugSelectionChange() {
    if (!regexDebug.range) return;

    // Our own programmatic re-selection above also queues selectionchange events, so distinguish by
    // identity: as long as the live selection's boundaries are exactly the corrected range we armed
    // with, stay armed. Anything else (click elsewhere collapses it, a new drag replaces it) disarms;
    // a fresh mouseup re-arms if the new selection qualifies.
    const selection = window.getSelection();
    const live = selection && selection.rangeCount > 0 && !selection.isCollapsed ? selection.getRangeAt(0) : null;

    const isStillArmedRange = live
        && live.startContainer === regexDebug.range.startContainer
        && live.startOffset === regexDebug.range.startOffset
        && live.endContainer === regexDebug.range.endContainer
        && live.endOffset === regexDebug.range.endOffset;

    if (isStillArmedRange) return;

    clearRegexDebugSelection();
}

function onRegexDebugScroll() {
    if (regexDebug.range) positionRegexDebugButton();
}

function positionRegexDebugButton() {
    const button = ensureRegexDebugButton();
    const rects = regexDebug.range.getClientRects();
    const anchor = rects.length > 0 ? rects[rects.length - 1] : regexDebug.range.getBoundingClientRect();

    button.style.display = 'block';

    // Diagonally up-and-right of the selection's end — roughly where the cursor sat on mouseup —
    // clamped to the viewport.
    const left = Math.min(anchor.right + 8, window.innerWidth - button.offsetWidth - 8);
    const top = Math.max(4, anchor.top - button.offsetHeight - 6);
    button.style.left = `${Math.max(4, left)}px`;
    button.style.top = `${top}px`;
}

function ensureRegexDebugButton() {
    if (regexDebug.button) return regexDebug.button;

    const button = document.createElement('button');
    button.type = 'button';
    button.className = regexDebugButtonClass;
    button.textContent = 'Debug regex';

    // preventDefault on mousedown so pressing the button doesn't collapse the selection before the
    // click lands (a collapse would disarm via selectionchange and the click would hit nothing).
    button.addEventListener('mousedown', e => { e.preventDefault(); e.stopPropagation(); });
    button.addEventListener('click', e => {
        e.preventDefault();
        e.stopPropagation();
        if (regexDebug.dotNetRef && regexDebug.text) {
            regexDebug.dotNetRef.invokeMethodAsync('OpenDebugRegexDialog', regexDebug.text);
        }
    });

    document.body.appendChild(button);
    regexDebug.button = button;
    return button;
}

// --- helpers ---

function regexDebugClosestPre(node) {
    const el = node && node.nodeType === Node.TEXT_NODE ? node.parentElement : node;
    return el && el.closest ? el.closest(regexDebugPreSelector) : null;
}

function regexDebugIsWs(ch) {
    return /\s/.test(ch);
}

/// Builds the pre's semantic-text model: the concatenated text of every text node except those inside
/// decoration elements (see regexDebugExcludedTextSelector), plus the node list mapping model offsets
/// back to DOM positions.
function regexDebugTextModel(root) {
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT, {
        acceptNode: node => node.parentElement && node.parentElement.closest(regexDebugExcludedTextSelector)
            ? NodeFilter.FILTER_REJECT
            : NodeFilter.FILTER_ACCEPT,
    });

    let text = '';
    const nodes = [];

    for (let node = walker.nextNode(); node; node = walker.nextNode()) {
        nodes.push({ node, start: text.length, length: node.textContent.length });
        text += node.textContent;
    }

    return { text, nodes };
}

/// Model-space character offset of the DOM position (container, offset).
function regexDebugModelOffset(model, container, offset) {
    if (container.nodeType === Node.TEXT_NODE) {
        const entry = model.nodes.find(e => e.node === container);
        if (entry) return entry.start + offset;
    }

    // Element container, or a text node the model excluded: count every model node that ends at or
    // before this DOM position.
    const probe = document.createRange();
    probe.setStart(container, offset);
    probe.collapse(true);

    let total = 0;

    for (const entry of model.nodes) {
        // comparePoint: -1 = the node's end sits before the probe position.
        if (probe.comparePoint(entry.node, entry.length) <= 0) {
            total = entry.start + entry.length;
            continue;
        }
        break;
    }

    return total;
}

/// Builds a Range spanning [start, end) model-space offsets.
function regexDebugRangeFromOffsets(model, start, end) {
    const range = document.createRange();
    let startSet = false;

    for (const entry of model.nodes) {
        const next = entry.start + entry.length;

        if (!startSet && start >= entry.start && start <= next) {
            range.setStart(entry.node, start - entry.start);
            startSet = true;
        }

        if (startSet && end >= entry.start && end <= next) {
            range.setEnd(entry.node, end - entry.start);
            return range;
        }
    }

    return null;
}

function regexDebugHexToRgba(hex, opacity) {
    const clean = hex.replace('#', '');
    const r = parseInt(clean.substring(0, 2), 16);
    const g = parseInt(clean.substring(2, 4), 16);
    const b = parseInt(clean.substring(4, 6), 16);
    return `rgba(${r}, ${g}, ${b}, ${opacity})`;
}
