<template>
  <aside :class="['sidebar-custom', { 'sidebar-open': open }]">
    <!-- Botón cerrar en móvil -->
    <div class="d-flex justify-content-between align-items-center mb-4 px-2">
      <small class="text-uppercase text-muted fw-bold sidebar-menu-label" style="font-size: 0.7rem; letter-spacing: 1px;">MENÚ PRINCIPAL</small>
      <button class="btn-close-sidebar d-md-none" @click="$emit('close')" aria-label="Cerrar menú">
        <i class="bi bi-x-lg"></i>
      </button>
    </div>

    <!-- MENÚ ADMIN -->
    <div v-if="authStore.rol === 'Administrador'">
      <router-link to="/admin/dashboard" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-speedometer2"></i> Dashboard Admin
      </router-link>

      <router-link to="/admin/reservas" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-calendar-check"></i> Reservas
      </router-link>

      <router-link to="/admin/servicios" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-droplet"></i> Servicios Lavado
      </router-link>

      <router-link to="/admin/clientes" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-people"></i> Clientes
      </router-link>

      <router-link to="/admin/empleados" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-person-badge"></i> Empleados
      </router-link>

      <router-link to="/admin/inventario" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-box-seam"></i> Inventario &amp; Productos
      </router-link>

      <router-link to="/admin/ventas" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-cash-stack"></i> Ventas &amp; Facturas
      </router-link>

      <router-link to="/admin/reportes" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-bar-chart-line"></i> Reportes
      </router-link>

      <router-link to="/admin/auditoria" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-journal-text"></i> Auditoría
      </router-link>

      <router-link to="/admin/usuarios" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-shield-lock"></i> Usuarios &amp; Roles
      </router-link>
    </div>

    <!-- MENÚ CLIENTE -->
    <div v-else-if="authStore.rol === 'Cliente'">
      <router-link to="/cliente/dashboard" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-house-door"></i> Inicio
      </router-link>

      <router-link to="/cliente/reservar" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-plus-circle"></i> Solicitar Lavado
      </router-link>

      <router-link to="/cliente/reservas" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-clock-history"></i> Mis Reservas
      </router-link>

      <router-link to="/cliente/vehiculos" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-car-front"></i> Mis Vehículos
      </router-link>

      <router-link to="/cliente/ubicaciones" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-geo-alt"></i> Mis Ubicaciones
      </router-link>
    </div>

    <!-- MENÚ EMPLEADO -->
    <div v-else-if="authStore.rol === 'Empleado'">
      <router-link to="/empleado/dashboard" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-speedometer"></i> Mi Panel
      </router-link>

      <router-link to="/empleado/reservas" class="nav-link-custom" active-class="active" @click="$emit('close')">
        <i class="bi bi-list-task"></i> Reservas Asignadas
      </router-link>
    </div>

    <div class="mt-5 px-2 pt-4 border-top">
      <router-link to="/perfil" class="nav-link-custom" @click="$emit('close')">
        <i class="bi bi-gear"></i> Configuración
      </router-link>
    </div>
  </aside>
</template>

<script setup>
import { useAuthStore } from '../stores/authStore'

defineProps({
  open: {
    type: Boolean,
    default: false
  }
})

defineEmits(['close'])

const authStore = useAuthStore()
</script>
