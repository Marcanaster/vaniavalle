<template>
  <div class="space-y-6">
    <!-- Bem-vindo -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Bem-vindo de volta!</h2>
        <p class="text-slate-500">Aqui está o resumo do que está acontecendo na academia hoje.</p>
      </div>

      <!-- Filtros -->
      <!-- Botões de Ação Administrativa -->
      <div class="flex items-center gap-2">
        <button @click="triggerBulkNotify" class="bg-amber-50 hover:bg-amber-100 text-amber-700 px-4 py-2 rounded-lg font-bold text-[10px] transition-colors flex items-center gap-2 border border-amber-200 shadow-sm uppercase tracking-wider">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg>
          Notificar Acessos (Emergencial)
        </button>
      </div>

      <div class="flex items-center gap-2 bg-white p-2 rounded-xl border border-slate-100 shadow-sm">
        <select v-model="filter.mes" @change="loadMetrics" class="bg-transparent text-sm font-bold text-slate-600 border-none focus:ring-0 cursor-pointer">
          <option v-for="m in meses" :key="m.id" :value="m.id">{{ m.nome }}</option>
        </select>
        <div class="w-px h-4 bg-slate-200"></div>
        <select v-model="filter.ano" @change="loadMetrics" class="bg-transparent text-sm font-bold text-slate-600 border-none focus:ring-0 cursor-pointer">
          <option v-for="a in anos" :key="a" :value="a">{{ a }}</option>
        </select>
      </div>
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
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4">
        <!-- Alunos Ativos -->
        <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-4 hover:shadow-md transition-shadow">
          <div class="p-3 bg-primary/10 rounded-xl text-primary">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
          </div>
          <div>
            <p class="text-[10px] font-semibold text-slate-400 uppercase tracking-wider">Alunos Ativos</p>
            <p class="text-xl font-bold text-slate-800">{{ metrics.alunosAtivos }}</p>
          </div>
        </div>
        
        <!-- Turmas -->
        <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-4 hover:shadow-md transition-shadow">
          <div class="p-3 bg-secondary/10 rounded-xl text-secondary">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path></svg>
          </div>
          <div>
            <p class="text-[10px] font-semibold text-slate-400 uppercase tracking-wider">Turmas</p>
            <p class="text-xl font-bold text-slate-800">{{ metrics.turmasAtivas }}</p>
          </div>
        </div>

        <!-- Receita Prevista (NEW CARD) -->
        <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-4 hover:shadow-md transition-shadow relative overflow-hidden group">
          <div class="p-3 bg-sky-100 rounded-xl text-sky-600 z-10">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
          </div>
          <div class="z-10">
            <p class="text-[10px] font-semibold text-slate-400 uppercase tracking-wider">Receita Prevista</p>
            <p class="text-xl font-bold text-slate-800">{{ formatCurrency(metrics.receitaPrevistaMes) }}</p>
            <p class="text-[10px] text-sky-500 font-medium">Total para {{ meses.find(m => m.id === filter.mes)?.nome }}</p>
          </div>
        </div>

        <!-- Receita Realizada -->
        <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-4 hover:shadow-md transition-shadow relative overflow-hidden group">
          <div class="p-3 bg-emerald-100 rounded-xl text-emerald-600 z-10">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
          </div>
          <div class="z-10">
            <p class="text-[10px] font-semibold text-slate-400 uppercase tracking-wider">Receita Realizada</p>
            <p class="text-xl font-bold text-slate-800">{{ formatCurrency(metrics.receitaMes) }}</p>
            <div class="flex items-center gap-2 mt-1">
              <span class="text-[9px] text-slate-400 font-bold uppercase">{{ Math.round((metrics.receitaMes / (metrics.receitaPrevistaMes || 1)) * 100) }}%</span>
              <div class="h-1 flex-1 bg-slate-100 rounded-full overflow-hidden max-w-[40px]">
                <div class="h-full bg-emerald-500 rounded-full" :style="{ width: `${Math.min((metrics.receitaMes / (metrics.receitaPrevistaMes || 1)) * 100, 100)}%` }"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Inadimplência -->
        <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-4 hover:shadow-md transition-shadow">
          <div class="p-3 bg-rose-100 rounded-xl text-rose-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
          </div>
          <div>
            <p class="text-[10px] font-semibold text-slate-400 uppercase tracking-wider">Inadimplência</p>
            <p class="text-xl font-bold text-slate-800">{{ formatCurrency(metrics.inadimplenciaTotal) }}</p>
            <p class="text-[9px] text-rose-500 font-medium">{{ metrics.alunosInadimplentes }} alunos atrasados</p>
          </div>
        </div>
      </div>

      <!-- Gráficos e Alertas -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <!-- Gráfico de Receita -->
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <h3 class="text-sm font-bold text-slate-800 mb-6 uppercase tracking-wider">Receita (Realizado vs Previsto)</h3>
          <div class="h-48 flex items-end justify-between gap-3 px-2">
            <div v-for="item in metrics.receitaMensalChart" :key="item.label" class="flex-1 flex flex-col items-center gap-2 group">
              <div class="w-full relative flex flex-col justify-end h-32">
                <!-- Tooltip -->
                <div class="absolute -top-16 left-1/2 -translate-x-1/2 bg-slate-900/95 backdrop-blur-sm text-white text-[10px] p-2 rounded-lg opacity-0 group-hover:opacity-100 transition-all duration-200 whitespace-nowrap z-20 shadow-2xl border border-slate-700 pointer-events-none scale-90 group-hover:scale-100">
                  <p class="font-bold border-b border-slate-700 mb-1 pb-1 flex justify-between gap-4">
                    <span>{{ item.label }}</span>
                    <span class="text-slate-400">Total</span>
                  </p>
                  <div class="space-y-0.5">
                    <p class="flex justify-between gap-4">
                      <span class="text-emerald-400">Arrecadado:</span>
                      <span class="font-mono">{{ formatCurrency(item.value) }}</span>
                    </p>
                    <p class="flex justify-between gap-4">
                      <span class="text-slate-300">Previsto:</span>
                      <span class="font-mono">{{ formatCurrency(item.secondaryValue || 0) }}</span>
                    </p>
                  </div>
                </div>
                
                <div class="flex items-end gap-1 w-full h-full">
                  <!-- Arrecadado -->
                  <div 
                    class="flex-1 bg-gradient-to-t from-emerald-600 to-emerald-400 group-hover:to-emerald-300 transition-all duration-300 rounded-t-sm relative"
                    :style="{ height: `${(item.value / Math.max(...metrics.receitaMensalChart.map(m => Math.max(m.value, m.secondaryValue || 0)), 1)) * 100}%` }"
                  >
                    <!-- Valor discreto no topo da barra se houver espaço -->
                    <span v-if="item.value > 0" class="absolute -top-4 left-1/2 -translate-x-1/2 text-[8px] font-bold text-emerald-600 opacity-0 group-hover:opacity-100 transition-opacity">
                      {{ (item.value / 1000).toFixed(1) }}k
                    </span>
                  </div>
                  <!-- Previsto -->
                  <div 
                    class="flex-1 bg-slate-200 group-hover:bg-slate-300 transition-all duration-300 rounded-t-sm"
                    :style="{ height: `${((item.secondaryValue || 0) / Math.max(...metrics.receitaMensalChart.map(m => Math.max(m.value, m.secondaryValue || 0)), 1)) * 100}%` }"
                  ></div>
                </div>
              </div>
              <span class="text-[9px] font-bold text-slate-400 uppercase group-hover:text-slate-600 transition-colors">{{ item.label }}</span>
            </div>
          </div>
          <div class="flex justify-center gap-4 mt-4">
            <div class="flex items-center gap-1.5">
              <span class="w-2 h-2 rounded-full bg-emerald-500"></span>
              <span class="text-[9px] text-slate-500 font-bold uppercase">Realizado</span>
            </div>
            <div class="flex items-center gap-1.5">
              <span class="w-2 h-2 rounded-full bg-slate-300"></span>
              <span class="text-[9px] text-slate-500 font-bold uppercase">Previsto</span>
            </div>
          </div>
        </div>

        <!-- Gráfico de Inadimplência -->
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <h3 class="text-sm font-bold text-slate-800 mb-6 uppercase tracking-wider">Evolução Inadimplência</h3>
          <div class="h-48 flex items-end justify-between gap-3 px-2">
            <div v-for="item in metrics.inadimplenciaMensalChart" :key="item.label" class="flex-1 flex flex-col items-center gap-2 group">
              <div class="w-full relative flex flex-col justify-end h-32">
                <!-- Tooltip -->
                <div class="absolute -top-10 left-1/2 -translate-x-1/2 bg-rose-900/95 backdrop-blur-sm text-white text-[10px] py-1 px-2 rounded-lg opacity-0 group-hover:opacity-100 transition-all duration-200 whitespace-nowrap z-20 shadow-xl border border-rose-700 pointer-events-none scale-90 group-hover:scale-100">
                  <span class="font-bold">{{ formatCurrency(item.value) }}</span>
                </div>
                <div 
                  class="w-full bg-gradient-to-t from-rose-600 to-rose-400 group-hover:to-rose-300 transition-all duration-300 rounded-t-lg shadow-sm"
                  :style="{ height: `${Math.max((item.value / Math.max(...metrics.inadimplenciaMensalChart.map(m => m.value || 0), 1)) * 100, item.value > 0 ? 5 : 0)}%` }"
                >
                  <span v-if="item.value > 0" class="absolute -top-4 left-1/2 -translate-x-1/2 text-[8px] font-bold text-rose-600 opacity-0 group-hover:opacity-100 transition-opacity">
                    {{ (item.value / 1).toFixed(0) }}
                  </span>
                </div>
              </div>
              <span class="text-[9px] font-bold text-slate-400 uppercase group-hover:text-slate-600 transition-colors">{{ item.label }}</span>
            </div>
          </div>
        </div>

        <!-- Alerta de Inadimplência -->
        <div v-if="metrics.alunosInadimplentes > 0" class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex flex-col justify-center">
          <div class="text-center space-y-4">
            <div class="inline-flex p-4 bg-rose-50 text-rose-500 rounded-full">
              <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path></svg>
            </div>
            <div>
              <h4 class="text-slate-800 font-bold">Saúde Financeira</h4>
              <p class="text-slate-500 text-sm">Existem <strong>{{ metrics.alunosInadimplentes }} alunos</strong> com pagamentos em atraso.</p>
            </div>
            <router-link :to="{ path: '/admin/financeiro', query: { filter: 'atrasado' } }" class="w-full py-3 bg-rose-500 hover:bg-rose-600 text-white rounded-xl font-bold text-sm transition-colors block text-center">
              Cobrar Alunos
            </router-link>
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
            <div v-for="(aula, index) in metrics.aulasHoje" :key="aula.id" class="flex items-center justify-between p-4 bg-slate-50 rounded-xl border border-slate-100" :class="{'opacity-50 grayscale': aula.status === 'Cancelada'}">
              <div class="flex items-center gap-4">
                <div :class="[
                  aula.status === 'Cancelada' ? 'bg-slate-200 text-slate-500' : (index % 2 === 0 ? 'bg-primary/20 text-primary' : 'bg-secondary/20 text-secondary'), 
                  'h-14 w-14 rounded-lg flex flex-col items-center justify-center flex-shrink-0'
                ]">
                  <span class="text-sm font-bold">{{ aula.horario }}</span>
                </div>
                <div>
                  <div class="flex items-center gap-2">
                    <p class="font-bold text-slate-800">{{ aula.nome }}</p>
                    <span v-if="aula.status === 'Cancelada'" class="bg-rose-100 text-rose-600 text-[10px] uppercase font-bold px-2 py-0.5 rounded">Cancelada</span>
                  </div>
                  <p class="text-xs text-slate-500 font-medium uppercase tracking-tight">{{ aula.modalidade }} • {{ aula.sala || 'Sem Sala' }}</p>
                  <p class="text-xs text-slate-400">{{ aula.alunosConfirmados }} matriculados</p>
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
  receitaPrevistaMes: 0,
  inadimplenciaTotal: 0,
  alunosInadimplentes: 0,
  receitaMensalChart: [],
  inadimplenciaMensalChart: [],
  aulasHoje: [],
  ultimosPagamentos: []
});

