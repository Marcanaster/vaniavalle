<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Modalidades</h2>
        <p class="text-slate-500">Cadastre os tipos de dança oferecidos pela academia.</p>
      </div>
      <button @click="openModal()" class="bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2 shadow-sm">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Nova Modalidade
      </button>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <table class="min-w-full divide-y divide-slate-200">
        <thead class="bg-slate-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Nome</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Descrição</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200">
          <tr v-if="loading">
            <td colspan="3" class="px-6 py-10 text-center text-slate-500">Carregando...</td>
          </tr>
          <tr v-for="item in modalidades" :key="item.id" class="hover:bg-slate-50 transition-colors">
            <td class="px-6 py-4 whitespace-nowrap font-medium text-slate-900">{{ item.nome }}</td>
            <td class="px-6 py-4 text-sm text-slate-500">{{ item.descricao }}</td>
            <td class="px-6 py-4 text-right space-x-3">
              <button @click="openModal(item)" class="text-primary hover:text-primary-dark">Editar</button>
              <button @click="remover(item.id)" class="text-rose-500 hover:text-rose-700">Excluir</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal de Cadastro/Edição -->
    <div v-if="showModal" class="fixed inset-0 bg-slate-900/50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden">
        <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center">
          <h3 class="text-lg font-bold text-slate-800">{{ form.id ? 'Editar' : 'Nova' }} Modalidade</h3>
          <button @click="showModal = false" class="text-slate-400 hover:text-slate-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          </button>
        </div>
        <form @submit.prevent="salvar" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Nome *</label>
            <input v-model="form.nome" type="text" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Descrição</label>
            <textarea v-model="form.descricao" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border" rows="3"></textarea>
          </div>
          <div class="flex justify-end gap-3 pt-4">
            <button type="button" @click="showModal = false" class="px-4 py-2 text-slate-600 font-medium">Cancelar</button>
            <button type="submit" :disabled="saving" class="bg-primary hover:bg-primary-dark text-white px-6 py-2 rounded-lg font-bold transition-colors disabled:opacity-50">
              {{ saving ? 'Salvando...' : 'Salvar' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../../services/api'
import { toast } from 'vue3-toastify'

const modalidades = ref([])
const loading = ref(true)
const saving = ref(false)
const showModal = ref(false)

const form = ref({
  id: null,
  nome: '',
  descricao: ''
})

const loadData = async () => {
  loading.value = true
  try {
    const res = await api.get('/modalidades')
    modalidades.value = res.data
  } catch (err) {
    toast.error('Erro ao carregar modalidades')
  } finally {
    loading.value = false
  }
}

const openModal = (item = null) => {
  if (item) {
    form.value = { ...item }
  } else {
    form.value = { id: null, nome: '', descricao: '' }
  }
  showModal.value = true
}

const salvar = async () => {
  saving.value = true
  try {
    if (form.value.id) {
      await api.put(`/modalidades/${form.value.id}`, form.value)
      toast.success('Modalidade atualizada')
    } else {
      await api.post('/modalidades', form.value)
      toast.success('Modalidade criada')
    }
    showModal.value = false
    loadData()
  } catch (err) {
    toast.error('Erro ao salvar modalidade')
  } finally {
    saving.value = false
  }
}

const remover = async (id) => {
  if (!confirm('Deseja excluir esta modalidade? Isso pode afetar turmas existentes.')) return
  try {
    await api.delete(`/modalidades/${id}`)
    toast.success('Modalidade excluída')
    loadData()
  } catch (err) {
    toast.error('Erro ao excluir. Verifique se existem turmas vinculadas.')
  }
}

onMounted(loadData)
</script>
