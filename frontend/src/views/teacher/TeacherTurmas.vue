<template>
  <div class="space-y-6 pb-20">
    <div class="flex items-center justify-between">
      <h2 class="text-2xl font-bold text-slate-800">Minhas Turmas</h2>
    </div>

    <div v-if="loading" class="text-center py-10 text-slate-500">Carregando turmas...</div>
    
    <div v-else-if="turmas.length === 0" class="bg-white p-10 rounded-2xl text-center border border-dashed border-slate-300">
      <p class="text-slate-500">Você não tem turmas vinculadas ao seu perfil.</p>
    </div>

    <div v-else class="grid gap-4">
      <div v-for="turma in turmas" :key="turma.id" 
           @click="$router.push({ name: 'TeacherChamada', params: { id: turma.id } })"
           class="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm hover:border-primary transition-all cursor-pointer">
        <div class="flex items-start justify-between">
          <div>
            <span class="inline-block px-2 py-1 bg-slate-100 text-slate-500 text-[10px] font-bold rounded uppercase mb-2">
              {{ turma.modalidades?.map(m => m.nome).join(', ') || 'Sem modalidade' }}
            </span>
            <h3 class="text-lg font-bold text-slate-800">{{ turma.nome }}</h3>
            <p class="text-sm text-slate-500 mt-1">{{ turma.gradeHorarios }}</p>
          </div>
          <div class="bg-primary/10 text-primary px-3 py-1 rounded-lg text-xs font-bold">
            {{ turma.alunosMatriculados?.length || 0 }} Alunos
          </div>
        </div>
        
        <div class="mt-4 flex items-center gap-2 text-primary font-bold text-xs">
          <span>Ver alunos e chamada</span>
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4 4H3"></path></svg>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const turmas = ref([])
const loading = ref(true)

onMounted(async () => {
  try {
    const res = await api.get('/professores/minhas-turmas')
    turmas.value = res.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
})
</script>
