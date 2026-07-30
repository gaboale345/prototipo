<template>
  <div class="row justify-content-center">
    <div class="col-lg-8">
      <div class="ecowash-card">
        <h3 class="fw-bold mb-3"><i class="bi bi-calendar-plus text-primary me-2"></i>Solicitar Servicio de Lavado</h3>
        <p class="text-muted small mb-4">Ingresa la información requerida para que un lavador profesional asista a tu domicilio en Santa Cruz de la Sierra.</p>

        <!-- AVISO HORARIO -->
        <div class="alert alert-info d-flex align-items-start gap-2 mb-4 py-2">
          <i class="bi bi-clock-fill mt-1 text-info"></i>
          <div class="small">
            <strong>Horario de atención:</strong> Lunes a Sábado de <strong>9:00 AM a 5:00 PM</strong>. No se aceptan reservas fuera de este horario ni en fechas pasadas.
          </div>
        </div>

        <!-- SIN VEHÍCULOS -->
        <div v-if="vehiculos.length === 0 && !cargando" class="alert alert-warning d-flex align-items-start gap-2 mb-4">
          <i class="bi bi-exclamation-triangle-fill mt-1"></i>
          <div>
            <strong>No tienes vehículos registrados.</strong><br/>
            <span class="small">Para hacer una reserva primero debes registrar un vehículo.</span>
            <div class="mt-2">
              <router-link to="/cliente/vehiculos" class="btn btn-sm btn-warning fw-bold">
                <i class="bi bi-plus-lg me-1"></i>Registrar Vehículo
              </router-link>
            </div>
          </div>
        </div>

        <form v-else @submit.prevent="handleReservar">
          <!-- SELECCIONAR SERVICIO -->
          <div class="mb-4">
            <label class="form-label fw-bold">1. Selecciona el Tipo de Lavado</label>
            <div class="row g-2">
              <div v-for="s in servicios" :key="s.id" class="col-md-6">
                <div
                  :class="['p-3 border rounded-3 cursor-pointer transition-all', form.servicioId === s.id ? 'border-primary bg-primary bg-opacity-10' : 'bg-white']"
                  @click="form.servicioId = s.id"
                  style="cursor: pointer;"
                >
                  <div class="d-flex justify-content-between align-items-center mb-1">
                    <span class="fw-bold">{{ s.nombre }}</span>
                    <span class="badge bg-primary fs-6">Bs. {{ s.precio }}</span>
                  </div>
                  <p class="text-muted extra-small mb-0">{{ s.descripcion }} ({{ s.duracionMinutos }} mins)</p>
                </div>
              </div>
            </div>
          </div>

          <!-- SELECCIONAR VEHÍCULO -->
          <div class="mb-4">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <label class="form-label fw-bold mb-0">2. Selecciona tu Vehículo</label>
              <router-link to="/cliente/vehiculos" class="small text-decoration-none">+ Agregar Vehículo</router-link>
            </div>
            <select v-model="form.vehiculoId" class="form-select py-2" required @change="verificarVehiculoActivo">
              <option value="" disabled>-- Selecciona un vehículo --</option>
              <option v-for="v in vehiculos" :key="v.id" :value="v.id">
                {{ v.marca }} {{ v.modelo }} - Placa: {{ v.placa }} ({{ v.tipo }})
              </option>
            </select>
            <!-- Advertencia de reserva activa -->
            <div v-if="vehiculoTieneReservaActiva" class="alert alert-warning mt-2 py-2 small d-flex align-items-center gap-2">
              <i class="bi bi-exclamation-circle-fill"></i>
              <span>Este vehículo ya tiene una reserva activa (<strong>Pendiente / En Proceso</strong>). Cancela la reserva anterior o selecciona otro vehículo.</span>
            </div>
          </div>

          <!-- SELECCIONAR UBICACIÓN -->
          <div class="mb-4">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <label class="form-label fw-bold mb-0">3. Ubicación del Servicio (Santa Cruz)</label>
              <router-link to="/cliente/ubicaciones" class="small text-decoration-none">+ Agregar Ubicación</router-link>
            </div>
            <select v-model="form.ubicacionId" class="form-select py-2" required>
              <option value="" disabled>-- Selecciona tu ubicación --</option>
              <option v-for="u in ubicaciones" :key="u.id" :value="u.id">
                {{ u.direccion }} ({{ u.zona }})
              </option>
            </select>
          </div>

          <!-- FECHA Y HORA -->
          <div class="mb-4">
            <label class="form-label fw-bold">4. Fecha y Hora del Servicio</label>
            <input
              type="datetime-local"
              v-model="form.fechaProgramada"
              class="form-control py-2"
              :min="fechaMinima"
              required
              @change="validarFechaHora"
            />
            <!-- Mensaje de error de hora -->
            <div v-if="errorFechaHora" class="alert alert-danger mt-2 py-2 small d-flex align-items-start gap-2">
              <i class="bi bi-clock-fill mt-1"></i>
              <div>
                <strong>Horario no disponible.</strong><br/>
                {{ errorFechaHora }}
              </div>
            </div>
            <div v-else-if="form.fechaProgramada && !errorFechaHora" class="text-success small mt-1">
              <i class="bi bi-check-circle-fill me-1"></i>Horario válido ✓
            </div>
          </div>

          <!-- OBSERVACIONES -->
          <div class="mb-4">
            <label class="form-label fw-semibold">Observaciones / Indicaciones adicionales</label>
            <textarea v-model="form.observaciones" class="form-control" rows="2" placeholder="Ej: Favor tocar el timbre del portón negro..."></textarea>
          </div>

          <button
            type="submit"
            class="btn btn-primary-custom w-100 py-3 fs-5"
            :disabled="loading || !!errorFechaHora || vehiculoTieneReservaActiva || !form.fechaProgramada"
          >
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            Confirmar Reserva
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'
import { useRouter } from 'vue-router'

