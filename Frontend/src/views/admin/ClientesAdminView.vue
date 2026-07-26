<template>
  <div class="container-fluid px-0">
    <!-- HEADER DEL DASHBOARD DE CLIENTES -->
    <div class="d-flex flex-wrap justify-content-between align-items-center mb-4 gap-3">
      <div>
        <h2 class="fw-bold text-dark mb-1">
          <i class="bi bi-people-fill text-primary me-2"></i>Dashboard & Gestión de Clientes
        </h2>
        <p class="text-muted small mb-0">
          Análisis global, métricas y base de datos activa de los clientes de <strong>EcoWash Móvil Santa Cruz</strong>.
        </p>
      </div>
      <div class="d-flex gap-2">
        <span class="badge bg-primary-subtle text-primary border border-primary-subtle d-flex align-items-center px-3 py-2 rounded-pill fs-6 fw-semibold">
          <i class="bi bi-person-check-fill me-2"></i>{{ summaryStats.totalClientes || clientes.length }} Clientes Registrados
        </span>
        <button @click="loadData" class="btn btn-outline-primary btn-sm rounded-pill px-3 shadow-sm d-flex align-items-center" :disabled="loading">
          <i class="bi bi-arrow-clockwise me-1" :class="{ 'spin-anim': loading }"></i> Actualizar
        </button>
      </div>
    </div>

    <!-- TARJETAS DE ESTADÍSTICAS / KPIS -->
    <div class="row g-3 mb-4">
      <div class="col-xl-3 col-md-6">
        <div class="card border-0 shadow-sm rounded-4 h-100 bg-white p-3 stat-card-hover">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <span class="text-uppercase text-muted fw-semibold micro-text">Total Clientes</span>
              <h3 class="fw-bold text-dark mb-0 mt-1">{{ summaryStats.totalClientes || clientes.length }}</h3>
              <span class="badge bg-success-subtle text-success mt-2 rounded-pill px-2 py-1 small">
                <i class="bi bi-check-circle me-1"></i>{{ summaryStats.clientesActivos || clientes.length }} Activos
              </span>
            </div>
            <div class="stat-icon bg-primary-subtle text-primary rounded-4 d-flex align-items-center justify-content-center p-3 fs-3">
              <i class="bi bi-people"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="col-xl-3 col-md-6">
        <div class="card border-0 shadow-sm rounded-4 h-100 bg-white p-3 stat-card-hover">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <span class="text-uppercase text-muted fw-semibold micro-text">Vehículos Registrados</span>
              <h3 class="fw-bold text-dark mb-0 mt-1">{{ summaryStats.totalVehiculosRegistrados || totalVehiculosCalculados }}</h3>
              <span class="text-muted small mt-2 d-inline-block">
                <i class="bi bi-car-front text-info me-1"></i>Promedio: {{ promedioVehiculos }} veh/cli
              </span>
            </div>
            <div class="stat-icon bg-info-subtle text-info rounded-4 d-flex align-items-center justify-content-center p-3 fs-3">
              <i class="bi bi-car-front-fill"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="col-xl-3 col-md-6">
        <div class="card border-0 shadow-sm rounded-4 h-100 bg-white p-3 stat-card-hover">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <span class="text-uppercase text-muted fw-semibold micro-text">Reservas de Clientes</span>
              <h3 class="fw-bold text-dark mb-0 mt-1">{{ summaryStats.totalReservasClientes || totalReservasCalculadas }}</h3>
              <span class="text-muted small mt-2 d-inline-block">
                <i class="bi bi-arrow-repeat text-warning me-1"></i>{{ summaryStats.promedioReservasPorCliente || 0 }} lavados / cliente
              </span>
            </div>
            <div class="stat-icon bg-warning-subtle text-warning rounded-4 d-flex align-items-center justify-content-center p-3 fs-3">
              <i class="bi bi-calendar2-check-fill"></i>
            </div>
          </div>
        </div>
      </div>

      <div class="col-xl-3 col-md-6">
        <div class="card border-0 shadow-sm rounded-4 h-100 bg-white p-3 stat-card-hover">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <span class="text-uppercase text-muted fw-semibold micro-text">Ingresos Generados</span>
              <h3 class="fw-bold text-success mb-0 mt-1">Bs. {{ (summaryStats.totalIngresosClientes || totalIngresosCalculados).toLocaleString('es-BO') }}</h3>
              <span class="text-muted small mt-2 d-inline-block">
                <i class="bi bi-cash-stack me-1 text-success"></i>Facturación en plataforma
              </span>
            </div>
            <div class="stat-icon bg-success-subtle text-success rounded-4 d-flex align-items-center justify-content-center p-3 fs-3">
              <i class="bi bi-cash-coin"></i>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- SECCIÓN DE ANÁLISIS POR ZONA Y TOP CLIENTES -->
    <div class="row g-4 mb-4">
      <!-- DISTRIBUCIÓN POR ZONAS EN SANTA CRUZ -->
      <div class="col-lg-6">
        <div class="card border-0 shadow-sm rounded-4 bg-white p-4 h-100">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold mb-0 text-dark"><i class="bi bi-geo-alt-fill text-danger me-2"></i>Distribución por Zonas (Santa Cruz)</h5>
            <span class="badge bg-light text-dark border">Zonas Principales</span>
          </div>
          <div v-if="summaryStats.distribucionPorZona && summaryStats.distribucionPorZona.length > 0">
            <div v-for="z in summaryStats.distribucionPorZona" :key="z.etiqueta" class="mb-3">
              <div class="d-flex justify-content-between small mb-1">
                <span class="fw-medium text-dark"><i class="bi bi-pin-map text-primary me-1"></i>{{ z.etiqueta }}</span>
                <span class="fw-bold text-primary">{{ z.valor }} clientes</span>
              </div>
              <div class="progress rounded-pill" style="height: 10px;">
                <div class="progress-bar bg-gradient-primary rounded-pill" :style="{ width: getZonaPercentage(z.valor) + '%' }"></div>
              </div>
            </div>
          </div>
          <div v-else class="text-center text-muted py-4">
            <i class="bi bi-geo fs-1 d-block mb-2"></i>
            Procesando geolocalización de zonas...
          </div>
        </div>
      </div>

      <!-- TOP CLIENTES VIP / FRECUENTES -->
      <div class="col-lg-6">
        <div class="card border-0 shadow-sm rounded-4 bg-white p-4 h-100">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold mb-0 text-dark"><i class="bi bi-star-fill text-warning me-2"></i>Clientes VIP Frecuentes</h5>
            <span class="badge bg-warning-subtle text-dark border border-warning-subtle">Top Fieles</span>
          </div>
          <div v-if="topClientesList.length > 0" class="list-group list-group-flush border-0">
            <div v-for="(top, idx) in topClientesList" :key="top.id" class="list-group-item px-0 py-2 border-bottom border-light bg-transparent d-flex align-items-center justify-content-between">
              <div class="d-flex align-items-center gap-3">
                <span class="avatar-circle rounded-circle bg-primary text-white fw-bold d-flex align-items-center justify-content-center" style="width: 38px; height: 38px;">
                  {{ idx + 1 }}
                </span>
                <div>
                  <h6 class="mb-0 fw-bold text-dark">{{ top.nombreCompleto }}</h6>
                  <small class="text-muted"><i class="bi bi-geo-alt me-1"></i>{{ top.zonaPrincipal || top.ciudad || 'Santa Cruz' }}</small>
                </div>
              </div>
              <div class="text-end">
                <span class="badge bg-primary rounded-pill px-2 py-1 mb-1 d-inline-block">{{ top.totalReservas }} lavados</span>
                <div class="small fw-bold text-success">Bs. {{ (top.totalGastado || 0).toLocaleString('es-BO') }}</div>
              </div>
            </div>
          </div>
          <div v-else class="text-center text-muted py-4">Cargando métricas de clientes VIP...</div>
        </div>
      </div>
    </div>

    <!-- CONTROLES DE BÚSQUEDA Y FILTROS -->
    <div class="card border-0 shadow-sm rounded-4 bg-white p-4 mb-4">
      <div class="row g-3 align-items-center">
        <div class="col-md-5">
          <div class="input-group">
            <span class="input-group-text bg-light border-end-0 text-muted"><i class="bi bi-search"></i></span>
            <input
              v-model="searchQuery"
              type="text"
              class="form-control bg-light border-start-0 ps-0 shadow-none"
              placeholder="Buscar por Nombre, Email, Teléfono, CI o Zona..."
            />
            <button v-if="searchQuery" @click="searchQuery = ''" class="btn btn-light border-start-0 text-muted" type="button">
              <i class="bi bi-x-lg"></i>
            </button>
          </div>
        </div>

        <div class="col-md-3">
          <select v-model="selectedZone" class="form-select bg-light shadow-none">
            <option value="">Todas las Zonas</option>
            <option v-for="z in zoneOptions" :key="z" :value="z">{{ z }}</option>
          </select>
        </div>

        <div class="col-md-2">
          <select v-model="selectedStatus" class="form-select bg-light shadow-none">
            <option value="">Todos los Estados</option>
            <option value="activo">Solo Activos</option>
            <option value="inactivo">Solo Inactivos</option>
          </select>
        </div>

        <div class="col-md-2 text-end">
          <select v-model.number="itemsPerPage" class="form-select bg-light shadow-none">
            <option :value="10">10 por pág.</option>
            <option :value="25">25 por pág.</option>
            <option :value="50">50 por pág.</option>
            <option :value="100">100 por pág.</option>
          </select>
        </div>
      </div>
    </div>

    <!-- TABLA DE CLIENTES -->
    <div class="card border-0 shadow-sm rounded-4 bg-white overflow-hidden">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th class="ps-4">Cliente</th>
              <th>Contacto</th>
              <th>CI / Documento</th>
              <th>Zona / Dirección</th>
              <th>Vehículos</th>
              <th>Reservas</th>
              <th>Total Gastado</th>
              <th class="text-center pe-4">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading" class="text-center">
              <td colspan="8" class="py-5 text-muted">
                <div class="spinner-border text-primary me-2" role="status"></div>
                Cargando 100 clientes de la base de datos...
              </td>
            </tr>
            <tr v-else-if="paginatedClientes.length === 0" class="text-center">
              <td colspan="8" class="py-5 text-muted">
                <i class="bi bi-person-exclamation fs-2 d-block mb-2"></i>
                No se encontraron clientes que coincidan con la búsqueda.
              </td>
            </tr>
            <tr v-for="c in paginatedClientes" :key="c.id" class="client-row">
              <td class="ps-4">
                <div class="d-flex align-items-center gap-3">
                  <div class="avatar-initials bg-primary-subtle text-primary fw-bold rounded-circle d-flex align-items-center justify-content-center">
                    {{ getInitials(c.nombreCompleto) }}
                  </div>
                  <div>
                    <div class="fw-bold text-dark">{{ c.nombreCompleto }}</div>
                    <small class="text-muted">Reg: {{ formatDate(c.fechaRegistro) }}</small>
                  </div>
                </div>
              </td>
              <td>
                <div><i class="bi bi-envelope text-muted me-1"></i><small>{{ c.email }}</small></div>
                <div v-if="c.telefono" class="small text-muted"><i class="bi bi-telephone text-muted me-1"></i>{{ c.telefono }}</div>
              </td>
              <td>
                <span class="badge bg-light text-dark border">{{ c.ci || 'Sin CI' }}</span>
              </td>
              <td>
                <div class="fw-medium text-dark"><i class="bi bi-geo-alt-fill text-danger me-1"></i>{{ c.zonaPrincipal || 'Santa Cruz' }}</div>
                <small class="text-muted d-inline-block text-truncate" style="max-width: 180px;" :title="c.direccion">{{ c.direccion || 'Sin dirección' }}</small>
              </td>
              <td>
                <span class="badge bg-info-subtle text-info-emphasis rounded-pill px-3 py-1">
                  <i class="bi bi-car-front me-1"></i>{{ c.totalVehiculos }} veh.
                </span>
              </td>
              <td>
                <span class="badge bg-primary-subtle text-primary rounded-pill px-3 py-1 fw-bold">
                  {{ c.totalReservas }} reservas
                </span>
              </td>
              <td class="fw-bold text-success">
                Bs. {{ (c.totalGastado || 0).toLocaleString('es-BO') }}
              </td>
              <td class="text-center pe-4">
                <button @click="verDetalle(c)" class="btn btn-sm btn-outline-primary rounded-pill px-3 shadow-sm">
                  <i class="bi bi-eye-fill me-1"></i>Detalles
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- PIE DE PAGINACIÓN -->
      <div v-if="filteredClientes.length > 0" class="card-footer bg-white border-top py-3 d-flex flex-wrap align-items-center justify-content-between gap-2 px-4">
        <small class="text-muted">
          Mostrando <strong>{{ startItem }}</strong> a <strong>{{ endItem }}</strong> de <strong>{{ filteredClientes.length }}</strong> clientes
        </small>
        <nav>
          <ul class="pagination pagination-sm mb-0 rounded-pill">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <button @click="currentPage--" class="page-item-btn page-link rounded-start-pill">Anterior</button>
            </li>
            <li v-for="page in totalPages" :key="page" class="page-item" :class="{ active: currentPage === page }">
              <button @click="currentPage = page" class="page-item-btn page-link">{{ page }}</button>
            </li>
            <li class="page-item" :class="{ disabled: currentPage === totalPages || totalPages === 0 }">
              <button @click="currentPage++" class="page-item-btn page-link rounded-end-pill">Siguiente</button>
            </li>
          </ul>
        </nav>
      </div>
    </div>

    <!-- MODAL DE DETALLE DEL CLIENTE -->
    <div v-if="selectedCliente" class="modal fade show d-block" tabindex="-1" style="background: rgba(0,0,0,0.5);" @click.self="selectedCliente = null">
      <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content border-0 shadow-lg rounded-4 overflow-hidden">
          <div class="modal-header bg-gradient-primary text-white p-4">
            <div class="d-flex align-items-center gap-3">
              <div class="avatar-lg bg-white text-primary rounded-circle d-flex align-items-center justify-content-center fw-bold fs-4" style="width: 54px; height: 54px;">
                {{ getInitials(selectedCliente.nombreCompleto) }}
              </div>
              <div>
                <h5 class="modal-title fw-bold text-white mb-0">{{ selectedCliente.nombreCompleto }}</h5>
                <small class="text-white-50"><i class="bi bi-envelope me-1"></i>{{ selectedCliente.email }} | <i class="bi bi-telephone me-1"></i>{{ selectedCliente.telefono || 'Sin teléfono' }}</small>
              </div>
            </div>
            <button type="button" class="btn-close btn-close-white" @click="selectedCliente = null"></button>
          </div>
          <div class="modal-body p-4">
            <div class="row g-3 mb-4">
              <div class="col-md-4">
                <div class="p-3 bg-light rounded-3 text-center">
                  <small class="text-muted d-block">CI / Documento</small>
                  <strong class="text-dark fs-6">{{ selectedCliente.ci || 'No especificado' }}</strong>
                </div>
              </div>
              <div class="col-md-4">
                <div class="p-3 bg-light rounded-3 text-center">
                  <small class="text-muted d-block">Ciudad / Zona</small>
                  <strong class="text-dark fs-6">{{ selectedCliente.zonaPrincipal }} ({{ selectedCliente.ciudad || 'Santa Cruz' }})</strong>
                </div>
              </div>
              <div class="col-md-4">
                <div class="p-3 bg-light rounded-3 text-center">
                  <small class="text-muted d-block">Total Gastado</small>
                  <strong class="text-success fs-6">Bs. {{ (selectedCliente.totalGastado || 0).toLocaleString('es-BO') }}</strong>
                </div>
              </div>
            </div>

            <!-- VEHÍCULOS DEL CLIENTE -->
            <h6 class="fw-bold mb-3 text-dark"><i class="bi bi-car-front-fill text-primary me-2"></i>Vehículos Registrados ({{ selectedCliente.vehiculos ? selectedCliente.vehiculos.length : 0 }})</h6>
            <div v-if="selectedCliente.vehiculos && selectedCliente.vehiculos.length > 0" class="row g-2 mb-4">
              <div v-for="v in selectedCliente.vehiculos" :key="v.id" class="col-md-6">
                <div class="border rounded-3 p-3 bg-white shadow-sm d-flex align-items-center justify-content-between">
                  <div>
                    <h6 class="fw-bold mb-0 text-primary"><i class="bi bi-car-front me-1"></i>{{ v.marca }} {{ v.modelo }} ({{ v.año }})</h6>
                    <small class="text-muted">Tipo: {{ v.tipo }} | Color: {{ v.color }}</small>
                  </div>
                  <span class="badge bg-dark px-3 py-2 font-monospace fs-6">{{ v.placa }}</span>
                </div>
              </div>
            </div>
            <div v-else class="text-muted small mb-4">No tiene vehículos registrados activamente.</div>

            <!-- DIRECCIONES / UBICACIONES -->
            <h6 class="fw-bold mb-3 text-dark"><i class="bi bi-geo-alt-fill text-danger me-2"></i>Ubicaciones Registradas</h6>
            <div v-if="selectedCliente.ubicaciones && selectedCliente.ubicaciones.length > 0" class="list-group mb-3">
              <div v-for="u in selectedCliente.ubicaciones" :key="u.id" class="list-group-item border rounded-3 mb-2 bg-light">
                <div class="d-flex justify-content-between align-items-center">
                  <span class="fw-bold text-dark"><i class="bi bi-pin-map me-1 text-danger"></i>{{ u.zona }}</span>
                  <span v-if="u.esPrincipal" class="badge bg-primary text-white">Principal</span>
                </div>
                <small class="d-block text-dark mt-1">{{ u.direccion }}</small>
                <small v-if="u.referencia" class="text-muted italic"><i class="bi bi-info-circle me-1"></i>Ref: {{ u.referencia }}</small>
              </div>
            </div>
            <div v-else class="text-muted small">No hay direcciones registradas.</div>
          </div>
          <div class="modal-footer bg-light px-4 py-3 border-0">
            <button type="button" class="btn btn-secondary rounded-pill px-4" @click="selectedCliente = null">Cerrar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../../services/api'

