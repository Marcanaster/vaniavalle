<template>
  <Transition name="fade">
    <div v-if="show" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="$emit('cancel')"></div>
      
      <!-- Modal Content -->
      <Transition name="zoom">
        <div v-if="show" class="relative bg-white rounded-2xl shadow-2xl w-full max-w-md overflow-hidden border border-slate-200">
          <div class="p-6 sm:p-8">
            <div class="flex items-center justify-center w-12 h-12 mb-6 rounded-full bg-amber-100 text-amber-600 mx-auto">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path>
              </svg>
            </div>
            
            <h3 class="text-xl font-bold text-slate-900 text-center mb-2">{{ title }}</h3>
            <p class="text-slate-500 text-center">{{ message }}</p>
          </div>
          
          <div class="flex items-center gap-3 p-4 bg-slate-50 border-t border-slate-100">
            <button 
              @click="$emit('cancel')" 
              class="flex-1 px-4 py-2.5 text-sm font-semibold text-slate-700 bg-white border border-slate-300 rounded-xl hover:bg-slate-50 transition-colors shadow-sm"
            >
              Cancelar
            </button>
            <button 
              @click="$emit('confirm')" 
              class="flex-1 px-4 py-2.5 text-sm font-semibold text-white bg-indigo-600 rounded-xl hover:bg-indigo-700 transition-all shadow-md shadow-indigo-200 active:scale-95"
            >
              Confirmar
            </button>
          </div>
        </div>
      </Transition>
    </div>
  </Transition>
</template>

<script setup>
defineProps({
  show: Boolean,
  title: {
    type: String,
    default: 'Confirmação'
  },
  message: {
    type: String,
    default: 'Você tem certeza que deseja realizar esta ação?'
  }
})

defineEmits(['confirm', 'cancel'])
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.zoom-enter-active {
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.zoom-leave-active {
  transition: all 0.2s ease-in;
}

.zoom-enter-from,
.zoom-leave-to {
  opacity: 0;
  transform: scale(0.9) translateY(10px);
}
</style>
