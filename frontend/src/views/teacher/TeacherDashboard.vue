<template>
  <div class="space-y-6">
    <div class="bg-gradient-to-r from-primary to-primary-dark p-6 rounded-2xl text-white shadow-lg relative overflow-hidden">
      <div class="relative z-10">
        <h2 class="text-2xl font-bold">Olá, Professor(a)!</h2>
        <p class="opacity-90">Confira suas aulas de hoje e faça a chamada.</p>
      </div>
      <div class="absolute -right-4 -bottom-4 opacity-20 transform rotate-12">
        <svg class="w-32 h-32" fill="currentColor" viewBox="0 0 20 20"><path d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z"></path></svg>
      </div>
    </div>

    <!-- Destaque: Próxima Aula -->
    <div v-if="proximaAula" class="bg-indigo-50 border border-indigo-100 p-4 rounded-xl">
      <div class="flex items-center justify-between mb-2">
        <span class="bg-indigo-600 text-white text-[10px] font-bold px-2 py-0.5 rounded-full uppercase">Próxima Aula</span>
        <span class="text-indigo-600 font-bold text-sm">{{ proximaAula.horario }}</span>
      </div>
      <h3 class="font-bold text-slate-800 text-lg">{{ proximaAula.nome }}</h3>
      <p class="text-slate-500 text-xs mb-4">{{ proximaAula.modalidade?.nome }} • {{ proximaAula.alunosMatriculados?.length || 0 }} alunos</p>
      <button @click="$router.push({ name: 'TeacherChamada', params: { id: proximaAula.id } })" class="w-full bg-white border border-indigo-200 text-indigo-600 py-2 rounded-lg font-bold text-sm hover:bg-indigo-600 hover:text-white transition-all shadow-sm">
        Iniciar Chamada agora
      </button>
    </div>

    <div v-if="loading" class="text-center py-10 text-slate-500">Carregando suas turmas...</div>
    
    <div v-else-if="turmasHoje.length === 0" class="bg-white p-10 rounded-2xl text-center border border-dashed border-slate-300">
      <p class="text-slate-500">Você não tem aulas agendadas para hoje.</p>
    </div>

    <div v-else class="grid gap-4">
      <h3 class="text-sm font-bold text-slate-500 uppercase tracking-wider px-1">Aulas de Hoje</h3>
      <div v-for="turma in turmasHoje" :key="turma.id" 
           class="bg-white p-4 rounded-xl border border-slate-200 shadow-sm flex items-center justify-between transition-all"
           :class="{'opacity-50 grayscale': turma.status === 'Cancelada'}">
        <div class="flex items-center gap-4 cursor-pointer flex-1" @click="turma.status !== 'Cancelada' && $router.push({ name: 'TeacherChamada', params: { id: turma.id } })">
          <div class="w-12 h-12 bg-slate-50 rounded-lg flex flex-col items-center justify-center border border-slate-100">
            <span class="text-[10px] font-bold text-slate-400 uppercase">{{ turma.diaSemanaNome }}</span>
            <span class="text-xs font-bold text-primary">{{ turma.horario }}</span>
          </div>
          <div>
            <div class="flex items-center gap-2">
              <h4 class="font-bold text-slate-800">{{ turma.nome }}</h4>
              <span v-if="turma.status === 'Cancelada'" class="bg-rose-100 text-rose-600 text-[10px] font-bold px-2 py-0.5 rounded">CANCELADA</span>
            </div>
            <p class="text-xs text-slate-500">{{ turma.modalidade?.nome }} • {{ turma.alunosMatriculados?.length || 0 }} alunos</p>
            <p v-if="turma.status === 'Cancelada'" class="text-[10px] text-rose-400 italic">Motivo: {{ turma.motivoCancelamento || 'Não informado' }}</p>
          </div>
        </div>
        
        <div class="flex items-center gap-2">
          <button v-if="turma.status !== 'Cancelada'" @click="cancelarAula(turma)" class="p-2 text-slate-400 hover:text-rose-500 hover:bg-rose-50 rounded-lg transition-colors" title="Cancelar Aula">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
          </button>
          <div v-else class="text-rose-500 p-2">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path></svg>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../../services/api'

const turmas = ref([])
const loading = ref(true)

const diasSemanaMap = {
  0: 'Dom', 1: 'Seg', 2: 'Ter', 3: 'Qua', 4: 'Qui', 5: 'Sex', 6: 'Sab'
}

onMounted(async () => {
  try {
    const res = await api.get('/professores/minhas-turmas')
    turmas.value = res.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
})

const turmasHoje = computed(() => {
  const hojeIdx = new Date().getDay()
  const hojeNome = diasSemanaMap[hojeIdx]
  
  const list = []
  turmas.value.forEach(t => {
    // Procura nos horários estruturados
    const horariosHoje = t.horarios?.filter(h => h.diaSemana === hojeIdx) || []
    horariosHoje.forEach(h => {
      list.push({
        ...t,
        diaSemanaNome: hojeNome,
        horario: h.horaInicio.substring(0, 5),
        status: 'Ativa' // TODO: Buscar status real das ocorrências
      })
    })

    // Fallback para legado se não houver estruturado
    if (horariosHoje.length === 0 && t.gradeHorarios.includes(hojeNome)) {
      list.push({
        ...t,
        diaSemanaNome: hojeNome,
        horario: t.gradeHorarios.split(' ').pop(),
        status: 'Ativa'
      })
    }
  })

  return list.sort((a, b) => a.horario.localeCompare(b.horario))
})

const cancelarAula = async (turma) => {
  const motivo = prompt(`Motivo para cancelar a aula de ${turma.nome} às ${turma.horario}:`)
  if (motivo === null) return // Cancelou o prompt

  try {
    loading.value = true
    const dataHora = new Date()
    const [h, m] = turma.horario.split(':')
    dataHora.setHours(parseInt(h), parseInt(m), 0, 0)

    await api.post('/aulas/cancelar', {
      turmaId: turma.id,
      dataHora: dataHora.toISOString(),
      motivo: motivo || 'Cancelamento pelo professor'
    })
    
    // Atualiza localmente para feedback imediato
    turma.status = 'Cancelada'
    turma.motivoCancelamento = motivo
    alert('Aula cancelada com sucesso!')
  } catch (err) {
    alert('Erro ao cancelar aula.')
  } finally {
    loading.value = false
  }
}

const proximaAula = computed(() => {
  if (turmasHoje.value.length === 0) return null
  const agora = new Date()
  const horaAtual = `${agora.getHours().toString().padStart(2, '0')}:${agora.getMinutes().toString().padStart(2, '0')}`
  
  // Encontra a primeira aula que ainda não começou ou está acontecendo
  return turmasHoje.value.find(t => t.horario >= horaAtual) || turmasHoje.value[0]
})
</script>
