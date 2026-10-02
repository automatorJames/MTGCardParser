// Client-side half of the definition viewer (Dialogs/DefinitionViewerDialog.razor): Tab and Shift+Tab, or the right and left arrows, move between its tabs.
window.definitionViewer = {
    listenForTabKey(viewer) {
        this.stopListeningForTabKey();

        this.tabKeydown = e => {
            if (e.ctrlKey || e.altKey || e.metaKey)
                return;

            const backwards = e.key === 'Tab' ? e.shiftKey
                : e.key === 'ArrowLeft' ? true
                : e.key === 'ArrowRight' ? false
                : null;

            if (backwards === null)
                return;

            e.preventDefault();
            viewer.invokeMethodAsync('OnTabKey', backwards);
        };

        document.addEventListener('keydown', this.tabKeydown);
    },

    stopListeningForTabKey() {
        if (this.tabKeydown) {
            document.removeEventListener('keydown', this.tabKeydown);
            this.tabKeydown = null;
        }
    }
};
