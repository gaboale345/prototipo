<template>
  <div class="container py-5 d-flex align-items-center justify-content-center min-vh-100">
    <div class="card border-0 shadow-lg rounded-4 p-5 text-center" style="max-width: 550px; width: 100%;">
      <!-- Icono animado -->
      <div class="success-icon-circle mx-auto mb-4">
        <i class="bi bi-check-lg fs-1 text-white"></i>
      </div>

      <h2 class="fw-bold text-emerald mb-2">¡Pago Confirmado!</h2>
      <p class="text-muted mb-4">
        Gracias por tu pago. Tu solicitud de servicio ha sido procesada correctamente y se ha generado tu comprobante de pago.
      </p>

      <div v-if="isSandbox" class="alert alert-warning py-2 small mb-4">
        <i class="bi bi-info-circle-fill me-1"></i>
        <strong>Modo Sandbox Activo:</strong> El pago fue simulado exitosamente utilizando credenciales de pruebas.
      </div>

      <div class="p-3 bg-light rounded-3 mb-4 text-start">
        <div class="d-flex justify-content-between mb-2">
          <span class="text-muted">Estado del Pago:</span>
          <span class="badge bg-success rounded-pill px-3 py-2">Completado</span>
        </div>
        <div v-if="orderId" class="d-flex justify-content-between">
          <span class="text-muted">ID de Orden:</span>
          <span class="fw-bold text-dark">#{{ orderId }}</span>
        </div>
      </div>

      <div class="d-grid gap-2">
        <router-link to="/cliente/pagos" class="btn btn-emerald py-3 fw-bold rounded-3">
          <i class="bi bi-wallet2 me-2"></i> Ver Mis Pagos &amp; Comprobantes
        </router-link>
        <router-link to="/cliente/dashboard" class="btn btn-outline-secondary py-2 rounded-3">
          <i class="bi bi-house me-1"></i> Volver al Inicio
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '../../services/api'

const route = useRoute()
const orderId = computed(() => route.query.order_id || '')
const sessionId = computed(() => route.query.session_id || '')
const isSandbox = computed(() => route.query.sandbox === 'true' || sessionId.value.includes('simulated'))

onMounted(async () => {
  if (sessionId.value) {
    try {
      await api.post(`/Payment/confirm-session/${sessionId.value}`)
    } catch (e) {
      console.error('Error al confirmar sesión de pago:', e)
    }
  }
})
</script>

<style scoped>
.text-emerald {
  color: #2d6a4f;
}

.btn-emerald {
  background: linear-gradient(135deg, #2d6a4f 0%, #40916c 100%);
  color: white;
  border: none;
}

.success-icon-circle {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  background: linear-gradient(135deg, #2d6a4f 0%, #52b788 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 10px 25px rgba(45, 106, 79, 0.3);
  animation: popIn 0.5s ease-out;
}

@keyframes popIn {
  0% { transform: scale(0); }
  80% { transform: scale(1.1); }
  100% { transform: scale(1); }
}
</style>
