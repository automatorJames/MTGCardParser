// Client-side half of the Grammar Tools chat pane (Components/GrammarChatPane.razor).
window.grammarChat = {
    // Follows the conversation as it grows, unless the person has scrolled up to read something.
    follow(container, force) {
        if (container.follows === undefined) {
            container.follows = true;

            // Only the person's scrolling fires this while a reply grows, so it records their intent: whether
            // they left the view at the bottom or moved away from it.
            container.addEventListener('scroll', () => {
                container.follows = container.scrollHeight - container.scrollTop - container.clientHeight < 40;
            });
        }

        if (force || container.follows)
            container.scrollTop = container.scrollHeight;
    }
};
