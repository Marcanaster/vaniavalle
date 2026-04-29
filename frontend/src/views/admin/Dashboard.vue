<template>
  <div class="space-y-6">
    <!-- Bem-vindo -->
    <div>
      <h2 class="text-2xl font-bold text-slate-800">Bem-vindo de volta!</h2>
      <p class="text-slate-500">Aqui está o resumo do que está acontecendo na academia hoje.</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
    </div>

    <!-- Conteúdo do Dashboard -->
    <template v-else>
      <!-- Alerta de Leads Pendentes -->
      <div v-if="metrics.leadsPendentes > 0" class="bg-rose-50 border-l-4 border-rose-500 p-4 rounded-r-xl shadow-sm flex items-start sm:items-center justify-between gap-4">
        <div class="flex items-center gap-3">
          <div class="text-rose-500 bg-rose-100 p-2 rounded-full">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"></path></svg>
          </div>
          <div>
            <h3 class="text-rose-800 font-bold text-lg">Atenção Necessária</h3>
            <p class="text-rose-600 text-sm">Você tem <strong>{{ metrics.leadsPendentes }} solicitações pendentes</strong> de aula experimental do WhatsApp aguardando retorno.</p>
          </div>
        </div>
        <button class="bg-rose-500 hover:bg-rose-600 text-white px-4 py-2 rounded-lg font-medium text-sm transition-colors whitespace-nowrap">
          Ver Solicitações
        </button>
      </div>

      <!-- Cards de Métricas -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-5 hover:shadow-md transition-shadow">
          <div class="p-4 bg-primary/10 rounded-xl text-primary">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
          </div>
          <div>
            <p class="text-sm font-semibold text-slate-400 uppercase tracking-wider">Alunos Ativos</p>
            <p class="text-3xl font-bold text-slate-800">{{ metrics.alunosAtivos }}</p>
          </div>
        </div>
        
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-5 hover:shadow-md transition-shadow">
          <div class="p-4 bg-secondary/10 rounded-xl text-secondary">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path></svg>
          </div>
          <div>
            <p class="text-sm font-semibold text-slate-400 uppercase tracking-wider">Turmas</p>
            <p class="text-3xl font-bold text-slate-800">{{ metrics.turmasAtivas }}</p>
          </div>
        </div>

        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-5 hover:shadow-md transition-shadow">
          <div class="p-4 bg-emerald-100 rounded-xl text-emerald-600">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
          </div>
          <div>
            <p class="text-sm font-semibold text-slate-400 uppercase tracking-wider">Receita Mês</p>
            <p class="text-3xl font-bold text-slate-800">{{ formatCurrency(metrics.receitaMes) }}</p>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Próximas Aulas -->
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <div class="flex justify-between items-center mb-6">
            <h3 class="text-lg font-bold text-slate-800">Aulas de Hoje</h3>
            <router-link to="/admin/turmas" class="text-primary hover:text-primary-dark text-sm font-medium">Ver Grade Completa</router-link>
          </div>
          
          <div v-if="metrics.aulasHoje.length === 0" class="text-center py-6 text-slate-500">
            <p>Nenhuma aula programada para hoje.</p>
          </div>
          
          <div v-else class="space-y-4">
            <div v-for="(aula, index) in metrics.aulasHoje" :key="aula.id" class="flex items-center justify-between p-4 bg-slate-50 rounded-xl border border-slate-100">
              <div class="flex items-center gap-4">
                <div :class="[index % 2 === 0 ? 'bg-primary/20 text-primary' : 'bg-secondary/20 text-secondary', 'h-12 w-12 rounded-lg flex flex-col items-center justify-center']">
                  <span class="text-sm font-bold">{{ aula.horario }}</span>
                </div>
                <div>
                  <p class="font-bold text-slate-800">{{ aula.nome }}</p>
                  <p class="text-sm text-slate-500">{{ aula.alunosConfirmados }} Alunos matriculados</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Faturas Recentes -->
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <div class="flex justify-between items-center mb-6">
            <h3 class="text-lg font-bold text-slate-800">Últimos Pagamentos</h3>
            <router-link to="/admin/financeiro" class="text-primary hover:text-primary-dark text-sm font-medium">Ir para Financeiro</router-link>
          </div>
          
          <div v-if="metrics.ultimosPagamentos.length === 0" class="text-center py-6 text-slate-500">
            <p>Nenhum pagamento registrado recente.</p>
          </div>
          
          <div v-else class="space-y-4">
            <div v-for="pagamento in metrics.ultimosPagamentos" :key="pagamento.id" class="flex items-center justify-between py-3 border-b border-slate-50 last:border-0">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-500 font-bold">
                  {{ pagamento.iniciais }}
                </div>
                <div>
                  <p class="font-semibold text-slate-800">{{ pagamento.alunoNome }}</p>
                  <p class="text-xs text-slate-500">{{ pagamento.planoNome }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-bold text-emerald-600">{{ formatCurrency(pagamento.valor) }}</p>
                <p class="text-xs text-slate-400">{{ formatDate(pagamento.dataPagamento) }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../services/api';
import { toast } from 'vue3-toastify';

const loading = ref(true);
const metrics = ref({
  alunosAtivos: 0,
  turmasAtivas: 0,
  leadsPendentes: 0,
  receitaMes: 0,
  aulasHoje: [],
  ultimosPagamentos: []
});

const loadMetrics = async () => {
  try {
    loading.value = true;
    const response = await api.get('/dashboard/metrics');
    metrics.value = response.data;
  } catch (error) {
    console.error('Erro ao carregar métricas:', error);
    toast.error('Não foi possível carregar as métricas do painel.');
  } finally {
    loading.value = false;
  }
};

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value);
};

const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

onMounted(() => {
  loadMetrics();
});
</script>
