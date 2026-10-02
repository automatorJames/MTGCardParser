// Client-side half of the definition viewer (Dialogs/DefinitionViewerDialog.razor): Tab and Shift+Tab move between its tabs.
window.definitionViewer = {
    listenForTabKey(viewer) {
        this.stopListeningForTabKey();

        this.tabKeydown = e => {
            if (e.key !== 'Tab' || e.ctrlKey || e.altKey || e.metaKey)
                return;

            e.preventDefault();
            viewer.invokeMethodAsync('OnTabKey', e.shiftKey);
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
