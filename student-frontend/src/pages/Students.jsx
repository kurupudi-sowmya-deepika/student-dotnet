import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import api from '../api/axios'

const emptyForm = { name: '', age: '', course: '' }

export default function Students() {
  const { isAdmin } = useAuth()
  const [students, setStudents] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState(emptyForm)
  const [editId, setEditId] = useState(null)
  const [saving, setSaving] = useState(false)

  const fetchStudents = async () => {
    try {
      const { data } = await api.get('/student')
      setStudents(data)
      setError('')
    } catch {
      setError('Failed to fetch students.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchStudents() }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    setSaving(true)
    setError('')
    try {
      const payload = { ...form, age: parseInt(form.age, 10) }
      if (editId !== null) {
        await api.put(`/student?id=${editId}`, { ...payload, id: editId })
      } else {
        await api.post('/student', payload)
      }
      await fetchStudents()
      resetForm()
    } catch {
      setError('Failed to save student.')
    } finally {
      setSaving(false)
    }
  }

  const handleEdit = (student) => {
    setForm({ name: student.name, age: String(student.age), course: student.course })
    setEditId(student.id)
    setShowForm(true)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this student?')) return
    setError('')
    try {
      await api.delete(`/student?id=${id}`)
      setStudents((prev) => prev.filter((s) => s.id !== id))
    } catch {
      setError('Failed to delete student.')
    }
  }

  const resetForm = () => {
    setShowForm(false)
    setForm(emptyForm)
    setEditId(null)
  }

  if (loading) return <div className="loading">Loading students…</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h2>Students</h2>
        {isAdmin && !showForm && (
          <button className="btn btn-primary" onClick={() => setShowForm(true)}>+ Add Student</button>
        )}
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {showForm && (
        <div className="form-card">
          <h3>{editId !== null ? 'Edit Student' : 'New Student'}</h3>
          <form onSubmit={handleSubmit}>
            <div className="form-row">
              <div className="form-group">
                <label>Name</label>
                <input
                  type="text"
                  placeholder="Full name"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label>Age</label>
                <input
                  type="number"
                  placeholder="e.g. 21"
                  value={form.age}
                  min={1}
                  max={100}
                  onChange={(e) => setForm({ ...form, age: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label>Course</label>
                <input
                  type="text"
                  placeholder="e.g. Computer Science"
                  value={form.course}
                  onChange={(e) => setForm({ ...form, course: e.target.value })}
                  required
                />
              </div>
            </div>
            <div className="form-actions">
              <button type="submit" className="btn btn-primary" disabled={saving}>
                {saving ? 'Saving…' : editId !== null ? 'Update' : 'Add'}
              </button>
              <button type="button" className="btn btn-outline" onClick={resetForm}>Cancel</button>
            </div>
          </form>
        </div>
      )}

      {students.length === 0 ? (
        <p className="empty-state">No students found.{isAdmin ? ' Add one above!' : ''}</p>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>Age</th>
                <th>Course</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {students.map((s) => (
                <tr key={s.id}>
                  <td>{s.id}</td>
                  <td>{s.name}</td>
                  <td>{s.age}</td>
                  <td>{s.course}</td>
                  {isAdmin && (
                    <td className="row-actions">
                      <button className="btn btn-sm btn-secondary" onClick={() => handleEdit(s)}>Edit</button>
                      <button className="btn btn-sm btn-danger" onClick={() => handleDelete(s.id)}>Delete</button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
