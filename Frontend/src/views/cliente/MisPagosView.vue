<template>
  <div class="container-fluid py-4">
    <!-- Header -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
      <div>
        <h2 class="fw-bold text-emerald mb-1">
          <i class="bi bi-wallet2 me-2"></i>Mis Pagos
        </h2>
        <p class="text-muted mb-0">Consulta tu historial de transacciones y descarga tus comprobantes</p>
      </div>
    </div>

    <!-- Filtros -->
    <div class="card border-0 shadow-sm rounded-4 p-3 mb-4">
      <div class="row g-3 align-items-center">
        <!-- Filtro por Estado -->
        <div class="col-12 col-md-4">
          <label class="form-label small fw-bold text-muted">Estado del Pago</label>
          <select v-model="filtroEstado" class="form-select rounded-3" @change="fetchPagos">
            <option value="">Todos los Estados</option>
            <option value="Pagado">Pagados</option>
            <option value="Pendiente">Pendientes</option>
            <option value="Fallido">Fallidos</option>
            <option value="Cancelado">Cancelados</option>
          </select>
        </div>

        <!-- Filtro por Fecha Inicio -->
        <div class="col-12 col-md-3">
          <label class="form-label small fw-bold text-muted">Desde</label>
          <input type="date" v-model="filtroFechaInicio" class="form-control rounded-3" @change="fetchPagos" />
        </div>

        <!-- Filtro por Fecha Fin -->
        <div class="col-12 col-md-3">
          <label class="form-label small fw-bold text-muted">Hasta</label>
          <input type="date" v-model="filtroFechaFin" class="form-control rounded-3" @change="fetchPagos" />
        </div>

        <!-- Botón Limpiar -->
        <div class="col-12 col-md-2 d-flex align-items-end">
          <button @click="limpiarFiltros" class="btn btn-outline-secondary w-100 rounded-3">
            <i class="bi bi-x-circle me-1"></i> Limpiar
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-emerald" role="status"></div>
      <p class="text-muted mt-2">Cargando historial de pagos...</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="pagos.length === 0" class="text-center py-5 bg-white rounded-4 shadow-sm">
      <i class="bi bi-receipt fs-1 text-muted"></i>
      <h5 class="mt-3 text-muted">No se encontraron pagos registrados</h5>
      <p class="text-muted small">Tus transacciones de servicios aparecerán aquí.</p>
    </div>

    <!-- Lista de Pagos -->
    <div v-else class="row g-3">
      <div v-for="pago in pagos" :key="pago.id" class="col-12">
        <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
          <div class="card-body p-4">
            <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
              <!-- Información Básica -->
              <div>
                <div class="d-flex align-items-center gap-2 mb-1">
                  <span class="fw-bold fs-5 text-dark">Orden: {{ pago.numeroOrden }}</span>
                  <span :class="['badge rounded-pill px-3 py-2', getStatusBadge(pago.estado)]">
                    {{ pago.estado }}
                  </span>
                </div>
                <small class="text-muted d-block">
                  <i class="bi bi-calendar3 me-1"></i> {{ formatDate(pago.fechaCreacion) }}
                  <span v-if="pago.fechaPago"> | <i class="bi bi-check2-circle text-success me-1"></i> Pago: {{ formatDate(pago.fechaPago) }}</span>
                </small>
                <small class="text-muted d-block mt-1">
                  <i class="bi bi-credit-card me-1"></i> Método: {{ pago.metodoPago || 'Tarjeta / QR' }}
                </small>
              </div>

              <!-- Monto y Acciones -->
              <div class="d-flex align-items-center gap-3 ms-md-auto">
                <div class="text-end">
                  <small class="text-muted d-block">Monto Total</small>
                  <span class="fs-4 fw-bold text-emerald">Bs. {{ pago.monto.toFixed(2) }}</span>
                </div>

                <div class="d-flex gap-2">
                  <!-- Botón Descargar PDF -->
                  <a v-if="pago.estado === 'Pagado'" :href="getPdfUrl(pago.id)" target="_blank" class="btn btn-outline-emerald rounded-pill btn-sm px-3" title="Descargar PDF">
                    <i class="bi bi-download me-1"></i> Descargar PDF
                  </a>

                  <!-- Botón Ver Recibo (Accordion PDF) -->
                  <button
                    v-if="pago.estado === 'Pagado'"
                    @click="abrirRecibo(pago)"
                    class="btn btn-emerald rounded-pill btn-sm px-3 text-white"
                    title="Ver Recibo"
                  >
                    <i class="bi bi-receipt me-1"></i> Ver Recibo
                  </button>
                </div>
              </div>
            </div>

            <!-- Acordeón de Recibo Embebido -->
            <div v-if="reciboAbiertoId === pago.id" class="mt-4 pt-3 border-top bg-light p-3 rounded-3 text-center">

              <div class="border rounded bg-white shadow-sm text-start mx-auto overflow-hidden" style="max-width: 800px; font-family: 'Inter', sans-serif;">
                
                <!-- Header -->
                <div class="bg-emerald text-white p-3 px-4 d-flex justify-content-between align-items-center">
                  <div class="d-flex align-items-center">
                    <i class="bi bi-droplet-fill me-2 fs-3"></i>
                    <div>
                      <h5 class="fw-bold mb-1">EcoWash Direct</h5>
                      <p class="mb-0 small text-white-50">Comprobante de Pago Electrónico</p>
                    </div>
                  </div>
                  <div>
                    <span class="badge bg-dark rounded-pill px-3 py-2 border border-secondary shadow-sm"><i class="bi bi-check-circle-fill text-success me-1"></i> PAGADO EXITOSAMENTE</span>
                  </div>
                </div>
                
                <div class="p-4 px-md-5">
                  <!-- Order & Client Details -->
                  <div class="row mb-4 border rounded p-3 mx-0 bg-light">
                    <div class="col-md-6 mb-3 mb-md-0" style="border-right: 1px solid #dee2e6;">
                      <h6 class="fw-bold text-muted small mb-3">DETALLES DE ORDEN</h6>
                      <div class="d-flex justify-content-between mb-1 small">
                        <span class="text-secondary">Orden Nº:</span>
                        <span class="fw-bold text-dark">{{ pago.numeroOrden }}</span>
                      </div>
                      <div class="d-flex justify-content-between mb-1 small">
                        <span class="text-secondary">Fecha:</span>
                        <span class="fw-bold text-dark">{{ new Date(pago.fechaPago).toLocaleDateString() }}</span>
                      </div>
                      <div class="d-flex justify-content-between small">
                        <span class="text-secondary">Hora:</span>
                        <span class="fw-bold text-dark">{{ new Date(pago.fechaPago).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) }}</span>
                      </div>
                    </div>
                    <div class="col-md-6 ps-md-4">
                      <h6 class="fw-bold text-muted small mb-3">DATOS DEL CLIENTE</h6>
                      <div class="d-flex justify-content-between mb-1 small">
                        <span class="text-secondary">Nombre:</span>
                        <span class="fw-bold text-dark text-end">{{ pago.nombreCliente || 'Cliente Registrado' }}</span>
                      </div>
                      <div class="d-flex justify-content-between small">
                        <span class="text-secondary">Email:</span>
                        <span class="fw-bold text-dark text-end">{{ pago.emailCliente || 'N/A' }}</span>
                      </div>
                    </div>
                  </div>
                  
                  <!-- Services Table -->
                  <div class="mb-4 border rounded overflow-hidden">
                    <div class="bg-emerald text-white px-3 py-2 d-flex fw-bold" style="font-size: 0.8rem; letter-spacing: 0.5px;">
                      <div style="flex: 3">SERVICIO</div>
                      <div style="flex: 1" class="text-center">CANT.</div>
                      <div style="flex: 1.5" class="text-end">P. UNIT.</div>
                      <div style="flex: 1.5" class="text-end">SUBTOTAL</div>
                    </div>
                    <div v-for="item in pago.detallesOrden" :key="item.id" class="px-3 py-3 d-flex border-bottom small align-items-center text-dark">
                      <div style="flex: 3">{{ item.nombreServicio }}</div>
                      <div style="flex: 1" class="text-center">{{ item.cantidad }}</div>
                      <div style="flex: 1.5" class="text-end">Bs. {{ item.precioUnitario.toFixed(2) }}</div>
                      <div style="flex: 1.5" class="text-end fw-bold">Bs. {{ item.subtotal.toFixed(2) }}</div>
                    </div>
                    <div class="bg-light px-4 py-3 d-flex justify-content-end align-items-center">
                      <span class="fw-bold text-muted me-4 small">TOTAL PAGADO</span>
                      <span class="fw-bold text-dark fs-4">Bs. {{ pago.monto.toFixed(2) }}</span>
                    </div>
                  </div>
                  
                  <!-- Footer -->
                  <div class="row align-items-center mt-2">
                    <div class="col-md-7">
                      <h6 class="fw-bold text-muted small mb-2">INFORMACIÓN DE PAGO</h6>
                      <div class="small mb-1 text-dark"><span class="text-secondary">Método:</span> <span class="fw-bold">{{ pago.metodoPago || 'Tarjeta Débito/Crédito' }}</span></div>
                      <div class="small text-dark"><span class="text-secondary">Transacción ID:</span> <span class="text-muted font-monospace">TX-{{ pago.transactionId || pago.id }}</span></div>
                    </div>
                    <div class="col-md-5 mt-3 mt-md-0">
                      <div class="border rounded p-2 bg-light d-flex align-items-center gap-3">
                        <div class="bg-white border p-1 rounded d-flex align-items-center justify-content-center" style="min-width: 50px; height: 50px;">
                          <i class="bi bi-qr-code text-dark fs-3"></i>
                        </div>
                        <p class="mb-0 text-muted lh-sm" style="font-size: 0.7rem;">
                          Escanea el código QR para validar la autenticidad de este comprobante electrónico.
                        </p>
                      </div>
                    </div>
                  </div>
                  
                </div>
              </div>
              <button class="btn btn-outline-secondary btn-sm mt-3 rounded-pill px-4" @click="reciboAbiertoId = null">Cerrar Recibo</button>
            </div>
        </div>
      </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const pagos = ref([])
