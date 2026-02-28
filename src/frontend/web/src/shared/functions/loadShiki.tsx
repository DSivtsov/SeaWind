// Shiki requires async code to load the highlighter
export async function loadShiki() {
    const { createHighlighter } = await import('shiki');
    const shiki = await createHighlighter({
        langs: ['tsx', 'scss', 'html', 'bash', 'json', `csharp`],
        // You can load supported themes here
        themes: [],
    });

    return shiki;
}
