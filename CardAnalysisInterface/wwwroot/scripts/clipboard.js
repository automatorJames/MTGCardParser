function copyTextToClipboard(text) {
    // This function writes the given text to the clipboard.
    // It returns a promise that resolves if the copy is successful
    // and rejects if it fails.
    navigator.clipboard.writeText(text).then(function () {
        // Success. No visual feedback needed here anymore.
    }, function (err) {
        // Log error to the console if copy fails.
        console.error('Failed to copy text: ', err);
    });
}