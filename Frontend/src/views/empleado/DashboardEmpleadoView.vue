<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
      <div>
        <h2 class="fw-bold mb-1">Panel de Empleado</h2>
        <p class="text-muted small">Gestión y atención de servicios asignados a domicilio</p>
      </div>
      <button @click="loadReservas" class="btn btn-outline-secondary btn-sm"><i class="bi bi-arrow-clockwise me-1"></i>Actualizar</button>
    </div>

    <!-- FILTROS AVANZADOS POR FECHA, HORA Y ESTADO -->
    <div class="ecowash-card mb-4 bg-light">
      <div class="row g-3 align-items-center">
        <!-- Filtro Rápido -->
        <div class="col-md-3">
          <label class="form-label fw-bold small text-muted">Periodo / Rápido</label>
          <select v-model="filtroPeriodo" class="form-select form-select-sm" @change="aplicarFiltroPeriodo">
            <option value="todos">Todas las fechas</option>
            <option value="hoy">Hoy</option>
            <option value="semana">Esta Semana</option>
            <option value="mes">Este Mes</option>
          </select>
        </div>

        <!-- Filtro por Fecha Específica -->
        <div class="col-md-3">
          <label class="form-label fw-bold small text-muted">Filtrar por Día</label>
          <input type="date" v-model="filtroFechaEspecifica" class="form-control form-control-sm" />
        </div>

        <!-- Filtro por Turno / Hora -->
        <div class="col-md-3">
          <label class="form-label fw-bold small text-muted">Turno de Atención</label>
          <select v-model="filtroTurno" class="form-select form-select-sm">
            <option value="todos">Todos los horarios (9 AM - 5 PM)</option>
            <option value="manana">Mañana (9:00 AM - 1:00 PM)</option>
            <option value="tarde">Tarde (1:00 PM - 5:00 PM)</option>
          </select>
        </div>

        <!-- Filtro por Estado -->
        <div class="col-md-3">
          <label class="form-label fw-bold small text-muted">Estado del Servicio</label>
          <select v-model="filtroEstado" class="form-select form-select-sm">
            <option value="todos">Todos los estados</option>
            <option value="Pendiente">Pendiente</option>
            <option value="Aceptada">Aceptada</option>
            <option value="EnProceso">En Proceso</option>
            <option value="Finalizada">Finalizada</option>
          </select>
        </div>
      </div>

      <!-- Resumen de Filtros Activos -->
      <div class="mt-3 d-flex justify-content-between align-items-center flex-wrap gap-2 pt-2 border-top">
        <span class="small text-muted">
          Mostrando <strong>{{ reservasFiltradas.length }}</strong> de {{ reservas.length }} reservas (Ordenadas por <strong>Fecha de Solicitud</strong>)
        </span>
        <button v-if="tieneFiltrosActivos" @click="limpiarFiltros" class="btn btn-sm btn-link text-decoration-none p-0 text-danger">
          <i class="bi bi-x-circle me-1"></i>Limpiar Filtros
        </button>
      </div>
    </div>

    <!-- LISTA DE RESERVAS -->
    <div class="ecowash-card">
      <h5 class="fw-bold mb-3"><i class="bi bi-calendar-check text-primary me-2"></i>Agenda de Servicios</h5>
      <div v-if="reservasFiltradas.length === 0" class="text-center py-5 text-muted">
        <i class="bi bi-calendar-x fs-1 d-block mb-2"></i>
        No hay servicios que coincidan con los filtros seleccionados.
      </div>
      <div v-else class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>ID</th>
              <th>Solicitado El</th>
              <th>Cliente</th>
              <th>Servicio</th>
              <th>Vehículo</th>
              <th>Dirección</th>
              <th>Fecha Programada</th>
              <th>Estado</th>
              <th>Acción</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in reservasFiltradas" :key="r.id">
              <td>#{{ r.id }}</td>
              <td class="small text-muted">{{ formatDate(r.fechaCreacion) }}</td>
              <td class="fw-semibold">{{ r.nombreCliente }}</td>
              <td><span class="badge bg-light text-dark border">{{ r.nombreServicio }}</span></td>
              <td>{{ r.placaVehiculo }}</td>
              <td><i class="bi bi-geo-alt me-1 text-danger"></i>{{ r.direccion }}</td>
              <td class="fw-bold text-primary">{{ formatDate(r.fechaProgramada) }}</td>
              <td><span :class="['badge-status', getBadgeClass(r.estado)]">{{ r.estado }}</span></td>
              <td>
                <div class="btn-group btn-group-sm">
                  <button v-if="r.estado === 'Pendiente'" @click="cambiarEstado(r.id, 'Aceptada')" class="btn btn-success fw-bold">
                    <i class="bi bi-check-lg me-1"></i>Aceptar
                  </button>
                  <button v-if="r.estado === 'Aceptada'" @click="cambiarEstado(r.id, 'EnProceso')" class="btn btn-primary fw-bold">
                    <i class="bi bi-play-fill me-1"></i>Iniciar
                  </button>
                  <button v-if="r.estado === 'EnProceso'" @click="cambiarEstado(r.id, 'Finalizada')" class="btn btn-success fw-bold">
                    <i class="bi bi-check2-all me-1"></i>Finalizar
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'

