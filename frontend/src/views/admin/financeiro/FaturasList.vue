<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Financeiro</h2>
        <p class="text-slate-500">Controle de faturas e pagamentos de alunos.</p>
      </div>
      <button @click="gerarMensalidades" :disabled="gerando" class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2 shadow-sm disabled:opacity-50">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
        {{ gerando ? 'Gerando...' : 'Gerar Faturas do Mês' }}
      </button>
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
            <template v-for="fatura in paginatedFaturas" :key="fatura.id">
              <tr class="hover:bg-slate-50 transition-colors cursor-pointer" @click="toggleExpand(fatura.id)">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                  <div class="flex items-center gap-2">
                    <svg class="w-4 h-4 text-slate-400 transition-transform" :class="expandedId === fatura.id ? 'rotate-90' : ''" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
                    {{ fatura.aluno?.nomeCompleto || 'Desconhecido' }}
                  </div>
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
                  <button v-if="fatura.status === 'Pendente'" @click.stop="pagar(fatura.id)" class="text-primary hover:text-primary-dark font-semibold">Confirmar Pgto</button>
                  <span v-else class="text-slate-400">Ok</span>
                </td>
              </tr>
              <!-- Itens Expandidos -->
              <tr v-if="expandedId === fatura.id" class="bg-slate-50 shadow-inner">
                <td colspan="5" class="px-10 py-4">
                  <div class="text-xs font-bold text-slate-500 uppercase mb-2">Composição do Valor</div>
                  <div class="space-y-2 border-l-2 border-primary/30 pl-4">
                    <div v-for="item in fatura.items" :key="item.id" class="flex justify-between text-sm">
                      <span class="text-slate-600">{{ item.descricao }}</span>
                      <div class="space-x-4">
                        <span v-if="item.descontoPercentual > 0" class="text-emerald-600 text-xs">-{{ item.descontoPercentual }}% Desc.</span>
                        <span class="font-medium">R$ {{ item.valorFinal.toFixed(2) }}</span>
                      </div>
                    </div>
                  </div>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
      <Pagination 
        :totalItems="filteredFaturas.length" 
        v-model:pageSize="pageSize" 
        v-model:page="currentPage" 
      />
    </div>

    <!-- Modal de Confirmação Moderno -->
    <ConfirmModal 
      :show="showConfirmModal"
      title="Confirmar Pagamento"
      message="Deseja confirmar o recebimento deste valor via PIX/Dinheiro? Esta ação não pode ser desfeita."
      @confirm="handleConfirmarPagamento"
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

const faturas = ref([])
const loading = ref(true)
const gerando = ref(false)
const filterStatus = ref('Todos')
const expandedId = ref(null)

// Controle do Modal
const showConfirmModal = ref(false)
const faturaIdParaPagar = ref(null)

const gerarMensalidades = async () => {
  try {
    gerando.value = true
    const res = await api.post('/financeiro/faturas/gerar-mensais')
    toast.success(res.data.message)
    // Recarregar lista
    const resList = await api.get('/financeiro/faturas')
    faturas.value = resList.data
  } catch (err) {
    toast.error('Erro ao gerar mensalidades.')
  } finally {
    gerando.value = false
  }
}

const toggleExpand = (id) => {
  expandedId.value = expandedId.value === id ? null : id
}

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
  if (!faturas.value) return []
  if (filterStatus.value === 'Todos') return faturas.value
  return faturas.value.filter(f => f.status === filterStatus.value)
})

const paginatedFaturas = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredFaturas.value.slice(start, end)
})

const pagar = (id) => {
  faturaIdParaPagar.value = id
  showConfirmModal.value = true
}

const handleConfirmarPagamento = async () => {
  const id = faturaIdParaPagar.value
  try {
    await api.patch(`/financeiro/faturas/${id}/pagar`, { metodoPagamento: 'Pix' })
    const fatura = faturas.value.find(f => f.id === id)
    if(fatura) fatura.status = 'Pago'
    toast.success('Pagamento confirmado com sucesso!')
  } catch(err) {
    toast.error('Erro ao processar pagamento.')
  } finally {
    showConfirmModal.value = false
    faturaIdParaPagar.value = null
  }
}
</script>
