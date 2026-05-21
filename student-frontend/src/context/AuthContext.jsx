import { createContext, useContext, useState, useEffect } from 'react'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [token, setToken] = useState(() => localStorage.getItem('token'))

  useEffect(() => {
    const storedUser = localStorage.getItem('user')
    if (storedUser && token) {
      setUser(JSON.parse(storedUser))
    }
  }, [token])

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

export const useAuth = () => useContext(AuthContext)
