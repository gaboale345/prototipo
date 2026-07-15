<script setup>
const props = defineProps({
  title: String,
  description: String,
  price: String,
  icon: String,
})
</script>

<template>
  <div class="service-card">
    <div class="icon">{{ icon }}</div>
    <h3>{{ title }}</h3>
    <p>{{ description }}</p>
    <span>{{ price }}</span>
  </div>
</template>

<style scoped>
.service-card {
  background: #fff;
  border-radius: 16px;
  padding: 1.2rem;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.08);
  text-align: left;
}
.icon {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}
h3 {
  margin: 0 0 0.4rem;
  color: #1f2937;
}
p {
  color: #6b7280;
  margin-bottom: 0.8rem;
}
span {
  font-weight: 700;
  color: #2563eb;
}
</style>