const filter = ref({
  mes: new Date().getMonth() + 1,
  ano: new Date().getFullYear()
});

const meses = [
  { id: 1, nome: 'Janeiro' }, { id: 2, nome: 'Fevereiro' }, { id: 3, nome: 'Março' },
  { id: 4, nome: 'Abril' }, { id: 5, nome: 'Maio' }, { id: 6, nome: 'Junho' },
  { id: 7, nome: 'Julho' }, { id: 8, nome: 'Agosto' }, { id: 9, nome: 'Setembro' },
  { id: 10, nome: 'Outubro' }, { id: 11, nome: 'Novembro' }, { id: 12, nome: 'Dezembro' }
];

const anos = ref([
  new Date().getFullYear() - 1,
  new Date().getFullYear(),
  new Date().getFullYear() + 1
]);

const loadMetrics = async () => {
  try {
    loading.value = true;
    const response = await api.get('/dashboard/metrics', {
      params: { 
        mes: filter.value.mes,
        ano: filter.value.ano
      }
    });
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

const triggerBulkNotify = async () => {
  if (!confirm('Deseja realmente resetar as senhas e enviar e-mails de acesso para TODOS os alunos e professores? Esta ação não pode ser desfeita.')) {
    return;
  }
  
  try {
    toast.info('Iniciando envio em massa... Isso pode demorar alguns segundos.', { autoClose: false, toastId: 'bulk-loading' });
    const response = await api.post('/admin/emergency-bulk-notify');
    toast.update('bulk-loading', {
      render: `Sucesso! ${response.data.successCount} usuários notificados. ${response.data.errorCount} falhas.`,
      type: 'success',
      autoClose: 5000
    });
  } catch (error) {
    console.error('Erro ao disparar notificações:', error);
    toast.dismiss('bulk-loading');
    toast.error('Erro ao disparar notificações em massa.');
  }
};

onMounted(() => {
  loadMetrics();
});
</script>
