import { Navigate, useLocation } from 'react-router-dom'
import { type ReactNode } from 'react'

interface ProtectedProps {
  children: ReactNode
}

export default function Protected({ children }: ProtectedProps) {
  const token = sessionStorage.getItem('token')
  const location = useLocation()

  if (!token) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  return children
}
