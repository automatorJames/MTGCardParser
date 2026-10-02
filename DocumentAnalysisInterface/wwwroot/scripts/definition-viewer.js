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

// Rich tooltips for definition names (Components/DefinitionTooltip.razor): after the pointer rests on an element marked
// data-preview-kind / data-preview-name, the component shows that definition beside it, until the pointer leaves.
window.definitionTooltip = {
    delay: 450,

    attach(tooltip) {
        this.detach();

        let current = null;
        let timer = null;
        let shown = false;

        const hide = () => {
            clearTimeout(timer);
            current = null;

            if (shown) {
                shown = false;
                tooltip.invokeMethodAsync('Hide');
            }
        };

        this.mouseover = e => {
            const target = e.target.closest ? e.target.closest('[data-preview-name]') : null;

            if (target === current)
                return;

            hide();

            if (!target)
                return;

            current = target;
            timer = setTimeout(() => {
                if (current !== target || !target.isConnected)
                    return;

                const rect = target.getBoundingClientRect();
                shown = true;
                tooltip.invokeMethodAsync('Show', target.dataset.previewKind, target.dataset.previewName,
                    rect.left, rect.top, rect.bottom, window.innerWidth, window.innerHeight);
            }, this.delay);
        };

        this.hide = hide;
        document.addEventListener('mouseover', this.mouseover);
        document.addEventListener('mousedown', hide, true);
        document.addEventListener('scroll', hide, true);
        window.addEventListener('blur', hide);
    },

    detach() {
        if (!this.mouseover)
            return;

        this.hide();
        document.removeEventListener('mouseover', this.mouseover);
        document.removeEventListener('mousedown', this.hide, true);
        document.removeEventListener('scroll', this.hide, true);
        window.removeEventListener('blur', this.hide);
        this.mouseover = null;
    }
};
