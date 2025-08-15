// span-tree-event-handler.ts
import { CardElement } from "./models.js";
import { WordTree } from "./word-tree-animator.js";

const globalEventState = {
    initialized: false,
    lastHovered: {
        card: null as CardElement | null,
        cardKeys: new Set<string>(),
        typeSeed: null as string | null,
        // This new property is key to distinguishing between global and local text effects
        textHighlightNodeContext: null as SVGGElement | null
    }
};

function areSetsEqual(setA: Set<string>, setB: Set<string>): boolean {
    if (setA.size !== setB.size) return false;
    for (const item of setA) {
        if (!setB.has(item)) return false;
    }
    return true;
}

/**
 * Applies type-based highlighting. It ONLY affects text and type header items.
 * Its behavior changes based on whether a specific node context is provided.
 */
function setTypeHighlight(card: CardElement, activeSeed: string | null, contextNode: SVGGElement | null) {
    card.classList.toggle('type-highlight-active', !!activeSeed);

    card.querySelectorAll<HTMLElement>('.type-name-item').forEach(item => {
        const seed = item.dataset.typeSeed || '';
        const isHighlighted = seed === activeSeed;
        item.classList.toggle('highlight', isHighlighted);
        item.classList.toggle('lowlight', !isHighlighted);
        item.style.color = isHighlighted ? item.style.getPropertyValue('--highlight-color') : '';
    });

    const svg = card.querySelector('svg');
    if (!svg) return;

    // If a specific node is the context (i.e., hovering a subspan), only dim text within it.
    if (contextNode) {
        // First, ensure all text OUTSIDE the context node is reset to full opacity.
        svg.querySelectorAll<SVGGElement>('.node-group').forEach(node => {
            if (node !== contextNode) {
                node.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => tspan.style.opacity = '1');
            }
        });
        // Then, apply dimming/highlighting only INSIDE the context node.
        contextNode.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            const tspanSeed = tspan.dataset.typeSeed;
            if (activeSeed && tspanSeed !== activeSeed) {
                tspan.style.opacity = '0.2';
            } else {
                tspan.style.opacity = '1';
                if (tspanSeed === activeSeed) tspan.style.fill = tspan.dataset.hoverColor!;
                else if (tspan.dataset.baseColor) tspan.style.fill = tspan.dataset.baseColor;
            }
        });
    } else { // No specific context means a global type hover (from the header).
        svg.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            const tspanSeed = tspan.dataset.typeSeed;
            if (activeSeed) {
                if (tspanSeed === activeSeed) {
                    tspan.style.fill = tspan.dataset.hoverColor!;
                    tspan.style.opacity = '1';
                } else {
                    tspan.style.opacity = '0.2';
                }
            } else { // Reset all text
                tspan.style.opacity = '1';
                if (tspan.dataset.baseColor) tspan.style.fill = tspan.dataset.baseColor;
            }
        });
    }
}

/**
 * Applies card-based highlighting. It ONLY affects node/connector structures and card header items.
 * Smoothly animates the group (<g>) opacity via Animator to avoid snapping conflicts with overlay fades.
 */
