<template>
  <div class="row justify-content-center align-items-center min-vh-75">
    <div class="col-md-5 col-lg-4">
      <div class="ecowash-card p-4 shadow-lg border-0">
        <div class="text-center mb-4">
          <div class="d-inline-flex p-3 rounded-circle bg-primary bg-opacity-10 mb-2">
            <i class="bi bi-droplet-fill text-primary fs-1"></i>
          </div>
          <h3 class="fw-bold">EcoWash Móvil</h3>
          <p class="text-muted small">Inicia sesión en tu cuenta de lavado a domicilio</p>
        </div>

        <div v-if="error" class="alert alert-danger py-2 small">{{ error }}</div>

        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label class="form-label fw-semibold">Correo Electrónico</label>
            <div class="input-group">
              <span class="input-group-text bg-white text-muted"><i class="bi bi-envelope"></i></span>
              <input type="email" v-model="email" class="form-control" placeholder="ejemplo@ecowash.bo" required />
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label fw-semibold">Contraseña</label>
            <div class="input-group">
              <span class="input-group-text bg-white text-muted"><i class="bi bi-lock"></i></span>
              <input type="password" v-model="password" class="form-control" placeholder="••••••••" required />
            </div>
          </div>

          <div class="d-flex justify-content-between align-items-center mb-4">
            <div class="form-check">
              <input type="checkbox" class="form-check-input" id="remember" />
              <label class="form-check-label small" for="remember">Recordarme</label>
            </div>
            <router-link to="/forgot-password" class="small text-decoration-none">¿Olvidaste tu contraseña?</router-link>
          </div>

          <button type="submit" class="btn btn-primary-custom w-100 py-2 mb-3" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            Iniciar Sesión
          </button>

          <div class="text-center">
            <span class="small text-muted">¿No tienes una cuenta? </span>
            <router-link to="/registro" class="small fw-bold text-decoration-none">Regístrate gratis</router-link>
          </div>
        </form>

        <div class="mt-4 pt-3 border-top">
          <div class="small text-center text-muted mb-2">Cuentas demo de prueba:</div>
          <div class="d-flex justify-content-center gap-1">
            <button @click="fillDemo('admin@ecowash.bo', 'Admin@1234')" class="btn btn-outline-secondary btn-sm">Admin</button>
            <button @click="fillDemo('cliente@ecowash.bo', 'Cliente@1234')" class="btn btn-outline-secondary btn-sm">Cliente</button>
            <button @click="fillDemo('empleado@ecowash.bo', 'Empleado@1234')" class="btn btn-outline-secondary btn-sm">Empleado</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '../../stores/authStore'
import { useRouter } from 'vue-router'

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

const fillDemo = (e, p) => {
  email.value = e
  password.value = p
}

const handleLogin = async () => {
  error.value = ''
  loading.value = true
  const res = await authStore.login(email.value, password.value)
  loading.value = false

  if (res.success) {
    if (res.rol === 'Administrador') router.push('/admin/dashboard')
    else if (res.rol === 'Empleado') router.push('/empleado/dashboard')
    else router.push('/cliente/dashboard')
  } else {
    error.value = res.message
  }
}
</script>
