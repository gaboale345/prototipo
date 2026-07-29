<template>
  <nav class="navbar navbar-expand-lg navbar-custom px-3">
    <div class="container-fluid px-0">

      <!-- Hamburger para móvil (solo visible cuando está autenticado) -->
      <button
        v-if="authStore.isAuthenticated"
        class="btn btn-hamburger d-md-none me-2"
        @click="$emit('toggle-sidebar')"
        aria-label="Abrir menú"
      >
        <i class="bi bi-list fs-4"></i>
      </button>

      <router-link to="/" class="navbar-brand brand-logo">
        <i class="bi bi-droplet-fill text-primary"></i>
        <span class="brand-text">EcoWash Móvil</span>
      </router-link>

      <div class="d-flex align-items-center gap-2 ms-auto">
        <!-- Botón Modo Oscuro -->
        <button
          @click="themeStore.toggleTheme"
          class="btn btn-theme-toggle rounded-circle border-0"
          :title="themeStore.isDark ? 'Cambiar a Modo Claro' : 'Cambiar a Modo Oscuro'"
        >
          <i v-if="themeStore.isDark" class="bi bi-sun-fill text-warning fs-5"></i>
          <i v-else class="bi bi-moon-stars-fill text-secondary fs-5"></i>
        </button>

        <!-- Autenticado -->
        <div class="d-flex align-items-center gap-2" v-if="authStore.isAuthenticated">
          <!-- Notificaciones -->
          <div class="dropdown">
            <button class="btn btn-notif rounded-circle position-relative border-0" data-bs-toggle="dropdown">
              <i class="bi bi-bell-fill fs-5 text-secondary"></i>
              <span v-if="notificaciones.length > 0" class="position-absolute top-0 start-100 translate-middle p-1 bg-danger border border-light rounded-circle"></span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end shadow-sm notif-dropdown">
              <li class="dropdown-header font-semibold">Notificaciones</li>
              <li><hr class="dropdown-divider"></li>
              <li v-if="notificaciones.length === 0" class="dropdown-item text-muted text-center py-3">Sin notificaciones</li>
              <li v-for="n in notificaciones" :key="n.id" class="dropdown-item py-2">
                <div class="fw-bold small">{{ n.titulo }}</div>
                <div class="text-muted extra-small">{{ n.mensaje }}</div>
              </li>
            </ul>
          </div>

          <!-- Usuario -->
          <div class="dropdown">
            <button class="btn btn-user dropdown-toggle d-flex align-items-center gap-2 rounded-pill px-2 px-md-3" data-bs-toggle="dropdown">
              <i class="bi bi-person-circle fs-5"></i>
              <span class="user-name d-none d-md-inline">{{ authStore.nombreUsuario }}</span>
              <span class="badge bg-primary ms-1 d-none d-sm-inline">{{ authStore.rol }}</span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end shadow-sm">
              <li><router-link to="/perfil" class="dropdown-item"><i class="bi bi-person me-2"></i>Mi Perfil</router-link></li>
              <li><hr class="dropdown-divider"></li>
              <li><button @click="logout" class="dropdown-item text-danger"><i class="bi bi-box-arrow-right me-2"></i>Cerrar Sesión</button></li>
            </ul>
          </div>
        </div>

        <!-- No autenticado -->
        <div class="d-flex gap-2" v-else>
          <router-link to="/login" class="btn btn-outline-primary rounded-pill px-3 btn-sm-mobile">Iniciar Sesión</router-link>
          <router-link to="/registro" class="btn btn-primary-custom rounded-pill px-3 btn-sm-mobile d-none d-sm-inline-flex">Registrarse</router-link>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../stores/authStore'
import { useThemeStore } from '../stores/themeStore'
import { useRouter } from 'vue-router'
import api from '../services/api'

defineEmits(['toggle-sidebar'])

const authStore = useAuthStore()
const themeStore = useThemeStore()
const router = useRouter()
const notificaciones = ref([])

onMounted(async () => {
  if (authStore.isAuthenticated) {
    try {
      const res = await api.get('/Notificacion')
      if (res.data.success) notificaciones.value = res.data.data.slice(0, 5)
    } catch (e) {}
  }
})

const logout = () => {
  authStore.logout()
  router.push('/')
}
</script>
