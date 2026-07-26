<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Registro de Auditoría del Sistema</h3>
        <p class="text-muted small">Trazabilidad completa de acciones y cambios (Regla de Negocio)</p>
      </div>
    </div>

    <div class="ecowash-card">
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>Acción</th>
              <th>Módulo</th>
              <th>Entidad</th>
              <th>Usuario ID</th>
              <th>IP</th>
              <th>Fecha</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="a in auditorias" :key="a.id">
              <td class="fw-bold text-primary">{{ a.accion }}</td>
              <td><span class="badge bg-secondary">{{ a.modulo }}</span></td>
              <td>{{ a.entidad || 'N/A' }}</td>
              <td>User #{{ a.usuarioId || 'Anónimo' }}</td>
              <td><code>{{ a.ip || '127.0.0.1' }}</code></td>
              <td>{{ formatDate(a.fecha) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const auditorias = ref([])

onMounted(async () => {
  try {
    const res = await api.get('/Auditoria')
    if (res.data.success) auditorias.value = res.data.data
  } catch (e) {}
})

const formatDate = (d) => new Date(d).toLocaleString('es-BO', { dateStyle: 'short', timeStyle: 'short' })
</script>
