// span-tree-event-handler.ts
import { CardElement } from "./models.js";
import { WordTree } from "./word-tree-animator.js";

const globalEventState = {
    initialized: false,
    lastHovered: {
        card: null as CardElement | null,
        cardKeys: new Set<string>(),
        typeSeed: null as string | null,
        textHighlightNodeContext: null as SVGGElement | null,
        mainAnchorHover: false
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

    svg.querySelectorAll<SVGGElement>('[data-source-keys]').forEach(element => {
        const sourceKeys = JSON.parse(element.dataset.sourceKeys || '[]');
        const isHighlighted = activeKeys.size > 0 && sourceKeys.some((key: string) => activeKeys.has(key));

        if (isHighlighted) {
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
 */
function setTypeHighlight(card: CardElement, activeSeed: string | null, contextNode: SVGGElement | null, activeKeys: Set<string>) {
    card.classList.toggle('type-highlight-active', !!activeSeed);
    const highlightedPathSeeds = getHighlightedPathTypeSeeds(card, activeKeys);

    card.querySelectorAll<HTMLElement>('.type-name-item').forEach(item => {
        const seed = item.dataset.typeSeed || '';
        const isDirectlyHighlighted = seed === activeSeed;
        const isInHighlightedPath = highlightedPathSeeds.has(seed);

        if (activeSeed) {
            if (contextNode === null) {
                item.classList.toggle('highlight', isDirectlyHighlighted);
                item.classList.toggle('lowlight', !isDirectlyHighlighted);
                item.style.color = isDirectlyHighlighted ? item.style.getPropertyValue('--highlight-color') : '';
            } else {
                if (isDirectlyHighlighted) {
                    item.classList.add('highlight');
                    item.classList.remove('lowlight');
                    item.style.color = item.style.getPropertyValue('--highlight-color');
                } else {
                    item.classList.remove('highlight');
                    item.classList.add('lowlight');
                    item.style.color = '';
                    if (isInHighlightedPath) {
                        item.style.opacity = '1';
                    }
                }
            }
        } else if (activeKeys.size > 0) {
            item.classList.toggle('lowlight', !isInHighlightedPath);
            item.classList.remove('highlight');
            item.style.color = '';
        } else {
            item.classList.remove('highlight', 'lowlight');
            item.style.color = '';
        }
    });

    const svg = card.querySelector('svg');
    if (!svg) return;

    if (contextNode) {
        svg.querySelectorAll<SVGGElement>('.node-group').forEach(node => {
            if (node !== contextNode) {
                node.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => tspan.style.opacity = '1');
            }
        });
        contextNode.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            const tspanSeed = tspan.dataset.typeSeed;
            tspan.style.opacity = (activeSeed && tspanSeed !== activeSeed) ? '0.2' : '1';
            if (tspanSeed === activeSeed) {
                tspan.style.fill = tspan.dataset.hoverColor!;
            } else if (tspan.dataset.baseColor) {
                tspan.style.fill = tspan.dataset.baseColor;
            }
        });
    } else if (activeSeed) {
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
        svg.querySelectorAll<SVGTSpanElement>('.node-text-content').forEach(tspan => {
            tspan.style.opacity = '1';
            if (tspan.dataset.baseColor) tspan.style.fill = tspan.dataset.baseColor;
        });
    }
}

/**
 * Smoothly animates the white overlay for node borders and connectors on anchor hover.
 */
function setAnchorHoverEffect(card: CardElement, isHovering: boolean): void {
    const svg = card.querySelector('svg');
    if (!svg) return;

    const elementsToAnimate = new Map<HTMLElement, { start: number; end: number }>();
    const overlays = svg.querySelectorAll<SVGPathElement | SVGRectElement>('.anchor-hover-overlay');

    overlays.forEach(overlay => {
        const current = parseFloat(getComputedStyle(overlay).opacity) || 0;
        const end = isHovering ? 1 : 0;
        if (Math.abs(current - end) > 0.001) {
            elementsToAnimate.set(overlay as unknown as HTMLElement, { start: current, end });
        }
    });

    const controller =
        (card as any).__anchorHoverController ??
        ((card as any).__anchorHoverController = { animationFrameId: null });

    if (elementsToAnimate.size > 0) {
        WordTree.Animator.animateOpacity(elementsToAnimate, controller);
    }
}


/**
 * Applies card-based highlighting. Affects node/connector structures and card header items.
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
    const elementsToAnimate = new Map<HTMLElement, { start: number; end: number }>();

    svg.querySelectorAll<SVGGElement>('[data-source-keys]').forEach(element => {
        const sourceKeys = JSON.parse(element.dataset.sourceKeys || '[]');
        let isHighlighted = hasActiveKeys && sourceKeys.some((key: string) => activeKeys.has(key));
        if (activeSeed && !hasActiveKeys) {
            isHighlighted = !!element.querySelector(`[data-type-seed="${activeSeed}"]`);
        }

        const computed = getComputedStyle(element);
        const current = parseFloat(computed.opacity) || 1;
        let end = 1;
        if ((activeSeed && !hasActiveKeys) || hasActiveKeys) {
            end = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;
        }

        if (Math.abs(current - end) > 0.001) {
            elementsToAnimate.set(element as unknown as HTMLElement, { start: current, end });
        }
        const highlightOverlay = element.querySelector<HTMLElement>('.highlight-overlay');
        if (highlightOverlay) {
            highlightOverlay.style.opacity = isHighlighted ? '1' : '0';
        }
    });

    if (activeSeed && !hasActiveKeys) {
        svg.querySelectorAll<SVGPathElement>('.connector-path.base-layer').forEach(path => {
            const computed = getComputedStyle(path);
            const current = parseFloat(computed.opacity) || 1;
            const end = WordTree.Animator.config.lowlightOpacity;
            if (Math.abs(current - end) > 0.001) {
                elementsToAnimate.set(path as unknown as HTMLElement, { start: current, end });
            }
        });
    }

    const controller = (card as any).__cardHighlightController ?? ((card as any).__cardHighlightController = { animationFrameId: null });
    if (elementsToAnimate.size > 0) {
        WordTree.Animator.animateOpacity(elementsToAnimate, controller);
    }
}

/**
 * Resets all highlighting on a card.
 */
function animateReset(card: CardElement) {
    setCardHighlight(card, new Set(), null);
    setTypeHighlight(card, null, null, new Set());
    setAnchorHoverEffect(card, false); // Add this call to ensure reset
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
                globalEventState.lastHovered = { card: null, cardKeys: new Set(), typeSeed: null, textHighlightNodeContext: null, mainAnchorHover: false };
            }
            return;
        }

        const interactiveEl = target.closest<HTMLElement>('[data-card-name], .type-name-item, .node-group, .interactive-subspan');
        let newCardKeys = new Set<string>();
        let newTypeSeed: string | null = null;
        let newTextHighlightNodeContext: SVGGElement | null = null;
        let newMainAnchorHover = false;

        if (interactiveEl) {
            if (interactiveEl.matches('.main-anchor-span')) {
                newMainAnchorHover = true;
            } else if (interactiveEl.matches('.interactive-subspan')) {
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
        if (card === last.card && newTypeSeed === last.typeSeed && areSetsEqual(newCardKeys, last.cardKeys) && newMainAnchorHover === last.mainAnchorHover) {
            return;
        }

        setCardHighlight(card, newCardKeys, newTypeSeed);
        setTypeHighlight(card, newTypeSeed, newTextHighlightNodeContext, newCardKeys);
        setAnchorHoverEffect(card, newMainAnchorHover); // Call the new animation function

        globalEventState.lastHovered = { card, cardKeys: newCardKeys, typeSeed: newTypeSeed, textHighlightNodeContext: newTextHighlightNodeContext, mainAnchorHover: newMainAnchorHover };
    });
}