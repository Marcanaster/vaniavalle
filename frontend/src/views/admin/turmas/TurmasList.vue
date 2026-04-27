<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold text-slate-800">Turmas</h2>
        <p class="text-slate-500">Gerenciamento das turmas e modalidades da academia.</p>
      </div>
      <router-link :to="{ name: 'AdminTurmasNova' }" class="bg-primary hover:bg-primary-dark text-white px-4 py-2 rounded-lg font-medium transition-colors flex items-center gap-2 shadow-sm">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Nova Turma
      </router-link>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden flex flex-col">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Nome da Turma</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Nível</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Idade</th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Capacidade</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider">Ações</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-200">
            <tr v-if="loading">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Carregando turmas...</td>
            </tr>
            <tr v-else-if="turmas.length === 0">
              <td colspan="5" class="px-6 py-10 text-center text-slate-500">Nenhuma turma cadastrada.</td>
            </tr>
            <tr v-for="turma in turmas" :key="turma.id" class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-slate-900">{{ turma.nome }}</div>
                <div class="text-sm text-slate-500">{{ turma.gradeHorarios }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ turma.nivel }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ turma.idadeMinima }} a {{ turma.idadeMaxima }} anos</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{{ turma.capacidadeAlunos }} vagas</td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <a href="#" class="text-primary hover:text-primary-dark mr-3">Editar</a>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../../services/api'

const turmas = ref([])
const loading = ref(true)

onMounted(async () => {
  loading.value = true
  try {
    const res = await api.get('/turmas')
    turmas.value = res.data
  } catch (err) {
    console.error(err)
  }
  loading.value = false
})
</script>
