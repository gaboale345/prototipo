<template>
  <div class="row justify-content-center">
    <div class="col-md-6">
      <div class="ecowash-card">
        <h4 class="fw-bold mb-3"><i class="bi bi-person-circle text-primary me-2"></i>Mi Perfil de Usuario</h4>
        <div class="mb-3">
          <label class="form-label text-muted small">Nombre Completo</label>
          <div class="fs-5 fw-bold">{{ authStore.nombreUsuario }}</div>
        </div>
        <div class="mb-3">
          <label class="form-label text-muted small">Correo Electrónico</label>
          <div class="fs-6">{{ authStore.usuario?.email }}</div>
        </div>
        <div class="mb-3">
          <label class="form-label text-muted small">Rol asignado en el sistema</label>
          <div><span class="badge bg-primary fs-6">{{ authStore.rol }}</span></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useAuthStore } from '../../stores/authStore'
const authStore = useAuthStore()
</script>
