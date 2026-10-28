// lowered-rules.js
// Hover highlighting on the Lowered page, and wherever else lowered trees are shown (the page's guide). Three things
// link up within one line (a .lowered-line boundary):
// - a node's place in its tree (data-ir-path, an underscore-joined chain from the root), so hovering a node lights
//   it and its ancestors and mutes the rest of the line's nodes;
// - the text a node came from (data-ir-span, shared by the node and the span of the line's text), so hovering a node
//   lights its text, and hovering text lights the nodes that came from it;
// - a binding (data-ir-ref, shared by the node that introduces "#1" and every reference to it), so hovering either
//   lights both.
// Each root element (by id) keeps its own handlers, so the page and a dialog over it can both be live.
const loweredRoots = new Map();

const loweredHighlightClass = 'highlight-active';
const loweredMuteClass = 'mute-active';
const loweredBoundaryClass = 'lowered-line';
const loweredHoverSelector = '[data-ir-path], [data-ir-span]';

function loweredIsSelfOrAncestorPath(path, paths) {
    if (paths.has(path)) return true;
    for (const collected of paths) {
        if (collected.startsWith(path + '_')) return true;
    }
    return false;
}

function initLoweredHover(rootId) {
    rootId = rootId || 'lowered-rules';
    const root = document.getElementById(rootId);
    if (!root || loweredRoots.has(rootId)) return;

    const state = { lastHovered: null, marked: [] };

    const clear = () => {
        state.marked.forEach(el => el.classList.remove(loweredHighlightClass, loweredMuteClass));
        state.marked = [];
    };

    const mouseover = (event) => {
        const hovered = event.target.closest(loweredHoverSelector);
        if (hovered === state.lastHovered) return;
        state.lastHovered = hovered;
        clear();
        if (!hovered) return;

        const boundary = hovered.closest('.' + loweredBoundaryClass);
        if (!boundary) return;

        // What's hovered: a node (with a path), or a span of the text (with a span key only).
        const paths = new Set();
        const spans = new Set();
        const refs = new Set();

        if (hovered.dataset.irPath) {
            let current = hovered;
            while (current && current !== boundary) {
                if (current.dataset.irPath) paths.add(current.dataset.irPath);
                current = current.parentElement;
            }
            if (hovered.dataset.irSpan) spans.add(hovered.dataset.irSpan);
            if (hovered.dataset.irRef) refs.add(hovered.dataset.irRef);
        } else if (hovered.dataset.irSpan) {
            spans.add(hovered.dataset.irSpan);
            // The nodes that came from this text, and their ancestors.
            boundary.querySelectorAll('[data-ir-path][data-ir-span="' + CSS.escape(hovered.dataset.irSpan) + '"]').forEach(node => {
                let current = node;
                while (current && current !== boundary) {
                    if (current.dataset.irPath) paths.add(current.dataset.irPath);
                    current = current.parentElement;
                }
                if (node.dataset.irRef) refs.add(node.dataset.irRef);
            });
        }

        const nodes = Array.from(boundary.querySelectorAll('[data-ir-path]'));
        nodes.forEach(el => {
            const lit = loweredIsSelfOrAncestorPath(el.dataset.irPath, paths)
                || (el.dataset.irSpan && spans.has(el.dataset.irSpan))
                || (el.dataset.irRef && refs.has(el.dataset.irRef));
            el.classList.add(lit ? loweredHighlightClass : loweredMuteClass);
        });
        state.marked = nodes;

        boundary.querySelectorAll('[data-ir-span]:not([data-ir-path])').forEach(el => {
            if (spans.has(el.dataset.irSpan)) {
                el.classList.add(loweredHighlightClass);
                state.marked.push(el);
            }
        });
    };

    const mouseleave = () => {
        state.lastHovered = null;
        clear();
    };

    root.addEventListener('mouseover', mouseover);
    root.addEventListener('mouseleave', mouseleave);
    loweredRoots.set(rootId, { root, mouseover, mouseleave });
}

function disposeLoweredHover(rootId) {
    rootId = rootId || 'lowered-rules';
    const entry = loweredRoots.get(rootId);
    if (!entry) return;
    entry.root.removeEventListener('mouseover', entry.mouseover);
    entry.root.removeEventListener('mouseleave', entry.mouseleave);
    loweredRoots.delete(rootId);
}
