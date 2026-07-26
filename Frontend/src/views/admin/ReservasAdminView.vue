<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Gestión de Reservas</h3>
        <p class="text-muted small">Administración y seguimiento de todas las reservas de la plataforma</p>
      </div>
    </div>

    <div class="ecowash-card">
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>ID</th>
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
            <tr v-for="r in reservas" :key="r.id">
              <td>#{{ r.id }}</td>
              <td class="fw-semibold">{{ r.nombreCliente }}</td>
              <td>{{ r.nombreEmpleado }}</td>
              <td>{{ r.nombreServicio }}</td>
              <td>{{ r.placaVehiculo }}</td>
              <td>{{ formatDate(r.fechaProgramada) }}</td>
              <td class="fw-bold">Bs. {{ r.precioTotal }}</td>
              <td><span :class="['badge-status', getBadgeClass(r.estado)]">{{ r.estado }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const reservas = ref([])

onMounted(async () => {
  try {
    const res = await api.get('/Reserva')
    if (res.data.success) reservas.value = res.data.data
  } catch (e) {}
})

const formatDate = (d) => new Date(d).toLocaleString('es-BO', { dateStyle: 'short', timeStyle: 'short' })
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
