<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Mis Vehículos</h3>
        <p class="text-muted small">Registra y administra tus vehículos para los lavados</p>
      </div>
      <button @click="showModal = true" class="btn btn-primary-custom">
        <i class="bi bi-plus-lg me-1"></i> Registrar Vehículo
      </button>
    </div>

    <!-- LISTA -->
    <div class="row g-3">
      <div v-for="v in vehiculos" :key="v.id" class="col-md-4">
        <div class="ecowash-card">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <span class="badge bg-secondary">{{ v.tipo }}</span>
            <button @click="eliminar(v.id)" class="btn btn-sm btn-outline-danger border-0"><i class="bi bi-trash"></i></button>
          </div>
          <h4 class="fw-bold mb-1">{{ v.marca }} {{ v.modelo }}</h4>
          <div class="fs-5 text-primary fw-extrabold mb-2">Placa: {{ v.placa }}</div>
          <div class="small text-muted">Año: {{ v.año || 'N/A' }} | Color: {{ v.color || 'N/A' }}</div>
        </div>
      </div>

      <div v-if="vehiculos.length === 0" class="col-12 text-center py-5 text-muted">
        No tienes vehículos registrados aún.
      </div>
    </div>

    <!-- MODAL REGISTRO VEHÍCULO -->
    <div v-if="showModal" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold">Registrar Nuevo Vehículo</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <form @submit.prevent="guardar">
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label fw-semibold">Placa</label>
                <input type="text" v-model="form.placa" class="form-control" placeholder="Ej: 4589-XYZ" required />
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Tipo de Vehículo</label>
                <select v-model="form.tipo" class="form-select" required>
                  <option value="Auto">Auto / Vagoneta</option>
                  <option value="Camioneta">Camioneta / SUV</option>
                  <option value="Moto">Motocicleta</option>
                </select>
              </div>
              <div class="row g-2 mb-3">
                <div class="col-6">
                  <label class="form-label fw-semibold">Marca</label>
                  <input type="text" v-model="form.marca" class="form-control" placeholder="Toyota, Suzuki..." />
                </div>
                <div class="col-6">
                  <label class="form-label fw-semibold">Modelo</label>
                  <input type="text" v-model="form.modelo" class="form-control" placeholder="Corolla, Jimny..." />
                </div>
              </div>
              <div class="row g-2">
                <div class="col-6">
                  <label class="form-label fw-semibold">Año</label>
                  <input type="text" v-model="form.año" class="form-control" placeholder="2022" />
                </div>
                <div class="col-6">
                  <label class="form-label fw-semibold">Color</label>
                  <input type="text" v-model="form.color" class="form-control" placeholder="Blanco" />
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="showModal = false">Cancelar</button>
              <button type="submit" class="btn btn-primary-custom">Guardar Vehículo</button>
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

const vehiculos = ref([])
const showModal = ref(false)

const form = ref({
  placa: '',
  tipo: 'Auto',
  marca: '',
  modelo: '',
  año: '',
  color: ''
})

const cargarVehiculos = async () => {
  try {
    const res = await api.get('/Vehiculo')
    if (res.data.success) vehiculos.value = res.data.data
  } catch (e) {}
}

onMounted(cargarVehiculos)

const guardar = async () => {
  try {
    const res = await api.post('/Vehiculo', form.value)
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Registrado', text: res.data.message, timer: 1500, showConfirmButton: false })
      showModal.value = false
      form.value = { placa: '', tipo: 'Auto', marca: '', modelo: '', año: '', color: '' }
      cargarVehiculos()
    }
  } catch (e) {
    Swal.fire('Error', e.response?.data?.message || 'Error al guardar', 'error')
  }
}

const eliminar = async (id) => {
  const conf = await Swal.fire({ title: '¿Eliminar vehículo?', text: 'Esta acción no se puede deshacer', icon: 'warning', showCancelButton: true })
  if (conf.isConfirmed) {
    try {
      await api.delete(`/Vehiculo/${id}`)
      cargarVehiculos()
    } catch (e) {}
  }
}
</script>
