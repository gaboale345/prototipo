<template>
  <div class="row justify-content-center align-items-center min-vh-75">
    <div class="col-md-6 col-lg-5">
      <div class="ecowash-card p-4 shadow-lg border-0">
        <div class="text-center mb-4">
          <h3 class="fw-bold">Crea tu cuenta</h3>
          <p class="text-muted small">Registrate para solicitar lavados de auto a domicilio</p>
        </div>

        <div v-if="error" class="alert alert-danger py-2 small">{{ error }}</div>
        <div v-if="success" class="alert alert-success py-2 small">{{ success }}</div>

        <form @submit.prevent="handleRegister">
          <div class="row g-2 mb-3">
            <div class="col-6">
              <label class="form-label fw-semibold">Nombre</label>
              <input type="text" v-model="form.nombre" class="form-control" required />
            </div>
            <div class="col-6">
              <label class="form-label fw-semibold">Apellido</label>
              <input type="text" v-model="form.apellido" class="form-control" required />
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label fw-semibold">Correo Electrónico</label>
            <input type="email" v-model="form.email" class="form-control" placeholder="ejemplo@ecowash.bo" required />
          </div>

          <div class="mb-3">
            <label class="form-label fw-semibold">Teléfono / WhatsApp</label>
            <input type="text" v-model="form.telefono" class="form-control" placeholder="+591 77000000" />
          </div>

          <div class="mb-4">
            <label class="form-label fw-semibold">Contraseña</label>
            <input type="password" v-model="form.password" class="form-control" placeholder="Mínimo 8 caracteres, mayúscula, minúscula y número" required />
          </div>

          <button type="submit" class="btn btn-primary-custom w-100 py-2 mb-3" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            Crear Mi Cuenta
          </button>

          <div class="text-center">
            <span class="small text-muted">¿Ya tienes cuenta? </span>
            <router-link to="/login" class="small fw-bold text-decoration-none">Inicia sesión</router-link>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '../../stores/authStore'
import { useRouter } from 'vue-router'

const form = ref({
  nombre: '',
  apellido: '',
  email: '',
  telefono: '',
  password: ''
})

const error = ref('')
const success = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

const handleRegister = async () => {
  error.value = ''
  success.value = ''
  loading.value = true

  const res = await authStore.registro(form.value)
  loading.value = false

  if (res.success) {
    success.value = '¡Cuenta registrada con éxito! Te enviamos un código de verificación.'
    localStorage.setItem('unverified_email', form.value.email)
    setTimeout(() => {
      router.push({ path: '/verify-email', query: { email: form.value.email } })
    }, 1500)
  } else {
    error.value = res.message
  }
}
</script>