const loading = ref(true)
const filtroEstado = ref('')
const filtroFechaInicio = ref('')
const filtroFechaFin = ref('')
const selectedPagoId = ref(null)
const reciboAbiertoId = ref(null)

onMounted(() => {
  fetchPagos()
})

const fetchPagos = async () => {
  loading.value = true
  try {
    const params = {}
    if (filtroEstado.value) params.estado = filtroEstado.value
    if (filtroFechaInicio.value) params.fechaInicio = filtroFechaInicio.value
    if (filtroFechaFin.value) params.fechaFin = filtroFechaFin.value

    const res = await api.get('/Payment/history', { params })
    if (res.data.success) {
      pagos.value = res.data.data
    }
  } catch (err) {
    console.error('Error al obtener historial de pagos:', err)
  } finally {
    loading.value = false
  }
}

const limpiarFiltros = () => {
  filtroEstado.value = ''
  filtroFechaInicio.value = ''
  filtroFechaFin.value = ''
  fetchPagos()
}

const verDetalle = (pago) => {
  if (selectedPagoId.value === pago.id) {
    selectedPagoId.value = null
  } else {
    selectedPagoId.value = pago.id
    reciboAbiertoId.value = null // Close receipt if open
  }
}

const abrirRecibo = (pago) => {
  if (reciboAbiertoId.value === pago.id) {
    reciboAbiertoId.value = null
  } else {
    reciboAbiertoId.value = pago.id
    selectedPagoId.value = null // Close detail if open
  }
}

const getPdfUrl = (transactionId) => {
  const token = localStorage.getItem('ecowash_token')
  let baseURL = api.defaults.baseURL || 'http://localhost:5275/api'
  if (baseURL.endsWith('/')) baseURL = baseURL.slice(0, -1)
  return `${baseURL}/Receipt/${transactionId}/download?token=${token}`
}

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  const d = new Date(dateStr)
  return d.toLocaleDateString('es-BO', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getStatusBadge = (estado) => {
  switch (estado) {
    case 'Pagado': return 'bg-success text-white'
    case 'Pendiente': return 'bg-warning text-dark'
    case 'Fallido': return 'bg-danger text-white'
    case 'Cancelado': return 'bg-secondary text-white'
    default: return 'bg-light text-dark'
  }
}
</script>

<style scoped>
.text-emerald {
  color: #2563EB;
}

.btn-emerald {
  background: linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%);
  border: none;
}

.btn-outline-emerald {
  border-color: #2563EB;
  color: #2563EB;
}

.btn-outline-emerald:hover {
  background-color: #2563EB;
  color: white;
}

.bg-emerald {
  background-color: #2563EB;
}

.border-dashed {
  border-bottom-style: dashed !important;
  border-bottom-color: #e0e0e0 !important;
}
</style>
