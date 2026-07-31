<template>
  <div class="container py-4" style="max-width: 900px;">
    <!-- Banner Modo Demostración -->
    <div class="alert alert-info border-0 shadow-sm rounded-4 p-3 mb-4 d-flex align-items-center gap-3 bg-gradient-info text-white">
      <i class="bi bi-info-circle-fill fs-2"></i>
      <div>
        <h6 class="fw-bold mb-0">Entorno de Pruebas / Demostración</h6>
        <small>Este checkout utiliza un <strong>método de pago ficticio y simulado</strong>. No se realizará ningún cobro real ni se procesarán datos financieros.</small>
      </div>
    </div>

    <!-- Header -->
    <div class="mb-4">
      <router-link to="/cliente/catalogo" class="text-decoration-none text-muted small mb-2 d-inline-block">
        <i class="bi bi-arrow-left me-1"></i> Volver al Catálogo
      </router-link>
      <h2 class="fw-bold text-emerald mb-1">
        <i class="bi bi-receipt-cutoff me-2"></i>Resumen y Pago Simulado
      </h2>
      <p class="text-muted">Verifica los detalles de tu solicitud y completa el pago de prueba</p>
    </div>

    <div v-if="cartItems.length === 0" class="card border-0 shadow-sm rounded-4 p-5 text-center">
      <i class="bi bi-cart-x fs-1 text-muted"></i>
      <h4 class="mt-3 text-muted">No has seleccionado ningún servicio</h4>
      <router-link to="/cliente/catalogo" class="btn btn-emerald rounded-pill mt-3 px-4">
        Ver Catálogo de Servicios
      </router-link>
    </div>

    <div v-else class="row g-4">
      <!-- Tabla de ítems seleccionados y notas -->
      <div class="col-12 col-lg-6">
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
            :disabled="processing || successResult"
          ></textarea>
        </div>
      </div>

      <!-- Formulario de Pago Simulado -->
      <div class="col-12 col-lg-6">
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <h5 class="fw-bold text-emerald mb-3">Resumen del Pedido</h5>

          <div class="d-flex justify-content-between mb-2">
            <span class="text-muted">Subtotal:</span>
            <span class="fw-semibold">Bs. {{ total.toFixed(2) }}</span>
          </div>

          <div class="d-flex justify-content-between mb-3 border-bottom pb-3">
            <span class="text-muted">Impuestos / Recargos:</span>
            <span class="fw-semibold text-success">Bs. 0.00</span>
          </div>

          <div class="d-flex justify-content-between mb-4">
            <span class="fw-bold fs-5">Total a Pagar:</span>
            <span class="fw-bold fs-3 text-emerald">Bs. {{ total.toFixed(2) }}</span>
          </div>

          <!-- Selección de método de pago -->
          <div class="mb-4">
            <label class="fw-bold text-dark mb-2">Selecciona Método de Pago (Simulado)</label>
            <div class="d-grid gap-2">
              <label :class="['payment-option p-3 rounded-3 border cursor-pointer d-flex align-items-center gap-3', selectedMethod === 'card' ? 'active-option' : '']">
                <input type="radio" v-model="selectedMethod" value="card" class="form-check-input" :disabled="processing || successResult" />
                <i class="bi bi-credit-card-2-front fs-3 text-emerald"></i>
                <div>
                  <strong class="d-block text-dark">💳 Tarjeta (Ficticia)</strong>
                  <small class="text-muted">Formulario de tarjeta de crédito/débito simulado</small>
                </div>
              </label>

              <label :class="['payment-option p-3 rounded-3 border cursor-pointer d-flex align-items-center gap-3', selectedMethod === 'qr' ? 'active-option' : '']">
                <input type="radio" v-model="selectedMethod" value="qr" class="form-check-input" :disabled="processing || successResult" />
                <i class="bi bi-qr-code-scan fs-3 text-emerald"></i>
                <div>
                  <strong class="d-block text-dark">📱 Código QR (Ficticio)</strong>
                  <small class="text-muted">Generador de QR demostrativo instantáneo</small>
                </div>
              </label>
            </div>
          </div>

          <!-- FORMULARIO TARJETA FICTICIA -->
          <div v-if="selectedMethod === 'card' && !successResult" class="p-3 bg-light rounded-3 mb-4">
            <h6 class="fw-bold text-dark mb-3">
              <i class="bi bi-credit-card me-1"></i>Datos de Tarjeta Ficticia
            </h6>
            
            <div class="mb-3">
              <label class="form-label small fw-semibold">Número de Tarjeta (Ficticio)</label>
              <input
                type="text"
                v-model="cardForm.number"
                @input="formatCardNumber"
                class="form-control rounded-3"
                placeholder="4532 1234 5678 9012"
                maxlength="19"
                :class="{'is-invalid': cardErrors.number}"
                :disabled="processing"
              />
              <div v-if="cardErrors.number" class="invalid-feedback">{{ cardErrors.number }}</div>
            </div>

            <div class="mb-3">
              <label class="form-label small fw-semibold">Nombre del Titular</label>
              <input
                type="text"
                v-model="cardForm.name"
                class="form-control rounded-3"
                placeholder="Ej. JUAN PÉREZ"
                :class="{'is-invalid': cardErrors.name}"
                :disabled="processing"
              />
              <div v-if="cardErrors.name" class="invalid-feedback">{{ cardErrors.name }}</div>
            </div>

            <div class="row g-2">
              <div class="col-6">
                <label class="form-label small fw-semibold">Vencimiento (MM/YY)</label>
                <input
                  type="text"
                  v-model="cardForm.expiry"
                  @input="formatExpiry"
                  class="form-control rounded-3 text-center"
                  placeholder="12/28"
                  maxlength="5"
                  :class="{'is-invalid': cardErrors.expiry}"
                  :disabled="processing"
                />
                <div v-if="cardErrors.expiry" class="invalid-feedback">{{ cardErrors.expiry }}</div>
              </div>
              <div class="col-6">
                <label class="form-label small fw-semibold">CVV (3-4 dígitos)</label>
                <input
                  type="password"
                  v-model="cardForm.cvv"
                  @input="formatCvv"
                  class="form-control rounded-3 text-center"
                  placeholder="123"
                  maxlength="4"
                  :class="{'is-invalid': cardErrors.cvv}"
                  :disabled="processing"
                />
                <div v-if="cardErrors.cvv" class="invalid-feedback">{{ cardErrors.cvv }}</div>
              </div>
            </div>
            <small class="text-muted d-block mt-2 fst-italic" style="font-size: 0.75rem;">
              🔒 Los datos ingresados son puramente demostrativos y no se almacenan ni se envían a servidores externos.
            </small>
          </div>

          <!-- VISTA PAGO POR CÓDIGO QR FICTICIO -->
          <div v-if="selectedMethod === 'qr' && !successResult" class="p-3 bg-light rounded-3 mb-4 text-center">
            <h6 class="fw-bold text-dark mb-2">
              <i class="bi bi-qr-code me-1"></i>Código QR de Demostración
            </h6>
            <p class="text-muted small mb-3">Escanea el código QR simulado desde tu app bancaria de prueba</p>
            
            <!-- SVG QR Simulado Generado Dinámicamente -->
            <div class="bg-white p-3 rounded-3 d-inline-block border shadow-sm mb-3">
              <svg xmlns="http://www.w3.org/2000/svg" width="160" height="160" viewBox="0 0 200 200">
                <rect width="200" height="200" fill="#ffffff"/>
                <!-- Esquinas Finder Patterns -->
                <rect x="10" y="10" width="50" height="50" fill="#2d6a4f" />
                <rect x="20" y="20" width="30" height="30" fill="#ffffff" />
                <rect x="25" y="25" width="20" height="20" fill="#2d6a4f" />
                <rect x="140" y="10" width="50" height="50" fill="#2d6a4f" />
                <rect x="150" y="20" width="30" height="30" fill="#ffffff" />
                <rect x="155" y="25" width="20" height="20" fill="#2d6a4f" />
                <rect x="10" y="140" width="50" height="50" fill="#2d6a4f" />
                <rect x="20" y="150" width="30" height="30" fill="#ffffff" />
                <rect x="25" y="155" width="20" height="20" fill="#2d6a4f" />
                <!-- Datos Simulados QR Matrix Pattern -->
                <rect x="70" y="20" width="15" height="15" fill="#2d6a4f" />
                <rect x="95" y="20" width="15" height="15" fill="#2d6a4f" />
                <rect x="70" y="45" width="15" height="15" fill="#2d6a4f" />
                <rect x="115" y="45" width="15" height="15" fill="#2d6a4f" />
                <rect x="20" y="70" width="15" height="15" fill="#2d6a4f" />
                <rect x="45" y="70" width="15" height="15" fill="#2d6a4f" />
                <rect x="70" y="70" width="20" height="20" fill="#1b4332" />
                <rect x="100" y="70" width="15" height="15" fill="#2d6a4f" />
                <rect x="130" y="70" width="25" height="15" fill="#2d6a4f" />
                <rect x="165" y="70" width="15" height="15" fill="#2d6a4f" />
                <rect x="20" y="95" width="25" height="15" fill="#2d6a4f" />
                <rect x="55" y="95" width="15" height="15" fill="#2d6a4f" />
                <rect x="80" y="95" width="15" height="15" fill="#2d6a4f" />
                <rect x="110" y="95" width="20" height="20" fill="#1b4332" />
                <rect x="140" y="95" width="15" height="15" fill="#2d6a4f" />
                <rect x="165" y="95" width="15" height="15" fill="#2d6a4f" />
                <rect x="70" y="125" width="15" height="15" fill="#2d6a4f" />
                <rect x="95" y="125" width="20" height="15" fill="#2d6a4f" />
                <rect x="125" y="125" width="15" height="15" fill="#2d6a4f" />
                <rect x="70" y="150" width="25" height="15" fill="#2d6a4f" />
                <rect x="105" y="150" width="15" height="15" fill="#2d6a4f" />
                <rect x="130" y="150" width="20" height="20" fill="#2d6a4f" />
                <rect x="160" y="150" width="15" height="15" fill="#2d6a4f" />
              </svg>
            </div>

            <div class="small text-muted">
              <strong>Monto:</strong> Bs. {{ total.toFixed(2) }}<br/>
              <strong>Concepto:</strong> Servicio EcoWash Móvil Demo<br/>
              <strong>Ref:</strong> QR-DEMO-{{ Date.now().toString().slice(-6) }}
            </div>
          </div>

          <!-- PANTALLA ÉXITO POST PAGO FICTICIO -->
          <div v-if="successResult" class="p-4 bg-success bg-opacity-10 border border-success rounded-4 text-center mb-4">
            <i class="bi bi-check-circle-fill text-success display-4 mb-2 d-block"></i>
            <h5 class="fw-bold text-success">✓ Pago realizado correctamente</h5>
            <p class="text-muted small mb-3">La operación fue procesada en modo simulación exitosamente.</p>

            <div class="bg-white p-3 rounded-3 text-start mb-3 border small">
              <div class="d-flex justify-content-between mb-1">
                <span class="text-muted">ID Transacción:</span>
                <span class="fw-mono fw-bold">{{ successResult.transactionId }}</span>
              </div>
              <div class="d-flex justify-content-between mb-1">
                <span class="text-muted">Orden N°:</span>
                <span class="fw-bold">{{ successResult.numeroOrden }}</span>
              </div>
              <div class="d-flex justify-content-between mb-1">
                <span class="text-muted">Monto Pagado:</span>
                <span class="fw-bold text-emerald">Bs. {{ successResult.montoTotal.toFixed(2) }}</span>
              </div>
              <div class="d-flex justify-content-between">
                <span class="text-muted">Método:</span>
                <span>{{ successResult.metodoPago }}</span>
              </div>
            </div>

            <div class="d-grid gap-2">
              <a
                v-if="successResult.comprobantePdfUrl"
                :href="getReceiptUrl(successResult.comprobantePdfUrl)"
                target="_blank"
                class="btn btn-emerald rounded-3 fw-bold"
              >
                <i class="bi bi-file-earmark-pdf-fill me-1"></i> Descargar Recibo PDF
              </a>
              <router-link to="/cliente/reservas" class="btn btn-outline-secondary rounded-3">
                Ver Mis Reservas
              </router-link>
            </div>
          </div>

          <!-- BOTÓN PAGO SIMULADO -->
          <button
            v-if="!successResult"
            @click="processSimulatedPayment"
            class="btn btn-emerald w-100 py-3 rounded-3 fw-bold shadow-sm"
            :disabled="processing"
          >
            <span v-if="processing" class="spinner-border spinner-border-sm me-2"></span>
            <i v-else class="bi bi-shield-lock-fill me-2"></i>
            {{ processing ? 'Procesando pago seguro...' : `Simular Pago de Bs. ${total.toFixed(2)}` }}
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
const successResult = ref(null)

