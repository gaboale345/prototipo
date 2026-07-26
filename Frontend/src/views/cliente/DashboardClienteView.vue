<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">¡Hola, {{ authStore.nombreUsuario }}! 👋</h2>
        <p class="text-muted small">Bienvenido a tu panel de cliente de EcoWash Móvil</p>
      </div>
      <router-link to="/cliente/reservar" class="btn btn-primary-custom">
        <i class="bi bi-plus-circle me-1"></i> Solicitar Lavado
      </router-link>
    </div>

    <!-- CARDS ACCESOS RÁPIDOS -->
    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div class="ecowash-card d-flex align-items-center gap-3">
          <div class="stat-icon-wrapper stat-icon-blue"><i class="bi bi-car-front"></i></div>
          <div>
            <div class="fw-bold fs-5">{{ vehiculosCount }}</div>
            <div class="text-muted small">Mis Vehículos</div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="ecowash-card d-flex align-items-center gap-3">
          <div class="stat-icon-wrapper stat-icon-amber"><i class="bi bi-clock-history"></i></div>
          <div>
            <div class="fw-bold fs-5">{{ reservasCount }}</div>
            <div class="text-muted small">Reservas Realizadas</div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="ecowash-card d-flex align-items-center gap-3">
          <div class="stat-icon-wrapper stat-icon-green"><i class="bi bi-geo-alt"></i></div>
          <div>
            <div class="fw-bold fs-5">{{ ubicacionesCount }}</div>
            <div class="text-muted small">Mis Ubicaciones</div>
          </div>
        </div>
      </div>
    </div>

    <!-- MIS ÚLTIMAS RESERVAS -->
    <div class="ecowash-card">
      <h5 class="fw-bold mb-3">Mis Servicios Recientes</h5>
      <div v-if="reservas.length === 0" class="text-center py-4 text-muted">
        Aún no has solicitado ningún servicio. ¡Haz tu primera reserva ahora!
      </div>
      <div v-else class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>Servicio</th>
              <th>Vehículo</th>
              <th>Ubicación</th>
              <th>Fecha</th>
              <th>Precio</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in reservas" :key="r.id">
              <td class="fw-semibold">{{ r.nombreServicio }}</td>
              <td>{{ r.placaVehiculo }}</td>
              <td>{{ r.direccion }}</td>
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
import { useAuthStore } from '../../stores/authStore'
import api from '../../services/api'

const authStore = useAuthStore()
const reservas = ref([])
const vehiculosCount = ref(0)
const reservasCount = ref(0)
const ubicacionesCount = ref(0)

onMounted(async () => {
  try {
    const resRes = await api.get('/Reserva')
    if (resRes.data.success) {
      reservas.value = resRes.data.data
      reservasCount.value = reservas.value.length
    }
    const resVeh = await api.get('/Vehiculo')
    if (resVeh.data.success) vehiculosCount.value = resVeh.data.data.length

    const resUbi = await api.get('/Ubicacion')
    if (resUbi.data.success) ubicacionesCount.value = resUbi.data.data.length
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
