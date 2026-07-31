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
      <div v-if="reservas.length === 0" class="text-center py-5 text-muted">
        <i class="bi bi-calendar-x fs-1 d-block mb-3"></i>
        No tienes reservas aún.
        <div class="mt-3">
          <router-link to="/cliente/reservar" class="btn btn-primary-custom btn-sm">Hacer mi primera reserva</router-link>
        </div>
      </div>
      <div v-else class="table-responsive">
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
              <th>Acciones</th>
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
                <div class="d-flex gap-1 flex-wrap">
                  <!-- Ver Detalle -->
                  <button @click="verDetalle(r)" class="btn btn-sm btn-outline-primary border-0" title="Ver detalle">
                    <i class="bi bi-eye"></i>
                  </button>
                  <!-- Editar (solo Pendiente) -->
                  <button v-if="r.estado === 'Pendiente'" @click="abrirEditar(r)" class="btn btn-sm btn-outline-warning border-0" title="Editar reserva">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <!-- Cancelar (solo Pendiente) -->
                  <button v-if="r.estado === 'Pendiente'" @click="cancelarReserva(r)" class="btn btn-sm btn-outline-danger border-0" title="Cancelar reserva">
                    <i class="bi bi-x-circle"></i>
                  </button>
                  <!-- Pagar (Aceptada o Finalizada) (REMOVED: payment is now automatic) -->
                  <!-- Calificar (solo Finalizada) -->
                  <button v-if="r.estado === 'Finalizada'" @click="abrirCalificacion(r)" class="btn btn-sm btn-outline-warning" title="Calificar servicio">
                    <i class="bi bi-star-fill me-1"></i>Calificar
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- MODAL DETALLE -->
    <div v-if="reservaDetalle" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);" @click.self="reservaDetalle = null">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header bg-primary text-white">
            <h5 class="modal-title fw-bold"><i class="bi bi-calendar-check me-2"></i>Detalle de Reserva #{{ reservaDetalle.id }}</h5>
            <button type="button" class="btn-close btn-close-white" @click="reservaDetalle = null"></button>
          </div>
          <div class="modal-body">
            <div class="row g-3">
              <div class="col-6">
                <div class="small text-muted fw-semibold">Servicio</div>
                <div class="fw-bold">{{ reservaDetalle.nombreServicio }}</div>
              </div>
              <div class="col-6">
                <div class="small text-muted fw-semibold">Estado</div>
                <span :class="['badge-status', getBadgeClass(reservaDetalle.estado)]">{{ reservaDetalle.estado }}</span>
              </div>
              <div class="col-6">
                <div class="small text-muted fw-semibold">Vehículo</div>
                <div class="fw-bold">{{ reservaDetalle.placaVehiculo }}</div>
              </div>
              <div class="col-6">
                <div class="small text-muted fw-semibold">Monto</div>
                <div class="fw-bold text-success">Bs. {{ reservaDetalle.precioTotal }}</div>
              </div>
              <div class="col-12">
                <div class="small text-muted fw-semibold">Ubicación</div>
                <div>{{ reservaDetalle.direccion }}</div>
              </div>
              <div class="col-6">
                <div class="small text-muted fw-semibold">Fecha Programada</div>
                <div>{{ formatDate(reservaDetalle.fechaProgramada) }}</div>
              </div>
              <div class="col-6">
                <div class="small text-muted fw-semibold">Empleado</div>
                <div>{{ reservaDetalle.nombreEmpleado || 'Sin asignar' }}</div>
              </div>
              <div v-if="reservaDetalle.observaciones" class="col-12">
                <div class="small text-muted fw-semibold">Observaciones</div>
                <div class="p-2 bg-light rounded small">{{ reservaDetalle.observaciones }}</div>
              </div>
              <div class="col-6" v-if="reservaDetalle.fechaInicio">
                <div class="small text-muted fw-semibold">Inicio del Servicio</div>
                <div>{{ formatDate(reservaDetalle.fechaInicio) }}</div>
              </div>
              <div class="col-6" v-if="reservaDetalle.fechaFin">
                <div class="small text-muted fw-semibold">Fin del Servicio</div>
                <div>{{ formatDate(reservaDetalle.fechaFin) }}</div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="reservaDetalle = null">Cerrar</button>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL EDITAR -->
    <div v-if="reservaEditar" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);" @click.self="reservaEditar = null">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold"><i class="bi bi-pencil-square me-2"></i>Editar Reserva #{{ reservaEditar.id }}</h5>
            <button type="button" class="btn-close" @click="reservaEditar = null"></button>
          </div>
          <form @submit.prevent="guardarEdicion">
            <div class="modal-body">
              <div class="alert alert-info small py-2">
                <i class="bi bi-info-circle me-1"></i>
                Solo puedes modificar la fecha/hora y las observaciones. Horario disponible: <strong>Lunes a Sábado, 9:00 AM – 5:00 PM</strong>.
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Nueva Fecha y Hora</label>
                <input
                  type="datetime-local"
                  v-model="editForm.fechaProgramada"
                  class="form-control"
                  :min="fechaMinima"
                  required
                  @change="validarEditFecha"
                />
                <div v-if="errorEditFecha" class="text-danger small mt-1">
                  <i class="bi bi-exclamation-circle me-1"></i>{{ errorEditFecha }}
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Observaciones</label>
                <textarea v-model="editForm.observaciones" class="form-control" rows="3" placeholder="Indicaciones adicionales..."></textarea>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="reservaEditar = null">Cancelar</button>
              <button type="submit" class="btn btn-warning fw-bold" :disabled="!!errorEditFecha || loadingEdit">
                <span v-if="loadingEdit" class="spinner-border spinner-border-sm me-1"></span>
                Guardar Cambios
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- MODAL PAGO -->
    <div v-if="selectedReserva" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);" @click.self="selectedReserva = null">
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

            <!-- Opción 1: Pasarela de Pagos (Simulada) -->
            <div class="p-3 border rounded-3 mb-3 bg-light">
              <h6 class="fw-bold text-success mb-1">
                <i class="bi bi-shield-lock-fill me-1"></i>Pagar Online (Pasarela Simulación)
              </h6>
              <p class="text-muted small mb-2">Pago instantáneo simulado (Tarjeta / QR Ficticio) con generación de recibo PDF.</p>
              <button type="button" class="btn btn-success w-100 fw-bold py-2" @click="pagarConPasarela" :disabled="loadingPasarela">
                <span v-if="loadingPasarela" class="spinner-border spinner-border-sm me-1"></span>
                <i v-else class="bi bi-credit-card-2-front me-1"></i>
                Simular Pago de Bs. {{ selectedReserva.precioTotal }}
              </button>
            </div>

            <hr class="my-3" />
            <div class="small text-muted fw-bold mb-2">O Registrar Pago Manual:</div>

            <div class="mb-3">
              <label class="form-label fw-semibold">Método de Pago Manual</label>
              <select v-model="pagoForm.metodoPagoId" class="form-select">
                <option :value="1">Efectivo al lavador</option>
                <option :value="2">Pago por QR (Manual)</option>
                <option :value="3">Transferencia Bancaria</option>
              </select>
            </div>

            <div class="mb-3">
              <label class="form-label fw-semibold">N° de Referencia / Transacción (opcional)</label>
              <input type="text" v-model="pagoForm.referencia" class="form-control" placeholder="Ej: REF-987456" />
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="selectedReserva = null">Cancelar</button>
            <button type="button" class="btn btn-outline-success fw-bold" @click="procesarPago">Registrar Pago Manual</button>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL CALIFICACIÓN (RF-10) -->
    <div v-if="reservaCalificar" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);" @click.self="reservaCalificar = null">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header bg-warning text-dark">
            <h5 class="modal-title fw-bold"><i class="bi bi-star-fill me-2"></i>Calificar Servicio #{{ reservaCalificar.id }}</h5>
            <button type="button" class="btn-close" @click="reservaCalificar = null"></button>
          </div>
          <form @submit.prevent="enviarCalificacion">
            <div class="modal-body text-center">
              <p class="text-muted small mb-2">¿Cómo evaluarías la atención del servicio <strong>{{ reservaCalificar.nombreServicio }}</strong>?</p>
              
              <!-- Selección de Estrellas (1 a 5) -->
              <div class="d-flex justify-content-center gap-2 mb-3">
                <button
                  type="button"
                  v-for="star in 5"
                  :key="star"
                  @click="calificacionForm.puntuacion = star"
                  class="btn p-1 fs-2 border-0 bg-transparent transition-all"
                  :title="star + ' Estrellas'"
                >
                  <i :class="['bi', star <= calificacionForm.puntuacion ? 'bi-star-fill text-warning' : 'bi-star text-muted']"></i>
                </button>
              </div>
              <div class="fw-bold fs-5 text-warning mb-3">
                {{ calificacionForm.puntuacion }} / 5 Estrellas
              </div>

              <div class="text-start mb-3">
                <label class="form-label fw-semibold">Comentario / Opinión (opcional)</label>
                <textarea v-model="calificacionForm.comentario" class="form-control" rows="3" placeholder="Ej: Excelente servicio, el auto quedó brillante y el lavador muy puntual..."></textarea>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="reservaCalificar = null">Cancelar</button>
              <button type="submit" class="btn btn-warning fw-bold" :disabled="loadingCalificacion">
                <span v-if="loadingCalificacion" class="spinner-border spinner-border-sm me-1"></span>
                Enviar Calificación
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'

