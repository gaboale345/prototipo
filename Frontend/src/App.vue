<template>
  <!-- Layout para vistas públicas (Landing, Login, Registro) -->
  <div v-if="route.meta.publicLayout" class="h-100">
    <router-view />
  </div>

  <!-- Layout para el sistema interno (Dashboard) -->
  <div v-else class="min-vh-100 d-flex flex-column bg-light">
    <Navbar @toggle-sidebar="toggleSidebar" />

    <div class="d-flex flex-grow-1 position-relative">
      <!-- Overlay oscuro en móvil cuando sidebar está abierto -->
      <div
        v-if="sidebarOpen"
        class="sidebar-overlay d-md-none"
        @click="sidebarOpen = false"
      ></div>

      <Sidebar
        v-if="authStore.isAuthenticated"
        :open="sidebarOpen"
        @close="sidebarOpen = false"
      />

      <main class="flex-grow-1 main-content overflow-auto">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Navbar from './components/Navbar.vue'
import Sidebar from './components/Sidebar.vue'
import { useAuthStore } from './stores/authStore'
import { useThemeStore } from './stores/themeStore'
import { useRoute } from 'vue-router'

const authStore = useAuthStore()
const themeStore = useThemeStore()
const route = useRoute()

const sidebarOpen = ref(false)

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

onMounted(() => {
  themeStore.applyTheme()
})
</script>
