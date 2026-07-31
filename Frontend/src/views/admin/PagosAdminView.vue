<template>
  <div class="container-fluid py-4">
    <!-- Header -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
      <div>
        <h2 class="fw-bold text-emerald mb-1">
          <i class="bi bi-cash-coin me-2"></i>Panel Administrativo de Pagos
        </h2>
        <p class="text-muted mb-0">Gestión de ingresos, auditoría de transacciones y reportes de recaudación</p>
      </div>

      <!-- Botones de Exportación -->
      <div class="d-flex gap-2">
        <button @click="exportarExcel" class="btn btn-success rounded-pill px-3 shadow-sm fw-semibold">
          <i class="bi bi-file-earmark-excel me-1"></i> Excel
        </button>
        <button @click="exportarPdf" class="btn btn-danger rounded-pill px-3 shadow-sm fw-semibold">
          <i class="bi bi-file-earmark-pdf me-1"></i> PDF
        </button>
      </div>
    </div>

    <!-- Cards de Estadísticas -->
    <div class="row g-3 mb-4">
      <div class="col-12 col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-emerald text-white">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <small class="text-white-50 text-uppercase fw-bold">Total Recaudado</small>
              <h3 class="fw-bold mb-0 mt-1">Bs. {{ stats.totalRecaudado.toFixed(2) }}</h3>
            </div>
            <div class="stat-icon-circle bg-white text-emerald">
              <i class="bi bi-currency-dollar fs-3"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-white">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <small class="text-muted text-uppercase fw-bold">Pagos Exitosos</small>
              <h3 class="fw-bold text-success mb-0 mt-1">{{ stats.pagosExitosos }}</h3>
            </div>
            <div class="stat-icon-circle bg-success-subtle text-success">
              <i class="bi bi-check-circle-fill fs-3"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-white">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <small class="text-muted text-uppercase fw-bold">Pagos Pendientes</small>
              <h3 class="fw-bold text-warning mb-0 mt-1">{{ stats.pagosPendientes }}</h3>
            </div>
            <div class="stat-icon-circle bg-warning-subtle text-warning">
              <i class="bi bi-clock-history fs-3"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-white">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <small class="text-muted text-uppercase fw-bold">Total Transacciones</small>
              <h3 class="fw-bold text-dark mb-0 mt-1">{{ stats.totalTransacciones }}</h3>
            </div>
            <div class="stat-icon-circle bg-light text-dark">
              <i class="bi bi-receipt fs-3"></i>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filtros de Búsqueda -->
    <div class="card border-0 shadow-sm rounded-4 p-3 mb-4">
      <div class="row g-3">
        <div class="col-12 col-md-3">
          <label class="form-label small fw-bold text-muted">Búsqueda General</label>
          <input
            type="text"
            v-model="filtroBusqueda"
            class="form-control rounded-3"
            placeholder="Orden, cliente, email..."
            @input="debouncedFetch"
          />
        </div>

        <div class="col-12 col-md-3">
          <label class="form-label small fw-bold text-muted">Estado</label>
          <select v-model="filtroEstado" class="form-select rounded-3" @change="fetchPagos">
            <option value="">Todos</option>
            <option value="Pagado">Pagado</option>
            <option value="Pendiente">Pendiente</option>
            <option value="Fallido">Fallido</option>
            <option value="Cancelado">Cancelado</option>
          </select>
        </div>

        <div class="col-12 col-md-2">
          <label class="form-label small fw-bold text-muted">Desde</label>
          <input type="date" v-model="filtroFechaInicio" class="form-control rounded-3" @change="fetchPagos" />
        </div>

        <div class="col-12 col-md-2">
          <label class="form-label small fw-bold text-muted">Hasta</label>
          <input type="date" v-model="filtroFechaFin" class="form-control rounded-3" @change="fetchPagos" />
        </div>

        <div class="col-12 col-md-2 d-flex align-items-end">
          <button @click="limpiarFiltros" class="btn btn-outline-secondary w-100 rounded-3">
            <i class="bi bi-x-lg me-1"></i> Limpiar
          </button>
        </div>
      </div>
    </div>

    <!-- Tabla de Pagos -->
    <div class="card border-0 shadow-sm rounded-4 overflow-hidden mb-4">
      <div class="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
        <h5 class="fw-bold mb-0 text-emerald">Historial de Transacciones</h5>
        <span class="badge bg-light text-dark border">{{ pagos.length }} registros</span>
      </div>

      <div class="card-body p-0">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-emerald" role="status"></div>
          <p class="text-muted mt-2">Cargando pagos...</p>
        </div>

        <div v-else-if="pagos.length === 0" class="text-center py-5">
          <i class="bi bi-inbox fs-1 text-muted"></i>
          <p class="text-muted mt-2">No se encontraron pagos con los filtros aplicados</p>
        </div>

        <div v-else class="table-responsive">
          <table class="table align-middle mb-0 table-hover">
            <thead class="bg-light">
              <tr>
                <th class="ps-4">ID</th>
                <th>Orden</th>
                <th>Cliente</th>
                <th>Monto</th>
                <th>Estado</th>
                <th>Método</th>
                <th>Fecha Pago</th>
                <th>Proveedor</th>
                <th class="pe-4 text-end">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="pago in pagos" :key="pago.id">
                <td class="ps-4 fw-bold">#{{ pago.id }}</td>
                <td>
                  <span class="fw-bold text-dark d-block">{{ pago.numeroOrden }}</span>
                  <small class="text-muted" v-if="pago.transactionId">Tx: {{ pago.transactionId }}</small>
                </td>
                <td>
                  <span class="fw-semibold text-dark d-block">{{ pago.nombreCliente }}</span>
                  <small class="text-muted">{{ pago.emailCliente }}</small>
                </td>
                <td class="fw-bold text-emerald">Bs. {{ pago.monto.toFixed(2) }}</td>
                <td>
                  <span :class="['badge rounded-pill px-3 py-2', getStatusBadge(pago.estado)]">
                    {{ pago.estado }}
                  </span>
                </td>
                <td>
                  <span class="badge bg-light text-dark border">
                    <i class="bi bi-credit-card me-1"></i> {{ pago.metodoPago || 'Tarjeta' }}
                  </span>
                </td>
                <td>
                  <small class="text-muted">{{ formatDate(pago.fechaPago || pago.fechaCreacion) }}</small>
                </td>
                <td>
                  <small class="badge bg-secondary-subtle text-secondary">{{ pago.proveedorPago }}</small>
                </td>
                <td class="pe-4 text-end">
                  <a
                    v-if="pago.estado === 'Pagado'"
                    :href="getPdfUrl(pago.id)"
                    target="_blank"
                    class="btn btn-sm btn-outline-emerald rounded-circle p-2"
                    title="Descargar Comprobante PDF"
                  >
                    <i class="bi bi-file-earmark-pdf"></i>
                  </a>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const pagos = ref([])
