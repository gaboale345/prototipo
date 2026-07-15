<script setup>
import ServiceCard from './components/ServiceCard.vue'
import BookingForm from './components/BookingForm.vue'

const services = [
  {
    title: 'Lavado básico',
    description: 'Limpieza exterior rápida y eficaz.',
    price: '$8.000',
    icon: '🚗',
  },
  {
    title: 'Lavado premium',
    description: 'Lavado completo con abrillantado.',
    price: '$15.000',
    icon: '✨',
  },
  {
    title: 'Lavado y encerado',
    description: 'Protección brillante para tu vehículo.',
    price: '$22.000',
    icon: '🧽',
  },
]
</script>

<template>
  <main class="page">
    <section class="hero">
      <div>
        <p class="eyebrow">Auto Lavado Premium</p>
        <h1>Tu vehículo merece un cuidado de primera.</h1>
        <p class="intro">
          Ofrecemos lavado rápido, profesional y con resultados impecables.
        </p>
      </div>
      <BookingForm />
    </section>

    <section class="services">
      <h2>Nuestros servicios</h2>
      <div class="cards">
        <ServiceCard
          v-for="service in services"
          :key="service.title"
          :title="service.title"
          :description="service.description"
          :price="service.price"
          :icon="service.icon"
        />
      </div>
    </section>
  </main>
</template>

<style scoped>
.page {
  min-height: 100vh;
  background: linear-gradient(135deg, #eff6ff, #f8fafc);
  color: #111827;
  padding: 2rem;
  font-family: Arial, sans-serif;
}
.hero {
  display: grid;
  grid-template-columns: 1.2fr 0.8fr;
  gap: 2rem;
  align-items: center;
  max-width: 1100px;
  margin: 0 auto 2rem;
}
.eyebrow {
  text-transform: uppercase;
  letter-spacing: 0.2em;
  color: #2563eb;
  font-weight: 700;
}
h1 {
  font-size: 2.2rem;
  margin: 0.3rem 0 0.8rem;
}
.intro {
  font-size: 1.05rem;
  color: #4b5563;
}
.services {
  max-width: 1100px;
  margin: 0 auto;
}
.services h2 {
  margin-bottom: 1rem;
}
.cards {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}
@media (max-width: 800px) {
  .hero,
  .cards {
    grid-template-columns: 1fr;
  }
}
</style>
