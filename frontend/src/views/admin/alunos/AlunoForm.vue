<template>
  <div class="space-y-6 max-w-4xl mx-auto">
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-4">
        <button @click="$router.back()" class="p-2 bg-white rounded-lg shadow-sm border border-slate-200 text-slate-500 hover:text-primary transition-colors">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
        </button>
        <div>
          <h2 class="text-2xl font-bold text-slate-800">Novo Aluno</h2>
          <p class="text-slate-500">Preencha os dados para matricular um novo aluno.</p>
        </div>
      </div>
    </div>

    <form @submit.prevent="salvar" class="space-y-6">
      <!-- Seção: Dados do Aluno -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
        <h3 class="text-lg font-bold text-slate-800 mb-4 border-b border-slate-100 pb-2">1. Dados Pessoais</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 mb-1">Nome Completo *</label>
            <input v-model="form.nomeCompleto" type="text" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">CPF *</label>
            <input v-model="form.cpf" type="text" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Data de Nascimento *</label>
            <input v-model="form.dataNascimento" type="date" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 mb-1">Restrições de Saúde</label>
            <textarea v-model="form.restricoesSaude" rows="2" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border" placeholder="Alergias, lesões prévias..."></textarea>
          </div>
        </div>
      </div>

      <!-- Seção: Responsável -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
        <div class="flex items-center justify-between mb-4 border-b border-slate-100 pb-2">
          <h3 class="text-lg font-bold text-slate-800">2. Responsável Financeiro</h3>
          <p class="text-xs text-secondary font-medium">Requerido Telefone</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Nome do Responsável</label>
            <input v-model="form.responsavel.nome" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">CPF Responsável</label>
            <input v-model="form.responsavel.documento" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">E-mail</label>
            <input v-model="form.responsavel.email" type="email" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Telefone *</label>
            <input v-model="form.responsavel.telefone" type="text" required class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
        </div>
      </div>

      <!-- Seção: Endereço -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
        <div class="flex items-center justify-between mb-4 border-b border-slate-100 pb-2">
          <h3 class="text-lg font-bold text-slate-800">3. Endereço</h3>
          <p class="text-xs text-slate-400 font-medium">Opcional</p>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">CEP</label>
            <input v-model="form.cep" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 mb-1">Logradouro</label>
            <input v-model="form.logradouro" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Número</label>
            <input v-model="form.numero" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-slate-700 mb-1">Complemento</label>
            <input v-model="form.complemento" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border" placeholder="Apto, Bloco, etc.">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Bairro</label>
            <input v-model="form.bairro" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Cidade / UF</label>
            <input v-model="form.cidade" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
        </div>
      </div>

      <div class="flex justify-end gap-4 pb-10">
        <button type="button" @click="$router.back()" class="px-6 py-2 border border-slate-300 rounded-lg text-slate-700 font-medium hover:bg-slate-50 transition-colors">Cancelar</button>
        <button type="submit" :disabled="loading" class="px-6 py-2 bg-primary hover:bg-primary-dark rounded-lg text-white font-medium transition-colors shadow-sm disabled:opacity-50">
          {{ loading ? 'Salvando...' : 'Salvar Aluno' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'
import api from '../../../services/api'

const router = useRouter()
const loading = ref(false)

const form = ref({
  nomeCompleto: '',
  cpf: '',
  dataNascimento: '',
  restricoesSaude: '',
  cep: '',
  logradouro: '',
  numero: '',
  complemento: '',
  bairro: '',
  cidade: '',
  estado: '',
  planoId: '00000000-0000-0000-0000-000000000000',
  responsavel: {
    nome: '',
    documento: '',
    email: '',
    telefone: ''
  }
})

const salvar = async () => {
  loading.value = true
  try {
    const planosRes = await api.get('/financeiro/planos')
    if(planosRes.data && planosRes.data.length > 0) {
      form.value.planoId = planosRes.data[0].id
    } else {
      const pRes = await api.post('/financeiro/planos', { nome: 'Mensal Padrão', valor: 150, duracaoMeses: 1 })
      form.value.planoId = pRes.data.id
    }

    await api.post('/alunos', form.value)
    toast.success('Aluno cadastrado com sucesso!')
    router.push({ name: 'AdminAlunos' })
  } catch (error) {
    console.error('Erro ao salvar aluno', error)
    toast.error('Ocorreu um erro ao salvar o aluno. Verifique os dados.')
  }
  loading.value = false
}
</script>
