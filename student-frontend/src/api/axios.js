import axios from 'axios'

const api = axios.create({
  baseURL: '/',
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const isLoggedIn = !!localStorage.getItem('token')
    const isNetworkError = !error.response        // server stopped / unreachable
    const isUnauthorized = error.response?.status === 401  // token expired
    if (isLoggedIn && (isNetworkError || isUnauthorized)) {
      window.dispatchEvent(new Event('auth:logout'))
    }
    return Promise.reject(error)
  }
)

export default api
