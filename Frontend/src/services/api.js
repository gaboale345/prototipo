import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5275/api', // URL donde corre la API C#
  headers: {
    'Content-Type': 'application/json'
  }
})

// Interceptor para adjuntar el Token JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('ecowash_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config;
}, (error) => Promise.reject(error))

export default api
