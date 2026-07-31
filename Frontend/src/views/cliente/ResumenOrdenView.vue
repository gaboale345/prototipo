<template>
  <div class="container py-4" style="max-width: 800px;">
    <!-- Header -->
    <div class="mb-4">
      <router-link to="/cliente/catalogo" class="text-decoration-none text-muted small mb-2 d-inline-block">
        <i class="bi bi-arrow-left me-1"></i> Volver al Catálogo
      </router-link>
      <h2 class="fw-bold text-emerald mb-1">
        <i class="bi bi-receipt-cutoff me-2"></i>Resumen de tu Solicitud
      </h2>
      <p class="text-muted">Verifica el detalle de tu servicio antes de proceder al pago seguro</p>
    </div>

    <div v-if="cartItems.length === 0" class="card border-0 shadow-sm rounded-4 p-5 text-center">
      <i class="bi bi-cart-x fs-1 text-muted"></i>
      <h4 class="mt-3 text-muted">No has seleccionado ningún servicio</h4>
      <router-link to="/cliente/catalogo" class="btn btn-emerald rounded-pill mt-3 px-4">
        Ver Catálogo de Servicios
      </router-link>
    </div>

    <div v-else class="row g-4">
      <!-- Tabla de ítems seleccionados -->
      <div class="col-12 col-lg-7">
        <div class="card border-0 shadow-sm rounded-4 overflow-hidden mb-4">
          <div class="card-header bg-white py-3 border-0">
            <h5 class="fw-bold mb-0 text-emerald">Detalle de Servicios</h5>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table align-middle mb-0">
                <thead class="bg-light">
                  <tr>
                    <th class="ps-4">Servicio</th>
                    <th class="text-center">Cant.</th>
                    <th class="text-end pe-4">Subtotal</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in cartItems" :key="item.servicioId">
                    <td class="ps-4">
                      <span class="fw-bold text-dark d-block">{{ item.nombre }}</span>
                      <small class="text-muted">Bs. {{ item.precio.toFixed(2) }} c/u</small>
                    </td>
                    <td class="text-center fw-semibold">{{ item.cantidad }}</td>
                    <td class="text-end pe-4 fw-bold text-emerald">Bs. {{ (item.precio * item.cantidad).toFixed(2) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Observaciones -->
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <label class="fw-bold text-dark mb-2">Notas u Observaciones (Opcional)</label>
          <textarea
            v-model="observaciones"
            class="form-control rounded-3"
            rows="3"
            placeholder="Indicaciones especiales para el equipo de lavado..."
          ></textarea>
        </div>
      </div>

      <!-- Resumen de Costos y Método de Pago -->
      <div class="col-12 col-lg-5">
        <div class="card border-0 shadow-sm rounded-4 p-4 sticky-top" style="top: 20px;">
          <h5 class="fw-bold text-emerald mb-3">Resumen de Pago</h5>

          <div class="d-flex justify-content-between mb-2">
            <span class="text-muted">Subtotal:</span>
            <span class="fw-semibold">Bs. {{ total.toFixed(2) }}</span>
          </div>

          <div class="d-flex justify-content-between mb-3 border-bottom pb-3">
            <span class="text-muted">Impuestos incluidos:</span>
            <span class="fw-semibold text-success">Bs. 0.00</span>
          </div>

          <div class="d-flex justify-content-between mb-4">
            <span class="fw-bold fs-5">Total a Pagar:</span>
            <span class="fw-bold fs-4 text-emerald">Bs. {{ total.toFixed(2) }}</span>
          </div>

          <!-- Selección de método de pago -->
          <div class="mb-4">
            <label class="fw-bold text-dark mb-2">Método de Pago</label>
            <div class="d-grid gap-2">
              <label :class="['payment-option p-3 rounded-3 border cursor-pointer d-flex align-items-center gap-3', selectedMethod === 'card' ? 'active-option' : '']">
                <input type="radio" v-model="selectedMethod" value="card" class="form-check-input" />
                <i class="bi bi-credit-card-2-front fs-4 text-emerald"></i>
                <div>
                  <strong class="d-block text-dark">Tarjeta de Débito / Crédito</strong>
                  <small class="text-muted">Visa, Mastercard y tarjetas locales</small>
                </div>
              </label>

              <label :class="['payment-option p-3 rounded-3 border cursor-pointer d-flex align-items-center gap-3', selectedMethod === 'qr' ? 'active-option' : '']">
                <input type="radio" v-model="selectedMethod" value="qr" class="form-check-input" />
                <i class="bi bi-qr-code-scan fs-4 text-emerald"></i>
                <div>
                  <strong class="d-block text-dark">Pago por Código QR</strong>
                  <small class="text-muted">Transferencia bancaria directa por QR</small>
                </div>
              </label>
            </div>
          </div>

          <!-- Banner de Seguridad -->
          <div class="p-3 bg-light rounded-3 mb-4 d-flex align-items-center gap-2">
            <i class="bi bi-shield-check text-emerald fs-4"></i>
            <small class="text-muted">Pago seguro encriptado de 256 bits mediante pasarela bancaria oficial.</small>
          </div>

          <!-- Botón de Pago -->
          <button
            @click="processPayment"
            class="btn btn-emerald w-100 py-3 rounded-3 fw-bold shadow-sm"
            :disabled="processing"
          >
            <span v-if="processing" class="spinner-border spinner-border-sm me-2"></span>
            <i v-else class="bi bi-lock-fill me-2"></i>
            Pagar Bs. {{ total.toFixed(2) }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../../services/api'
import Swal from 'sweetalert2'

const router = useRouter()

const cartItems = ref([])
const observaciones = ref('')
const selectedMethod = ref('card')
const processing = ref(false)

onMounted(() => {
  const data = sessionStorage.getItem('pending_cart')
  if (data) {
    cartItems.value = JSON.parse(data)
  }
})

const total = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.precio * item.cantidad), 0)
})