const stats = ref({
  totalRecaudado: 0,
  totalTransacciones: 0,
  pagosExitosos: 0,
  pagosPendientes: 0,
  pagosFallidos: 0,
  pagosCancelados: 0
})

const loading = ref(true)
const filtroBusqueda = ref('')
const filtroEstado = ref('')
const filtroFechaInicio = ref('')
const filtroFechaFin = ref('')

let debounceTimer = null

onMounted(() => {
  fetchStats()
  fetchPagos()
})

const fetchStats = async () => {
  try {
    const res = await api.get('/AdminPayment/stats')
    if (res.data.success) {
      stats.value = res.data.data
    }
  } catch (err) {
    console.error('Error cargando stats:', err)
  }
}

const fetchPagos = async () => {
  loading.value = true
  try {
    const params = {}
    if (filtroBusqueda.value) params.busqueda = filtroBusqueda.value
    if (filtroEstado.value) params.estado = filtroEstado.value
    if (filtroFechaInicio.value) params.fechaInicio = filtroFechaInicio.value
    if (filtroFechaFin.value) params.fechaFin = filtroFechaFin.value

    const res = await api.get('/AdminPayment/all', { params })
    if (res.data.success) {
      pagos.value = res.data.data
    }
  } catch (err) {
    console.error('Error cargando pagos admin:', err)
  } finally {
    loading.value = false
  }
}

const debouncedFetch = () => {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(fetchPagos, 400)
}

const limpiarFiltros = () => {
  filtroBusqueda.value = ''
  filtroEstado.value = ''
  filtroFechaInicio.value = ''
  filtroFechaFin.value = ''
  fetchPagos()
}

const exportarExcel = () => {
  const token = localStorage.getItem('ecowash_token')
  let url = `/api/AdminPayment/export/excel?token=${token}`
  if (filtroEstado.value) url += `&estado=${filtroEstado.value}`
  if (filtroFechaInicio.value) url += `&fechaInicio=${filtroFechaInicio.value}`
  if (filtroFechaFin.value) url += `&fechaFin=${filtroFechaFin.value}`
  window.open(url, '_blank')
}

const exportarPdf = () => {
  const token = localStorage.getItem('ecowash_token')
  let url = `/api/AdminPayment/export/pdf?token=${token}`
  if (filtroEstado.value) url += `&estado=${filtroEstado.value}`
  if (filtroFechaInicio.value) url += `&fechaInicio=${filtroFechaInicio.value}`
  if (filtroFechaFin.value) url += `&fechaFin=${filtroFechaFin.value}`
  window.open(url, '_blank')
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

.bg-emerald {
  background: linear-gradient(135deg, #2d6a4f 0%, #40916c 100%);
}

.btn-outline-emerald {
  border-color: #2d6a4f;
  color: #2d6a4f;
}

.btn-outline-emerald:hover {
  background-color: #2d6a4f;
  color: white;
}

.stat-icon-circle {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
