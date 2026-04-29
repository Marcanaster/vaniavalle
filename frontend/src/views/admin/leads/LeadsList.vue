<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Aulas Experimentais (Leads)</h2>
        <p class="text-slate-500">Acompanhe as pessoas que entraram em contato via WhatsApp.</p>
      </div>
      <button @click="openModal()" class="bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Novo Contato
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
    </div>

    <!-- Lista de Leads -->
    <div v-else class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-100">
          <thead class="bg-slate-50">
            <tr>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Contato</th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Telefone (WhatsApp)</th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Interesse</th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Data Agendada</th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
              <th scope="col" class="px-6 py-4 text-right text-xs font-semibold text-slate-500 uppercase tracking-wider">Ações</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-100">
            <tr v-if="leads.length === 0">
              <td colspan="6" class="px-6 py-10 text-center text-slate-500">
                Nenhum contato registrado no momento.
              </td>
            </tr>
            <tr v-for="lead in leads" :key="lead.id" class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-600 font-bold">
                    {{ getInitials(lead.nome) }}
                  </div>
                  <div>
                    <div class="text-sm font-bold text-slate-800">{{ lead.nome }}</div>
                    <div class="text-xs text-slate-500">{{ formatDate(lead.dataSolicitacao) }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <a :href="`https://wa.me/${lead.telefoneWhatsApp}`" target="_blank" class="inline-flex items-center gap-1 text-emerald-600 hover:text-emerald-700 font-medium">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path></svg>
                  {{ lead.telefoneWhatsApp || 'Não informado' }}
                </a>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-slate-800">{{ lead.modalidadeInteresse || '-' }}</div>
                <div v-if="lead.idade" class="text-xs text-slate-500">{{ lead.idade }} anos</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span v-if="lead.dataAgendada" class="text-sm font-medium text-slate-800">{{ formatDateTime(lead.dataAgendada) }}</span>
                <span v-else class="text-sm text-slate-400">Não agendada</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="[
                  'px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full',
                  getStatusClass(lead.status)
                ]">
                  {{ lead.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button @click="openModal(lead)" class="text-primary hover:text-primary-dark mr-4">Editar</button>
                <button @click="confirmDelete(lead.id)" class="text-rose-500 hover:text-rose-700">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="fixed inset-0 bg-slate-900/50 flex items-center justify-center z-50 px-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-lg overflow-hidden">
        <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50">
          <h3 class="text-lg font-bold text-slate-800">{{ form.id ? 'Editar Contato' : 'Novo Contato' }}</h3>
          <button @click="closeModal" class="text-slate-400 hover:text-slate-600 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          </button>
        </div>
        
        <form @submit.prevent="saveLead" class="p-6 space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="col-span-2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Nome Completo *</label>
              <input v-model="form.nome" type="text" required class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all" />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">WhatsApp *</label>
              <input v-model="form.telefoneWhatsApp" type="text" required placeholder="Ex: 21999999999" class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all" />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Idade (opcional)</label>
              <input v-model="form.idade" type="number" class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all" />
            </div>

            <div class="col-span-2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Modalidade de Interesse</label>
              <input v-model="form.modalidadeInteresse" type="text" placeholder="Ex: Ballet Infantil" class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all" />
            </div>

            <div class="col-span-2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Data Agendada (Aula Experimental)</label>
              <input v-model="form.dataAgendada" type="datetime-local" class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all" />
            </div>
            
            <div class="col-span-2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Status</label>
              <select v-model="form.status" class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all">
                <option value="Pendente">Pendente (Aguardando contato/agendamento)</option>
                <option value="Agendada">Agendada (Data marcada)</option>
                <option value="Realizada">Realizada (Veio à aula)</option>
                <option value="Convertida">Convertida (Virou aluno! 🎉)</option>
                <option value="Cancelada">Cancelada (Desistiu)</option>
              </select>
            </div>

            <div class="col-span-2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Anotações da Secretaria</label>
              <textarea v-model="form.observacoesAgent" rows="3" class="w-full px-4 py-2 border border-slate-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all"></textarea>
            </div>
          </div>
          
          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="closeModal" class="px-4 py-2 text-slate-600 font-medium hover:bg-slate-100 rounded-lg transition-colors">Cancelar</button>
            <button type="submit" :disabled="saving" class="bg-primary hover:bg-primary-dark text-white px-6 py-2 rounded-lg font-bold transition-all disabled:opacity-50">
              {{ saving ? 'Salvando...' : 'Salvar Contato' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '../../../services/api';
import { toast } from 'vue3-toastify';

const leads = ref([]);
const loading = ref(true);
const saving = ref(false);
const showModal = ref(false);
const router = useRouter();

const initialFormState = {
  id: null,
  nome: '',
  telefoneWhatsApp: '',
  idade: null,
  modalidadeInteresse: '',
  dataAgendada: '',
  status: 'Pendente',
  observacoesAgent: ''
};

const form = ref({ ...initialFormState });

const fetchLeads = async () => {
  try {
    loading.value = true;
    const response = await api.get('/leads');
    // Filtra os leads para exibir apenas os ativos na grid (esconde Convertida e Cancelada)
    leads.value = response.data.filter(l => l.status !== 'Convertida' && l.status !== 'Cancelada');
  } catch (error) {
    console.error('Erro ao buscar contatos:', error);
    toast.error('Erro ao carregar lista de contatos.');
  } finally {
    loading.value = false;
  }
};

const openModal = (lead = null) => {
  if (lead) {
    form.value = { 
      ...lead,
      // Formata a data para o input datetime-local (YYYY-MM-DDThh:mm)
      dataAgendada: lead.dataAgendada ? new Date(lead.dataAgendada).toISOString().slice(0, 16) : ''
    };
  } else {
    form.value = { ...initialFormState };
  }
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  form.value = { ...initialFormState };
};

const saveLead = async () => {
  try {
    saving.value = true;
    const payload = { ...form.value };
    // Converter string vazia de data para null para a API
    if (!payload.dataAgendada) payload.dataAgendada = null;

    if (form.value.id) {
      await api.put(`/leads/${form.value.id}`, payload);
      toast.success('Contato atualizado com sucesso!');
    } else {
      await api.post('/leads', payload);
      toast.success('Contato registrado com sucesso!');
    }
    
    // Se o status for convertido, redireciona para o cadastro de aluno
    if (payload.status === 'Convertida') {
      closeModal();
      router.push({
        name: 'AdminAlunosNovo',
        query: {
          nome: payload.nome,
          telefone: payload.telefoneWhatsApp,
          idade: payload.idade
        }
      });
      return;
    }

    closeModal();
    fetchLeads();
  } catch (error) {
    console.error('Erro ao salvar:', error);
    toast.error('Não foi possível salvar os dados do contato.');
  } finally {
    saving.value = false;
  }
};

const confirmDelete = async (id) => {
  if (confirm('Tem certeza que deseja excluir este contato?')) {
    try {
      await api.delete(`/leads/${id}`);
      toast.success('Contato excluído com sucesso.');
      fetchLeads();
    } catch (error) {
      toast.error('Erro ao excluir contato.');
    }
  }
};

// Utils Formatadores
const getInitials = (nome) => {
  if (!nome) return '?';
  const parts = nome.split(' ').filter(Boolean);
  if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
  return `${parts[0][0]}${parts[parts.length-1][0]}`.toUpperCase();
};

const formatDate = (dateStr) => {
  if (!dateStr) return '';
  return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(new Date(dateStr));
};

const formatDateTime = (dateStr) => {
  if (!dateStr) return '';
  return new Intl.DateTimeFormat('pt-BR', { 
    day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' 
  }).format(new Date(dateStr));
};

const getStatusClass = (status) => {
  switch (status) {
    case 'Pendente': return 'bg-amber-100 text-amber-800';
    case 'Agendada': return 'bg-blue-100 text-blue-800';
    case 'Realizada': return 'bg-indigo-100 text-indigo-800';
    case 'Convertida': return 'bg-emerald-100 text-emerald-800';
    case 'Cancelada': return 'bg-rose-100 text-rose-800';
    default: return 'bg-slate-100 text-slate-800';
  }
};

onMounted(() => {
  fetchLeads();
});
</script>
