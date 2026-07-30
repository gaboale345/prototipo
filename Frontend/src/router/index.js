import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

import LoginView from '../views/auth/LoginView.vue'
import RegisterView from '../views/auth/RegisterView.vue'
import LandingView from '../views/LandingView.vue'
import DashboardAdminView from '../views/admin/DashboardAdminView.vue'
import DashboardClienteView from '../views/cliente/DashboardClienteView.vue'
import DashboardEmpleadoView from '../views/empleado/DashboardEmpleadoView.vue'
import ReservarView from '../views/cliente/ReservarView.vue'
import MisVehiculosView from '../views/cliente/MisVehiculosView.vue'
import MisReservasView from '../views/cliente/MisReservasView.vue'
import MisUbicacionesView from '../views/cliente/MisUbicacionesView.vue'
import ReservasAdminView from '../views/admin/ReservasAdminView.vue'
import ServiciosAdminView from '../views/admin/ServiciosAdminView.vue'
import InventarioAdminView from '../views/admin/InventarioAdminView.vue'
import VentasAdminView from '../views/admin/VentasAdminView.vue'
import ReportesAdminView from '../views/admin/ReportesAdminView.vue'
import AuditoriaAdminView from '../views/admin/AuditoriaAdminView.vue'
import ClientesAdminView from '../views/admin/ClientesAdminView.vue'
import EmpleadosAdminView from '../views/admin/EmpleadosAdminView.vue'
import PerfilView from '../views/shared/PerfilView.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView, meta: { guestOnly: true, publicLayout: true } },
  { path: '/registro', component: RegisterView, meta: { guestOnly: true, publicLayout: true } },

  // Admin Routes
  { path: '/admin/dashboard', component: DashboardAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/reservas', component: ReservasAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/servicios', component: ServiciosAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/clientes', component: ClientesAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/empleados', component: EmpleadosAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/inventario', component: InventarioAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/ventas', component: VentasAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/reportes', component: ReportesAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/auditoria', component: AuditoriaAdminView, meta: { requiresAuth: true, role: 'Administrador' } },
  { path: '/admin/usuarios', component: ClientesAdminView, meta: { requiresAuth: true, role: 'Administrador' } },

  // Cliente Routes
  { path: '/cliente/dashboard', component: DashboardClienteView, meta: { requiresAuth: true, role: 'Cliente' } },
  { path: '/cliente/reservar', component: ReservarView, meta: { requiresAuth: true, role: 'Cliente' } },
  { path: '/cliente/reservas', component: MisReservasView, meta: { requiresAuth: true, role: 'Cliente' } },
  { path: '/cliente/vehiculos', component: MisVehiculosView, meta: { requiresAuth: true, role: 'Cliente' } },
  { path: '/cliente/ubicaciones', component: MisUbicacionesView, meta: { requiresAuth: true, role: 'Cliente' } },

  // Empleado Routes
  { path: '/empleado/dashboard', component: DashboardEmpleadoView, meta: { requiresAuth: true, role: 'Empleado' } },
  { path: '/empleado/reservas', component: DashboardEmpleadoView, meta: { requiresAuth: true, role: 'Empleado' } },

  // Shared Routes
  { path: '/perfil', component: PerfilView, meta: { requiresAuth: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next('/login')
  }

  if (to.meta.guestOnly && authStore.isAuthenticated) {
    if (authStore.rol === 'Administrador') return next('/admin/dashboard')
    if (authStore.rol === 'Empleado') return next('/empleado/dashboard')
    return next('/cliente/dashboard')
  }

  if (to.meta.role && authStore.rol !== to.meta.role) {
    if (authStore.rol === 'Administrador') return next('/admin/dashboard')
    if (authStore.rol === 'Empleado') return next('/empleado/dashboard')
    return next('/cliente/dashboard')
  }

  next()
})

export default router
