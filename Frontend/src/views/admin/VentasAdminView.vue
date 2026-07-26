<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-1">Ventas y Facturación</h3>
        <p class="text-muted small">Registro de ventas generadas al finalizar servicios y sus facturas</p>
      </div>
    </div>

    <div class="ecowash-card mb-4">
      <h5 class="fw-bold mb-3">Ventas Registradas</h5>
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>N° Venta</th>
              <th>Cliente</th>
              <th>Reserva ID</th>
              <th>Total (Bs.)</th>
              <th>Fecha Venta</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="v in ventas" :key="v.id">
              <td class="fw-bold text-primary">{{ v.numeroVenta }}</td>
              <td>{{ v.nombreCliente }}</td>
              <td>#{{ v.reservaId }}</td>
              <td class="fw-extrabold text-success">Bs. {{ v.total }}</td>
              <td>{{ formatDate(v.fechaVenta) }}</td>
              <td>
                <span :class="['badge', v.estado === 'Pagada' ? 'bg-success' : 'bg-warning text-dark']">{{ v.estado }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="ecowash-card">
      <h5 class="fw-bold mb-3">Facturas Emitidas</h5>
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>N° Factura</th>
              <th>Razón Social</th>
              <th>NIT/CI</th>
              <th>Monto Total</th>
              <th>Fecha Emisión</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="f in facturas" :key="f.id">
              <td class="fw-bold text-dark">{{ f.numeroFactura }}</td>
              <td>{{ f.nombreCliente }}</td>
              <td>{{ f.nit || 'S/N' }}</td>
              <td class="fw-bold text-success">Bs. {{ f.total }}</td>
              <td>{{ formatDate(f.fechaEmision) }}</td>
              <td><span class="badge bg-success">EMITIDA</span></td>
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

const ventas = ref([])
const facturas = ref([])

onMounted(async () => {
  try {
    const vRes = await api.get('/Venta')
    if (vRes.data.success) ventas.value = vRes.data.data

    const fRes = await api.get('/Factura')
    if (fRes.data.success) facturas.value = fRes.data.data
  } catch (e) {}
})

const formatDate = (d) => new Date(d).toLocaleString('es-BO', { dateStyle: 'short', timeStyle: 'short' })
</script>
