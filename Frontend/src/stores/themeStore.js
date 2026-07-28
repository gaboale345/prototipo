import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useThemeStore = defineStore('theme', () => {
  const isDark = ref(localStorage.getItem('ecowash_theme') === 'dark')

  const applyTheme = () => {
    if (isDark.value) {
      document.documentElement.setAttribute('data-bs-theme', 'dark')
      document.documentElement.classList.add('dark-mode')
      document.body.classList.add('dark-mode')
    } else {
      document.documentElement.setAttribute('data-bs-theme', 'light')
      document.documentElement.classList.remove('dark-mode')
      document.body.classList.remove('dark-mode')
    }
  }

  const toggleTheme = () => {
    isDark.value = !isDark.value
    localStorage.setItem('ecowash_theme', isDark.value ? 'dark' : 'light')
    applyTheme()
  }

  // Inicializar al cargar
  applyTheme()

  return { isDark, toggleTheme, applyTheme }
})
