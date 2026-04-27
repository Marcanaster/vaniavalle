<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Financeiro</h2>
        <p class="text-slate-500">Controle de faturas e pagamentos de alunos.</p>
      </div>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden flex flex-col">
      <div class="p-4 border-b border-slate-200 bg-slate-50 flex items-center gap-4">
        <select v-model="filterStatus" class="border-slate-300 rounded-lg text-sm focus:ring-primary focus:border-primary py-2 px-3">
          <option value="Todos">Todos os Status</option>
          <option value="Pendente">Pendentes</option>
          <option value="Pago">Pagos</option>
        </select>
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Aluno</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Vencimento</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Valor</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Status</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider">Ação</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-200">
            <tr v-if="loading">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Carregando faturas...</td>
            </tr>
            <tr v-else-if="paginatedFaturas.length === 0">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Nenhuma fatura encontrada.</td>
            </tr>
            <tr v-for="fatura in paginatedFaturas" :key="fatura.id" class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                {{ fatura.aluno?.nomeCompleto || 'Desconhecido' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                {{ new Date(fatura.dataVencimento).toLocaleDateString('pt-BR') }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-slate-700">
                R$ {{ fatura.valorTotal.toFixed(2).replace('.', ',') }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="fatura.status === 'Pago' ? 'bg-emerald-100 text-emerald-800' : 'bg-amber-100 text-amber-800'" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                  {{ fatura.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button v-if="fatura.status === 'Pendente'" @click="pagar(fatura.id)" class="text-primary hover:text-primary-dark font-semibold">Confirmar Pgto</button>
                <span v-else class="text-slate-400">Ok</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <Pagination 
        :totalItems="filteredFaturas.length" 
        v-model:pageSize="pageSize" 
        v-model:page="currentPage" 
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { toast } from 'vue3-toastify'
import api from '../../../services/api'
import Pagination from '../../../components/Pagination.vue'

const faturas = ref([])
const loading = ref(true)
const filterStatus = ref('Todos')

const currentPage = ref(1)
const pageSize = ref(10)

onMounted(async () => {
  loading.value = true
  try {
    const res = await api.get('/financeiro/faturas')
    faturas.value = res.data
  } catch (err) {
    console.error(err)
  }
  loading.value = false
})

const filteredFaturas = computed(() => {
  if (filterStatus.value === 'Todos') return faturas.value
  return faturas.value.filter(f => f.status === filterStatus.value)
})

const paginatedFaturas = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredFaturas.value.slice(start, end)
})

const pagar = async (id) => {
  if(confirm('Confirmar o recebimento deste valor via PIX/Dinheiro?')) {
    try {
      await api.patch(`/financeiro/faturas/${id}/pagar`, { metodoPagamento: 'Pix' })
      const fatura = faturas.value.find(f => f.id === id)
      if(fatura) fatura.status = 'Pago'
      toast.success('Pagamento confirmado com sucesso!')
    } catch(err) {
      toast.error('Erro ao processar pagamento.')
    }
  }
}
</script>
