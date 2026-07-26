<template>
  <div class="row justify-content-center">
    <div class="col-lg-8">
      <div class="ecowash-card">
        <h3 class="fw-bold mb-3"><i class="bi bi-calendar-plus text-primary me-2"></i>Solicitar Servicio de Lavado</h3>
        <p class="text-muted small mb-4">Ingresa la información requerida para que un lavador profesional asista a tu domicilio en Santa Cruz de la Sierra.</p>

        <form @submit.prevent="handleReservar">
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
            <select v-model="form.vehiculoId" class="form-select py-2" required>
              <option value="" disabled>-- Selecciona un vehículo --</option>
              <option v-for="v in vehiculos" :key="v.id" :value="v.id">
                {{ v.marca }} {{ v.modelo }} - Placa: {{ v.placa }} ({{ v.tipo }})
              </option>
            </select>
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
            <input type="datetime-local" v-model="form.fechaProgramada" class="form-control py-2" required />
          </div>

          <!-- OBSERVACIONES -->
          <div class="mb-4">
            <label class="form-label fw-semibold">Observaciones / Indicaciones adicionales</label>
            <textarea v-model="form.observaciones" class="form-control" rows="2" placeholder="Ej: Favor tocar el timbre del portón negro..."></textarea>
          </div>

          <button type="submit" class="btn btn-primary-custom w-100 py-3 fs-5" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            Confirmar Reserva
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'
import { useRouter } from 'vue-router'

const router = useRouter()
const servicios = ref([])
const vehiculos = ref([])
const ubicaciones = ref([])
const loading = ref(false)

const form = ref({
  servicioId: null,
  vehiculoId: '',
  ubicacionId: '',
  fechaProgramada: '',
  observaciones: ''
})

onMounted(async () => {
  try {
    const sRes = await api.get('/Servicio')
    if (sRes.data.success) {
      servicios.value = sRes.data.data
      if (servicios.value.length > 0) form.value.servicioId = servicios.value[0].id
    }

    const vRes = await api.get('/Vehiculo')
    if (vRes.data.success) {
      vehiculos.value = vRes.data.data
      if (vehiculos.value.length > 0) form.value.vehiculoId = vehiculos.value[0].id
    }

    const uRes = await api.get('/Ubicacion')
    if (uRes.data.success) {
      ubicaciones.value = uRes.data.data
      if (ubicaciones.value.length > 0) form.value.ubicacionId = ubicaciones.value[0].id
    }
  } catch (e) {}
})

const handleReservar = async () => {
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
