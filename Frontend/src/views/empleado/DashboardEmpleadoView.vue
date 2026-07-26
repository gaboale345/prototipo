<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">Panel de Empleado</h2>
        <p class="text-muted small">Gestión de servicios y lavados asignados</p>
      </div>
      <button @click="loadReservas" class="btn btn-outline-secondary btn-sm"><i class="bi bi-arrow-clockwise me-1"></i>Actualizar</button>
    </div>

    <!-- LISTA DE RESERVAS PARA EMPLEADO -->
    <div class="ecowash-card">
      <h5 class="fw-bold mb-3">Reservas Disponibles / Asignadas</h5>
      <div v-if="reservas.length === 0" class="text-center py-4 text-muted">
        No tienes servicios pendientes asignados por el momento.
      </div>
      <div v-else class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>Cliente</th>
              <th>Servicio</th>
              <th>Vehículo</th>
              <th>Dirección</th>
              <th>Fecha Programada</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in reservas" :key="r.id">
              <td class="fw-semibold">{{ r.nombreCliente }}</td>
              <td>{{ r.nombreServicio }}</td>
              <td>{{ r.placaVehiculo }}</td>
              <td>{{ r.direccion }}</td>
              <td>{{ formatDate(r.fechaProgramada) }}</td>
              <td><span :class="['badge-status', getBadgeClass(r.estado)]">{{ r.estado }}</span></td>
              <td>
                <div class="btn-group btn-group-sm">
                  <button v-if="r.estado === 'Pendiente'" @click="cambiarEstado(r.id, 'Aceptada')" class="btn btn-success">Aceptar</button>
                  <button v-if="r.estado === 'Aceptada'" @click="cambiarEstado(r.id, 'EnProceso')" class="btn btn-primary">Iniciar</button>
                  <button v-if="r.estado === 'EnProceso'" @click="cambiarEstado(r.id, 'Finalizada')" class="btn btn-success">Finalizar</button>
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
import { ref, onMounted } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'

const reservas = ref([])

const loadReservas = async () => {
  try {
    const res = await api.get('/Reserva')
    if (res.data.success) reservas.value = res.data.data
  } catch (e) {}
}

onMounted(loadReservas)

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
