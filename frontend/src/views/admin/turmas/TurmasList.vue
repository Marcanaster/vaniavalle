<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Turmas</h2>
        <p class="text-slate-500">Gerenciamento das turmas e modalidades da academia.</p>
      </div>
      <router-link :to="{ name: 'AdminTurmasNova' }" class="bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2 shadow-sm">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Nova Turma
      </router-link>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden flex flex-col">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Nome da Turma</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Nível</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Idade</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Sala</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Capacidade</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider">Ações</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-200">
            <tr v-if="loading">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Carregando turmas...</td>
            </tr>
            <tr v-else-if="turmas.length === 0">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Nenhuma turma cadastrada.</td>
            </tr>
            <tr v-for="turma in turmas" :key="turma.id" class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-slate-900">{{ turma.nome }}</div>
                <div class="text-sm text-slate-500">{{ turma.gradeHorarios }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ turma.nivel }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ turma.idadeMinima }} a {{ turma.idadeMaxima }} anos</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-700 font-medium">{{ turma.sala || '-' }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ turma.capacidadeAlunos }} vagas</td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button @click="openMatriculasModal(turma)" class="text-indigo-600 hover:text-indigo-800 mr-4 font-bold flex inline-flex items-center gap-1">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
                  Alunos
                </button>
                <router-link :to="{ name: 'AdminTurmasEditar', params: { id: turma.id } }" class="text-primary hover:text-primary-dark">Editar</router-link>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal de Matrículas -->
    <div v-if="showMatriculasModal" class="fixed inset-0 bg-slate-900/50 flex items-center justify-center z-50 px-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-2xl overflow-hidden flex flex-col max-h-[90vh]">
        <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50">
          <div>
            <h3 class="text-lg font-bold text-slate-800">Alunos da Turma</h3>
            <p class="text-xs text-slate-500">{{ turmaSelecionada?.nome }} ({{ turmaSelecionada?.gradeHorarios }})</p>
          </div>
          <button @click="closeMatriculasModal" class="text-slate-400 hover:text-slate-600 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          </button>
        </div>
        
        <div class="p-6 overflow-y-auto flex-1">
          <!-- Form de nova matrícula -->
          <div class="bg-slate-50 p-4 rounded-xl border border-slate-200 mb-6 space-y-4">
            <div class="flex gap-3 items-end">
              <div class="flex-1">
                <label class="block text-sm font-medium text-slate-700 mb-1 text-slate-600 uppercase text-[10px] font-bold">Adicionar Aluno</label>
                <select v-model="novoAlunoId" class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary text-sm">
                  <option value="">Selecione um aluno...</option>
                  <option v-for="aluno in alunosDisponiveis" :key="aluno.id" :value="aluno.id">
                    {{ aluno.nomeCompleto }}
                  </option>
                </select>
              </div>
              <div class="w-24">
                <label class="block text-sm font-medium text-slate-700 mb-1 text-slate-600 uppercase text-[10px] font-bold">Mensal (R$)</label>
                <input v-model="matriculaForm.valorMensal" type="number" step="0.01" class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary text-sm">
              </div>
              <div class="w-20">
                <label class="block text-sm font-medium text-slate-700 mb-1 text-slate-600 uppercase text-[10px] font-bold">Desc %</label>
                <input v-model="matriculaForm.descontoPercentual" type="number" class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary text-sm">
              </div>
              <div class="w-24">
                <label class="block text-sm font-medium text-slate-700 mb-1 text-slate-600 uppercase text-[10px] font-bold">Matric. (R$)</label>
                <input v-model="matriculaForm.valorMatricula" type="number" step="0.01" class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary text-sm">
              </div>
            </div>
            
            <div v-if="novoAlunoId" class="flex items-center justify-between text-[11px] bg-indigo-50 p-2 rounded-lg border border-indigo-100">
              <div class="text-indigo-700 font-medium">
                <span class="font-bold">Regra de Desconto:</span> 
                Bolsa ({{ alunoSelecionado?.descontoBolsa || 0 }}%) vs Negociado ({{ matriculaForm.descontoPercentual || 0 }}%)
              </div>
              <div class="text-indigo-800 font-bold uppercase">
                Desconto Real na Fatura: {{ Math.Max(alunoSelecionado?.descontoBolsa || 0, matriculaForm.descontoPercentual || 0) || 0 }}%
              </div>
            </div>
            <div class="flex justify-end">
              <button @click="matricularAluno" :disabled="!novoAlunoId || salvandoMatricula" class="bg-indigo-600 hover:bg-indigo-700 text-white px-6 py-2 rounded-lg font-bold transition-colors disabled:opacity-50 shadow-sm flex items-center gap-2">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
                {{ salvandoMatricula ? 'Adicionando...' : 'Confirmar Matrícula' }}
              </button>
            </div>
          </div>

          <!-- Lista de Matriculados -->
          <h4 class="font-bold text-slate-700 mb-3 border-b border-slate-100 pb-2 flex items-center justify-between">
            <span>Alunos Atuais</span>
            <span class="bg-slate-200 text-slate-600 text-[10px] px-2 py-0.5 rounded-full">{{ turmaSelecionada?.alunosMatriculados?.filter(m => m.ativo).length || 0 }}</span>
          </h4>
          <div class="space-y-2">
            <div v-if="!turmaSelecionada?.alunosMatriculados || turmaSelecionada.alunosMatriculados.filter(m => m.ativo).length === 0" class="text-center py-6 text-slate-400 text-sm italic">
              Nenhum aluno matriculado nesta turma.
            </div>
            
            <div v-for="matricula in turmaSelecionada?.alunosMatriculados?.filter(m => m.ativo)" :key="matricula.id" class="flex justify-between items-center p-3 hover:bg-slate-50 border border-slate-100 rounded-lg group transition-all">
              <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-slate-600 font-bold text-xs group-hover:bg-primary/10 group-hover:text-primary transition-colors">
                  {{ matricula.aluno?.nomeCompleto?.substring(0,2).toUpperCase() }}
                </div>
                <div>
                  <div class="font-medium text-slate-800 text-sm">{{ matricula.aluno?.nomeCompleto || 'Aluno não identificado' }}</div>
                  <div class="text-[10px] text-slate-500 uppercase font-semibold">
                    Acordado: R$ {{ (matricula.valorMensal || 0).toFixed(2) }} 
                    <span v-if="matricula.descontoPercentual > 0" class="text-emerald-600 ml-1">(-{{ matricula.descontoPercentual }}%)</span>
                  </div>
                </div>
              </div>
              <button @click="removerMatricula(matricula.alunoId)" class="text-slate-300 hover:text-rose-600 transition-colors">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de Confirmação Moderno para Desmatrícula -->
    <ConfirmModal 
      :show="showConfirmRemocao"
      title="Remover da Turma"
      message="Tem certeza que deseja remover este aluno desta turma? As faturas já geradas não serão excluídas automaticamente."
      @confirm="handleConfirmarRemocaoMatricula"
      @cancel="showConfirmRemocao = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import api from '../../../services/api'
