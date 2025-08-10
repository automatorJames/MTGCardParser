// word-tree-animator.ts

export namespace WordTree.Animator {
    export interface AnimationManager {
        animationFrameId: number | null;
    }

    export const config = {
        duration: 100, // in milliseconds
        lowlightOpacity: 0.15
    };

    function lerp(start: number, end: number, amount: number): number {
        return start * (1 - amount) + end * amount;
    }

    export function animateOpacity(elementsToAnimate: Map<HTMLElement, { start: number; end: number }>, animationManager: AnimationManager): void {
        if (animationManager.animationFrameId) {
            cancelAnimationFrame(animationManager.animationFrameId);
        }

        const startTime = performance.now();

        const animationStep = (now: number) => {
            const elapsed = now - startTime;
            const progress = Math.min(elapsed / config.duration, 1);

            elementsToAnimate.forEach((targets, element) => {
                if (element) {
                    const currentOpacity = lerp(targets.start, targets.end, progress);
                    element.style.opacity = currentOpacity.toString();
                }
            });

            if (progress < 1) {
                animationManager.animationFrameId = requestAnimationFrame(animationStep);
            } else {
                elementsToAnimate.forEach((targets, element) => {
                    if (element) {
                        element.style.opacity = targets.end.toString();
                    }
                });
                animationManager.animationFrameId = null;
            }
        };

        animationManager.animationFrameId = requestAnimationFrame(animationStep);
    }
}