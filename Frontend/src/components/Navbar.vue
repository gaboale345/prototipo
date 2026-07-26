<template>
  <nav class="navbar navbar-expand-lg navbar-custom px-4">
    <div class="container-fluid">
      <router-link to="/" class="navbar-brand brand-logo">
        <i class="bi bi-droplet-fill text-primary"></i> EcoWash Móvil
      </router-link>

      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarText">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarText">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0"></ul>

        <div class="d-flex align-items-center gap-3" v-if="authStore.isAuthenticated">
          <div class="dropdown">
            <button class="btn btn-light rounded-circle position-relative dropdown-toggle border-0" data-bs-toggle="dropdown">
              <i class="bi bi-bell-fill fs-5 text-secondary"></i>
              <span v-if="notificaciones.length > 0" class="position-absolute top-0 start-100 translate-middle p-1 bg-danger border border-light rounded-circle"></span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end shadow-sm" style="width: 320px;">
              <li class="dropdown-header font-semibold">Notificaciones</li>
              <li><hr class="dropdown-divider"></li>
              <li v-if="notificaciones.length === 0" class="dropdown-item text-muted text-center py-3">Sin notificaciones</li>
              <li v-for="n in notificaciones" :key="n.id" class="dropdown-item py-2">
                <div class="fw-bold small">{{ n.titulo }}</div>
                <div class="text-muted extra-small">{{ n.mensaje }}</div>
              </li>
            </ul>
          </div>

          <div class="dropdown">
            <button class="btn btn-outline-primary dropdown-toggle d-flex align-items-center gap-2 rounded-pill px-3" data-bs-toggle="dropdown">
              <i class="bi bi-person-circle fs-5"></i>
              <span>{{ authStore.nombreUsuario }}</span>
              <span class="badge bg-primary ms-1">{{ authStore.rol }}</span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end shadow-sm">
              <li><router-link to="/perfil" class="dropdown-item"><i class="bi bi-person me-2"></i>Mi Perfil</router-link></li>
              <li><hr class="dropdown-divider"></li>
              <li><button @click="logout" class="dropdown-item text-danger"><i class="bi bi-box-arrow-right me-2"></i>Cerrar Sesión</button></li>
            </ul>
          </div>
        </div>

        <div class="d-flex gap-2" v-else>
          <router-link to="/login" class="btn btn-outline-primary rounded-pill px-4">Iniciar Sesión</router-link>
          <router-link to="/registro" class="btn btn-primary-custom rounded-pill px-4">Registrarse</router-link>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../stores/authStore'
import { useRouter } from 'vue-router'
import api from '../services/api'

const authStore = useAuthStore()
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
  router.push('/login')
}
</script>
