<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
      <div>
        <h3 class="fw-bold mb-1">Gestión de Reservas</h3>
        <p class="text-muted small">Administración y seguimiento de todas las reservas (Ordenadas por fecha de solicitud)</p>
      </div>
      <div class="d-flex gap-2">
        <input type="text" v-model="busqueda" class="form-control form-control-sm" placeholder="Buscar por cliente, placa o ID..." style="max-width: 250px;" />
        <button @click="cargar" class="btn btn-outline-secondary btn-sm"><i class="bi bi-arrow-clockwise me-1"></i>Actualizar</button>
      </div>
    </div>

    <div class="ecowash-card">
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>ID</th>
              <th>Solicitado El</th>
              <th>Cliente</th>
              <th>Empleado Asignado</th>
              <th>Servicio</th>
              <th>Vehículo</th>
              <th>Fecha Programada</th>
              <th>Monto</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in reservasFiltradas" :key="r.id">
              <td>#{{ r.id }}</td>
              <td class="small text-muted">{{ formatDate(r.fechaCreacion) }}</td>
              <td class="fw-semibold">{{ r.nombreCliente }}</td>
              <td>{{ r.nombreEmpleado }}</td>
              <td><span class="badge bg-light text-dark border">{{ r.nombreServicio }}</span></td>
              <td>{{ r.placaVehiculo }}</td>
              <td class="fw-bold text-primary">{{ formatDate(r.fechaProgramada) }}</td>
              <td class="fw-bold text-success">Bs. {{ r.precioTotal }}</td>
              <td><span :class="['badge-status', getBadgeClass(r.estado)]">{{ r.estado }}</span></td>
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

const reservas = ref([])
const busqueda = ref('')

const cargar = async () => {
  try {
    const res = await api.get('/Reserva')
    if (res.data.success) reservas.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

const reservasFiltradas = computed(() => {
  if (!busqueda.value.trim()) return reservas.value
  const q = busqueda.value.toLowerCase()
  return reservas.value.filter(r =>
    r.id.toString().includes(q) ||
    r.nombreCliente.toLowerCase().includes(q) ||
    r.placaVehiculo.toLowerCase().includes(q) ||
    r.nombreServicio.toLowerCase().includes(q)
  )
})

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
