import { useState } from "react";
import { useNavigate } from "react-router-dom";

function Hello() {
  const navigate = useNavigate();
  const [forecast, setForecast] = useState(null);

  const logout = () => {
    sessionStorage.removeItem("token");
    navigate("/login", { replace: true });
  };

  const testApi = async () => {
    const r = await fetch("/api/WeatherForecast/Get");
    const data = await r.json();
    setForecast(data);
  };

  return (
    <div style={{ padding: 24 }}>
      <h1>Hello World!</h1>
      <p>Protected area</p>
      <button onClick={logout}>Logout</button>
      <br />
      <button onClick={testApi}>Test API</button>
      {forecast && <pre>{JSON.stringify(forecast, null, 2)}</pre>}
    </div>
  );
}

export default Hello;
