<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">Dashboard Administrador</h2>
        <p class="text-muted small">Resumen general de operaciones de EcoWash Móvil Santa Cruz</p>
      </div>
      <button @click="loadData" class="btn btn-outline-secondary btn-sm"><i class="bi bi-arrow-clockwise me-1"></i>Actualizar</button>
    </div>

    <!-- CARDS ESTADÍSTICAS -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="stat-card">
          <div>
            <div class="text-muted small">Total Clientes</div>
            <div class="fs-3 fw-bold">{{ stats.totalClientes }}</div>
          </div>
          <div class="stat-icon-wrapper stat-icon-blue"><i class="bi bi-people"></i></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div>
            <div class="text-muted small">Reservas Hoy</div>
            <div class="fs-3 fw-bold">{{ stats.reservasHoy }}</div>
          </div>
          <div class="stat-icon-wrapper stat-icon-amber"><i class="bi bi-calendar-event"></i></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div>
            <div class="text-muted small">Ventas de Hoy</div>
            <div class="fs-3 fw-bold">Bs. {{ stats.ventasHoy }}</div>
          </div>
          <div class="stat-icon-wrapper stat-icon-green"><i class="bi bi-currency-dollar"></i></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div>
            <div class="text-muted small">Stock Bajo</div>
            <div class="fs-3 fw-bold text-danger">{{ stats.productosStockBajo }}</div>
          </div>
          <div class="stat-icon-wrapper stat-icon-purple"><i class="bi bi-exclamation-triangle"></i></div>
        </div>
      </div>
    </div>

    <!-- TABLAS Y DETALLES -->
    <div class="row g-4">
      <div class="col-lg-7">
        <div class="ecowash-card">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold mb-0">Últimas Reservas</h5>
            <router-link to="/admin/reservas" class="small text-decoration-none">Ver todas</router-link>
          </div>
          <div class="table-responsive">
            <table class="table table-hover align-middle">
              <thead>
                <tr>
                  <th>Cliente</th>
                  <th>Servicio</th>
                  <th>Fecha</th>
                  <th>Estado</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in stats.ultimasReservas" :key="r.id">
                  <td class="fw-semibold">{{ r.nombreCliente }}</td>
                  <td>{{ r.nombreServicio }}</td>
                  <td>{{ formatDate(r.fechaProgramada) }}</td>
                  <td><span :class="['badge-status', getBadgeClass(r.estado)]">{{ r.estado }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="col-lg-5">
        <div class="ecowash-card">
          <h5 class="fw-bold mb-3">Servicios Más Solicitados</h5>
          <div v-for="s in stats.serviciosMasSolicitados" :key="s.etiqueta" class="mb-3">
            <div class="d-flex justify-content-between small mb-1">
              <span>{{ s.etiqueta }}</span>
              <span class="fw-bold">{{ s.valor }} solicitudes</span>
            </div>
            <div class="progress" style="height: 8px;">
              <div class="progress-bar bg-primary" :style="{ width: (s.valor * 10) + '%' }"></div>
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

const stats = ref({
  totalClientes: 0,
  totalEmpleados: 0,
  reservasHoy: 0,
  serviciosRealizados: 0,
  ventasHoy: 0,
  ingresosMensuales: 0,
  productosStockBajo: 0,
  ultimasReservas: [],
  serviciosMasSolicitados: []
})

const loadData = async () => {
  try {
    const res = await api.get('/Dashboard/admin')
    if (res.data.success) {
      stats.value = res.data.data
    }
  } catch (e) {}
}

onMounted(loadData)

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
