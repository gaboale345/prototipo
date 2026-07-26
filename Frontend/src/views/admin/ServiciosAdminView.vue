<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Catálogo de Servicios</h3>
        <p class="text-muted small">Configura los tipos de lavado de vehículo y sus tarifas</p>
      </div>
      <button @click="showModal = true" class="btn btn-primary-custom"><i class="bi bi-plus-lg me-1"></i>Nuevo Servicio</button>
    </div>

    <div class="row g-3">
      <div v-for="s in servicios" :key="s.id" class="col-md-4">
        <div class="ecowash-card">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <span class="badge bg-primary">Duración: {{ s.duracionMinutos }}m</span>
            <span class="fs-4 fw-extrabold text-success">Bs. {{ s.precio }}</span>
          </div>
          <h4 class="fw-bold mb-1">{{ s.nombre }}</h4>
          <p class="text-muted small mb-2">{{ s.descripcion }}</p>
          <div class="small text-secondary">Para vehículo: {{ s.tipoVehiculo || 'Todos' }}</div>
        </div>
      </div>
    </div>

    <!-- MODAL -->
    <div v-if="showModal" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold">Crear Nuevo Servicio</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <form @submit.prevent="guardar">
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label fw-semibold">Nombre del Servicio</label>
                <input type="text" v-model="form.nombre" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Descripción</label>
                <textarea v-model="form.descripcion" class="form-control" rows="2"></textarea>
              </div>
              <div class="row g-2 mb-3">
                <div class="col-6">
                  <label class="form-label fw-semibold">Precio (Bs.)</label>
                  <input type="number" v-model="form.precio" class="form-control" step="0.5" required />
                </div>
                <div class="col-6">
                  <label class="form-label fw-semibold">Duración (Mins)</label>
                  <input type="number" v-model="form.duracionMinutos" class="form-control" required />
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="showModal = false">Cancelar</button>
              <button type="submit" class="btn btn-primary-custom">Guardar Servicio</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'

const servicios = ref([])
const showModal = ref(false)

const form = ref({
  nombre: '',
  descripcion: '',
  precio: 0,
  duracionMinutos: 60,
  tipoVehiculo: 'Todos'
})

const cargar = async () => {
  try {
    const res = await api.get('/Servicio')
    if (res.data.success) servicios.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

const guardar = async () => {
  try {
    const res = await api.post('/Servicio', form.value)
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Servicio Creado', text: res.data.message, timer: 1500, showConfirmButton: false })
      showModal.value = false
      cargar()
    }
  } catch (e) {}
}
</script>
