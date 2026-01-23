class RegexEditor {
    dotNetRef = null;
    el = null;
    typeColors = {};
    isInternallyChanging = false;
    initialize(ref, element, colors) {
        this.dotNetRef = ref;
        this.el = element;
        this.typeColors = colors;
        this.el.addEventListener('beforeinput', this.onBeforeInput);
        this.el.addEventListener('input', this.onInput);
        this.el.addEventListener('keydown', this.onKeyDown);
        this.el.addEventListener('mousedown', this.onMouseDown);
        this.el.addEventListener('contextmenu', this.onContextMenu);
        document.addEventListener('mousedown', this.onGlobalMouseDown);
        document.addEventListener('keydown', this.onGlobalKeyDown);
        setTimeout(() => this.el?.focus(), 10);
    }
    dispose() {
        if (this.el) {
            this.el.removeEventListener('beforeinput', this.onBeforeInput);
            this.el.removeEventListener('input', this.onInput);
            this.el.removeEventListener('keydown', this.onKeyDown);
            this.el.removeEventListener('mousedown', this.onMouseDown);
            this.el.removeEventListener('contextmenu', this.onContextMenu);
        }
        document.removeEventListener('mousedown', this.onGlobalMouseDown);
        document.removeEventListener('keydown', this.onGlobalKeyDown);
        this.dotNetRef = null;
        this.el = null;
    }
    /**
     * Focuses the C# Class Name input and handles cursor placement at the end.
     */
    focusClassNameInput(selector) {
        const el = document.querySelector(selector);
        if (el) {
            el.focus();
            const val = el.value;
            el.value = ''; // Reset to force cursor to end
            el.value = val;
        }
    }
    syncPills(text, cursorPos, metadata) {
        if (this.isInternallyChanging || !this.el)
            return;
        this.isInternallyChanging = true;
        this.el.innerHTML = '';
        const tokenRegex = /(@[\w<>]+(\([^)]*\))?)/g;
        let lastIndex = 0;
        let match;
        const metaQueue = [...metadata];
        while ((match = tokenRegex.exec(text)) !== null) {
            // Append text before the match
            if (match.index > lastIndex) {
                this.el.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
            }
            const fullMatchText = match[0];
            const metaIdx = metaQueue.findIndex(m => m.typeName === fullMatchText);
            if (metaIdx !== -1) {
                const meta = metaQueue.splice(metaIdx, 1)[0];
                this.el.appendChild(this.createPillElement(fullMatchText, meta.id));
            }
            else {
                this.el.appendChild(document.createTextNode(fullMatchText));
            }
            lastIndex = match.index + fullMatchText.length;
        }
        if (lastIndex < text.length) {
            this.el.appendChild(document.createTextNode(text.substring(lastIndex)));
        }
        // Ensure there is always a text node to focus
        if (this.el.childNodes.length === 0 || this.el.lastChild?.nodeType !== Node.TEXT_NODE) {
            this.el.appendChild(document.createTextNode(''));
        }
        if (cursorPos >= 0) {
            this.restoreCursor(cursorPos);
        }
        this.isInternallyChanging = false;
    }
    insertPill(textToReplace, fullTokenText) {
        if (!this.el)
            return;
        const pos = this.getCaretOffset();
        const fullText = this.el.textContent || "";
        const textToInsert = fullTokenText + '\u00A0';
        const newText = fullText.substring(0, pos - textToReplace.length) + textToInsert + fullText.substring(pos);
        this.isInternallyChanging = false;
        this.el.textContent = newText;
        this.onInput();
        setTimeout(() => this.el?.focus(), 0);
    }
    createPillElement(text, id) {
        const baseTypeName = text.match(/<([^>]+)>/)?.[1] || text.substring(1);
        const colors = this.typeColors[baseTypeName] || { normal: '#4A5568', highlight: '#718096' };
        const span = document.createElement('span');
        span.className = 'token-style';
        span.contentEditable = 'false';
        span.style.backgroundColor = colors.normal;
        span.setAttribute('data-type-name', text);
        span.setAttribute('data-snippet-id', id);
        // The hidden @ ensures the .textContent of the parent div includes the @ symbol
        span.innerHTML = `<span style="display:none">@</span><span>${text.substring(1)}</span>`;
        return span;
    }
    onInput = () => {
        if (this.isInternallyChanging || !this.el || !this.dotNetRef)
            return;
        const info = this.getCaretPositionInfo();
        this.dotNetRef.invokeMethodAsync('NotifyContentChanged', this.el.textContent, info.currentWord);
    };
    onBeforeInput = (e) => {
        if (!this.el)
            return;
        const selection = window.getSelection();
        if (!selection?.rangeCount)
            return;
        const range = selection.getRangeAt(0);
        const tokensToDelete = new Set();
        const allTokens = Array.from(this.el.querySelectorAll('.token-style'));
        // Handle direct deletion of highlighted pill
        const highlighted = this.el.querySelector('.token-selected');
        if (highlighted && e.inputType.startsWith('delete')) {
            e.preventDefault();
            highlighted.remove();
            this.onInput();
            return;
        }
        // Detect pills intersected by a selection range
        allTokens.forEach(token => {
            const tokenRange = document.createRange();
            tokenRange.selectNode(token);
            const intersect = !(range.compareBoundaryPoints(Range.START_TO_END, tokenRange) >= 0 ||
                range.compareBoundaryPoints(Range.END_TO_START, tokenRange) <= 0);
            if (intersect)
                tokensToDelete.add(token);
        });
        if (tokensToDelete.size > 0) {
            e.preventDefault();
            tokensToDelete.forEach(t => t.remove());
            this.onInput();
        }
    };
    onKeyDown = (e) => {
        const dropdown = document.getElementById('autocomplete-dropdown-list');
        if (dropdown && dropdown.offsetParent !== null) {
            if (['Enter', 'Tab', 'ArrowUp', 'ArrowDown'].includes(e.key))
                e.preventDefault();
            return;
        }
        // Navigation logic into/out of pills
        if (e.key === 'ArrowRight' || e.key === 'ArrowLeft') {
            this.handlePillNavigation(e);
        }
        else {
            this.clearTokenHighlights();
        }
    };
    handlePillNavigation(e) {
        const selection = window.getSelection();
        if (!selection?.rangeCount || !this.el)
            return;
        const range = selection.getRangeAt(0);
        if (!range.collapsed)
            return;
        const isRight = e.key === 'ArrowRight';
        const container = range.startContainer;
        const offset = range.startOffset;
        let target = null;
        if (isRight) {
            if (container.nodeType === Node.TEXT_NODE && offset === container.length)
                target = container.nextSibling;
            else if (container.nodeType === Node.ELEMENT_NODE)
                target = container.childNodes[offset];
        }
        else {
            if (container.nodeType === Node.TEXT_NODE && offset === 0)
                target = container.previousSibling;
            else if (container.nodeType === Node.ELEMENT_NODE && offset > 0)
                target = container.childNodes[offset - 1];
        }
        if (target instanceof HTMLElement && target.classList.contains('token-style')) {
            e.preventDefault();
            if (!target.classList.contains('token-selected')) {
                this.clearTokenHighlights();
                this.setTokenHighlight(target, true);
                const newRange = document.createRange();
                newRange.selectNode(target);
                selection.removeAllRanges();
                selection.addRange(newRange);
            }
            else {
                this.setTokenHighlight(target, false);
                const newRange = document.createRange();
                isRight ? newRange.setStartAfter(target) : newRange.setStartBefore(target);
                newRange.collapse(true);
                selection.removeAllRanges();
                selection.addRange(newRange);
            }
        }
    }
    onMouseDown = (e) => {
        const token = e.target.closest('.token-style');
        if (token) {
            e.preventDefault();
            e.stopPropagation();
            this.clearTokenHighlights();
            this.setTokenHighlight(token, true);
            const range = document.createRange();
            range.selectNode(token);
            const selection = window.getSelection();
            selection?.removeAllRanges();
            selection?.addRange(range);
            this.el?.focus();
        }
        else {
            this.clearTokenHighlights();
        }
    };
    onContextMenu = (e) => {
        const token = e.target.closest('.token-style');
        if (token && this.dotNetRef) {
            e.preventDefault();
            const typeName = token.getAttribute('data-type-name') || "";
            const snippetId = token.getAttribute('data-snippet-id') || "";
            this.dotNetRef.invokeMethodAsync('OpenPillMenu', typeName, snippetId, e.clientX, e.clientY);
        }
    };
    onGlobalMouseDown = (e) => {
        const item = e.target.closest('.autocomplete-item');
        if (!item)
            return;
        e.preventDefault();
        const typeName = item.querySelector('.type-name')?.textContent?.trim();
        if (typeName)
            this.dotNetRef?.invokeMethodAsync('SelectSuggestionFromJS', typeName);
    };
    onGlobalKeyDown = (e) => {
        if (e.key === 'Escape')
            this.dotNetRef?.invokeMethodAsync('HandleGlobalEscape');
    };
    setTokenHighlight(token, isHighlighted) {
        const typeName = token.getAttribute('data-type-name') || "";
        const baseTypeName = typeName.match(/<([^>]+)>/)?.[1] || typeName.replace('@', '');
        const colors = this.typeColors[baseTypeName] || { normal: '#4A5568', highlight: '#718096' };
        if (isHighlighted) {
            token.classList.add('token-selected');
            token.style.backgroundColor = colors.highlight;
        }
        else {
            token.classList.remove('token-selected');
            token.style.backgroundColor = colors.normal;
        }
    }
    clearTokenHighlights() {
        this.el?.querySelectorAll('.token-selected').forEach(t => this.setTokenHighlight(t, false));
    }
    getCaretOffset() {
        const selection = window.getSelection();
        if (!selection?.rangeCount || !this.el)
            return 0;
        const range = selection.getRangeAt(0);
        const preCaretRange = range.cloneRange();
        preCaretRange.selectNodeContents(this.el);
        preCaretRange.setEnd(range.startContainer, range.startOffset);
        return preCaretRange.toString().length;
    }
    restoreCursor(charOffset) {
        if (!this.el)
            return;
        const walker = document.createTreeWalker(this.el, NodeFilter.SHOW_TEXT, null);
        let cumulativeOffset = 0;
        let node;
        while (node = walker.nextNode()) {
            if (cumulativeOffset + node.length >= charOffset) {
                const range = document.createRange();
                const selection = window.getSelection();
                range.setStart(node, charOffset - cumulativeOffset);
                range.collapse(true);
                selection?.removeAllRanges();
                selection?.addRange(range);
                return;
            }
            cumulativeOffset += node.length;
        }
    }
    getCaretPositionInfo() {
        const selection = window.getSelection();
        const node = selection?.anchorNode;
        if (!node || node.nodeType !== Node.TEXT_NODE)
            return { currentWord: "" };
        const textUpToCaret = node.textContent?.substring(0, selection.anchorOffset) || "";
        const words = textUpToCaret.split(/[\s\u00A0]+/);
        return { currentWord: words[words.length - 1] };
    }
}
// Global singleton/instance for Blazor
window.regexEditor = new RegexEditor();
//# sourceMappingURL=regex-editor.js.map