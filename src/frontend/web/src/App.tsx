import { BrowserRouter, Routes, Route, useNavigate, useLocation, Navigate } from 'react-router-dom';
import Protected from './components/Protected';
import Hello from './Hello';
import "./App.css";
import ManTineTest from '@/pages/ManTineTest';
import DemoPage from "@/pages/DemoPage";

//Mantine Provider & styles
import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';

function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const next = (location.state as { from?: Location })?.from?.pathname || '/';

  const setAndGo = () => {
    sessionStorage.setItem('token', '123');
    navigate(next, { replace: true });
  };

  return (
    <div className='center-page'>
      <h2>Login (stub)</h2>
      <p>Protected page: http://localhost:5173/</p>
      <p>Login page: http://localhost:5173/login</p>
      <div >
        <button onClick={setAndGo}>Pass Login &amp; go</button>
      </div>
    </div>
  );
}

export default function App() {
  return (
    <MantineProvider
      theme={{
        fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial",
      }}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<Protected> <Hello /></Protected>} />
          <Route path="*" element={<Navigate to="/login" replace />} />
          <Route path="/mantinetest" element={<ManTineTest />} />
          <Route path="/test" element={<DemoPage />} />
        </Routes>
      </BrowserRouter>
    </MantineProvider>
  );
}