const router = useRouter()
const servicios = ref([])
const vehiculos = ref([])
const ubicaciones = ref([])
const reservasActivas = ref([])
const loading = ref(false)
const cargando = ref(true)
const errorFechaHora = ref('')
const vehiculoTieneReservaActiva = ref(false)

const form = ref({
  servicioId: null,
  vehiculoId: '',
  ubicacionId: '',
  fechaProgramada: '',
  observaciones: ''
})

// Fecha mínima = ahora (formato para datetime-local)
const fechaMinima = computed(() => {
  const now = new Date()
  // Ajustar a hora local Bolivia (UTC-4)
  const offset = now.getTimezoneOffset()
  const local = new Date(now.getTime() - offset * 60000)
  return local.toISOString().slice(0, 16)
})

onMounted(async () => {
  try {
    const [sRes, vRes, uRes, rRes] = await Promise.all([
      api.get('/Servicio'),
      api.get('/Vehiculo'),
      api.get('/Ubicacion'),
      api.get('/Reserva')
    ])

    if (sRes.data.success) {
      servicios.value = sRes.data.data
      if (servicios.value.length > 0) form.value.servicioId = servicios.value[0].id
    }
    if (vRes.data.success) {
      vehiculos.value = vRes.data.data
      if (vehiculos.value.length > 0) form.value.vehiculoId = vehiculos.value[0].id
    }
    if (uRes.data.success) {
      ubicaciones.value = uRes.data.data
      if (ubicaciones.value.length > 0) form.value.ubicacionId = ubicaciones.value[0].id
    }
    if (rRes.data.success) {
      // Guardar reservas activas para validar duplicados
      reservasActivas.value = rRes.data.data.filter(r =>
        ['Pendiente', 'Aceptada', 'EnProceso'].includes(r.estado)
      )
      // Verificar si el vehículo seleccionado por defecto ya tiene reserva activa
      verificarVehiculoActivo()
    }
  } catch (e) {
  } finally {
    cargando.value = false
  }
})

// Verificar si el vehículo seleccionado tiene una reserva activa
const verificarVehiculoActivo = () => {
  if (!form.value.vehiculoId) {
    vehiculoTieneReservaActiva.value = false
    return
  }
  vehiculoTieneReservaActiva.value = reservasActivas.value.some(
    r => r.vehiculoId === form.value.vehiculoId
  )
}

// Validar que la hora esté entre 9:00 y 17:00
const validarFechaHora = () => {
  if (!form.value.fechaProgramada) {
    errorFechaHora.value = ''
    return
  }
  const fecha = new Date(form.value.fechaProgramada)
  const hora = fecha.getHours()
  const minutos = fecha.getMinutes()
  const diaSemana = fecha.getDay() // 0=Dom, 1=Lun, ..., 6=Sab

  // Validar que sea en el futuro
  if (fecha <= new Date()) {
    errorFechaHora.value = 'La fecha y hora deben ser posteriores al momento actual.'
    return
  }

  // Validar día (Lunes a Sábado, no Domingos)
  if (diaSemana === 0) {
    errorFechaHora.value = 'No atendemos los domingos. Por favor elige un día de Lunes a Sábado.'
    return
  }

  // Validar hora: 9:00 AM a 5:00 PM (17:00)
  const horaTotal = hora + minutos / 60
  if (horaTotal < 9) {
    errorFechaHora.value = 'El horario más temprano disponible es las 9:00 AM. Por favor selecciona una hora entre 9:00 AM y 5:00 PM.'
    return
  }
  if (horaTotal >= 17) {
    errorFechaHora.value = 'El horario máximo de atención es las 5:00 PM. Por favor selecciona una hora entre 9:00 AM y 5:00 PM.'
    return
  }

  errorFechaHora.value = ''
}

const handleReservar = async () => {
  // Validaciones de seguridad antes de enviar
  validarFechaHora()
  if (errorFechaHora.value) {
    Swal.fire({
      icon: 'warning',
      title: '⏰ Horario Inválido',
      html: `<p>${errorFechaHora.value}</p><p class="text-muted small">Horario disponible: <strong>Lunes a Sábado de 9:00 AM a 5:00 PM</strong></p>`,
      confirmButtonText: 'Entendido'
    })
    return
  }

  if (vehiculoTieneReservaActiva.value) {
    Swal.fire('Reserva Duplicada', 'Este vehículo ya tiene una reserva activa. Cancela la reserva anterior o selecciona otro vehículo.', 'warning')
    return
  }

  if (!form.value.servicioId || !form.value.vehiculoId || !form.value.ubicacionId || !form.value.fechaProgramada) {
    Swal.fire('Atención', 'Por favor completa todos los campos requeridos.', 'warning')
    return
  }

  loading.value = true
  try {
    const res = await api.post('/Reserva', form.value)
    loading.value = false
    if (res.data.success) {
      Swal.fire({
        icon: 'success',
        title: '¡Reserva Creada!',
        text: 'Tu solicitud de lavado ha sido registrada. Te notificaremos cuando el empleado acepte el servicio.',
        confirmButtonText: 'Ver Mis Reservas'
      }).then(() => router.push('/cliente/reservas'))
    }
  } catch (e) {
    loading.value = false
    Swal.fire('Error', e.response?.data?.message || 'Error al procesar la reserva', 'error')
  }
}
</script>