const cardForm = ref({
  number: '',
  name: '',
  expiry: '',
  cvv: ''
})

const cardErrors = ref({
  number: '',
  name: '',
  expiry: '',
  cvv: ''
})

onMounted(() => {
  const data = sessionStorage.getItem('pending_cart')
  if (data) {
    cartItems.value = JSON.parse(data)
  }
})

const total = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.precio * item.cantidad), 0)
})

// Formateador automático del número de tarjeta XXXX XXXX XXXX XXXX
const formatCardNumber = (e) => {
  let val = e.target.value.replace(/\D/g, '')
  if (val.length > 16) val = val.substring(0, 16)
  const parts = val.match(/.{1,4}/g)
  cardForm.value.number = parts ? parts.join(' ') : val
  cardErrors.value.number = ''
}

// Formateador automático de fecha MM/YY
const formatExpiry = (e) => {
  let val = e.target.value.replace(/\D/g, '')
  if (val.length > 4) val = val.substring(0, 4)
  if (val.length >= 3) {
    cardForm.value.expiry = val.substring(0, 2) + '/' + val.substring(2)
  } else {
    cardForm.value.expiry = val
  }
  cardErrors.value.expiry = ''
}

// Formateador CVV
const formatCvv = (e) => {
  let val = e.target.value.replace(/\D/g, '')
  if (val.length > 4) val = val.substring(0, 4)
  cardForm.value.cvv = val
  cardErrors.value.cvv = ''
}

