/**
 * The Animator namespace provides utility functions for visual animations,
 * such as smoothly transitioning opacity.
 */
export var WordTree;
(function (WordTree) {
    var Animator;
    (function (Animator) {
        /**
         * Defines the configuration for animations.
         */
        Animator.config = {
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
        function lerp(start, end, progress) {
            return start * (1 - progress) + end * progress;
        }
        /**
         * Animates the opacity of a collection of HTML elements over a set duration.
            * @param elementsToAnimate A map where keys are the elements to animate and
            *   values are objects containing the start and end opacity values.
            * @param animationController An object to manage the animation frame ID,
            *   ensuring only one animation runs at a time for the given controller.
         */
        function animateOpacity(elementsToAnimate, animationController) {
            if (animationController.animationFrameId) {
                cancelAnimationFrame(animationController.animationFrameId);
            }
            const startTime = performance.now();
            const animationStep = (now) => {
                const elapsed = now - startTime;
                const progress = Math.min(elapsed / Animator.config.duration, 1);
                elementsToAnimate.forEach((targets, element) => {
                    const currentOpacity = lerp(targets.start, targets.end, progress);
                    element.style.opacity = currentOpacity.toString();
                });
                if (progress < 1) {
                    animationController.animationFrameId = requestAnimationFrame(animationStep);
                }
                else {
                    // Ensure final state is set perfectly
                    elementsToAnimate.forEach((targets, element) => {
                        element.style.opacity = targets.end.toString();
                    });
                    animationController.animationFrameId = null;
                }
            };
            animationController.animationFrameId = requestAnimationFrame(animationStep);
        }
        Animator.animateOpacity = animateOpacity;
    })(Animator = WordTree.Animator || (WordTree.Animator = {}));
})(WordTree || (WordTree = {}));
//# sourceMappingURL=word-tree-animator.js.map