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
 * Gets all type seeds represented in the currently highlighted node path
 */
function getHighlightedPathTypeSeeds(card: CardElement, activeKeys: Set<string>): Set<string> {
    const highlightedSeeds = new Set<string>();
    const svg = card.querySelector('svg');
    if (!svg) return highlightedSeeds;

    // Find all nodes that are part of the highlighted path
    svg.querySelectorAll<SVGGElement>('[data-source-keys]').forEach(element => {
        const sourceKeys = JSON.parse(element.dataset.sourceKeys || '[]');
        const isHighlighted = activeKeys.size > 0 && sourceKeys.some((key: string) => activeKeys.has(key));

        if (isHighlighted) {
            // Collect all type seeds from this highlighted node
            element.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
                const tspanSeed = tspan.dataset.typeSeed;
                if (tspanSeed) {
                    highlightedSeeds.add(tspanSeed);
                }
            });
        }
    });

    return highlightedSeeds;
}

/**
 * Applies type-based highlighting. It ONLY affects text and type header items.
 * Its behavior changes based on whether a specific node context is provided.
 */
function setTypeHighlight(card: CardElement, activeSeed: string | null, contextNode: SVGGElement | null, activeKeys: Set<string>) {
    card.classList.toggle('type-highlight-active', !!activeSeed);

    // Get the type seeds that are represented in the highlighted path
    const highlightedPathSeeds = getHighlightedPathTypeSeeds(card, activeKeys);

    card.querySelectorAll<HTMLElement>('.type-name-item').forEach(item => {
        const seed = item.dataset.typeSeed || '';
        const isDirectlyHighlighted = seed === activeSeed;
        const isInHighlightedPath = highlightedPathSeeds.has(seed);

        if (activeSeed) {
            // Case 1: Type-name-item hover - dim all except the hovered one
            if (contextNode === null) {
                item.classList.toggle('highlight', isDirectlyHighlighted);
                item.classList.toggle('lowlight', !isDirectlyHighlighted);
                item.style.color = isDirectlyHighlighted ? item.style.getPropertyValue('--highlight-color') : '';
            }
            // Case 3: Data-type-seed span hover - special handling
            else {
                if (isDirectlyHighlighted) {
                    // The directly hovered type transitions from white to Palette.Hex
                    item.classList.add('highlight');
                    item.classList.remove('lowlight');
                    item.style.color = item.style.getPropertyValue('--highlight-color');
                } else {
                    // Other type-name-items are dimmed, but keep borders if they're in highlighted path
                    item.classList.remove('highlight');
                    item.classList.add('lowlight');
                    item.style.color = '';

                    // Keep border color if this type is represented in the highlighted path
                    if (isInHighlightedPath) {
                        // Keep the HexLight border (no dimming)
                        item.style.opacity = '1';
                    }
                }
            }
        }
        // Case 2: Node hover (but not a specific span) - dim types not in highlighted path
        else if (activeKeys.size > 0) {
            if (isInHighlightedPath) {
                item.classList.remove('lowlight');
                item.classList.remove('highlight');
                item.style.color = '';
            } else {
                item.classList.add('lowlight');
                item.classList.remove('highlight');
                item.style.color = '';
            }
        }
        // Reset case
        else {
            item.classList.remove('highlight');
            item.classList.remove('lowlight');
            item.style.color = '';
        }
    });

    const svg = card.querySelector('svg');
    if (!svg) return;

    // Handle SVG text content
    if (contextNode) {
        // Case 3: Hovering a data-type-seed span - only affect text within the context node
        svg.querySelectorAll<SVGGElement>('.node-group').forEach(node => {
            if (node !== contextNode) {
                node.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => tspan.style.opacity = '1');
            }
        });

        contextNode.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            const tspanSeed = tspan.dataset.typeSeed;
            if (activeSeed && tspanSeed !== activeSeed) {
                tspan.style.opacity = '0.2';
            } else {
                tspan.style.opacity = '1';
                if (tspanSeed === activeSeed) {
                    tspan.style.fill = tspan.dataset.hoverColor!;
                } else if (tspan.dataset.baseColor) {
                    tspan.style.fill = tspan.dataset.baseColor;
                }
            }
        });
    } else if (activeSeed) {
        // Case 1: Type-name-item hover - global text effects
        svg.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            const tspanSeed = tspan.dataset.typeSeed;
            if (tspanSeed === activeSeed) {
                tspan.style.fill = tspan.dataset.hoverColor!;
                tspan.style.opacity = '1';
            } else {
                tspan.style.opacity = '0.2';
            }
        });
    } else {
        // Reset all text or leave as-is for node hover case
        svg.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            tspan.style.opacity = '1';
            if (tspan.dataset.baseColor) tspan.style.fill = tspan.dataset.baseColor;
        });
    }
}

/**
 * Applies card-based highlighting. It ONLY affects node/connector structures and card header items.
 * Smoothly animates the group (<g>) opacity via Animator to avoid snapping conflicts with overlay fades.
 */
function setCardHighlight(card: CardElement, activeKeys: Set<string>, activeSeed: string | null) {
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
        let isHighlighted = hasActiveKeys && sourceKeys.some((key: string) => activeKeys.has(key));

        // Case 1: Type-name-item hover - additional logic for dimming nodes
        if (activeSeed && !hasActiveKeys) {
            // Check if this node contains any instances of the hovered type seed
            const nodeHasTypeSeed = element.querySelector(`[data-type-seed="${activeSeed}"]`) !== null;
            isHighlighted = nodeHasTypeSeed;
        }

        // Compute animation endpoints for the group (<g>) itself.
        const computed = getComputedStyle(element);
        const current = parseFloat(computed.opacity) || 1;

        let end: number;
        if (activeSeed && !hasActiveKeys) {
            // Case 1: Type hover - dim nodes that don't contain the type
            end = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;
        } else if (hasActiveKeys) {
            // Case 2 & 3: Normal card highlighting
            end = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;
        } else {
            // Reset case
            end = 1;
        }

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

    // Handle lines (connectors) - Case 1: dim all lines when hovering type-name-item
    if (activeSeed && !hasActiveKeys) {
        svg.querySelectorAll<SVGPathElement>('.connector-path.base-layer').forEach(path => {
            const computed = getComputedStyle(path);
            const current = parseFloat(computed.opacity) || 1;
            const end = WordTree.Animator.config.lowlightOpacity;

            if (Math.abs(current - end) > 0.001) {
                elementsToAnimate.set(path as unknown as HTMLElement, { start: current, end });
            }
        });
    } else {
        // Reset lines to full opacity for other cases
        svg.querySelectorAll<SVGPathElement>('.connector-path.base-layer').forEach(path => {
            const computed = getComputedStyle(path);
            const current = parseFloat(computed.opacity) || 1;
            const end = 1;

            if (Math.abs(current - end) > 0.001) {
                elementsToAnimate.set(path as unknown as HTMLElement, { start: current, end });
            }
        });
    }

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
    setCardHighlight(card, new Set(), null);
    setTypeHighlight(card, null, null, new Set());
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
        setCardHighlight(card, newCardKeys, newTypeSeed);
        setTypeHighlight(card, newTypeSeed, newTextHighlightNodeContext, newCardKeys);

        globalEventState.lastHovered = { card, cardKeys: newCardKeys, typeSeed: newTypeSeed, textHighlightNodeContext: newTextHighlightNodeContext };
    });
}