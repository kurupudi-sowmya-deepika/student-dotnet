import { NavLink } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Sidebar() {
  const { user } = useAuth()

  if (!user) return null

  return (
    <aside className="sidebar">
      <NavLink to="/students" className="sidebar-link">
        <span className="sidebar-icon">🎓</span>
        <span>Students</span>
      </NavLink>
      {user.role === 'Admin' && (
        <NavLink to="/users" className="sidebar-link">
          <span className="sidebar-icon">👥</span>
          <span>Manage Users</span>
        </NavLink>
      )}
    </aside>
  )
}