const processPayment = async () => {
  if (cartItems.value.length === 0) return
  processing.value = true

  try {
    // Revalidar que los servicios del carrito siguen disponibles en el backend
    const catalogRes = await api.get('/Servicio')
    if (catalogRes.data.success) {
      const availableIds = new Set(catalogRes.data.data.map(s => s.id))
      const invalidItems = cartItems.value.filter(i => !availableIds.has(i.servicioId))

      if (invalidItems.length > 0) {
        const invalidNames = invalidItems.map(i => i.nombre).join(', ')
        const result = await Swal.fire({
          title: 'Servicios no disponibles',
          html: `Los siguientes servicios ya no están disponibles:<br><strong>${invalidNames}</strong><br><br>¿Deseas quitarlos y continuar con los demás?`,
          icon: 'warning',
          showCancelButton: true,
          confirmButtonText: 'Quitar y continuar',
          cancelButtonText: 'Volver al catálogo',
          confirmButtonColor: '#2d6a4f'
        })

        if (result.isConfirmed) {
          // Quitar los servicios inválidos del carrito
          cartItems.value = cartItems.value.filter(i => availableIds.has(i.servicioId))
          sessionStorage.setItem('pending_cart', JSON.stringify(cartItems.value))

          if (cartItems.value.length === 0) {
            Swal.fire('Carrito vacío', 'No quedan servicios válidos en tu selección.', 'info')
            processing.value = false
            return
          }
        } else {
          sessionStorage.removeItem('pending_cart')
          router.push('/cliente/catalogo')
          processing.value = false
          return
        }
      }
    }

    // 1. Crear Orden de Servicio en el Backend
    const orderPayload = {
      items: cartItems.value.map(i => ({
        servicioId: i.servicioId,
        cantidad: i.cantidad
      })),
      observaciones: observaciones.value
    }

    const orderRes = await api.post('/ServiceOrder/create', orderPayload)

    if (!orderRes.data.success) {
      Swal.fire({
        title: 'Error al crear la orden',
        text: orderRes.data.message || 'No se pudo crear la orden',
        icon: 'error',
        confirmButtonText: 'Volver al catálogo',
        confirmButtonColor: '#2d6a4f'
      }).then(() => {
        sessionStorage.removeItem('pending_cart')
        router.push('/cliente/catalogo')
      })
      processing.value = false
      return
    }

    const orderId = orderRes.data.data.id

    // 2. Crear Sesión de Pago con la Pasarela (Stripe)
    const sessionRes = await api.post('/Payment/create-session', {
      orderId: orderId,
      metodoPago: selectedMethod.value
    })

    if (sessionRes.data.success) {
      sessionStorage.removeItem('pending_cart')
      // Redirigir a la pasarela de pagos
      window.location.href = sessionRes.data.data.paymentUrl
    } else {
      Swal.fire('Error de Pago', sessionRes.data.message || 'Error al iniciar la pasarela de pagos', 'error')
    }
  } catch (err) {
    console.error('Error procesando pago:', err)
    const errorMsg = err.response?.data?.message || 'Ocurrió un error inesperado al procesar la solicitud'

    // Si el error es de servicios no encontrados, ofrecer volver al catálogo
    if (errorMsg.includes('no encontrado') || errorMsg.includes('no disponible') || errorMsg.includes('catálogo')) {
      Swal.fire({
        title: 'Servicio no disponible',
        text: errorMsg,
        icon: 'warning',
        confirmButtonText: 'Ir al catálogo',
        showCancelButton: true,
        cancelButtonText: 'Reintentar',
        confirmButtonColor: '#2d6a4f'
      }).then((result) => {
        if (result.isConfirmed) {
          sessionStorage.removeItem('pending_cart')
          router.push('/cliente/catalogo')
        }
      })
    } else {
      Swal.fire('Error', errorMsg, 'error')
    }
  } finally {
    processing.value = false
  }
}
</script>

<style scoped>
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

.payment-option {
  transition: all 0.2s ease;
}

.payment-option:hover {
  background-color: #f8f9fa;
}

.active-option {
  border-color: #2d6a4f !important;
  background-color: #f4fbf7 !important;
}

.cursor-pointer {
  cursor: pointer;
}
</style>