const reservas = ref([])
const reservaDetalle = ref(null)
const reservaEditar = ref(null)
const reservaCalificar = ref(null)
const selectedReserva = ref(null)
const loadingEdit = ref(false)
const loadingCalificacion = ref(false)
const loadingPasarela = ref(false)
const errorEditFecha = ref('')

const editForm = ref({
  fechaProgramada: '',
  observaciones: ''
})

const calificacionForm = ref({
  puntuacion: 5,
  comentario: ''
})

const pagoForm = ref({
  metodoPagoId: 2,
  referencia: ''
})

// Fecha mínima para edición
const fechaMinima = computed(() => {
  const now = new Date()
  const offset = now.getTimezoneOffset()
  const local = new Date(now.getTime() - offset * 60000)
  return local.toISOString().slice(0, 16)
})

const cargar = async () => {
  try {
    const res = await api.get('/Reserva')
    if (res.data.success) reservas.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

// Ver detalle
const verDetalle = (r) => {
  reservaDetalle.value = r
}

// Abrir edición
const abrirEditar = (r) => {
  reservaEditar.value = r
  // Formatear fecha para datetime-local
  const fecha = new Date(r.fechaProgramada)
  const offset = fecha.getTimezoneOffset()
  const local = new Date(fecha.getTime() - offset * 60000)
  editForm.value = {
    fechaProgramada: local.toISOString().slice(0, 16),
    observaciones: r.observaciones || ''
  }
  errorEditFecha.value = ''
}

// Validar fecha en edición
const validarEditFecha = () => {
  if (!editForm.value.fechaProgramada) {
    errorEditFecha.value = ''
    return
  }
  const fecha = new Date(editForm.value.fechaProgramada)
  const hora = fecha.getHours()
  const minutos = fecha.getMinutes()
  const diaSemana = fecha.getDay()

  if (fecha <= new Date()) {
    errorEditFecha.value = 'La fecha debe ser posterior al momento actual.'
    return
  }
  if (diaSemana === 0) {
    errorEditFecha.value = 'No atendemos los domingos. Elige de Lunes a Sábado.'
    return
  }
  const horaTotal = hora + minutos / 60
  if (horaTotal < 9 || horaTotal >= 17) {
    errorEditFecha.value = 'El horario de atención es de 9:00 AM a 5:00 PM.'
    return
  }
  errorEditFecha.value = ''
}

// Guardar edición
const guardarEdicion = async () => {
  validarEditFecha()
  if (errorEditFecha.value) return

  loadingEdit.value = true
  try {
    const res = await api.put(`/Reserva/${reservaEditar.value.id}`, {
      fechaProgramada: editForm.value.fechaProgramada,
      observaciones: editForm.value.observaciones
    })
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: '¡Reserva Actualizada!', text: 'Los datos de tu reserva han sido modificados.', timer: 2000, showConfirmButton: false })
      reservaEditar.value = null
      cargar()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al actualizar la reserva', 'error')
  } finally {
    loadingEdit.value = false
  }
}

