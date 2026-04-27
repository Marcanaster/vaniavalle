<template>
  <div class="space-y-6">
    <!-- Bem-vindo -->
    <div>
      <h2 class="text-2xl font-bold text-slate-800">Bem-vindo de volta!</h2>
      <p class="text-slate-500">Aqui está o resumo do que está acontecendo na academia hoje.</p>
    </div>

    <!-- Cards de Métricas -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-5 hover:shadow-md transition-shadow">
        <div class="p-4 bg-primary/10 rounded-xl text-primary">
          <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
        </div>
        <div>
          <p class="text-sm font-semibold text-slate-400 uppercase tracking-wider">Alunos Ativos</p>
          <p class="text-3xl font-bold text-slate-800">124</p>
        </div>
      </div>
      
      <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-5 hover:shadow-md transition-shadow">
        <div class="p-4 bg-secondary/10 rounded-xl text-secondary">
          <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path></svg>
        </div>
        <div>
          <p class="text-sm font-semibold text-slate-400 uppercase tracking-wider">Turmas</p>
          <p class="text-3xl font-bold text-slate-800">12</p>
        </div>
      </div>

      <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center gap-5 hover:shadow-md transition-shadow">
        <div class="p-4 bg-emerald-100 rounded-xl text-emerald-600">
          <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
        </div>
        <div>
          <p class="text-sm font-semibold text-slate-400 uppercase tracking-wider">Receita Mês</p>
          <p class="text-3xl font-bold text-slate-800">R$ 15.420</p>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Próximas Aulas -->
      <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
        <div class="flex justify-between items-center mb-6">
          <h3 class="text-lg font-bold text-slate-800">Aulas de Hoje</h3>
          <button class="text-primary hover:text-primary-dark text-sm font-medium">Ver Grade Completa</button>
        </div>
        <div class="space-y-4">
          <div class="flex items-center justify-between p-4 bg-slate-50 rounded-xl border border-slate-100">
            <div class="flex items-center gap-4">
              <div class="h-12 w-12 bg-primary/20 rounded-lg flex flex-col items-center justify-center text-primary">
                <span class="text-sm font-bold">18:00</span>
              </div>
              <div>
                <p class="font-bold text-slate-800">Ballet Infantil - Nível 1</p>
                <p class="text-sm text-slate-500">15 Alunos confirmados</p>
              </div>
            </div>
          </div>
          <div class="flex items-center justify-between p-4 bg-slate-50 rounded-xl border border-slate-100">
            <div class="flex items-center gap-4">
              <div class="h-12 w-12 bg-secondary/20 rounded-lg flex flex-col items-center justify-center text-secondary">
                <span class="text-sm font-bold">19:30</span>
              </div>
              <div>
                <p class="font-bold text-slate-800">Jazz Contemporâneo</p>
                <p class="text-sm text-slate-500">12 Alunos confirmados</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Faturas Recentes -->
      <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
        <div class="flex justify-between items-center mb-6">
          <h3 class="text-lg font-bold text-slate-800">Últimos Pagamentos</h3>
          <button class="text-primary hover:text-primary-dark text-sm font-medium">Ir para Financeiro</button>
        </div>
        <div class="space-y-4">
          <div class="flex items-center justify-between py-3 border-b border-slate-50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-500 font-bold">AC</div>
              <div>
                <p class="font-semibold text-slate-800">Alice Carvalho</p>
                <p class="text-xs text-slate-500">Plano Mensal</p>
              </div>
            </div>
            <div class="text-right">
              <p class="font-bold text-emerald-600">R$ 180,00</p>
              <p class="text-xs text-slate-400">Há 2 horas</p>
            </div>
          </div>
          
          <div class="flex items-center justify-between py-3 border-b border-slate-50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-500 font-bold">MB</div>
              <div>
                <p class="font-semibold text-slate-800">Miguel Brito</p>
                <p class="text-xs text-slate-500">Plano Trimestral</p>
              </div>
            </div>
            <div class="text-right">
              <p class="font-bold text-emerald-600">R$ 450,00</p>
              <p class="text-xs text-slate-400">Há 5 horas</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>
