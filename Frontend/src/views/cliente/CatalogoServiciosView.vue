<template>
  <div class="container-fluid py-4">
    <!-- Header -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
      <div>
        <h2 class="fw-bold text-emerald mb-1">
          <i class="bi bi-grid-3x3-gap-fill me-2"></i>Catálogo de Servicios
        </h2>
        <p class="text-muted mb-0">Selecciona los servicios ecológicos de tu preferencia</p>
      </div>

      <!-- Carrito flotante / resumen selección -->
      <div v-if="selectedCount > 0" class="card shadow-sm border-0 rounded-4 p-2 bg-emerald text-white d-flex flex-row align-items-center gap-3 px-3">
        <div>
          <span class="badge bg-white text-emerald rounded-circle fs-6 me-2">{{ selectedCount }}</span>
          <span class="fw-bold">Total: Bs. {{ selectedTotal.toFixed(2) }}</span>
        </div>
        <button @click="proceedToOrder" class="btn btn-light btn-sm rounded-pill fw-bold text-emerald ms-auto">
          Continuar <i class="bi bi-arrow-right ms-1"></i>
        </button>
      </div>
    </div>

    <!-- Filtros por tipo de vehículo -->
    <div class="d-flex gap-2 overflow-auto pb-2 mb-4">
      <button
        v-for="tipo in filterOptions"
        :key="tipo"
        @click="activeFilter = tipo"
        :class="['btn btn-sm rounded-pill px-4 fw-semibold transition-all', activeFilter === tipo ? 'btn-emerald' : 'btn-outline-secondary']"
      >
        {{ tipo }}
      </button>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-emerald" role="status"></div>
      <p class="text-muted mt-2">Cargando servicios...</p>
    </div>

    <!-- Empty state -->
    <div v-else-if="filteredServicios.length === 0" class="text-center py-5 bg-white rounded-4 shadow-sm">
      <i class="bi bi-inbox fs-1 text-muted"></i>
      <p class="text-muted mt-2">No hay servicios disponibles para esta categoría</p>
    </div>

    <!-- Grid de Servicios -->
    <div v-else class="row g-4">
      <div v-for="servicio in filteredServicios" :key="servicio.id" class="col-12 col-md-6 col-lg-4">
        <div :class="['card service-card h-100 border-0 shadow-sm rounded-4 overflow-hidden position-relative', { 'selected-card': isSelected(servicio.id) }]">
          <!-- Badge de tipo vehículo -->
          <div class="position-absolute top-0 end-0 m-3">
            <span class="badge bg-light text-emerald border fw-semibold rounded-pill px-3 py-2">
              <i class="bi bi-car-front me-1"></i> {{ servicio.tipoVehiculo || 'Todos' }}
            </span>
          </div>

          <div class="card-body p-4 d-flex flex-column">
            <div class="icon-circle mb-3">
              <i class="bi bi-droplet-half fs-3 text-emerald"></i>
            </div>
            <h5 class="fw-bold mb-2 text-dark">{{ servicio.nombre }}</h5>
            <p class="text-muted small flex-grow-1 mb-3">{{ servicio.descripcion || 'Sin descripción disponible' }}</p>

            <div class="d-flex justify-content-between align-items-center mb-3">
              <div>
                <small class="text-muted d-block">Duración estimada</small>
                <span class="fw-semibold text-dark"><i class="bi bi-clock me-1"></i>{{ servicio.duracionMinutos }} min</span>
              </div>
              <div class="text-end">
                <small class="text-muted d-block">Precio</small>
                <span class="fs-4 fw-bold text-emerald">Bs. {{ servicio.precio.toFixed(2) }}</span>
              </div>
            </div>

            <!-- Botones de selección -->
            <div class="d-flex align-items-center gap-2 mt-auto">
              <template v-if="isSelected(servicio.id)">
                <button class="btn btn-emerald w-100 rounded-pill py-2 fw-bold" @click="removeService()">
                  <i class="bi bi-check-circle-fill me-1"></i> Seleccionado
                </button>
              </template>
              <template v-else>
                <button class="btn btn-outline-emerald w-100 rounded-pill py-2 fw-semibold" @click="selectService(servicio)">
                  Seleccionar
                </button>
              </template>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../../services/api'

const router = useRouter()

const servicios = ref([])
const loading = ref(true)
const activeFilter = ref('Todos')
const filterOptions = ['Todos', 'Auto', 'Moto', 'Camioneta']

// Carrito local (Selección única para reserva)
const selectedService = ref(null)

onMounted(async () => {
  try {
    const res = await api.get('/Servicio')
    if (res.data.success) {
      servicios.value = res.data.data
    }
  } catch (err) {
    console.error('Error cargando servicios:', err)
  } finally {
    loading.value = false
  }
})

const filteredServicios = computed(() => {
  if (activeFilter.value === 'Todos') return servicios.value
  return servicios.value.filter(s => s.tipoVehiculo === activeFilter.value || s.tipoVehiculo === 'Todos')
})

const isSelected = (id) => selectedService.value && selectedService.value.id === id

const selectService = (servicio) => {
  selectedService.value = servicio
}

const removeService = () => {
  selectedService.value = null
}

const selectedCount = computed(() => selectedService.value ? 1 : 0)

const selectedTotal = computed(() => selectedService.value ? selectedService.value.precio : 0)

const proceedToOrder = () => {
  if (!selectedService.value) return
  const cartData = [{
    servicioId: selectedService.value.id,
    cantidad: 1,
    nombre: selectedService.value.nombre,
    precio: selectedService.value.precio,
    duracion: selectedService.value.duracionMinutos
  }]
  sessionStorage.setItem('pending_cart', JSON.stringify(cartData))
  router.push('/cliente/reservar')
}
</script>

<style scoped>
.text-emerald {
  color: #2563EB;
}

.bg-emerald {
  background: linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%);
}

.btn-emerald {
  background: linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%);
  color: white;
  border: none;
}

.btn-outline-emerald {
  border-color: #2563EB;
  color: #2563EB;
}

.btn-outline-emerald:hover {
  background-color: #2563EB;
  color: white;
}

.service-card {
  transition: all 0.3s ease;
  background: white;
}

.service-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.08) !important;
}

.selected-card {
  border: 2px solid #2563EB !important;
  background: #EFF6FF;
}

.icon-circle {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  background: #e8f5e9;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