// Validaciones del formulario de tarjeta
const validateCardForm = () => {
  let isValid = true
  cardErrors.value = { number: '', name: '', expiry: '', cvv: '' }

  if (selectedMethod.value === 'card') {
    const rawNumber = cardForm.value.number.replace(/\s/g, '')
    if (!rawNumber || rawNumber.length < 13) {
      cardErrors.value.number = 'Ingrese un número de tarjeta válido (13 a 16 dígitos).'
      isValid = false
    }

    if (!cardForm.value.name || cardForm.value.name.trim().length < 3) {
      cardErrors.value.name = 'Ingrese el nombre completo del titular.'
      isValid = false
    }

    if (!cardForm.value.expiry || !/^\d{2}\/\d{2}$/.test(cardForm.value.expiry)) {
      cardErrors.value.expiry = 'Formato inválido. Use MM/YY.'
      isValid = false
    } else {
      const [month, year] = cardForm.value.expiry.split('/').map(Number)
      if (month < 1 || month > 12) {
        cardErrors.value.expiry = 'Mes inválido (01-12).'
        isValid = false
      }
    }

    if (!cardForm.value.cvv || cardForm.value.cvv.length < 3) {
      cardErrors.value.cvv = 'CVV debe tener 3 o 4 dígitos.'
      isValid = false
    }
  }

  return isValid
}

