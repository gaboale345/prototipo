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
                  <!-- Botón Detalle -->
                  <button @click="verDetalle(pago)" class="btn btn-outline-emerald rounded-pill btn-sm px-3" title="Ver Detalle">
                    <i class="bi bi-eye me-1"></i> Detalle
                  </button>

                  <!-- Botón Descargar PDF (solo si está pagado) -->
                  <a
                    v-if="pago.estado === 'Pagado'"
                    :href="getPdfUrl(pago.id)"
                    target="_blank"
                    class="btn btn-emerald rounded-pill btn-sm px-3 text-white"
                    title="Descargar Comprobante PDF"
                  >
                    <i class="bi bi-file-earmark-pdf me-1"></i> PDF
                  </a>
                </div>
              </div>
            </div>

            <!-- Acordeón de Detalle Expandible -->
            <div v-if="selectedPagoId === pago.id" class="mt-4 pt-3 border-top bg-light p-3 rounded-3">
              <h6 class="fw-bold text-emerald mb-3">Detalle de la Orden</h6>
              <div class="table-responsive">
                <table class="table table-sm align-middle mb-0 bg-white rounded-3 overflow-hidden">
                  <thead class="bg-light">
                    <tr>
                      <th>Servicio</th>
                      <th class="text-center">Cantidad</th>
                      <th class="text-end">Precio Unit.</th>
                      <th class="text-end">Subtotal</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="item in pago.detallesOrden" :key="item.id">
                      <td>{{ item.nombreServicio }}</td>
                      <td class="text-center">{{ item.cantidad }}</td>
                      <td class="text-end">Bs. {{ item.precioUnitario.toFixed(2) }}</td>
                      <td class="text-end fw-bold">Bs. {{ item.subtotal.toFixed(2) }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
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
  }
}

const getPdfUrl = (transactionId) => {
  const token = localStorage.getItem('ecowash_token')
  return `/api/Receipt/${transactionId}/download?token=${token}`
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
  color: #2d6a4f;
}

.btn-emerald {
  background: linear-gradient(135deg, #2d6a4f 0%, #40916c 100%);
  border: none;
}

.btn-outline-emerald {
  border-color: #2d6a4f;
  color: #2d6a4f;
}

.btn-outline-emerald:hover {
  background-color: #2d6a4f;
  color: white;
}
</style>
