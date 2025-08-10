// word-tree-animator.ts
export var WordTree;
(function (WordTree) {
    var Animator;
    (function (Animator) {
        Animator.config = {
            duration: 100, // in milliseconds
            lowlightOpacity: 0.15
        };
        function lerp(start, end, amount) {
            return start * (1 - amount) + end * amount;
        }
        function animateOpacity(elementsToAnimate, animationManager) {
            if (animationManager.animationFrameId) {
                cancelAnimationFrame(animationManager.animationFrameId);
            }
            const startTime = performance.now();
            const animationStep = (now) => {
                const elapsed = now - startTime;
                const progress = Math.min(elapsed / Animator.config.duration, 1);
                elementsToAnimate.forEach((targets, element) => {
                    if (element) {
                        const currentOpacity = lerp(targets.start, targets.end, progress);
                        element.style.opacity = currentOpacity.toString();
                    }
                });
                if (progress < 1) {
                    animationManager.animationFrameId = requestAnimationFrame(animationStep);
                }
                else {
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
        Animator.animateOpacity = animateOpacity;
    })(Animator = WordTree.Animator || (WordTree.Animator = {}));
})(WordTree || (WordTree = {}));
//# sourceMappingURL=word-tree-animator.js.map