const processSimulatedPayment = async () => {
  if (cartItems.value.length === 0) return

  if (selectedMethod.value === 'card' && !validateCardForm()) {
    return
  }

  processing.value = true

  try {
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
        confirmButtonColor: '#2d6a4f'
      })
      processing.value = false
      return
    }

    const orderId = orderRes.data.data.id

    // Simulación de retraso de red (1.5 segundos) para experiencia realista
    await new Promise(resolve => setTimeout(resolve, 1500))

    // 2. Procesar Pago Ficticio en el Backend
    const paymentRes = await api.post('/Payment/process-simulated', {
      orderId: orderId,
      metodoPago: selectedMethod.value,
      titularTarjeta: cardForm.value.name || 'Cliente Demo',
      ultimosDigitosTarjeta: cardForm.value.number ? cardForm.value.number.slice(-4) : '4321'
    })

    if (paymentRes.data.success) {
      sessionStorage.removeItem('pending_cart')
      successResult.value = paymentRes.data.data

      Swal.fire({
        title: '✓ Pago realizado correctamente',
        html: `Transacción de prueba procesada.<br>ID: <strong>${paymentRes.data.data.transactionId}</strong>`,
        icon: 'success',
        confirmButtonText: 'Entendido',
        confirmButtonColor: '#2d6a4f'
      })
    } else {
      Swal.fire('Error', paymentRes.data.message || 'Error al procesar el pago simulado.', 'error')
    }
  } catch (err) {
    console.error('Error procesando pago simulado:', err)
    const errorMsg = err.response?.data?.message || 'Error de conexión al procesar el pago ficticio'
    Swal.fire('Error', errorMsg, 'error')
  } finally {
    processing.value = false
  }
}

const getReceiptUrl = (path) => {
  const baseURL = api.defaults.baseURL.replace('/api', '')
  return `${baseURL}${path}`
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

.bg-gradient-info {
  background: linear-gradient(135deg, #023e8a 0%, #0077b6 100%);
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

.fw-mono {
  font-family: 'Courier New', Courier, monospace;
}
</style>