import { toast } from 'vue3-toastify'
import ConfirmModal from '../../../components/ConfirmModal.vue'

const turmas = ref([])
const alunos = ref([])
const loading = ref(true)

// Modal de matrículas
const showMatriculasModal = ref(false)
const turmaSelecionada = ref(null)
const novoAlunoId = ref('')
const salvandoMatricula = ref(false)

// Estado para o modal de confirmação de remoção
const showConfirmRemocao = ref(false)
const alunoIdParaRemoverDaTurma = ref(null)

const matriculaForm = ref({
  valorMensal: 120,
  descontoPercentual: 0,
  valorMatricula: 0
})

const alunoSelecionado = computed(() => {
  if (!novoAlunoId.value) return null
  return alunos.value.find(a => a.id === novoAlunoId.value)
})

// Observar quando um aluno é selecionado para sugerir o desconto da bolsa dele
watch(novoAlunoId, (id) => {
  if (id) {
    const aluno = alunoSelecionado.value
    if (aluno && aluno.descontoBolsa > 0) {
      matriculaForm.value.descontoPercentual = aluno.descontoBolsa
    } else {
      matriculaForm.value.descontoPercentual = 0
    }
  }
})

const loadTurmas = async () => {
  try {
    const res = await api.get('/turmas')
    turmas.value = res.data
  } catch (err) {
    console.error(err)
  }
}

const loadAlunos = async () => {
  try {
    const res = await api.get('/alunos')
    alunos.value = res.data
  } catch (err) {
    console.error(err)
  }
}

onMounted(async () => {
  loading.value = true
  await Promise.all([loadTurmas(), loadAlunos()])
  loading.value = false
})

const openMatriculasModal = (turma) => {
  turmaSelecionada.value = turma
  novoAlunoId.value = ''
  showMatriculasModal.value = true
}

const closeMatriculasModal = () => {
  showMatriculasModal.value = false
  turmaSelecionada.value = null
}

const alunosDisponiveis = computed(() => {
  if (!turmaSelecionada.value) return []
  // Filtra alunos que ainda não estão matriculados e ativos nesta turma
  const idsMatriculados = turmaSelecionada.value.alunosMatriculados
    ?.filter(m => m.ativo)
    .map(m => m.alunoId) || []
    
  return alunos.value.filter(a => !idsMatriculados.includes(a.id))
})

const matricularAluno = async () => {
  if (!novoAlunoId.value || !turmaSelecionada.value) return
  
  try {
    salvandoMatricula.value = true
    await api.post(`/turmas/${turmaSelecionada.value.id}/matricular`, {
      alunoId: novoAlunoId.value,
      valorMensal: matriculaForm.value.valorMensal,
      descontoPercentual: matriculaForm.value.descontoPercentual,
      valorMatricula: matriculaForm.value.valorMatricula
    })
    toast.success('Aluno matriculado com as condições negociadas!')
    await loadTurmas() 
    if (turmaSelecionada.value) {
      const atualizada = turmas.value.find(t => t.id === turmaSelecionada.value.id)
      if (atualizada) {
        turmaSelecionada.value = atualizada
      }
    }
    novoAlunoId.value = ''
  } catch (error) {
    toast.error('Erro ao matricular aluno')
  } finally {
    salvandoMatricula.value = false
  }
}

const removerMatricula = (alunoId) => {
  alunoIdParaRemoverDaTurma.value = alunoId
  showConfirmRemocao.value = true
}

const handleConfirmarRemocaoMatricula = async () => {
  const alunoId = alunoIdParaRemoverDaTurma.value
  try {
    await api.delete(`/turmas/${turmaSelecionada.value.id}/desmatricular/${alunoId}`)
    toast.success('Aluno removido da turma.')
    await loadTurmas()
    turmaSelecionada.value = turmas.value.find(t => t.id === turmaSelecionada.value.id)
  } catch (error) {
    toast.error('Erro ao remover aluno')
  } finally {
    showConfirmRemocao.value = false
    alunoIdParaRemoverDaTurma.value = null
  }
}
</script>
