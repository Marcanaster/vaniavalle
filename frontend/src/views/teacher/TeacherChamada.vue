<template>
  <div class="space-y-6">
    <div class="flex items-center gap-4">
      <button @click="$router.back()" class="p-2 bg-white rounded-lg shadow-sm border border-slate-200 text-slate-500">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
      </button>
      <div>
        <h2 class="text-xl font-bold text-slate-800">{{ turma?.nome }}</h2>
        <p class="text-xs text-slate-500">{{ new Date().toLocaleDateString('pt-BR', { weekday: 'long', day: 'numeric', month: 'long' }) }}</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-10 text-slate-500">Carregando lista de alunos...</div>

    <div v-else class="space-y-4">
      <div v-for="matricula in turma.alunosMatriculados" :key="matricula.id" 
           class="bg-white p-4 rounded-xl border border-slate-200 shadow-sm flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 bg-slate-100 rounded-full flex items-center justify-center font-bold text-slate-400">
            {{ matricula.aluno.nomeCompleto.charAt(0) }}
          </div>
          <div>
            <p class="font-bold text-slate-800 text-sm">{{ matricula.aluno.nomeCompleto }}</p>
            <p class="text-[10px] text-slate-400">Matrícula ativa</p>
          </div>
        </div>
        
        <button @click="togglePresenca(matricula.alunoId)" 
                :class="estaPresente(matricula.alunoId) ? 'bg-emerald-500 text-white' : 'bg-slate-100 text-slate-400'"
                class="px-6 py-2 rounded-lg font-bold text-xs transition-colors shadow-sm">
          {{ estaPresente(matricula.alunoId) ? 'PRESENTE' : 'FALTOU' }}
        </button>
      </div>

      <div class="pt-6">
        <button @click="salvarChamada" :disabled="salvando" class="w-full bg-primary hover:bg-primary-dark text-white py-4 rounded-2xl font-bold shadow-lg transition-transform active:scale-95 disabled:opacity-50">
          {{ salvando ? 'Salvando...' : 'FINALIZAR CHAMADA' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'
import api from '../../services/api'

const route = useRoute()
const router = useRouter()
const loading = ref(true)
const salvando = ref(false)
const turma = ref(null)
const presencas = ref([])

onMounted(async () => {
  try {
    const res = await api.get('/professores/minhas-turmas')
    turma.value = res.data.find(t => t.id === route.params.id)
    if (turma.value) {
      // Inicializa todos como presentes
      presencas.value = turma.value.alunosMatriculados.map(m => ({
        alunoId: m.alunoId,
        presente: true
      }))
    }
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
})

const estaPresente = (alunoId) => {
  const p = presencas.value.find(x => x.alunoId === alunoId)
  return p ? p.presente : false
}

const togglePresenca = (alunoId) => {
  const p = presencas.value.find(x => x.alunoId === alunoId)
  if (p) p.presente = !p.presente
}

const salvarChamada = async () => {
  salvando.value = true
  try {
    const payload = presencas.value.map(p => ({
      turmaId: turma.value.id,
      alunoId: p.alunoId,
      presente: p.presente
    }))
    await api.post('/professores/chamada', payload)
    toast.success('Chamada realizada com sucesso!')
    router.push({ name: 'TeacherDashboard' })
  } catch (err) {
    toast.error('Erro ao salvar chamada.')
  } finally {
    salvando.value = false
  }
}
</script>
