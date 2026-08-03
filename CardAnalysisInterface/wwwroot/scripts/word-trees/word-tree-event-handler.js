import { WordTree } from "./word-tree-animator.js";
import { createGradientStops } from "./word-tree-svg-drawer.js";
const globalEventState = {
    initialized: false,
    lastHovered: {
        card: null,
        cardKeys: new Set(),
        mainAnchorHover: false
    }
};
function areSetsEqual(setA, setB) {
    if (setA.size !== setB.size)
        return false;
    for (const item of setA) {
        if (!setB.has(item))
            return false;
    }
    return true;
}
/**
 * Smoothly animates the white overlay for node borders and connectors on anchor hover.
 */
function setAnchorHoverEffect(card, isHovering) {
    const svg = card.querySelector('svg');
    if (!svg)
        return;
    const elementsToAnimate = new Map();
    const overlays = svg.querySelectorAll('.anchor-hover-overlay');
    overlays.forEach(overlay => {
        const current = parseFloat(getComputedStyle(overlay).opacity) || 0;
        const end = isHovering ? 1 : 0;
        if (Math.abs(current - end) > 0.001) {
            elementsToAnimate.set(overlay, { start: current, end });
        }
    });
    const controller = card.__anchorHoverController ??
        (card.__anchorHoverController = { animationFrameId: null });
    if (elementsToAnimate.size > 0) {
        WordTree.Animator.animateOpacity(elementsToAnimate, controller);
    }
}
/**
 * Applies card-based highlighting. Affects node/connector structures and card header items.
 */
function setCardHighlight(card, activeKeys) {
    const hasActiveKeys = activeKeys.size > 0;
    card.classList.toggle('highlight-active', hasActiveKeys);
    card.querySelectorAll('[data-card-name]').forEach(item => {
        const cardName = item.dataset.cardName || '';
        const isHighlighted = hasActiveKeys && activeKeys.has(cardName);
        item.classList.toggle('highlight', isHighlighted);
        item.classList.toggle('lowlight', hasActiveKeys && !isHighlighted);
    });
    const svg = card.querySelector('svg');
    const processedData = card.__data;
    if (!svg || !processedData)
        return;
    const elementsToAnimate = new Map();
    const defs = svg.querySelector('defs');
    svg.querySelectorAll('[data-source-keys]').forEach(element => {
        const sourceKeys = JSON.parse(element.dataset.sourceKeys || '[]');
        const isHighlighted = hasActiveKeys && sourceKeys.some((key) => activeKeys.has(key));
        const computed = getComputedStyle(element);
        const current = parseFloat(computed.opacity) || 1;
        let end = 1;
        if (hasActiveKeys) {
            end = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;
        }
        if (Math.abs(current - end) > 0.001) {
            elementsToAnimate.set(element, { start: current, end });
        }
        const highlightOverlay = element.querySelector('.highlight-overlay');
        if (highlightOverlay) {
            highlightOverlay.style.opacity = isHighlighted ? '1' : '0';
        }
        if (isHighlighted && hasActiveKeys && defs) {
            const idParts = element.id.split('-');
            if (idParts.length >= 4) {
                const elementType = idParts[1];
                const elementIdSuffix = idParts.slice(2).join('-');
                const highlightGradId = `grad-${elementType}-highlight-${elementIdSuffix}`;
                const highlightGrad = defs.querySelector(`#${highlightGradId}`);
                if (highlightGrad) {
                    const keysForGradient = sourceKeys.filter((key) => activeKeys.has(key));
                    const gradientTransitionRatio = 0.1;
                    highlightGrad.innerHTML = createGradientStops(keysForGradient, processedData.cardPalettes, 'sat', gradientTransitionRatio);
                }
            }
        }
    });
    const controller = card.__cardHighlightController ?? (card.__cardHighlightController = { animationFrameId: null });
    if (elementsToAnimate.size > 0) {
        WordTree.Animator.animateOpacity(elementsToAnimate, controller);
    }
}
/**
 * Resets all highlighting on a card.
 */
function animateReset(card) {
    setCardHighlight(card, new Set());
    setAnchorHoverEffect(card, false);
}
/**
 * Sets up the single, comprehensive global event listener.
 */
export function setupGlobalEventHandlers() {
    if (window.unifiedHighlighterInitialized)
        return;
    window.unifiedHighlighterInitialized = true;
    document.addEventListener('mouseover', (event) => {
        const target = event.target;
        const card = target.closest('.word-trees-card');
        const last = globalEventState.lastHovered;
        // *** FIX: Explicitly reset the last card if we have moved to a different card or off all cards. ***
        if (last.card && last.card !== card) {
            animateReset(last.card);
            // Clear the state entirely now that we've left the old card's context.
            globalEventState.lastHovered = { card: null, cardKeys: new Set(), mainAnchorHover: false };
        }
        // If we are not on a card, our work is done.
        if (!card) {
            return;
        }
        // --- We are on a card. Determine the new state. ---
        const interactiveEl = target.closest('[data-card-name], .node-group, .interactive-subspan');
        let newCardKeys = new Set();
        let newMainAnchorHover = false;
        if (interactiveEl) {
            if (interactiveEl.matches('.main-anchor-span')) {
                newMainAnchorHover = true;
            }
            else if (interactiveEl.matches('.interactive-subspan')) {
                const parentNode = interactiveEl.closest('.node-group');
                if (parentNode) {
                    newCardKeys = new Set(JSON.parse(parentNode.dataset.sourceKeys || '[]'));
                }
            }
            else if (interactiveEl.matches('[data-card-name]')) {
                newCardKeys = new Set([interactiveEl.dataset.cardName]);
            }
            else if (interactiveEl.matches('.node-group')) {
                newCardKeys = new Set(JSON.parse(interactiveEl.dataset.sourceKeys || '[]'));
            }
        }
        // Re-read the state as it may have been cleared above.
        const currentLastState = globalEventState.lastHovered;
        if (card === currentLastState.card &&
            areSetsEqual(newCardKeys, currentLastState.cardKeys) &&
            newMainAnchorHover === currentLastState.mainAnchorHover) {
            return;
        }
        // Apply new state and update the global tracker.
        setCardHighlight(card, newCardKeys);
        setAnchorHoverEffect(card, newMainAnchorHover);
        globalEventState.lastHovered = { card, cardKeys: newCardKeys, mainAnchorHover: newMainAnchorHover };
    });
}
//# sourceMappingURL=word-tree-event-handler.js.map