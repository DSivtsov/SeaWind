import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import "./App.css"

interface Forecast {
  date: string
  temperatureC: number
  temperatureF: number
  summary: string
}

export default function Hello() {
  const navigate = useNavigate()
  const [forecast, setForecast] = useState<Forecast[] | null>(null)

  const logout = () => {
    sessionStorage.removeItem('token')
    navigate('/login', { replace: true })
  }

  const testApi = async () => {
    try {
      const r = await fetch('/api/Courses')
      if (!r.ok) throw new Error(`HTTP ${r.status}`)
      const data: Forecast[] = await r.json()
      setForecast(data)
    } catch (e) {
      console.error(e)
    }
  }

  return (
    <div className="hello-page">
      <h1>Hello World!</h1>
      <p>Protected area</p>
      <button onClick={logout}>Logout</button>
      <br />
      <br />
      <button onClick={testApi}>Test API</button>
      {forecast && <pre>{JSON.stringify(forecast, null, 2)}</pre>}
    </div>
  )
}
