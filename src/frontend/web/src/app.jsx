import { BrowserRouter, Routes, Route, useNavigate, useLocation } from "react-router-dom";
import Protected from "./components/Protected";
import Hello from "./Hello";

function Login() {
  const navigate = useNavigate();
  const { state } = useLocation();
  const next = state?.from?.pathname || "/";

  const setAndGo = () => {
    sessionStorage.setItem("token", "123");
    navigate(next, { replace: true });
  };

  return (
    <div style={{ padding: 24, textAlign: "center" }}>
      <h2>Login (stub)</h2>
            <p>Protected page : http://localhost:5173/ </p>
            <p>Login page: http://localhost:5173/login </p>
      <div style={{ display: "flex", gap: 12, justifyContent: "center" }}>
        <button onClick={setAndGo}>Pass Login &amp; go</button>
      </div>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<Protected><Hello /></Protected>} />
      </Routes>
    </BrowserRouter>
  );
}
