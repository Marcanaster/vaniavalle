<template>
  <div class="min-h-screen bg-slate-50 flex">
    <!-- Sidebar -->
    <aside class="w-64 bg-white shadow-md flex flex-col z-10 relative">
      <div class="h-24 flex items-center justify-center border-b border-slate-100">
        <div class="flex items-center gap-3">
          <div class="h-16 w-16 bg-primary rounded-full flex items-center justify-center shadow-sm">
            <span class="text-white font-bold text-2xl">VV</span>
          </div>
          <span class="text-2xl font-bold text-slate-800 tracking-tight">Painel Gestão</span>
        </div>
      </div>
      
      <nav class="flex-1 p-4 space-y-1">
        <router-link 
          :to="{ name: 'AdminDashboard' }" 
          class="flex items-center gap-3 px-4 py-3 rounded-lg text-slate-600 hover:bg-primary-light/10 hover:text-primary transition-all font-medium"
          active-class="bg-primary text-white hover:bg-primary hover:text-white shadow-md"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"></path></svg>
          Dashboard
        </router-link>
        
        <!-- Outros links de CRUD -->
        <router-link :to="{ name: 'AdminAlunos' }" class="flex items-center gap-3 px-4 py-3 rounded-lg text-slate-600 hover:bg-primary-light/10 hover:text-primary transition-all font-medium" active-class="bg-primary text-white hover:bg-primary hover:text-white shadow-md">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
          Alunos
        </router-link>
        
        <router-link :to="{ name: 'AdminTurmas' }" class="flex items-center gap-3 px-4 py-3 rounded-lg text-slate-600 hover:bg-primary-light/10 hover:text-primary transition-all font-medium" active-class="bg-primary text-white hover:bg-primary hover:text-white shadow-md">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path></svg>
          Turmas
        </router-link>

        <router-link :to="{ name: 'AdminFinanceiro' }" class="flex items-center gap-3 px-4 py-3 rounded-lg text-slate-600 hover:bg-primary-light/10 hover:text-primary transition-all font-medium" active-class="bg-primary text-white hover:bg-primary hover:text-white shadow-md">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
          Financeiro
        </router-link>
      </nav>
      
      <div class="p-4 border-t border-slate-100">
        <button @click="handleLogout" class="flex items-center gap-3 px-4 py-3 w-full rounded-lg text-secondary hover:bg-secondary/10 hover:text-secondary-dark transition-all font-medium">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"></path></svg>
          Sair do Sistema
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="flex-1 flex flex-col">
      <!-- Topbar -->
      <header class="h-16 bg-white shadow-sm flex items-center justify-between px-8 z-0">
        <h1 class="text-xl font-semibold text-slate-800">
          {{ currentRouteName }}
        </h1>
        <div class="flex items-center gap-4">
          <span class="text-sm font-medium text-slate-500">Administrador</span>
          <div class="h-9 w-9 bg-primary-light rounded-full flex items-center justify-center text-white font-bold border-2 border-white shadow-sm">
            A
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <div class="p-8 flex-1 overflow-auto bg-slate-50">
        <router-view></router-view>
      </div>
    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const currentRouteName = computed(() => {
  if (route.name === 'AdminDashboard') return 'Visão Geral'
  return route.name || 'Painel'
})

const handleLogout = () => {
  authStore.logout()
  router.push({ name: 'Login' })
}
</script>