const reservas = ref([])
const filtroPeriodo = ref('todos')
const filtroFechaEspecifica = ref('')
const filtroTurno = ref('todos')
const filtroEstado = ref('todos')

const loadReservas = async () => {
  try {
    const res = await api.get('/Reserva')
    if (res.data.success) {
      reservas.value = res.data.data
    }
  } catch (e) {}
}

onMounted(loadReservas)

// Aplicar período predeterminado
const aplicarFiltroPeriodo = () => {
  filtroFechaEspecifica.value = ''
}

const tieneFiltrosActivos = computed(() => {
  return filtroPeriodo.value !== 'todos' || filtroFechaEspecifica.value !== '' || filtroTurno.value !== 'todos' || filtroEstado.value !== 'todos'
})

const limpiarFiltros = () => {
  filtroPeriodo.value = 'todos'
  filtroFechaEspecifica.value = ''
  filtroTurno.value = 'todos'
  filtroEstado.value = 'todos'
}

// Computada de reservas filtradas por fecha, día, mes, hora y estado
const reservasFiltradas = computed(() => {
  return reservas.value.filter(r => {
    const fechaProg = new Date(r.fechaProgramada)
    const fechaCreacion = new Date(r.fechaCreacion || r.fechaProgramada)
    const hoy = new Date()

    // 1. Filtro por Fecha Específica (input date)
    if (filtroFechaEspecifica.value) {
      const targetDate = new Date(filtroFechaEspecifica.value + 'T00:00:00')
      const sameDay = fechaProg.getFullYear() === targetDate.getFullYear() &&
                      fechaProg.getMonth() === targetDate.getMonth() &&
                      fechaProg.getDate() === targetDate.getDate()
      if (!sameDay) return false
    }

    // 2. Filtro Rápido Periodo
    if (filtroPeriodo.value === 'hoy') {
      const sameDay = fechaProg.getFullYear() === hoy.getFullYear() &&
                      fechaProg.getMonth() === hoy.getMonth() &&
                      fechaProg.getDate() === hoy.getDate()
      if (!sameDay) return false
    } else if (filtroPeriodo.value === 'semana') {
      const diffTime = Math.abs(hoy - fechaProg)
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
      if (diffDays > 7) return false
    } else if (filtroPeriodo.value === 'mes') {
      const sameMonth = fechaProg.getFullYear() === hoy.getFullYear() &&
                        fechaProg.getMonth() === hoy.getMonth()
      if (!sameMonth) return false
    }

    // 3. Filtro por Turno / Hora
    const hora = fechaProg.getHours()
    if (filtroTurno.value === 'manana' && (hora < 9 || hora >= 13)) return false
    if (filtroTurno.value === 'tarde' && (hora < 13 || hora >= 17)) return false

    // 4. Filtro por Estado
    if (filtroEstado.value !== 'todos' && r.estado.toLowerCase() !== filtroEstado.value.toLowerCase()) {
      return false
    }

    return true
  })
})

const cambiarEstado = async (id, nuevoEstado) => {
  try {
    const res = await api.put(`/Reserva/${id}/estado`, { estado: nuevoEstado })
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Estado Actualizado', text: res.data.message, timer: 1500, showConfirmButton: false })
      loadReservas()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al cambiar estado', 'error')
  }
}

const formatDate = (d) => d ? new Date(d).toLocaleString('es-BO', { dateStyle: 'short', timeStyle: 'short' }) : 'N/A'
const getBadgeClass = (estado) => {
  switch (estado) {
    case 'Pendiente': return 'badge-pendiente'
    case 'Aceptada': return 'badge-aceptada'
    case 'EnProceso': return 'badge-enproceso'
    case 'Finalizada': return 'badge-finalizada'
    default: return 'badge-cancelada'
  }
}
</script>
