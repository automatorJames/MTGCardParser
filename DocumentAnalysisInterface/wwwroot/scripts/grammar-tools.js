// Client-side half of the Grammar Tools page (Pages/GrammarToolsPage.razor): Ctrl+Z / Ctrl+Y undo and redo.
window.grammarTools = {
    listenForUndo(page) {
        this.stopListeningForUndo();

        this.undoKeydown = e => {
            if (!(e.ctrlKey || e.metaKey) || e.altKey)
                return;

            const key = e.key.toLowerCase();

            if (key !== 'z' && key !== 'y')
                return;

            // Text being edited keeps its own undo.
            if (e.target.closest && e.target.closest('input, textarea, select, [contenteditable=""], [contenteditable="true"]'))
                return;

            e.preventDefault();
            page.invokeMethodAsync('OnUndoKey', key === 'y' || e.shiftKey);
        };

        document.addEventListener('keydown', this.undoKeydown);
    },

    stopListeningForUndo() {
        if (this.undoKeydown) {
            document.removeEventListener('keydown', this.undoKeydown);
            this.undoKeydown = null;
        }
    }
};
