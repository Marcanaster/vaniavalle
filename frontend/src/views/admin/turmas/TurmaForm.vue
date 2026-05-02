<template>
  <div class="space-y-6 max-w-3xl mx-auto">
    <div class="flex items-center gap-4">
      <button @click="$router.back()" class="p-2 bg-white rounded-lg shadow-sm border border-slate-200 text-slate-500 hover:text-primary transition-colors">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
      </button>
      <div>
        <h2 class="text-2xl font-bold text-slate-800">{{ isEditing ? 'Editar Turma' : 'Nova Turma' }}</h2>
        <p class="text-slate-500">{{ isEditing ? 'Atualize as informações da turma.' : 'Crie uma nova turma e defina seus limites de ocupação.' }}</p>
      </div>
    </div>

    <form @submit.prevent="salvar" class="bg-white p-6 rounded-xl shadow-sm border border-slate-200 space-y-6">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="md:col-span-2">
          <label class="block text-sm font-medium text-slate-700 mb-1">Nome da Turma *</label>
          <input v-model="form.nome" type="text" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
        </div>
        
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Nível *</label>
          <select v-model="form.nivel" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
            <option value="Iniciante">Iniciante</option>
            <option value="Intermediário">Intermediário</option>
            <option value="Avançado">Avançado</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Capacidade de Alunos *</label>
          <input v-model="form.capacidadeAlunos" type="number" min="1" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Idade Mínima</label>
          <input v-model="form.idadeMinima" type="number" min="0" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Idade Máxima</label>
          <input v-model="form.idadeMaxima" type="number" min="0" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
        </div>

        <div class="md:col-span-2">
          <label class="block text-sm font-medium text-slate-700 mb-2">Modalidades *</label>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-3 bg-slate-50 p-4 rounded-xl border border-slate-200">
            <label v-for="m in modalidades" :key="m.id" class="flex items-center gap-2 cursor-pointer group">
              <div class="relative flex items-center justify-center">
                <input 
                  type="checkbox" 
                  :value="m.id" 
                  v-model="form.modalidadeIds"
                  class="peer h-5 w-5 cursor-pointer appearance-none rounded border border-slate-300 checked:bg-primary checked:border-primary transition-all"
                >
                <svg class="absolute w-3.5 h-3.5 text-white opacity-0 peer-checked:opacity-100 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7"></path></svg>
              </div>
              <span class="text-sm text-slate-600 group-hover:text-primary transition-colors">{{ m.nome }}</span>
            </label>
          </div>
          <p v-if="form.modalidadeIds.length === 0" class="mt-1 text-xs text-rose-500">Selecione pelo menos uma modalidade.</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Professor Responsável</label>
          <select v-model="form.professorId" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
            <option :value="null">Nenhum (Definir depois)</option>
            <option v-for="p in professores" :key="p.id" :value="p.id">{{ p.nome }}</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Sala *</label>
          <select v-model="form.sala" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
            <option value="">Selecione a Sala</option>
            <option value="Sala Neida valle">Sala Neida valle</option>
            <option value="Sala Fernando Valle">Sala Fernando Valle</option>
          </select>
        </div>

        <div class="md:col-span-2 space-y-4">
          <label class="block text-sm font-medium text-slate-700">Grade de Horários Estruturada (Para o Calendário)</label>
          
          <div v-for="(h, index) in form.horarios" :key="index" class="flex flex-wrap items-center gap-3 p-3 bg-slate-50 rounded-xl border border-slate-200">
            <div class="flex-1 min-w-[140px]">
              <select v-model="h.diaSemana" class="w-full text-sm border-slate-300 rounded-lg focus:ring-primary focus:border-primary">
                <option :value="1">Segunda-feira</option>
                <option :value="2">Terça-feira</option>
                <option :value="3">Quarta-feira</option>
                <option :value="4">Quinta-feira</option>
                <option :value="5">Sexta-feira</option>
                <option :value="6">Sábado</option>
                <option :value="0">Domingo</option>
              </select>
            </div>
            
            <div class="flex items-center gap-2">
              <input v-model="h.horaInicio" type="time" class="text-sm border-slate-300 rounded-lg focus:ring-primary focus:border-primary">
              <span class="text-slate-400">até</span>
              <input v-model="h.horaFim" type="time" class="text-sm border-slate-300 rounded-lg focus:ring-primary focus:border-primary">
            </div>

            <button type="button" @click="removerHorario(index)" class="p-2 text-rose-500 hover:bg-rose-50 rounded-lg transition-colors">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
            </button>
          </div>

          <button type="button" @click="addHorario" class="flex items-center gap-2 text-primary hover:text-primary-dark font-medium text-sm p-2 rounded-lg hover:bg-primary/5 transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path></svg>
            Adicionar Novo Horário
          </button>
        </div>

        <div class="md:col-span-2 opacity-50">
          <label class="block text-sm font-medium text-slate-700 mb-1">Resumo do Horário (Opcional)</label>
          <input v-model="form.gradeHorarios" type="text" placeholder="Ex: Seg e Qua 18:00" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
        </div>
      </div>

      <div class="flex justify-end gap-4 pt-4 border-t border-slate-100">
        <button type="button" @click="$router.back()" class="px-6 py-2 border border-slate-300 rounded-lg text-slate-700 font-medium hover:bg-slate-50 transition-colors">Cancelar</button>
        <button type="submit" :disabled="loading" class="px-6 py-2 bg-primary hover:bg-primary-dark rounded-lg text-white font-medium transition-colors shadow-sm disabled:opacity-50">
          {{ loading ? 'Salvando...' : (isEditing ? 'Atualizar Turma' : 'Criar Turma') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { toast } from 'vue3-toastify'
import api from '../../../services/api'

const router = useRouter()
const route = useRoute()
const isEditing = computed(() => !!route.params.id)
const loading = ref(false)
const modalidades = ref([])
const professores = ref([])

const form = ref({
  nome: '',
  nivel: 'Iniciante',
  idadeMinima: 0,
  idadeMaxima: 99,
  capacidadeAlunos: 15,
  gradeHorarios: '',
  sala: '',
  horarios: [],
  modalidadeIds: [],
  professorId: null
})

const addHorario = () => {
  form.value.horarios.push({
    diaSemana: 1,
    horaInicio: '18:00',
    horaFim: '19:00'
  })
}

const removerHorario = (index) => {
  form.value.horarios.splice(index, 1)
}

onMounted(async () => {
  try {
    const [modRes, profRes] = await Promise.all([
      api.get('/modalidades'),
      api.get('/professores')
    ])
    modalidades.value = modRes.data
    professores.value = profRes.data

    if (isEditing.value) {
      const res = await api.get(`/turmas`) // Pega da lista por enquanto ou cria endpoint GetById
      const turmas = res.data
      const turma = turmas.find(t => t.id === route.params.id)
      if (turma) {
        form.value = {
          ...turma,
          modalidadeIds: turma.modalidades ? turma.modalidades.map(m => m.id) : [],
          horarios: turma.horarios.map(h => ({
            ...h,
            horaInicio: h.horaInicio.substring(0, 5),
            horaFim: h.horaFim.substring(0, 5)
          }))
        }
      }
    } else {
      // Se houver apenas uma modalidade, seleciona por padrão
      if (modalidades.value.length === 1) {
        form.value.modalidadeIds = [modalidades.value[0].id]
      }
    }
  } catch (err) {
    console.error('Erro ao carregar dados auxiliares', err)
    toast.error('Erro ao carregar modalidades ou professores.')
  }
})

const salvar = async () => {
  if (!form.value.modalidadeIds || form.value.modalidadeIds.length === 0) {
    toast.warning('Selecione pelo menos uma modalidade!')
    return
  }
  
  loading.value = true
  try {
    if (isEditing.value) {
      await api.put(`/turmas/${route.params.id}`, form.value)
      toast.success('Turma atualizada com sucesso!')
    } else {
      await api.post('/turmas', form.value)
      toast.success('Turma criada com sucesso!')
    }
    router.push({ name: 'AdminTurmas' })
  } catch (error) {
    console.error('Erro ao salvar turma', error)
    toast.error('Erro ao salvar turma. Verifique se todos os campos estão corretos.')
  }
  loading.value = false
}
</script>
