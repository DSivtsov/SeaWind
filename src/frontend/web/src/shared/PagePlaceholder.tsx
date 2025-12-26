export function PagePlaceholder(props: { title: string }) {
    return (
        <div style={{ padding: 16 }}>
            <h2 style={{ margin: 0 }}>{props.title}</h2>
            <p style={{ marginTop: 8, opacity: 0.7 }}>TODO (MVP)</p>
        </div>
    );
}
