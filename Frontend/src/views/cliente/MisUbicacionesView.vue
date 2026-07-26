<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Mis Ubicaciones</h3>
        <p class="text-muted small">Administra tus direcciones para la atención a domicilio en Santa Cruz</p>
      </div>
      <button @click="showModal = true" class="btn btn-primary-custom"><i class="bi bi-plus-lg me-1"></i>Agregar Ubicación</button>
    </div>

    <div class="row g-3">
      <div v-for="u in ubicaciones" :key="u.id" class="col-md-4">
        <div class="ecowash-card">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <span v-if="u.esPrincipal" class="badge bg-success">Principal</span>
            <span v-else class="badge bg-light text-dark">Secundaria</span>
          </div>
          <h5 class="fw-bold mb-1"><i class="bi bi-geo-alt-fill text-primary me-2"></i>{{ u.zona || 'Santa Cruz' }}</h5>
          <p class="text-muted small mb-2">{{ u.direccion }}</p>
          <div v-if="u.referencia" class="extra-small text-secondary">Ref: {{ u.referencia }}</div>
        </div>
      </div>
    </div>

    <!-- MODAL -->
    <div v-if="showModal" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold">Registrar Ubicación</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <form @submit.prevent="guardar">
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label fw-semibold">Dirección Exacta</label>
                <input type="text" v-model="form.direccion" class="form-control" placeholder="Av. Banzer 4to Anillo, Calle 3 #120" required />
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Zona / Barrio</label>
                <input type="text" v-model="form.zona" class="form-control" placeholder="Equipetrol / Norte" />
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Referencia</label>
                <input type="text" v-model="form.referencia" class="form-control" placeholder="Frente a la farmacia..." />
              </div>
              <div class="form-check mb-3">
                <input type="checkbox" v-model="form.esPrincipal" class="form-check-input" id="principal" />
                <label class="form-check-label small" for="principal">Establecer como dirección principal</label>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="showModal = false">Cancelar</button>
              <button type="submit" class="btn btn-primary-custom">Guardar Ubicación</button>
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

const ubicaciones = ref([])
const showModal = ref(false)

const form = ref({
  direccion: '',
  zona: '',
  referencia: '',
  esPrincipal: false
})

const cargar = async () => {
  try {
    const res = await api.get('/Ubicacion')
    if (res.data.success) ubicaciones.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

const guardar = async () => {
  try {
    const res = await api.post('/Ubicacion', form.value)
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Ubicación Registrada', text: res.data.message, timer: 1500, showConfirmButton: false })
      showModal.value = false
      form.value = { direccion: '', zona: '', referencia: '', esPrincipal: false }
      cargar()
    }
  } catch (e) {}
}
</script>
