<template>
  <div class="flex items-center justify-between border-t border-slate-200 bg-white px-4 py-3 sm:px-6 mt-4 rounded-b-xl">
    <div class="flex flex-1 justify-between sm:hidden">
      <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1" class="relative inline-flex items-center rounded-md border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50">Anterior</button>
      <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages" class="relative ml-3 inline-flex items-center rounded-md border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50">Próxima</button>
    </div>
    <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
      <div class="flex items-center gap-4">
        <p class="text-sm text-slate-700">
          Mostrando <span class="font-medium">{{ startItem }}</span> até <span class="font-medium">{{ endItem }}</span> de <span class="font-medium">{{ totalItems }}</span> resultados
        </p>
        <select v-model="localPageSize" @change="changePageSize" class="text-sm border-slate-300 rounded-md focus:ring-primary focus:border-primary py-1 px-2">
          <option :value="10">10 por página</option>
          <option :value="20">20 por página</option>
          <option :value="50">50 por página</option>
        </select>
      </div>
      <div>
        <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
          <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1" class="relative inline-flex items-center rounded-l-md px-2 py-2 text-slate-400 ring-1 ring-inset ring-slate-300 hover:bg-slate-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50">
            <span class="sr-only">Anterior</span>
            <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z" clip-rule="evenodd" /></svg>
          </button>
          
          <button v-for="page in pagesArray" :key="page" @click="changePage(page)" 
                  :class="[page === currentPage ? 'relative z-10 inline-flex items-center bg-primary px-4 py-2 text-sm font-semibold text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary' : 'relative inline-flex items-center px-4 py-2 text-sm font-semibold text-slate-900 ring-1 ring-inset ring-slate-300 hover:bg-slate-50 focus:z-20 focus:outline-offset-0']">
            {{ page }}
          </button>
          
          <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages" class="relative inline-flex items-center rounded-r-md px-2 py-2 text-slate-400 ring-1 ring-inset ring-slate-300 hover:bg-slate-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50">
            <span class="sr-only">Próxima</span>
            <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" /></svg>
          </button>
        </nav>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  totalItems: { type: Number, required: true },
  pageSize: { type: Number, default: 10 },
  currentPage: { type: Number, default: 1 }
})

const emit = defineEmits(['update:page', 'update:pageSize'])

const localPageSize = ref(props.pageSize)

watch(() => props.pageSize, (newVal) => {
  localPageSize.value = newVal
})

const totalPages = computed(() => Math.max(1, Math.ceil(props.totalItems / localPageSize.value)))
const startItem = computed(() => props.totalItems === 0 ? 0 : (props.currentPage - 1) * localPageSize.value + 1)
const endItem = computed(() => Math.min(props.currentPage * localPageSize.value, props.totalItems))

const pagesArray = computed(() => {
  let pages = []
  // Lógica simples para MVP: Exibir até 5 páginas ou todas se <= 5.
  let start = Math.max(1, props.currentPage - 2)
  let end = Math.min(totalPages.value, props.currentPage + 2)
  
  if (end - start < 4) {
      if (start === 1) end = Math.min(totalPages.value, start + 4)
      else if (end === totalPages.value) start = Math.max(1, end - 4)
  }

  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

const changePage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    emit('update:page', page)
  }
}

const changePageSize = () => {
  emit('update:pageSize', localPageSize.value)
  emit('update:page', 1) 
}
</script>
