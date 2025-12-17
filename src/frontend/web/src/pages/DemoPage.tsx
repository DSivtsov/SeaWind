import React, { useEffect, useMemo, useState } from "react";

/**
 * React + TypeScript practical minimum:
 * - props typing
 * - children typing
 * - NO React.FC
 * - useState typing
 * - useEffect deps + cleanup
 * - events typing (ChangeEvent, MouseEvent)
 * - union types for UI states
 * - one generic component
 * - one custom hook (typed)
 */

type UiState = "loading" | "empty" | "error" | "default";

type PageShellProps = {
  title: string;
  state: UiState;
  errorText?: string;
  onRetry?: () => void;
  children?: React.ReactNode;
};

function PageShell(props: PageShellProps) {
  const { title, state, errorText, onRetry, children } = props;

  return (
    <div style={{ padding: 16, fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial" }}>
      <header style={{ marginBottom: 12 }}>
        <h2 style={{ margin: 0, fontSize: 20 }}>{title}</h2>
      </header>

      {(() => {
        switch (state) {
          case "loading":
            return <div>Loading…</div>;
          case "empty":
            return <div>Nothing here yet.</div>;
          case "error":
            return (
              <div style={{ display: "grid", gap: 8 }}>
                <div style={{ color: "crimson" }}>{errorText ?? "Something went wrong."}</div>
                {onRetry ? (
                  <button type="button" onClick={onRetry}>
                    Retry
                  </button>
                ) : null}
              </div>
            );
          case "default":
            return <div>{children}</div>;
          default: {
            // Exhaustiveness guard: if UiState changes, TS will error here.
            const _never: never = state;
            return _never;
          }
        }
      })()}
    </div>
  );
}

/** Custom hook (typed) + cleanup in useEffect */
function useDebouncedValue<T>(value: T, delayMs: number) {
  const [debounced, setDebounced] = useState<T>(value);

  useEffect(() => {
    const id = window.setTimeout(() => setDebounced(value), delayMs);
    return () => window.clearTimeout(id);
  }, [value, delayMs]);

  return debounced;
}

/** One generic component example */
type SelectProps<T> = {
  items: readonly T[];
  value: T | null;
  getKey: (item: T) => string;
  render: (item: T) => React.ReactNode;
  onSelect: (item: T) => void;
};

function Select<T>(props: SelectProps<T>) {
  const { items, value, getKey, render, onSelect } = props;

  return (
    <div style={{ display: "grid", gap: 6 }}>
      {items.map((it) => {
        const key = getKey(it);
        const isSelected = value != null && getKey(value) === key;

        return (
          <button
            key={key}
            type="button"
            onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
              e.preventDefault();
              onSelect(it);
            }}
            style={{
              textAlign: "left",
              padding: "8px 10px",
              borderRadius: 10,
              border: "1px solid #444",
              background: isSelected ? "#222" : "transparent",
              color: "inherit",
              cursor: "pointer",
            }}
          >
            {render(it)}
          </button>
        );
      })}
    </div>
  );
}

/** Demo: uses all points in one place */
type Course = {
  id: string;
  title: string;
};

const ALL_COURSES: Course[] = [
  { id: "csharp-base", title: "C# Base" },
  { id: "unity-base", title: "Unity Base" },
  { id: "aspnet-mvp", title: "ASP.NET MVP" },
];

function DemoPage() {
  // union type for UI state
  const [state, setState] = useState<UiState>("loading");

  // typed state for data
  const [courses, setCourses] = useState<Course[]>([]);
  const [selected, setSelected] = useState<Course | null>(null);

  // events (ChangeEvent)
  const [query, setQuery] = useState("");
  const debouncedQuery = useDebouncedValue(query, 250);

  // useEffect deps + cleanup (simulated fetch)
  useEffect(() => {
    setState("loading");

    const id = window.setTimeout(() => {
      // simulate "error" sometimes just to show the pattern
      const shouldFail = false;

      if (shouldFail) {
        setState("error");
        return;
      }

      setCourses(ALL_COURSES);

      if (ALL_COURSES.length === 0) {
        setState("empty");
      } else {
        setState("default");
      }
    }, 350);

    return () => window.clearTimeout(id);
  }, []); // deps are correct: runs once like "componentDidMount"

  const filtered = useMemo(() => {
    const q = debouncedQuery.trim().toLowerCase();
    if (q.length === 0) return courses;
    return courses.filter((c) => c.title.toLowerCase().includes(q));
  }, [courses, debouncedQuery]);

  const onRetry = () => {
    // mouse event typing example
    setState("loading");
    // simplest retry: re-run the same loading logic
    setCourses(ALL_COURSES);
    setState(ALL_COURSES.length === 0 ? "empty" : "default");
  };

  return (
    <PageShell title="React + TypeScript — practical minimum" state={state} errorText="Failed to load courses." onRetry={onRetry}>
      <div style={{ display: "grid", gap: 12, maxWidth: 520 }}>
        <label style={{ display: "grid", gap: 6 }}>
          <div style={{ fontSize: 12, opacity: 0.8 }}>Search</div>
          <input
            value={query}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setQuery(e.target.value)}
            placeholder="Type to filter…"
            style={{
              padding: "8px 10px",
              borderRadius: 10,
              border: "1px solid #444",
              background: "transparent",
              color: "inherit",
            }}
          />
        </label>

        <div style={{ fontSize: 12, opacity: 0.8 }}>
          Debounced query: <span style={{ opacity: 1 }}>{debouncedQuery || "—"}</span>
        </div>

        <Select<Course>
          items={filtered}
          value={selected}
          getKey={(c) => c.id}
          render={(c) => (
            <div style={{ display: "grid" }}>
              <span style={{ fontWeight: 600 }}>{c.title}</span>
              <span style={{ fontSize: 12, opacity: 0.75 }}>{c.id}</span>
            </div>
          )}
          onSelect={(c) => setSelected(c)}
        />

        <div style={{ paddingTop: 6 }}>
          <button
            type="button"
            onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
              e.preventDefault();
              setSelected(null);
            }}
            disabled={selected == null}
            style={{
              padding: "8px 10px",
              borderRadius: 10,
              border: "1px solid #444",
              background: "transparent",
              color: "inherit",
              cursor: selected == null ? "not-allowed" : "pointer",
              opacity: selected == null ? 0.6 : 1,
            }}
          >
            Clear selection
          </button>
        </div>

        <div style={{ fontSize: 14, lineHeight: 1.35 }}>
          <div style={{ opacity: 0.8 }}>Selected:</div>
          <div>{selected ? `${selected.title} (${selected.id})` : "—"}</div>
        </div>
      </div>
    </PageShell>
  );
}

export default DemoPage;
