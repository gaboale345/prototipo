<template>
  <div class="min-vh-100 d-flex flex-column bg-light">
    <Navbar />
    <div class="d-flex flex-grow-1">
      <Sidebar v-if="authStore.isAuthenticated" />
      <main class="flex-grow-1 p-4 overflow-auto">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import Navbar from './components/Navbar.vue'
import Sidebar from './components/Sidebar.vue'
import { useAuthStore } from './stores/authStore'

const authStore = useAuthStore()
</script>
