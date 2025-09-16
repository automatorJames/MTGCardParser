let dotNetRef;

function handleLayoutKeydown(e) {
    // --- GUARD CLAUSE ---
    // Exit immediately if the Ctrl key is not pressed, or if the key is not an arrow key we care about.
    // The key property values for arrows are "ArrowRight" and "ArrowLeft".
    if (e.key !== 'ArrowRight' && e.key !== 'ArrowLeft') {
        return;
    }

    // At this point, we know Ctrl + an arrow key was pressed.
    // We call preventDefault() to stop the browser from doing anything else,
    // like moving the cursor in a text box.
    e.preventDefault();

    // Check for "Ctrl + Right Arrow" to navigate forward
    if (e.key === 'ArrowRight') {
        console.log('Cycling page forward (Ctrl + ArrowRight)...');
        dotNetRef.invokeMethodAsync('CyclePage', 'forward');
    }
    // Check for "Ctrl + Left Arrow" to navigate backward
    else if (e.key === 'ArrowLeft') {
        console.log('Cycling page backward (Ctrl + ArrowLeft)...');
        dotNetRef.invokeMethodAsync('CyclePage', 'backward');
    }
}

function initializeLayoutListener(dotNetObjectReference) {
    dotNetRef = dotNetObjectReference;
    document.addEventListener('keydown', handleLayoutKeydown);
}

function disposeLayoutListener() {
    document.removeEventListener('keydown', handleLayoutKeydown);
}