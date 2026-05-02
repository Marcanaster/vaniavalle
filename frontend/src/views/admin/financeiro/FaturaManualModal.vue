<template>
  <div v-if="show" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
    <div class="bg-white rounded-2xl shadow-xl border border-slate-200 w-full max-w-md overflow-hidden animate-in fade-in zoom-in duration-200">
      <div class="p-6 border-b border-slate-100 flex justify-between items-center bg-slate-50">
        <h3 class="text-xl font-bold text-slate-800">Nova Fatura Manual</h3>
        <button @click="$emit('cancel')" class="text-slate-400 hover:text-slate-600 transition-colors">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
        </button>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-4">
        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-1">Aluno *</label>
          <select v-model="form.alunoId" required class="w-full rounded-xl border-slate-200 focus:border-indigo-500 focus:ring-indigo-500 transition-all text-sm">
            <option value="" disabled>Selecione um aluno</option>
            <option v-for="aluno in alunos" :key="aluno.id" :value="aluno.id">
              {{ aluno.nomeCompleto }}
            </option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-1">Motivo / Descrição *</label>
          <input v-model="form.descricao" type="text" required placeholder="Ex: Compra de Uniforme"
                 class="w-full rounded-xl border-slate-200 focus:border-indigo-500 focus:ring-indigo-500 transition-all text-sm" />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-1">Valor (R$) *</label>
            <input v-model.number="form.valor" type="number" step="0.01" required
                   class="w-full rounded-xl border-slate-200 focus:border-indigo-500 focus:ring-indigo-500 transition-all text-sm" />
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-1">Vencimento *</label>
            <input v-model="form.dataVencimento" type="date" required
                   class="w-full rounded-xl border-slate-200 focus:border-indigo-500 focus:ring-indigo-500 transition-all text-sm" />
          </div>
        </div>

        <div class="pt-4 flex gap-3">
          <button type="button" @click="$emit('cancel')"
                  class="flex-1 px-4 py-2.5 border border-slate-200 text-slate-600 rounded-xl font-semibold hover:bg-slate-50 transition-all">
            Cancelar
          </button>
          <button type="submit" :disabled="submitting"
                  class="flex-1 px-4 py-2.5 bg-indigo-600 text-white rounded-xl font-semibold hover:bg-indigo-700 transition-all disabled:opacity-50 shadow-md shadow-indigo-200">
            {{ submitting ? 'Salvando...' : 'Criar Fatura' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import api from '../../../services/api'
import { toast } from 'vue3-toastify'

const props = defineProps({
  show: Boolean
})

const emit = defineEmits(['confirm', 'cancel'])

const alunos = ref([])
const submitting = ref(false)

const form = reactive({
  alunoId: '',
  descricao: '',
  valor: 0,
  dataVencimento: new Date().toISOString().split('T')[0]
})

onMounted(async () => {
  try {
    const res = await api.get('/alunos')
    alunos.value = res.data
  } catch (err) {
    toast.error('Erro ao carregar lista de alunos')
  }
})

const handleSubmit = async () => {
  try {
    submitting.value = true
    const payload = {
      alunoId: form.alunoId,
      dataVencimento: form.dataVencimento,
      items: [
        {
          descricao: form.descricao,
          valorBase: form.valor,
          descontoPercentual: 0
        }
      ]
    }
    
    await api.post('/financeiro/faturas', payload)
    toast.success('Fatura criada com sucesso!')
    emit('confirm')
    
    // Limpar form
    form.alunoId = ''
    form.descricao = ''
    form.valor = 0
  } catch (err) {
    toast.error('Erro ao criar fatura')
  } finally {
    submitting.value = false
  }
}
</script>
