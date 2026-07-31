<template>
  <div class="stepper-container mb-4">
    <div class="stepper-wrapper">
      <div class="stepper-item" :class="{ 'active': currentStep === 1, 'completed': currentStep > 1 }">
        <div class="step-counter">
          <i v-if="currentStep > 1" class="bi bi-check-lg"></i>
          <span v-else>1</span>
        </div>
        <div class="step-name">Datos de Reserva</div>
      </div>
      
      <div class="stepper-divider" :class="{ 'completed': currentStep > 1 }"></div>
      
      <div class="stepper-item" :class="{ 'active': currentStep === 2, 'completed': currentStep > 2 }">
        <div class="step-counter">
          <i v-if="currentStep > 2" class="bi bi-check-lg"></i>
          <span v-else>2</span>
        </div>
        <div class="step-name">Resumen y Pago</div>
      </div>
      
      <div class="stepper-divider" :class="{ 'completed': currentStep > 2 }"></div>
      
      <div class="stepper-item" :class="{ 'active': currentStep === 3, 'completed': currentStep > 3 }">
        <div class="step-counter">
          <i v-if="currentStep > 3" class="bi bi-check-lg"></i>
          <span v-else>3</span>
        </div>
        <div class="step-name">Confirmación</div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  currentStep: {
    type: Number,
    required: true,
    default: 1
  }
})
</script>

<style scoped>
.stepper-container {
  width: 100%;
  padding: 0.5rem 0 2rem 0;
}

.stepper-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  max-width: 600px;
  margin: 0 auto;
}

.stepper-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  flex: 0 0 auto;
  min-width: 100px;
}

.stepper-divider {
  flex: 1;
  height: 3px;
  background-color: #e9ecef;
  margin: 0 5px;
  position: relative;
  top: -10px;
  transition: background-color 0.3s ease;
}

.stepper-divider.completed {
  background-color: #2d6a4f;
}

.step-counter {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background-color: #e9ecef;
  color: #6c757d;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 1rem;
  margin-bottom: 8px;
  transition: all 0.3s ease;
}

.step-name {
  font-size: 0.85rem;
  font-weight: 600;
  color: #6c757d;
  text-align: center;
  position: absolute;
  top: 40px;
  width: 120px;
}

.stepper-item.active .step-counter {
  background-color: #2d6a4f;
  color: white;
  box-shadow: 0 0 0 4px rgba(45, 106, 79, 0.2);
}

.stepper-item.active .step-name {
  color: #2d6a4f;
}

.stepper-item.completed .step-counter {
  background-color: #2d6a4f;
  color: white;
}

.stepper-item.completed .step-name {
  color: #2d6a4f;
}

@media (max-width: 576px) {
  .step-name {
    font-size: 0.7rem;
    width: 80px;
  }
}
</style>