const clientes = ref([])
const summaryStats = ref({
  totalClientes: 0,
  clientesActivos: 0,
  totalVehiculosRegistrados: 0,
  totalReservasClientes: 0,
  promedioReservasPorCliente: 0,
  totalIngresosClientes: 0,
  distribucionPorZona: [],
  topClientesFrecuentes: [],
  clientesRecientes: []
})

const loading = ref(false)
const searchQuery = ref('')
const selectedZone = ref('')
const selectedStatus = ref('')
const currentPage = ref(1)
const itemsPerPage = ref(10)
const selectedCliente = ref(null)

const loadData = async () => {
  loading.value = true
  try {
    const [resClientes, resSummary] = await Promise.all([
      api.get('/Cliente'),
      api.get('/Cliente/dashboard')
    ])

    if (resClientes.data && resClientes.data.success) {
      clientes.value = resClientes.data.data
    }
    if (resSummary.data && resSummary.data.success) {
      summaryStats.value = resSummary.data.data
    }
  } catch (e) {
    console.error('Error al cargar clientes:', e)
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

// Zonas únicas disponibles para filtro
const zoneOptions = computed(() => {
  const setZonas = new Set()
  clientes.value.forEach(c => {
    if (c.zonaPrincipal) setZonas.add(c.zonaPrincipal)
  })
  return Array.from(setZonas).sort()
})

// Clientes filtrados por búsqueda, zona y estado
const filteredClientes = computed(() => {
  return clientes.value.filter(c => {
    const query = searchQuery.value.toLowerCase().trim()
    const matchesQuery = !query || (
      c.nombreCompleto.toLowerCase().includes(query) ||
      c.email.toLowerCase().includes(query) ||
      (c.telefono && c.telefono.toLowerCase().includes(query)) ||
      (c.ci && c.ci.toLowerCase().includes(query)) ||
      (c.zonaPrincipal && c.zonaPrincipal.toLowerCase().includes(query))
    )

    const matchesZone = !selectedZone.value || c.zonaPrincipal === selectedZone.value

    let matchesStatus = true
    if (selectedStatus.value === 'activo') matchesStatus = c.activo === true
    if (selectedStatus.value === 'inactivo') matchesStatus = c.activo === false

    return matchesQuery && matchesZone && matchesStatus
  })
})

// Paginación
const totalPages = computed(() => Math.ceil(filteredClientes.value.length / itemsPerPage.value) || 1)

const paginatedClientes = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredClientes.value.slice(start, start + itemsPerPage.value)
})

