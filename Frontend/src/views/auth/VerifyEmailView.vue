<template>
  <div class="auth-wrapper d-flex align-items-center justify-content-center min-vh-100">
    <div class="card glass-card p-4 p-md-5 shadow-lg border-0" style="max-width: 480px; width: 100%;">
      <div class="text-center mb-4">
        <div class="brand-badge mb-3">
          <i class="bi bi-shield-lock-fill me-2 fs-4"></i>
          <span class="fw-bold fs-5">EcoWash Direct</span>
        </div>
        <h3 class="fw-bold text-gradient mb-2">Verifica tu Correo</h3>
        <p class="text-muted small">
          Hemos enviado un código OTP de 6 dígitos al correo: <br>
          <strong class="text-emerald">{{ email }}</strong>
        </p>
      </div>

      <!-- Alerta de mensaje/error -->
      <div v-if="mensaje" :class="['alert mb-4 alert-dismissible fade show', isError ? 'alert-danger' : 'alert-success']" role="alert">
        <i :class="isError ? 'bi bi-exclamation-triangle-fill me-2' : 'bi bi-check-circle-fill me-2'"></i>
        {{ mensaje }}
      </div>

      <form @submit.prevent="handleVerify">
        <!-- Inputs para los 6 dígitos del OTP -->
        <div class="d-flex justify-content-between mb-4 gap-2">
          <input
            v-for="(digit, index) in otp"
            :key="index"
            :id="'otp-input-' + index"
            type="text"
            maxlength="1"
            class="form-control text-center otp-box fw-bold fs-4"
            v-model="otp[index]"
            @input="onDigitInput(index, $event)"
            @keydown.delete="onDigitDelete(index, $event)"
            @paste="onPaste"
            autocomplete="off"
          />
        </div>

        <button type="submit" class="btn btn-emerald w-100 py-3 mb-3 fw-bold rounded-3 shadow-sm" :disabled="loading || !isOtpComplete">
          <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
          <i v-else class="bi bi-patch-check-fill me-2"></i>
          Verificar Correo
        </button>
      </form>

      <div class="text-center mt-3 border-top pt-3">
        <p class="text-muted small mb-2">¿No recibiste el código?</p>
        <button
          type="button"
          class="btn btn-outline-emerald btn-sm rounded-pill px-4"
          :disabled="resendCooldown > 0 || resending"
          @click="handleResend"
        >
          <span v-if="resending" class="spinner-border spinner-border-sm me-1"></span>
          <i v-else class="bi bi-arrow-clockwise me-1"></i>
          <span v-if="resendCooldown > 0">Reenviar en {{ resendCooldown }}s</span>
          <span v-else>Reenviar Código</span>
        </button>
      </div>

      <div class="text-center mt-4">
        <router-link to="/login" class="text-decoration-none text-muted small">
          <i class="bi bi-arrow-left me-1"></i> Volver al inicio de sesión
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../../services/api'

const route = useRoute()
const router = useRouter()

const email = ref(route.query.email || localStorage.getItem('unverified_email') || '')
const otp = ref(['', '', '', '', '', ''])
const loading = ref(false)
const resending = ref(false)
const mensaje = ref('')
const isError = ref(false)
const resendCooldown = ref(0)
let timer = null

const isOtpComplete = computed(() => otp.value.join('').length === 6)

onMounted(() => {
  if (!email.value) {
    router.push('/login')
  }
})

const onDigitInput = (index, event) => {
  const val = event.target.value
  if (val && index < 5) {
    const nextInput = document.getElementById(`otp-input-${index + 1}`)
    if (nextInput) nextInput.focus()
  }
}

const onDigitDelete = (index, event) => {
  if (!otp.value[index] && index > 0) {
    const prevInput = document.getElementById(`otp-input-${index - 1}`)
    if (prevInput) prevInput.focus()
  }
}

const onPaste = (event) => {
  event.preventDefault()
  const pasted = event.clipboardData.getData('text').trim()
  if (/^\d{6}$/.test(pasted)) {
    for (let i = 0; i < 6; i++) {
      otp.value[i] = pasted[i]
    }
  }
}

const handleVerify = async () => {
  if (!isOtpComplete.value) return
  loading.value = true
  mensaje.value = ''
  isError.value = false

  try {
    const code = otp.value.join('')
    const res = await api.post('/EmailVerification/verify', {
      email: email.value,
      codigo: code
    })

    if (res.data.success) {
      mensaje.value = res.data.message || 'Correo verificado con éxito'
      isError.value = false
      localStorage.removeItem('unverified_email')
      setTimeout(() => {
        router.push('/login')
      }, 1500)
    } else {
      mensaje.value = res.data.message || 'Código incorrecto'
      isError.value = true
    }
  } catch (err) {
    mensaje.value = err.response?.data?.message || 'Error al verificar el código'
    isError.value = true
  } finally {
    loading.value = false
  }
}

const handleResend = async () => {
  if (resendCooldown.value > 0 || resending.value) return
  resending.value = true
  mensaje.value = ''

  try {
    const res = await api.post('/EmailVerification/resend', { email: email.value })
    if (res.data.success) {
      mensaje.value = res.data.message || 'Se ha enviado un nuevo código'
      isError.value = false
      startCooldown(60)
    } else {
      mensaje.value = res.data.message
      isError.value = true
    }
  } catch (err) {
    mensaje.value = err.response?.data?.message || 'Error al reenviar el código'
    isError.value = true
  } finally {
    resending.value = false
  }
}

const startCooldown = (seconds) => {
  resendCooldown.value = seconds
  clearInterval(timer)
  timer = setInterval(() => {
    resendCooldown.value--
    if (resendCooldown.value <= 0) {
      clearInterval(timer)
    }
  }, 1000)
}
</script>

<style scoped>
.auth-wrapper {
  background: linear-gradient(135deg, #0f2027 0%, #203a43 50%, #2c5364 100%);
  padding: 1.5rem;
}

.glass-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(16px);
  border-radius: 20px;
}

.text-gradient {
  background: linear-gradient(45deg, #1b4332, #2d6a4f);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.text-emerald {
  color: #2d6a4f;
}

.btn-emerald {
  background: linear-gradient(135deg, #2d6a4f 0%, #40916c 100%);
  color: white;
  border: none;
  transition: all 0.3s ease;
}

.btn-emerald:hover:not(:disabled) {
  background: linear-gradient(135deg, #1b4332 0%, #2d6a4f 100%);
  transform: translateY(-2px);
}

.btn-outline-emerald {
  border-color: #2d6a4f;
  color: #2d6a4f;
}

.btn-outline-emerald:hover:not(:disabled) {
  background-color: #2d6a4f;
  color: white;
}

.otp-box {
  width: 50px;
  height: 60px;
  border-radius: 12px;
  border: 2px solid #ced4da;
  transition: all 0.2s ease;
}

.otp-box:focus {
  border-color: #2d6a4f;
  box-shadow: 0 0 0 0.25rem rgba(45, 106, 79, 0.25);
}
</style>
