import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import api from '../api/axios'

export default function Users() {
  const { user, isAdmin } = useAuth()
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [updatingId, setUpdatingId] = useState(null)

  const fetchUsers = async () => {
    try {
      const { data } = await api.get('/auth/users')
      setUsers(data)
      setError('')
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch users.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (isAdmin) {
      fetchUsers()
    } else {
      setLoading(false)
    }
  }, [isAdmin])

  const handleRoleChange = async (userId, newRole, username) => {
    setUpdatingId(userId)
    setError('')
    setSuccessMessage('')
    try {
      await api.put('/auth/update-role', { userId, role: newRole })
      
      // Update state locally
      setUsers((prev) =>
        prev.map((u) => (u.id === userId ? { ...u, role: newRole } : u))
      )
      setSuccessMessage(`Successfully updated role for ${username} to ${newRole}.`)
      
      // Automatically clear success message after 4 seconds
      setTimeout(() => setSuccessMessage(''), 4000)
    } catch (err) {
      setError(err.response?.data?.message || `Failed to update role for ${username}.`)
    } finally {
      setUpdatingId(null)
    }
  }

  if (!isAdmin) {
    return (
      <div className="page-container">
        <div className="alert alert-error">
          <h3>Access Denied</h3>
          <p>You do not have administrative privileges to access this page.</p>
        </div>
      </div>
    )
  }

  if (loading) return <div className="loading">Loading users access control…</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h2>User Role Administration</h2>
      </div>

      {error && <div className="alert alert-error">{error}</div>}
      {successMessage && <div className="alert alert-success">{successMessage}</div>}

      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>#</th>
              <th>Username</th>
              <th>Email</th>
              <th>Current Role</th>
              <th>Change Role</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td>{u.id}</td>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>
                  <span className={`user-chip ${u.role === 'Admin' ? 'chip-admin' : 'chip-user'}`}>
                    {u.role}
                  </span>
                </td>
                <td>
                  <select
                    className="select-role"
                    value={u.role}
                    disabled={updatingId === u.id || u.email === user?.email}
                    onChange={(e) => handleRoleChange(u.id, e.target.value, u.username)}
                  >
                    <option value="User">User</option>
                    <option value="Admin">Admin</option>
                  </select>
                  {u.email === user?.email && (
                    <span className="self-warning-text">(You cannot change your own role)</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
