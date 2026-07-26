<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Mis Reservas</h3>
        <p class="text-muted small">Historial y estado de tus solicitudes de lavado</p>
      </div>
      <router-link to="/cliente/reservar" class="btn btn-primary-custom"><i class="bi bi-plus-lg me-1"></i>Nueva Reserva</router-link>
    </div>

    <div class="ecowash-card">
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>ID</th>
              <th>Servicio</th>
              <th>Vehículo</th>
              <th>Ubicación</th>
              <th>Fecha / Hora</th>
              <th>Monto</th>
              <th>Estado</th>
              <th>Acción</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in reservas" :key="r.id">
              <td>#{{ r.id }}</td>
              <td class="fw-semibold">{{ r.nombreServicio }}</td>
              <td>{{ r.placaVehiculo }}</td>
              <td>{{ r.direccion }}</td>
              <td>{{ formatDate(r.fechaProgramada) }}</td>
              <td class="fw-bold text-success">Bs. {{ r.precioTotal }}</td>
              <td><span :class="['badge-status', getBadgeClass(r.estado)]">{{ r.estado }}</span></td>
              <td>
                <button v-if="r.estado === 'Aceptada' || r.estado === 'Finalizada'" @click="abrirModalPago(r)" class="btn btn-sm btn-outline-success">
                  <i class="bi bi-credit-card me-1"></i>Pagar / Ver Factura
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- MODAL PAGO -->
    <div v-if="selectedReserva" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold">Pagar Servicio #{{ selectedReserva.id }}</h5>
            <button type="button" class="btn-close" @click="selectedReserva = null"></button>
          </div>
          <div class="modal-body">
            <div class="p-3 bg-light rounded-3 mb-3">
              <div class="d-flex justify-content-between mb-1">
                <span>Servicio:</span>
                <span class="fw-bold">{{ selectedReserva.nombreServicio }}</span>
              </div>
              <div class="d-flex justify-content-between mb-1">
                <span>Total a Pagar:</span>
                <span class="fw-extrabold text-primary fs-5">Bs. {{ selectedReserva.precioTotal }}</span>
              </div>
            </div>

            <div class="mb-3">
              <label class="form-label fw-semibold">Método de Pago</label>
              <select v-model="pagoForm.metodoPagoId" class="form-select">
                <option :value="1">Efectivo al lavador</option>
                <option :value="2">Pago por QR</option>
                <option :value="3">Transferencia Bancaria</option>
                <option :value="4">Tarjeta Débito/Crédito</option>
              </select>
            </div>

            <div class="mb-3">
              <label class="form-label fw-semibold">N° de Referencia / Transacción (opcional)</label>
              <input type="text" v-model="pagoForm.referencia" class="form-control" placeholder="Ej: REF-987456" />
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="selectedReserva = null">Cancelar</button>
            <button type="button" class="btn btn-success fw-bold" @click="procesarPago">Confirmar Pago</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'

const reservas = ref([])
const selectedReserva = ref(null)
const pagoForm = ref({
  metodoPagoId: 2,
  referencia: ''
})

const cargar = async () => {
  try {
    const res = await api.get('/Reserva')
    if (res.data.success) reservas.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

const abrirModalPago = (r) => {
  selectedReserva.value = r
}

const procesarPago = async () => {
  try {
    // Buscar la venta asociada
    const vRes = await api.get('/Venta')
    const venta = vRes.data.data.find(v => v.reservaId === selectedReserva.value.id)

    if (!venta) {
      Swal.fire('Atención', 'La venta para esta reserva aún no ha sido emitida.', 'info')
      return
    }

    const res = await api.post('/Pago', {
      ventaId: venta.id,
      reservaId: selectedReserva.value.id,
      metodoPagoId: pagoForm.value.metodoPagoId,
      monto: selectedReserva.value.precioTotal,
      referencia: pagoForm.value.referencia
    })

    if (res.data.success) {
      Swal.fire('¡Pago Exitoso!', res.data.message, 'success')
      selectedReserva.value = null
      cargar()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al procesar el pago', 'error')
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
