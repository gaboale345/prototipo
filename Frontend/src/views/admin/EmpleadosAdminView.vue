<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Gestión de Empleados</h3>
        <p class="text-muted small">Administración del personal de lavadores</p>
      </div>
      <button @click="showModal = true" class="btn btn-primary-custom"><i class="bi bi-plus-lg me-1"></i>Nuevo Empleado</button>
    </div>

    <div class="ecowash-card">
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>Nombre Completo</th>
              <th>Email</th>
              <th>Teléfono</th>
              <th>Cargo</th>
              <th>Salario (Bs.)</th>
              <th>Disponibilidad</th>
              <th>Fecha Ingreso</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="e in empleados" :key="e.id">
              <td class="fw-bold">{{ e.nombreCompleto }}</td>
              <td>{{ e.email }}</td>
              <td>{{ e.telefono || 'N/A' }}</td>
              <td>{{ e.cargo }}</td>
              <td class="fw-semibold">Bs. {{ e.salario }}</td>
              <td>
                <span :class="['badge', e.disponible ? 'bg-success' : 'bg-secondary']">
                  {{ e.disponible ? 'DISPONIBLE' : 'OCUPADO' }}
                </span>
              </td>
              <td>{{ formatDate(e.fechaIngreso) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- MODAL -->
    <div v-if="showModal" class="modal fade show d-block" style="background: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header">
            <h5 class="modal-title fw-bold">Registrar Nuevo Empleado</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <form @submit.prevent="guardar">
            <div class="modal-body">
              <div class="row g-2 mb-3">
                <div class="col-6">
                  <label class="form-label fw-semibold">Nombre</label>
                  <input type="text" v-model="form.nombre" class="form-control" required />
                </div>
                <div class="col-6">
                  <label class="form-label fw-semibold">Apellido</label>
                  <input type="text" v-model="form.apellido" class="form-control" required />
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Email</label>
                <input type="email" v-model="form.email" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Contraseña</label>
                <input type="password" v-model="form.password" class="form-control" required />
              </div>
              <div class="row g-2 mb-3">
                <div class="col-6">
                  <label class="form-label fw-semibold">Cargo</label>
                  <input type="text" v-model="form.cargo" class="form-control" placeholder="Lavador Profesional" />
                </div>
                <div class="col-6">
                  <label class="form-label fw-semibold">Salario (Bs.)</label>
                  <input type="number" v-model="form.salario" class="form-control" step="100" />
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="showModal = false">Cancelar</button>
              <button type="submit" class="btn btn-primary-custom">Guardar Empleado</button>
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

const empleados = ref([])
const showModal = ref(false)

const form = ref({
  nombre: '',
  apellido: '',
  email: '',
  password: '',
  cargo: 'Lavador Profesional',
  salario: 2500
})

const cargar = async () => {
  try {
    const res = await api.get('/Empleado')
    if (res.data.success) empleados.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

const guardar = async () => {
  try {
    const res = await api.post('/Empleado', form.value)
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Empleado Registrado', text: res.data.message, timer: 1500, showConfirmButton: false })
      showModal.value = false
      cargar()
    }
  } catch (e) {}
}

const formatDate = (d) => new Date(d).toLocaleDateString('es-BO')
</script>
