// general.js
// Small, page-independent JS helpers that don't belong to any single feature area.

const rainbowSpinnerHandles = new Map();

// Continuously rotates an SVG gradient's coordinate system so its colors sweep
// around a ring at their own pace, independent of any CSS animation applied to
// the shape the gradient is painting.
function initRainbowSpinner(gradientId, centerX, centerY, durationMs) {
    stopRainbowSpinner(gradientId);

    const start = performance.now();

    const step = (now) => {
        const gradientEl = document.getElementById(gradientId);
        if (!gradientEl) {
            rainbowSpinnerHandles.delete(gradientId);
            return;
        }

        const elapsed = (now - start) % durationMs;
        const angle = (elapsed / durationMs) * 360;
        gradientEl.setAttribute('gradientTransform', `rotate(${angle} ${centerX} ${centerY})`);

        rainbowSpinnerHandles.set(gradientId, requestAnimationFrame(step));
    };

    rainbowSpinnerHandles.set(gradientId, requestAnimationFrame(step));
}

function stopRainbowSpinner(gradientId) {
    const handle = rainbowSpinnerHandles.get(gradientId);
    if (handle !== undefined) {
        cancelAnimationFrame(handle);
        rainbowSpinnerHandles.delete(gradientId);
    }
}
