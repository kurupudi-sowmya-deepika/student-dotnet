import { useState } from 'react'
import { AuthContext } from './AuthContext'

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem('token'))
  const [user, setUser] = useState(() => {
    const storedUser = localStorage.getItem('user')
    return storedUser ? JSON.parse(storedUser) : null
  })

  const login = (authResponse) => {
    const userData = {
      username: authResponse.username,
      email: authResponse.email,
      role: authResponse.role,
    }
    localStorage.setItem('token', authResponse.token)
    localStorage.setItem('user', JSON.stringify(userData))
    setToken(authResponse.token)
    setUser(userData)
  }

  const logout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    setToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isAdmin: user?.role === 'Admin' }}>
      {children}
    </AuthContext.Provider>
  )
}
