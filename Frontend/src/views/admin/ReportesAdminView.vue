<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Módulo de Reportes Estadísticos</h3>
        <p class="text-muted small">Generación exclusiva para el perfil Administrador (Regla de Negocio)</p>
      </div>
    </div>

    <div class="row g-4">
      <div class="col-md-4">
        <div class="ecowash-card">
          <h5 class="fw-bold mb-3">Generar Reporte</h5>
          <form @submit.prevent="generar">
            <div class="mb-3">
              <label class="form-label fw-semibold">Tipo de Reporte</label>
              <select v-model="form.tipo" class="form-select">
                <option value="VentasDiarias">Ventas Diarias</option>
                <option value="VentasMensuales">Ventas Mensuales</option>
                <option value="ServiciosMasSolicitados">Servicios Más Solicitados</option>
                <option value="ClientesFrecuentes">Clientes Frecuentes</option>
              </select>
            </div>
            <button type="submit" class="btn btn-primary-custom w-100" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span> Generar Reporte
            </button>
          </form>
        </div>
      </div>

      <div class="col-md-8">
        <div class="ecowash-card">
          <h5 class="fw-bold mb-3">Historial de Reportes Generados</h5>
          
          <div v-if="loadingCarga" class="text-center py-4 text-muted">
            <span class="spinner-border spinner-border-sm me-2"></span> Cargando reportes...
          </div>

          <div v-else-if="reportes.length === 0" class="text-center py-5">
            <i class="bi bi-bar-chart-line text-muted display-4 d-block mb-3"></i>
            <h6 class="fw-semibold text-dark">No hay reportes en el historial</h6>
            <p class="text-muted small">Selecciona el tipo de reporte en el formulario de la izquierda y haz clic en <strong>"Generar Reporte"</strong> para procesar las estadísticas.</p>
          </div>

          <div v-else class="table-responsive">
            <table class="table table-hover align-middle">
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Tipo</th>
                  <th>Fecha Generación</th>
                  <th>Datos</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in reportes" :key="r.id">
                  <td class="fw-semibold">{{ r.nombre }}</td>
                  <td><span class="badge bg-info text-dark">{{ r.tipo }}</span></td>
                  <td>{{ formatDate(r.fechaGeneracion) }}</td>
                  <td>
                    <div class="bg-light p-2 rounded small" style="max-height: 120px; overflow-y: auto;">
                      {{ parseDatos(r.datos) }}
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'
import Swal from 'sweetalert2'

const reportes = ref([])
const loading = ref(false)
const loadingCarga = ref(true)
const form = ref({ tipo: 'VentasDiarias' })

const cargar = async () => {
  loadingCarga.value = true
  try {
    const res = await api.get('/Reporte')
    if (res.data.success) reportes.value = res.data.data
  } catch (e) {
    console.error("Error al cargar reportes:", e)
  } finally {
    loadingCarga.value = false
  }
}

onMounted(cargar)

const generar = async () => {
  loading.value = true
  try {
    const res = await api.post('/Reporte/generar', form.value)
    loading.value = false
    if (res.data.success) {
      Swal.fire({ icon: 'success', title: 'Reporte Generado', text: res.data.message, timer: 1500, showConfirmButton: false })
      cargar()
    }
  } catch (e) {
    loading.value = false
    Swal.fire({
      icon: 'error',
      title: 'Error',
      text: e.response?.data?.message || 'No se pudo generar el reporte. Verifica que tu usuario tenga rol Administrador.'
    })
  }
}

const parseDatos = (datosStr) => {
  try {
    const parsed = JSON.parse(datosStr)
    if (Array.isArray(parsed)) {
      if (parsed.length === 0) return 'Sin registros en el periodo'
      return parsed.map(item => JSON.stringify(item).replace(/[\{\}"]/g, '').replace(/,/g, ' | ')).join('\n')
    }
    return JSON.stringify(parsed)
  } catch {
    return datosStr || 'Sin datos'
  }
}

const formatDate = (d) => new Date(d).toLocaleString('es-BO', { dateStyle: 'short', timeStyle: 'short' })
</script>
