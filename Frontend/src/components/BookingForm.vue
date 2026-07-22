<script setup>
import { ref } from 'vue'

const name = ref('')
const phone = ref('')
const service = ref('Lavado básico')
const message = ref('')

const submit = () => {
  message.value = `Gracias ${name.value || 'cliente'}, tu reserva para ${service.value} fue registrada.`
}
</script>

<template>
  <form class="booking-form" @submit.prevent="submit">
    <h3>Reserva tu turno</h3>
    <input v-model="name" placeholder="Tu nombre" />
    <input v-model="phone" placeholder="Tu teléfono" />
    <select v-model="service">
      <option>Lavado básico</option>
      <option>Lavado premium</option>
      <option>Lavado y encerado</option>
    </select>
    <button type="submit">Reservar</button>
    <p v-if="message">{{ message }}</p>
  </form>
</template>

<style scoped>
.booking-form {
  display: flex;
  flex-direction: column;
  gap: 0.7rem;
  background: #f9fafb;
  padding: 1.2rem;
  border-radius: 16px;
}
input,
select,
button {
  padding: 0.8rem;
  border-radius: 10px;
  border: 1px solid #d1d5db;
  font-size: 1rem;
}
button {
  background: #2563eb;
  color: white;
  border: none;
  cursor: pointer;
}
p {
  color: #047857;
  font-weight: 600;
}
</style>
