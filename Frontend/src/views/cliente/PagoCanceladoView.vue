<template>
  <div class="container py-5 d-flex align-items-center justify-content-center min-vh-100">
    <div class="card border-0 shadow-lg rounded-4 p-5 text-center" style="max-width: 550px; width: 100%;">
      <!-- Icono animado -->
      <div class="cancel-icon-circle mx-auto mb-4">
        <i class="bi bi-x-lg fs-1 text-white"></i>
      </div>

      <h2 class="fw-bold text-danger mb-2">Pago No Completado</h2>
      <p class="text-muted mb-4">
        El proceso de pago fue cancelado o no pudo ser completado. No se ha realizado ningún cobro a tu tarjeta.
      </p>

      <div class="p-3 bg-light rounded-3 mb-4 text-start">
        <div class="d-flex justify-content-between">
          <span class="text-muted">Estado:</span>
          <span class="badge bg-danger rounded-pill px-3 py-2">Cancelado / Rechazado</span>
        </div>
      </div>

      <div class="d-grid gap-2">
        <router-link to="/cliente/catalogo" class="btn btn-emerald py-3 fw-bold rounded-3">
          <i class="bi bi-arrow-counterclockwise me-2"></i> Reintentar Solicitud
        </router-link>
        <router-link to="/cliente/dashboard" class="btn btn-outline-secondary py-2 rounded-3">
          <i class="bi bi-house me-1"></i> Volver al Inicio
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
.btn-emerald {
  background: linear-gradient(135deg, #2d6a4f 0%, #40916c 100%);
  color: white;
  border: none;
}

.cancel-icon-circle {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  background: linear-gradient(135deg, #e63946 0%, #d62828 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 10px 25px rgba(230, 57, 70, 0.3);
}
</style>