const startItem = computed(() => (filteredClientes.value.length === 0) ? 0 : (currentPage.value - 1) * itemsPerPage.value + 1)
const endItem = computed(() => Math.min(currentPage.value * itemsPerPage.value, filteredClientes.value.length))

// Cálculos auxiliares
const totalVehiculosCalculados = computed(() => clientes.value.reduce((acc, c) => acc + (c.totalVehiculos || 0), 0))
const totalReservasCalculadas = computed(() => clientes.value.reduce((acc, c) => acc + (c.totalReservas || 0), 0))
const totalIngresosCalculados = computed(() => clientes.value.reduce((acc, c) => acc + (c.totalGastado || 0), 0))
const promedioVehiculos = computed(() => clientes.value.length ? (totalVehiculosCalculados.value / clientes.value.length).toFixed(1) : '0')

const topClientesList = computed(() => {
  if (summaryStats.value.topClientesFrecuentes && summaryStats.value.topClientesFrecuentes.length > 0) {
    return summaryStats.value.topClientesFrecuentes
  }
  return [...clientes.value].sort((a, b) => (b.totalReservas || 0) - (a.totalReservas || 0)).slice(0, 5)
})

const getZonaPercentage = (val) => {
  const total = summaryStats.value.totalClientes || clientes.value.length || 1
  return Math.min(Math.round((val / total) * 100 * 3), 100) // Escala visual adecuada
}

const getInitials = (name) => {
  if (!name) return 'CL'
  const parts = name.trim().split(' ')
  if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase()
  return name.substring(0, 2).toUpperCase()
}

const formatDate = (d) => {
  if (!d) return 'N/A'
  return new Date(d).toLocaleDateString('es-BO', { year: 'numeric', month: 'short', day: 'numeric' })
}

const verDetalle = (c) => {
  selectedCliente.value = c
}
</script>

<style scoped>
.bg-gradient-primary {
  background: linear-gradient(135deg, #0d6efd 0%, #0a58ca 100%);
}
.stat-card-hover {
  transition: transform 0.2s ease, shadow 0.2s ease;
}
.stat-card-hover:hover {
  transform: translateY(-3px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.08) !important;
}
.micro-text {
  font-size: 0.72rem;
  letter-spacing: 0.5px;
}
.avatar-initials {
  width: 42px;
  height: 42px;
  font-size: 0.95rem;
}
.client-row:hover {
  background-color: rgba(13, 110, 253, 0.02);
}
.page-item-btn {
  color: #0d6efd;
  cursor: pointer;
}
.spin-anim {
  animation: spin 1s linear infinite;
}
@keyframes spin {
  100% { transform: rotate(360deg); }
}
</style>
