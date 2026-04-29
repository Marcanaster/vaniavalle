<template>
  <div class="space-y-6 max-w-4xl mx-auto">
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-4">
        <button @click="$router.back()" class="p-2 bg-white rounded-lg shadow-sm border border-slate-200 text-slate-500 hover:text-primary transition-colors">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
        </button>
        <div>
          <h2 class="text-2xl font-bold text-slate-800">{{ isEditing ? 'Editar Aluno' : 'Novo Aluno' }}</h2>
          <p class="text-slate-500">{{ isEditing ? 'Atualize os dados do aluno.' : 'Preencha os dados para matricular um novo aluno.' }}</p>
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
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Telefone (Aluno)</label>
            <input v-model="form.telefone" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border" placeholder="(00) 00000-0000">
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
            <div class="relative">
              <input v-model="form.cep" type="text" maxlength="9" @input="formatarCep" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
              <div v-if="buscandoCep" class="absolute right-3 top-2.5">
                <div class="animate-spin h-4 w-4 border-2 border-primary border-t-transparent rounded-full"></div>
              </div>
            </div>
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
            <label class="block text-sm font-medium text-slate-700 mb-1">Cidade</label>
            <input v-model="form.cidade" type="text" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Estado (UF)</label>
            <input v-model="form.estado" type="text" maxlength="2" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border uppercase">
          </div>
        </div>
      </div>

      <!-- Seção: Financeiro -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
        <h3 class="text-lg font-bold text-slate-800 mb-4 border-b border-slate-100 pb-2">4. Condições Financeiras</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Desconto de Bolsa (%)</label>
            <input v-model="form.descontoBolsa" type="number" step="0.1" min="0" max="100" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
            <p class="text-[10px] text-slate-400 mt-1">Este desconto fixo será aplicado caso seja maior que o desconto da turma.</p>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Dia de Vencimento Preferencial</label>
            <select v-model="form.diaVencimento" class="w-full border-slate-300 rounded-lg shadow-sm focus:border-primary focus:ring-primary px-3 py-2 border">
              <option :value="5">Dia 05</option>
              <option :value="10">Dia 10</option>
              <option :value="15">Dia 15</option>
              <option :value="20">Dia 20</option>
              <option :value="25">Dia 25</option>
            </select>
          </div>
        </div>
      </div>

      <div class="flex justify-end gap-4 pb-10">
        <button type="button" @click="$router.back()" class="px-6 py-2 border border-slate-300 rounded-lg text-slate-700 font-medium hover:bg-slate-50 transition-colors">Cancelar</button>
        <button type="submit" :disabled="loading" class="px-6 py-2 bg-primary hover:bg-primary-dark rounded-lg text-white font-medium transition-colors shadow-sm disabled:opacity-50">
          {{ loading ? 'Salvando...' : (isEditing ? 'Atualizar Dados' : 'Salvar Aluno') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { toast } from 'vue3-toastify'
import api from '../../../services/api'

const router = useRouter()
const route = useRoute()
const loading = ref(false)
const buscandoCep = ref(false)
const isEditing = computed(() => !!route.params.id)

const form = ref({
  nomeCompleto: '',
  cpf: '',
  dataNascimento: '',
  telefone: '',
  restricoesSaude: '',
  cep: '',
  logradouro: '',
  numero: '',
  complemento: '',
  bairro: '',
  cidade: '',
  estado: '',
  planoId: '00000000-0000-0000-0000-000000000000',
  diaVencimento: 5,
  descontoBolsa: 0,
  responsavel: {
    nome: '',
    documento: '',
    email: '',
    telefone: ''
  }
})

const formatarCep = () => {
  form.value.cep = form.value.cep.replace(/\D/g, '').replace(/^(\d{5})(\d)/, '$1-$2')
}

watch(() => form.value.cep, async (newCep) => {
  const cepLimpo = newCep.replace(/\D/g, '')
  if (cepLimpo.length === 8) {
    try {
      buscandoCep.value = true
      const res = await fetch(`https://viacep.com.br/ws/${cepLimpo}/json/`)
      const data = await res.json()
      
      if (!data.erro) {
        form.value.logradouro = data.logradouro
        form.value.bairro = data.bairro
        form.value.cidade = data.localidade
        form.value.estado = data.uf
        toast.info('Endereço preenchido automaticamente.')
      }
    } catch (err) {
      console.error('Erro ao buscar CEP', err)
    } finally {
      buscandoCep.value = false
    }
  }
})

const salvar = async () => {
  loading.value = true
  try {
    if (!isEditing.value) {
      const planosRes = await api.get('/financeiro/planos')
      if(planosRes.data && planosRes.data.length > 0) {
        form.value.planoId = planosRes.data[0].id
      } else {
        const pRes = await api.post('/financeiro/planos', { nome: 'Mensal Padrão', valor: 150, duracaoMeses: 1 })
        form.value.planoId = pRes.data.id
      }
    }

    if (isEditing.value) {
      await api.put(`/alunos/${route.params.id}`, form.value)
      toast.success('Dados atualizados com sucesso!')
    } else {
      await api.post('/alunos', form.value)
      toast.success('Aluno cadastrado com sucesso!')
    }
    router.push({ name: 'AdminAlunos' })
  } catch (error) {
    console.error('Erro ao salvar aluno', error)
    toast.error('Ocorreu um erro ao salvar o aluno. Verifique os dados.')
  }
  loading.value = false
}

onMounted(async () => {
  if (isEditing.value) {
    try {
      loading.value = true
      const res = await api.get(`/alunos`)
      const aluno = res.data.find(a => a.id === route.params.id)
      if (aluno) {
        form.value = {
          ...aluno,
          dataNascimento: aluno.dataNascimento.substring(0, 10),
          responsavel: aluno.responsavel || { nome: '', documento: '', email: '', telefone: '' }
        }
      }
    } catch (err) {
      toast.error('Erro ao carregar dados do aluno')
    } finally {
      loading.value = false
    }
  }

  if (route.query.nome) {
    form.value.nomeCompleto = route.query.nome
  }
  if (route.query.telefone) {
    form.value.responsavel.telefone = route.query.telefone
  }
})
</script>