function setCardHighlight(card: CardElement, activeKeys: Set<string>) {
    const hasActiveKeys = activeKeys.size > 0;
    card.classList.toggle('highlight-active', hasActiveKeys);

    card.querySelectorAll<HTMLElement>('[data-card-name]').forEach(item => {
        const cardName = item.dataset.cardName || '';
        const isHighlighted = hasActiveKeys && activeKeys.has(cardName);
        item.classList.toggle('highlight', isHighlighted);
        item.classList.toggle('lowlight', hasActiveKeys && !isHighlighted);
    });

    const svg = card.querySelector('svg');
    if (!svg) return;

    // Build a batch of opacity animations for all node/connector groups.
    const elementsToAnimate = new Map<HTMLElement, { start: number; end: number }>();

    svg.querySelectorAll<SVGGElement>('[data-source-keys]').forEach(element => {
        const sourceKeys = JSON.parse(element.dataset.sourceKeys || '[]');
        const isHighlighted = hasActiveKeys && sourceKeys.some((key: string) => activeKeys.has(key));

        // Compute animation endpoints for the group (<g>) itself.
        const computed = getComputedStyle(element);
        const current = parseFloat(computed.opacity) || 1;
        const end = hasActiveKeys ? (isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity) : 1;

        if (Math.abs(current - end) > 0.001) {
            elementsToAnimate.set(element as unknown as HTMLElement, { start: current, end });
        }

        // Let the overlay handle its own CSS-driven fade; do NOT also fade the group immediately.
        const highlightOverlay = element.querySelector<HTMLElement>('.highlight-overlay');
        if (highlightOverlay) {
            // The overlay rect has `transition: opacity 150ms ease-in-out` in CSS, so this fades smoothly.
            highlightOverlay.style.opacity = isHighlighted ? '1' : '0';
        }
    });

    // One controller per card to prevent overlapping animations within the same tree.
    const controller =
        (card as any).__cardHighlightController ??
        ((card as any).__cardHighlightController = { animationFrameId: null });

    if (elementsToAnimate.size > 0) {
        WordTree.Animator.animateOpacity(elementsToAnimate, controller);
    }
}

/**
 * Resets all highlighting on a card by calling the specific reset logic for each type.
 */
function animateReset(card: CardElement) {
    setCardHighlight(card, new Set());
    setTypeHighlight(card, null, null);
}

/**
 * Sets up the single, comprehensive global event listener.
 */
export function setupGlobalEventHandlers(): void {
    if ((window as any).unifiedHighlighterInitialized) return;
    (window as any).unifiedHighlighterInitialized = true;

    document.addEventListener('mouseover', (event: MouseEvent) => {
        const target = event.target as Element;
        const card = target.closest<CardElement>('.span-trees-card');

        if (!card) {
            if (globalEventState.lastHovered.card) {
                animateReset(globalEventState.lastHovered.card);
                globalEventState.lastHovered = { card: null, cardKeys: new Set(), typeSeed: null, textHighlightNodeContext: null };
            }
            return;
        }

        const interactiveEl = target.closest<HTMLElement>('[data-card-name], .type-name-item, .node-group, .interactive-subspan');
        let newCardKeys = new Set<string>();
        let newTypeSeed: string | null = null;
        let newTextHighlightNodeContext: SVGGElement | null = null;

        if (interactiveEl) {
            if (interactiveEl.matches('.interactive-subspan')) {
                newTypeSeed = interactiveEl.dataset.typeSeed!;
                const parentNode = interactiveEl.closest<SVGGElement>('.node-group');
                if (parentNode) {
                    newCardKeys = new Set(JSON.parse(parentNode.dataset.sourceKeys || '[]'));
                    newTextHighlightNodeContext = parentNode;
                }
            } else if (interactiveEl.matches('.type-name-item')) {
                newTypeSeed = interactiveEl.dataset.typeSeed!;
            } else if (interactiveEl.matches('[data-card-name]')) {
                newCardKeys = new Set([interactiveEl.dataset.cardName!]);
            } else if (interactiveEl.matches('.node-group')) {
                newCardKeys = new Set(JSON.parse(interactiveEl.dataset.sourceKeys || '[]'));
            }
        }

        const last = globalEventState.lastHovered;
        if (card === last.card && newTypeSeed === last.typeSeed && areSetsEqual(newCardKeys, last.cardKeys)) {
            return; // No change
        }

        // Apply new state without a full reset, allowing additive effects.
        setCardHighlight(card, newCardKeys);
        setTypeHighlight(card, newTypeSeed, newTextHighlightNodeContext);

        globalEventState.lastHovered = { card, cardKeys: newCardKeys, typeSeed: newTypeSeed, textHighlightNodeContext: newTextHighlightNodeContext };
    });
}
