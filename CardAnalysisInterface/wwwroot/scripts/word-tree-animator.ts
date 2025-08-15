// word-tree-animator.ts
import { AnimationController } from "./models.js";

/**
 * The Animator namespace provides utility functions for visual animations,
 * such as smoothly transitioning opacity.
 */
export namespace WordTree.Animator {
    /**
     * Defines the configuration for animations.
     */
    export const config = {
        duration: 100, // Animation duration in milliseconds
        lowlightOpacity: 0.15 // Target opacity for non-highlighted elements
    };

    /**
     * Performs linear interpolation between two values.
     * @param start The starting value.
     * @param end The ending value.
     * @param progress The interpolation progress (a value from 0 to 1).
     * @returns The interpolated value.
     */
    function lerp(start: number, end: number, progress: number): number {
        return start * (1 - progress) + end * progress;
    }

    /**
     * Animates the opacity of a collection of HTML elements over a set duration.
        * @param elementsToAnimate A map where keys are the elements to animate and
        *   values are objects containing the start and end opacity values.
        * @param animationController An object to manage the animation frame ID,
        *   ensuring only one animation runs at a time for the given controller.
     */
    export function animateOpacity(
        elementsToAnimate: Map<HTMLElement, { start: number; end: number }>,
        animationController: AnimationController
    ): void {
        if (animationController.animationFrameId) {
            cancelAnimationFrame(animationController.animationFrameId);
        }

        const startTime = performance.now();

        const animationStep = (now: number) => {
            const elapsed = now - startTime;
            const progress = Math.min(elapsed / config.duration, 1);

            elementsToAnimate.forEach((targets, element) => {
                const currentOpacity = lerp(targets.start, targets.end, progress);
                element.style.opacity = currentOpacity.toString();
            });

            if (progress < 1) {
                animationController.animationFrameId = requestAnimationFrame(animationStep);
            } else {
                // Ensure final state is set perfectly
                elementsToAnimate.forEach((targets, element) => {
                    element.style.opacity = targets.end.toString();
                });
                animationController.animationFrameId = null;
            }
        };

        animationController.animationFrameId = requestAnimationFrame(animationStep);
    }
}
