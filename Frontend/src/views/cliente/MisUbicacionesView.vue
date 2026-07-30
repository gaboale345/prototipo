<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Mis Ubicaciones</h3>
        <p class="text-muted small">Administra tus direcciones para la atención a domicilio en Santa Cruz</p>
      </div>
      <button @click="abrirNuevoModal" class="btn btn-primary-custom">
        <i class="bi bi-plus-lg me-1"></i>Agregar Ubicación
      </button>
    </div>

    <div class="row g-3">
      <div v-for="u in ubicaciones" :key="u.id" class="col-md-4">
        <div class="ecowash-card">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <span v-if="u.esPrincipal" class="badge bg-success">Principal</span>
            <span v-else class="badge bg-light text-dark">Secundaria</span>
            <div class="d-flex gap-1">
              <button @click="abrirEditarModal(u)" class="btn btn-sm btn-outline-warning border-0" title="Editar ubicación">
                <i class="bi bi-pencil"></i>
              </button>
              <button @click="eliminar(u.id)" class="btn btn-sm btn-outline-danger border-0" title="Eliminar ubicación">
                <i class="bi bi-trash"></i>
              </button>
            </div>
          </div>
          <h5 class="fw-bold mb-1"><i class="bi bi-geo-alt-fill text-primary me-2"></i>{{ u.zona || 'Santa Cruz' }}</h5>
          <p class="text-muted small mb-2">{{ u.direccion }}</p>
          <div v-if="u.referencia" class="extra-small text-secondary">Ref: {{ u.referencia }}</div>
        </div>
      </div>

      <div v-if="ubicaciones.length === 0" class="col-12 text-center py-5 text-muted">
        No tienes ubicaciones registradas aún.
      </div>
    </div>

    <!-- MODAL REGISTRO / EDICIÓN UBICACIÓN -->
    <div v-if="showModal" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);" @click.self="showModal = false">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold">{{ editMode ? 'Editar Ubicación' : 'Registrar Ubicación' }}</h5>
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
              <button type="submit" class="btn btn-primary-custom">
                {{ editMode ? 'Actualizar Ubicación' : 'Guardar Ubicación' }}
              </button>
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
const editMode = ref(false)
const selectedId = ref(null)

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

const abrirNuevoModal = () => {
  editMode.value = false
  selectedId.value = null
  form.value = { direccion: '', zona: '', referencia: '', esPrincipal: false }
  showModal.value = true
}

const abrirEditarModal = (u) => {
  editMode.value = true
  selectedId.value = u.id
  form.value = {
    direccion: u.direccion,
    zona: u.zona || '',
    referencia: u.referencia || '',
    esPrincipal: u.esPrincipal || false
  }
  showModal.value = true
}

const guardar = async () => {
  try {
    let res
    if (editMode.value) {
      res = await api.put(`/Ubicacion/${selectedId.value}`, form.value)
    } else {
      res = await api.post('/Ubicacion', form.value)
    }

    if (res.data.success) {
      Swal.fire({
        icon: 'success',
        title: editMode.value ? 'Ubicación Actualizada' : 'Ubicación Registrada',
        text: res.data.message,
        timer: 1500,
        showConfirmButton: false
      })
      showModal.value = false
      cargar()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al guardar la ubicación', 'error')
  }
}

const eliminar = async (id) => {
  const conf = await Swal.fire({ title: '¿Eliminar ubicación?', text: 'Esta ubicación ya no estará disponible para tus reservas', icon: 'warning', showCancelButton: true })
  if (conf.isConfirmed) {
    try {
      await api.delete(`/Ubicacion/${id}`)
      cargar()
    } catch (e) {}
  }
}
</script>