// Cancelar reserva
const cancelarReserva = async (r) => {
  const conf = await Swal.fire({
    title: '¿Cancelar Reserva?',
    html: `<p>¿Estás seguro que deseas cancelar la reserva <strong>#${r.id}</strong> — ${r.nombreServicio}?</p><p class="text-muted small">Esta acción no se puede deshacer.</p>`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí, Cancelar Reserva',
    cancelButtonText: 'No, Mantener',
    confirmButtonColor: '#dc3545'
  })
  if (!conf.isConfirmed) return

  try {
    const res = await api.put(`/Reserva/${r.id}/estado`, { estado: 'Cancelada' })
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Reserva Cancelada', timer: 1500, showConfirmButton: false })
      cargar()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al cancelar', 'error')
  }
}

// Abrir pago
const abrirModalPago = (r) => {
  selectedReserva.value = r
}

// Pagar con Pasarela de Pagos Simulatida
const pagarConPasarela = async () => {
  if (!selectedReserva.value) return
  loadingPasarela.value = true
  try {
    const res = await api.post('/Payment/process-simulated', {
      orderId: 0,
      reservaId: selectedReserva.value.id,
      metodoPago: 'card',
      titularTarjeta: 'Cliente Demo'
    })
    if (res.data.success) {
      Swal.fire({
        title: '✓ Pago Realizado Correctamente',
        html: `Tu pago de prueba para la reserva #${selectedReserva.value.id} fue procesado exitosamente.<br>ID Transacción: <strong>${res.data.data.transactionId}</strong>`,
        icon: 'success',
        confirmButtonText: 'Entendido',
        confirmButtonColor: '#2d6a4f'
      })
      selectedReserva.value = null
      cargar()
    } else {
      Swal.fire('Error', res.data.message || 'No se pudo procesar el pago simulado.', 'error')
    }
  } catch (e) {
    Swal.fire('Error de Pago', e.response?.data?.message || 'Error al procesar el pago simulado', 'error')
  } finally {
    loadingPasarela.value = false
  }
}

const procesarPago = async () => {
  try {
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

// Abrir Calificación (RF-10)
const abrirCalificacion = (r) => {
  reservaCalificar.value = r
  calificacionForm.value = {
    puntuacion: 5,
    comentario: ''
  }
}

// Enviar Calificación
const enviarCalificacion = async () => {
  if (!reservaCalificar.value) return
  loadingCalificacion.value = true
  try {
    const res = await api.post('/Calificacion', {
      reservaId: reservaCalificar.value.id,
      puntuacion: calificacionForm.value.puntuacion,
      comentario: calificacionForm.value.comentario
    })
    if (res.data.success) {
      Swal.fire({
        icon: 'success',
        title: '¡Gracias por tu opinión!',
        text: 'Tu calificación ha sido registrada con éxito.',
        timer: 2000,
        showConfirmButton: false
      })
      reservaCalificar.value = null
      cargar()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al enviar la calificación', 'error')
  } finally {
    loadingCalificacion.value = false
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
