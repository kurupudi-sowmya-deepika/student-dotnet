import { useState } from 'react'
import { Link } from 'react-router-dom'
import api from '../api/axios'

export default function ForgotPassword() {
  const [form, setForm] = useState({ email: '', newPassword: '', confirmPassword: '' })
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    setSuccess('')

    if (form.newPassword !== form.confirmPassword) {
      setError('Passwords do not match.')
      setLoading(false)
      return
    }

    const email = form.email.trim().toLowerCase()
    if (!email.endsWith('@intuceo.com')) {
      setError('Email must belong to the @intuceo.com domain.')
      setLoading(false)
      return
    }

    try {
      const response = await api.post('/auth/forgot-password', {
        email: form.email,
        newPassword: form.newPassword
      })
      setSuccess(response.data?.message || 'Password updated successfully.')
      setForm({ email: '', newPassword: '', confirmPassword: '' })
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to update password. User may not exist.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Reset your password</h2>
        <p className="auth-subtitle">Enter your email and a new password</p>
        
        {error && <div className="alert alert-error">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              placeholder="you@example.com"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              required
            />
          </div>
          <div className="form-group">
            <label>New Password</label>
            <input
              type="password"
              placeholder="••••••••"
              value={form.newPassword}
              onChange={(e) => setForm({ ...form, newPassword: e.target.value })}
              required
              minLength={6}
            />
          </div>
          <div className="form-group">
            <label>Confirm New Password</label>
            <input
              type="password"
              placeholder="••••••••"
              value={form.confirmPassword}
              onChange={(e) => setForm({ ...form, confirmPassword: e.target.value })}
              required
              minLength={6}
            />
          </div>
          <button type="submit" className="btn btn-primary btn-full" disabled={loading}>
            {loading ? 'Updating password…' : 'Reset Password'}
          </button>
        </form>
        <p className="auth-footer">
          Remember your password? <Link to="/login">Sign in here</Link>
        </p>
      </div>
    </div>
  )
}
