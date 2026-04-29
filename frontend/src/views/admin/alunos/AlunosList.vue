<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Alunos</h2>
        <p class="text-slate-500">Gerenciamento de matrículas e alunos ativos.</p>
      </div>
      <router-link :to="{ name: 'AdminAlunosNovo' }" class="bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2 shadow-sm">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Novo Aluno
      </router-link>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden flex flex-col">
      <div class="p-4 border-b border-slate-200 bg-slate-50 flex items-center gap-4">
        <div class="relative flex-1 max-w-md">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <svg class="h-5 w-5 text-slate-400" viewBox="0 0 20 20" fill="currentColor"><path fill-rule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clip-rule="evenodd" /></svg>
          </div>
          <input v-model="searchQuery" type="text" class="block w-full pl-10 pr-3 py-2 border border-slate-300 rounded-lg leading-5 bg-white placeholder-slate-400 focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary sm:text-sm" placeholder="Buscar aluno por nome ou CPF...">
        </div>
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Nome do Aluno</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">CPF</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Plano</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Status</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider">Ações</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-200">
            <tr v-if="loading">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Carregando alunos...</td>
            </tr>
            <tr v-else-if="paginatedAlunos.length === 0">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Nenhum aluno encontrado.</td>
            </tr>
            <tr v-for="aluno in paginatedAlunos" :key="aluno.id" class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10 bg-primary/10 rounded-full flex items-center justify-center text-primary font-bold">
                    {{ aluno.nomeCompleto.charAt(0) }}
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-slate-900">{{ aluno.nomeCompleto }}</div>
                    <div class="text-sm text-slate-500">{{ aluno.responsavel?.telefone || 'S/ Tel' }} - {{ aluno.responsavel?.email || 'Sem email' }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ formatCPF(aluno.cpf) }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ aluno.plano?.nome || 'Não vinculado' }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-emerald-100 text-emerald-800">
                  Ativo
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <a href="#" class="text-primary hover:text-primary-dark mr-3">Editar</a>
                <button @click="remover(aluno.id)" class="text-secondary hover:text-secondary-dark">Remover</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Componente Paginador -->
      />
    </div>

    <!-- Modal de Confirmação Moderno -->
    <ConfirmModal 
      :show="showConfirmModal"
      title="Excluir Aluno"
      message="Tem certeza que deseja remover este aluno? Ele deixará de aparecer na lista de ativos, mas seus registros financeiros serão mantidos."
      @confirm="handleConfirmarRemocao"
      @cancel="showConfirmModal = false"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { toast } from 'vue3-toastify'
import api from '../../../services/api'
import Pagination from '../../../components/Pagination.vue'
import ConfirmModal from '../../../components/ConfirmModal.vue'

const alunos = ref([])
const loading = ref(true)
const searchQuery = ref('')

const showConfirmModal = ref(false)
const alunoIdParaRemover = ref(null)

const currentPage = ref(1)
const pageSize = ref(10)

onMounted(async () => {
  await fetchAlunos()
})

const fetchAlunos = async () => {
  loading.value = true
  try {
    const res = await api.get('/alunos')
    alunos.value = res.data
  } catch (err) {
    console.error('Erro ao buscar alunos', err)
  }
  loading.value = false
}

const filteredAlunos = computed(() => {
  if (!searchQuery.value) return alunos.value
  const q = searchQuery.value.toLowerCase()
  return alunos.value.filter(a => 
    a.nomeCompleto.toLowerCase().includes(q) || 
    a.cpf.includes(q)
  )
})

const paginatedAlunos = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredAlunos.value.slice(start, end)
})

const remover = (id) => {
  alunoIdParaRemover.value = id
  showConfirmModal.value = true
}

const handleConfirmarRemocao = async () => {
  const id = alunoIdParaRemover.value
  try {
    await api.delete(`/alunos/${id}`)
    alunos.value = alunos.value.filter(a => a.id !== id)
    toast.success('Aluno removido com sucesso!')
  } catch(err) {
    toast.error('Erro ao excluir')
  } finally {
    showConfirmModal.value = false
    alunoIdParaRemover.value = null
  }
}

const formatCPF = (cpf) => {
  if (!cpf) return '';
  // Remove non-digits
  cpf = cpf.replace(/\D/g, '');
  if (cpf.length === 11) {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4");
  }
  return cpf;
}
</script>
