// word-tree-animator.js

// This file contains the self-contained animation engine.
// It knows nothing about word trees, only how to animate opacity.

window.wordTree = window.wordTree || {};

window.wordTree.Animator = {
    config: {
        duration: 100, // The one true speed control, in milliseconds
        lowlightOpacity: 0.15
    },

    // Linear Interpolation (lerp) helper
    lerp: function (start, end, amount) {
        return start * (1 - amount) + end * amount;
    },

    /**
     * The core animation function.
     * @param {Map<HTMLElement, {start: number, end: number}>} elementsToAnimate - A map of elements to their start/end opacities.
     * @param {object} animationManager - The object holding the animation frame ID for this container.
     */
    animateOpacity: function (elementsToAnimate, animationManager) {
        if (animationManager.animationFrameId) {
            cancelAnimationFrame(animationManager.animationFrameId);
        }

        const startTime = performance.now();

        const animationStep = (now) => {
            const elapsed = now - startTime;
            const progress = Math.min(elapsed / this.config.duration, 1);

            elementsToAnimate.forEach((targets, element) => {
                if (element) {
                    const currentOpacity = this.lerp(targets.start, targets.end, progress);
                    element.style.opacity = currentOpacity;
                }
            });

            if (progress < 1) {
                animationManager.animationFrameId = requestAnimationFrame(animationStep);
            } else {
                // Animation complete, ensure final state is set perfectly.
                elementsToAnimate.forEach((targets, element) => {
                    if (element) {
                        element.style.opacity = targets.end;
                    }
                });
                animationManager.animationFrameId = null;
            }
        };

        animationManager.animationFrameId = requestAnimationFrame(animationStep);
    }
};