// lowered-rules.js
// Hover highlighting on the Lowered page. Three things link up within one line (a .lowered-line boundary):
// - a node's place in its tree (data-ir-path, an underscore-joined chain from the root), so hovering a node lights
//   it and its ancestors and mutes the rest of the line's nodes;
// - the text a node came from (data-ir-span, shared by the node and the span of the line's text), so hovering a node
//   lights its text, and hovering text lights the nodes that came from it;
// - a binding (data-ir-ref, shared by the node that introduces "#1" and every reference to it), so hovering either
//   lights both.
let loweredMouseoverHandler;
let loweredMouseleaveHandler;
let loweredLastHovered = null;
let loweredMarked = [];

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

function initLoweredHover() {
    const root = document.getElementById('lowered-rules');
    if (!root) return;

    const clear = () => {
        loweredMarked.forEach(el => el.classList.remove(loweredHighlightClass, loweredMuteClass));
        loweredMarked = [];
    };

    loweredMouseoverHandler = (event) => {
        const hovered = event.target.closest(loweredHoverSelector);
        if (hovered === loweredLastHovered) return;
        loweredLastHovered = hovered;
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
        loweredMarked = nodes;

        boundary.querySelectorAll('[data-ir-span]:not([data-ir-path])').forEach(el => {
            if (spans.has(el.dataset.irSpan)) {
                el.classList.add(loweredHighlightClass);
                loweredMarked.push(el);
            }
        });
    };

    loweredMouseleaveHandler = () => {
        loweredLastHovered = null;
        clear();
    };

    root.addEventListener('mouseover', loweredMouseoverHandler);
    root.addEventListener('mouseleave', loweredMouseleaveHandler);
}

function disposeLoweredHover() {
    const root = document.getElementById('lowered-rules');
    if (root && loweredMouseoverHandler && loweredMouseleaveHandler) {
        root.removeEventListener('mouseover', loweredMouseoverHandler);
        root.removeEventListener('mouseleave', loweredMouseleaveHandler);
    }
    loweredMouseoverHandler = null;
    loweredMouseleaveHandler = null;
}
