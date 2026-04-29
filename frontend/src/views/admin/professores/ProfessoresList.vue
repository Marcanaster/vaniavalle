<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Professores</h2>
        <p class="text-slate-500">Gerencie a equipe de professores da academia.</p>
      </div>
      <button @click="openModal()" class="bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Novo Professor
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
    </div>

    <!-- Lista -->
    <div v-else class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-100">
          <thead class="bg-slate-50">
            <tr>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Professor</th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Contato</th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Especialidade</th>
              <th scope="col" class="px-6 py-4 text-right text-xs font-semibold text-slate-500 uppercase tracking-wider">Ações</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-100">
            <tr v-if="professores.length === 0">
              <td colspan="4" class="px-6 py-10 text-center text-slate-500">Nenhum professor cadastrado.</td>
            </tr>
            <tr v-for="prof in professores" :key="prof.id" class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-600 font-bold">
                    {{ getInitials(prof.nome) }}
                  </div>
                  <div class="text-sm font-bold text-slate-800">{{ prof.nome }}</div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-slate-800">{{ prof.email }}</div>
                <div class="text-xs text-slate-500">{{ prof.telefone }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-800">
                {{ prof.especialidade || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button @click="openModal(prof)" class="text-primary hover:text-primary-dark mr-4">Editar</button>
                <button @click="confirmDelete(prof.id)" class="text-rose-500 hover:text-rose-700">Excluir</button>
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
          <h3 class="text-lg font-bold text-slate-800">{{ form.id ? 'Editar Professor' : 'Novo Professor' }}</h3>
          <button @click="closeModal" class="text-slate-400 hover:text-slate-600 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          </button>
        </div>
        
        <form @submit.prevent="saveProfessor" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Nome Completo *</label>
            <input v-model="form.nome" type="text" required class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary" />
          </div>
          
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">E-mail (Login) *</label>
              <input v-model="form.email" type="email" required class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary" :disabled="!!form.id" />
              <p v-if="!form.id" class="text-xs text-slate-400 mt-1">A senha de acesso será Prof123$</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Telefone</label>
              <input v-model="form.telefone" type="text" class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary" />
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Especialidade (Modalidades)</label>
            <input v-model="form.especialidade" type="text" placeholder="Ex: Ballet e Jazz" class="w-full px-4 py-2 border border-slate-200 rounded-lg outline-none focus:border-primary focus:ring-1 focus:ring-primary" />
          </div>
          
          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="closeModal" class="px-4 py-2 text-slate-600 font-medium hover:bg-slate-100 rounded-lg">Cancelar</button>
            <button type="submit" :disabled="saving" class="bg-primary hover:bg-primary-dark text-white px-6 py-2 rounded-lg font-bold">
              {{ saving ? 'Salvando...' : 'Salvar' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../../services/api';
import { toast } from 'vue3-toastify';

const professores = ref([]);
const loading = ref(true);
const saving = ref(false);
const showModal = ref(false);

const initialForm = { id: null, nome: '', telefone: '', email: '', especialidade: '' };
const form = ref({ ...initialForm });

const loadProfessores = async () => {
  try {
    loading.value = true;
    const res = await api.get('/professores');
    professores.value = res.data;
  } catch (error) {
    toast.error('Erro ao carregar professores');
  } finally {
    loading.value = false;
  }
};

const openModal = (prof = null) => {
  if (prof) form.value = { ...prof };
  else form.value = { ...initialForm };
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  form.value = { ...initialForm };
};

const saveProfessor = async () => {
  try {
    saving.value = true;
    if (form.value.id) {
      await api.put(`/professores/${form.value.id}`, form.value);
      toast.success('Professor atualizado');
    } else {
      await api.post('/professores', form.value);
      toast.success('Professor cadastrado! Ele já pode fazer login no sistema.');
    }
    closeModal();
    loadProfessores();
  } catch (error) {
    toast.error('Erro ao salvar professor');
  } finally {
    saving.value = false;
  }
};

const confirmDelete = async (id) => {
  if (confirm('Tem certeza que deseja excluir este professor?')) {
    try {
      await api.delete(`/professores/${id}`);
      toast.success('Professor removido');
      loadProfessores();
    } catch (error) {
      toast.error('Erro ao excluir professor');
    }
  }
};

const getInitials = (nome) => {
  if (!nome) return '?';
  const parts = nome.split(' ').filter(Boolean);
  if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
  return `${parts[0][0]}${parts[parts.length-1][0]}`.toUpperCase();
};

onMounted(loadProfessores);
</script>
