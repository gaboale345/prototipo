<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Control de Inventario</h3>
        <p class="text-muted small">Control de insumos de lavado y alertas de stock mínimo</p>
      </div>
      <button @click="showModal = true" class="btn btn-primary-custom"><i class="bi bi-plus-lg me-1"></i>Nuevo Producto</button>
    </div>

    <div class="ecowash-card">
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>Producto</th>
              <th>Categoría</th>
              <th>Stock Actual</th>
              <th>Stock Mínimo</th>
              <th>Precio Unitario</th>
              <th>Estado Stock</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in productos" :key="p.id">
              <td class="fw-semibold">{{ p.nombre }}</td>
              <td>{{ p.nombreCategoria }}</td>
              <td class="fw-bold fs-6">{{ p.stockActual }} {{ p.unidadMedida }}</td>
              <td>{{ p.stockMinimo }}</td>
              <td>Bs. {{ p.precioUnitario }}</td>
              <td>
                <span v-if="p.stockBajo" class="badge bg-danger">¡STOCK BAJO!</span>
                <span v-else class="badge bg-success">NORMAL</span>
              </td>
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
            <h5 class="modal-title fw-bold">Registrar Nuevo Insumo / Producto</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <form @submit.prevent="guardar">
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label fw-semibold">Nombre del Producto</label>
                <input type="text" v-model="form.nombre" class="form-control" placeholder="Ej: Champú con Cera 5L" required />
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Categoría</label>
                <select v-model="form.categoriaId" class="form-select" required>
                  <option :value="1">Detergentes</option>
                  <option :value="2">Ceras y Protectores</option>
                  <option :value="3">Micropaños</option>
                  <option :value="4">Equipos</option>
                </select>
              </div>
              <div class="row g-2 mb-3">
                <div class="col-6">
                  <label class="form-label fw-semibold">Stock Inicial</label>
                  <input type="number" v-model="form.stockActual" class="form-control" required />
                </div>
                <div class="col-6">
                  <label class="form-label fw-semibold">Stock Mínimo Alerta</label>
                  <input type="number" v-model="form.stockMinimo" class="form-control" required />
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Precio Unitario (Bs.)</label>
                <input type="number" v-model="form.precioUnitario" class="form-control" step="0.1" required />
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="showModal = false">Cancelar</button>
              <button type="submit" class="btn btn-primary-custom">Guardar Producto</button>
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

const productos = ref([])
const showModal = ref(false)

const form = ref({
  categoriaId: 1,
  nombre: '',
  stockActual: 10,
  stockMinimo: 5,
  precioUnitario: 25
})

const cargar = async () => {
  try {
    const res = await api.get('/Producto')
    if (res.data.success) productos.value = res.data.data
  } catch (e) {}
}

onMounted(cargar)

const guardar = async () => {
  try {
    const res = await api.post('/Producto', form.value)
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Producto Creado', text: res.data.message, timer: 1500, showConfirmButton: false })
      showModal.value = false
      cargar()
    }
  } catch (e) {}
}
</script>
