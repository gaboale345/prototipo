import { defineStore } from 'pinia'
import api from '../services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('ecowash_token') || '',
    usuario: JSON.parse(localStorage.getItem('ecowash_user') || 'null')
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    rol: (state) => state.usuario?.rol || '',
    nombreUsuario: (state) => state.usuario?.nombre || ''
  },

  actions: {
    async login(email, password) {
      try {
        const res = await api.post('/Auth/login', { email, password })
        if (res.data.success) {
          this.token = res.data.data.token
          this.usuario = res.data.data
          localStorage.setItem('ecowash_token', this.token)
          localStorage.setItem('ecowash_user', JSON.stringify(this.usuario))
          return { success: true, rol: this.usuario.rol }
        }
        return { success: false, message: res.data.message }
      } catch (err) {
        return { success: false, message: err.response?.data?.message || 'Error al iniciar sesión' }
      }
    },

    async registro(data) {
      try {
        const res = await api.post('/Auth/registro', data)
        return { success: res.data.success, message: res.data.message }
      } catch (err) {
        return { success: false, message: err.response?.data?.message || 'Error en el registro' }
      }
    },

    logout() {
      this.token = ''
      this.usuario = null
      localStorage.removeItem('ecowash_token')
      localStorage.removeItem('ecowash_user')
    }
  }
})
