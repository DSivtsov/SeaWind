import { Link, Outlet } from "react-router-dom";

export function TestCoursesLayout() {
  return (
    <div style={{ minHeight: "100vh", fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial" }}>
      <header style={{ padding: 16, borderBottom: "1px solid #444", display: "flex", gap: 12, alignItems: "center" }}>
        <Link to="/courses" style={{ color: "inherit", textDecoration: "none", fontWeight: 700 }}>
          WorkshopCode
        </Link>

        <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
          <button type="button">Login</button>
          <button type="button">Register</button>
        </div>
      </header>

      <main>
        <Outlet />
      </main>
    </div>
  );
}
