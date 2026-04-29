<template>
  <div class="space-y-6 max-w-3xl mx-auto">
    <div class="flex items-center gap-4">
      <button @click="$router.back()" class="p-2 bg-white rounded-lg shadow-sm border border-slate-200 text-slate-500 hover:text-primary transition-colors">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
      </button>
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Nova Turma</h2>
        <p class="text-slate-500">Crie uma nova turma e defina seus limites de ocupação.</p>
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

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Modalidade *</label>
          <select v-model="form.modalidadeId" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
            <option value="" disabled>Selecione uma modalidade</option>
            <option v-for="m in modalidades" :key="m.id" :value="m.id">{{ m.nome }}</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 mb-1">Professor Responsável</label>
          <select v-model="form.professorId" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
            <option :value="null">Nenhum (Definir depois)</option>
            <option v-for="p in professores" :key="p.id" :value="p.id">{{ p.nome }}</option>
          </select>
        </div>

        <div class="md:col-span-2">
          <label class="block text-sm font-medium text-slate-700 mb-1">Grade de Horários *</label>
          <input v-model="form.gradeHorarios" type="text" required placeholder="Ex: Seg e Qua 18:00 - 19:30" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
        </div>
      </div>

      <div class="flex justify-end gap-4 pt-4 border-t border-slate-100">
        <button type="button" @click="$router.back()" class="px-6 py-2 border border-slate-300 rounded-lg text-slate-700 font-medium hover:bg-slate-50 transition-colors">Cancelar</button>
        <button type="submit" :disabled="loading" class="px-6 py-2 bg-primary hover:bg-primary-dark rounded-lg text-white font-medium transition-colors shadow-sm disabled:opacity-50">
          {{ loading ? 'Salvando...' : 'Criar Turma' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'
import api from '../../../services/api'

const router = useRouter()
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
  modalidadeId: '',
  professorId: null
})

onMounted(async () => {
  try {
    const [modRes, profRes] = await Promise.all([
      api.get('/modalidades'),
      api.get('/professores')
    ])
    modalidades.value = modRes.data
    professores.value = profRes.data
    
    // Se houver apenas uma modalidade, seleciona por padrão
    if (modalidades.value.length === 1) {
      form.value.modalidadeId = modalidades.value[0].id
    }
  } catch (err) {
    console.error('Erro ao carregar dados auxiliares', err)
    toast.error('Erro ao carregar modalidades ou professores.')
  }
})

const salvar = async () => {
  if (!form.value.modalidadeId) {
    toast.warning('Selecione uma modalidade!')
    return
  }
  
  loading.value = true
  try {
    await api.post('/turmas', form.value)
    toast.success('Turma criada com sucesso!')
    router.push({ name: 'AdminTurmas' })
  } catch (error) {
    console.error('Erro ao criar turma', error)
    toast.error('Erro ao salvar turma. Verifique se todos os campos estão corretos.')
  }
  loading.value = false
}
</script>
