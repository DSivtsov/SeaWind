import React, { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { TestPageShell } from "@/pages/demo/shared/TestPageShell";
import type { UiState } from "@/shared/types/UiState";

type CourseListItem = {
  id: string;
  title: string;
  description: string;
};

const DEMO_COURSES: CourseListItem[] = [
  { id: "csharp-base", title: "C# Base", description: "Basics of C# for game/web development." },
  { id: "unity-base", title: "Unity Base", description: "Unity fundamentals: scenes, prefabs, components." },
  { id: "aspnet-mvp", title: "ASP.NET MVP", description: "Build WorkshopCode MVP backend with .NET 8." },
];

export default function TestCourseListPage() {
  const navigate = useNavigate();

  const [state, setState] = useState<UiState>("loading");
  const [errorText, setErrorText] = useState<string | undefined>(undefined);
  const [courses, setCourses] = useState<CourseListItem[]>([]);
  const [query, setQuery] = useState("");

  const load = () => {
    setState("loading");
    setErrorText(undefined);

    // MVP: fake fetch (replace later with real GET /api/courses)
    window.setTimeout(() => {
      const shouldFail = false;

      if (shouldFail) {
        setState("error");
        setErrorText("Failed to load courses.");
        return;
      }

      const data = DEMO_COURSES;
      setCourses(data);
      setState(data.length === 0 ? "empty" : "ready");
    }, 250);
  };

  useEffect(() => {
    load();
    // no cleanup needed here because we don't store the timeout id (keep it simple for MVP)
  }, []);

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    if (q.length === 0) return courses;
    return courses.filter((c) => c.title.toLowerCase().includes(q) || c.description.toLowerCase().includes(q));
  }, [courses, query]);

  const contentState: UiState =
    state === "ready" && filtered.length === 0 ? "empty" : state;

  return (
    <TestPageShell title="Courses" state={contentState} errorText={errorText} onRetry={load}>
      <div style={{ display: "grid", gap: 12, maxWidth: 720 }}>
        <input
          value={query}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => setQuery(e.target.value)}
          placeholder="Search…"
          style={{ padding: "8px 10px", borderRadius: 10, border: "1px solid #444", background: "transparent", color: "inherit" }}
        />

        <div style={{ display: "grid", gap: 10 }}>
          {filtered.map((c) => (
            <button
              key={c.id}
              type="button"
              onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
                e.preventDefault();
                // MVP: go into course tabs (lectures by default)
                navigate(`/courses/${c.id}/lectures`);
              }}
            >
              <div style={{ fontWeight: 700 }}>{c.title}</div>
              <div style={{ opacity: 0.8 }}>{c.description}</div>
            </button>
          ))}
        </div>
      </div>
    </TestPageShell>
  );
}
