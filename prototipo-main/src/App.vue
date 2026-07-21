<script setup>
import ServiceCard from './components/ServiceCard.vue'
import BookingForm from './components/BookingForm.vue'
import EncabezadoApp from './components/EncabezadoApp.vue'
</script>

<template>
  <EncabezadoApp
    titulo="Panel de Cliente"
    subtitulo="toda la informacion del cliente"
  />

<main>
  <h1> Modulos prinpipales </h1>
  <p> Compras, Listado, agendas y lavados </p>
</main>
</template>

<style scoped>
main {
max-width: 900px;
margin: 40px auto;
font-family: Arial, sans-serif;
}
h1 {
  margin-bottom: 8px;
}
</style>