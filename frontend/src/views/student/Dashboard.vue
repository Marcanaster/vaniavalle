<template>
  <div class="space-y-8">
    <div class="bg-primary rounded-2xl p-6 sm:p-8 text-white shadow-lg overflow-hidden relative">
      <div class="relative z-10">
        <h1 class="text-3xl font-bold mb-2">Olá, Aluno!</h1>
        <p class="text-primary-light text-lg">Acompanhe suas aulas e faturas da academia.</p>
      </div>
      <!-- Decorativo -->
      <div class="absolute -top-24 -right-24 w-64 h-64 bg-white/10 rounded-full blur-3xl pointer-events-none"></div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Minhas Aulas -->
      <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6">
        <h2 class="text-xl font-bold text-slate-800 mb-6 flex items-center gap-2">
          <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
          Minha Grade de Aulas
        </h2>
        
        <div v-if="loadingAulas" class="text-center py-8 text-slate-500">Carregando aulas...</div>
        <div v-else-if="agendamentos.length === 0" class="text-center py-8 text-slate-500 bg-slate-50 rounded-xl border border-dashed border-slate-200">
          Você ainda não está matriculado em nenhuma turma.
        </div>
        <div v-else class="space-y-4">
          <div v-for="ag in agendamentos" :key="ag.id" class="flex items-start gap-4 p-4 rounded-xl border border-slate-100 hover:border-primary/30 hover:bg-primary/5 transition-colors">
            <div class="h-12 w-12 rounded-lg bg-primary/10 flex items-center justify-center text-primary font-bold text-lg flex-shrink-0">
              {{ ag.nome?.charAt(0) || 'T' }}
            </div>
            <div>
              <h3 class="font-bold text-slate-800">{{ ag.nome }}</h3>
              <p class="text-sm text-slate-500 mb-1">{{ ag.modalidade }} • {{ ag.sala || 'Sala não definida' }}</p>
              <div class="inline-flex items-center gap-1 text-xs font-medium bg-slate-100 text-slate-600 px-2 py-1 rounded-md">
                <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                {{ ag.gradeHorarios || 'Horário a definir' }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Minhas Faturas -->
      <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6">
        <h2 class="text-xl font-bold text-slate-800 mb-6 flex items-center gap-2">
          <svg class="w-6 h-6 text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
          Faturas
        </h2>

        <div v-if="loadingFaturas" class="text-center py-8 text-slate-500">Carregando faturas...</div>
        <div v-else-if="faturas.length === 0" class="text-center py-8 text-slate-500 bg-slate-50 rounded-xl border border-dashed border-slate-200">
          Nenhuma fatura encontrada.
        </div>
        <div v-else class="space-y-4">
          <div v-for="fatura in faturas" :key="fatura.id" class="flex items-center justify-between p-4 rounded-xl border border-slate-100">
            <div>
              <p class="font-bold text-slate-800">Mensalidade</p>
              <p class="text-sm text-slate-500">Vence em: {{ new Date(fatura.dataVencimento).toLocaleDateString('pt-BR') }}</p>
            </div>
            <div class="text-right">
              <p class="font-bold text-slate-800 mb-1">R$ {{ fatura.valorTotal.toFixed(2).replace('.', ',') }}</p>
              <span :class="fatura.status === 'Pago' ? 'bg-emerald-100 text-emerald-800' : 'bg-amber-100 text-amber-800'" class="px-2 py-1 text-xs font-bold rounded-md">
                {{ fatura.status }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'
import { useAuthStore } from '../../stores/authStore'

const authStore = useAuthStore()
const agendamentos = ref([])
const faturas = ref([])
const loadingAulas = ref(true)
const loadingFaturas = ref(true)

const perfil = ref(null)

onMounted(async () => {
  try {
    loadingAulas.value = true
    loadingFaturas.value = true
    
    const res = await api.get('/alunos/meu-perfil')
    perfil.value = res.data
    
    agendamentos.value = res.data.turmas || []
    faturas.value = res.data.faturas || []
    
  } catch(e) { 
    console.error('Erro ao carregar perfil do aluno:', e)
    agendamentos.value = []
    faturas.value = []
  } finally {
    loadingAulas.value = false
    loadingFaturas.value = false
  }
})
</script>
