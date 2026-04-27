<template>
  <div class="min-h-screen bg-slate-50 flex flex-col">
    <!-- Navbar Superior -->
    <header class="bg-white shadow-sm border-b border-slate-100 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center">
          <!-- Logo -->
          <div class="flex items-center gap-3">
            <div class="h-10 w-10 bg-primary rounded-full flex items-center justify-center shadow-sm">
              <span class="text-white font-bold">VV</span>
            </div>
            <span class="text-xl font-bold text-slate-800 tracking-tight hidden sm:block">Portal do Aluno</span>
          </div>

          <!-- Ações -->
          <div class="flex items-center gap-4">
            <div class="hidden sm:flex items-center gap-2">
              <div class="h-8 w-8 bg-slate-100 rounded-full flex items-center justify-center text-primary font-bold">
                A
              </div>
              <span class="text-sm font-medium text-slate-600">Meu Perfil</span>
            </div>
            <button @click="handleLogout" class="flex items-center gap-2 px-4 py-2 rounded-lg text-secondary hover:bg-secondary/10 hover:text-secondary-dark transition-all text-sm font-medium">
              Sair
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Conteúdo da Página -->
    <main class="flex-1 max-w-7xl w-full mx-auto p-4 sm:p-6 lg:p-8">
      <router-view></router-view>
    </main>

    <!-- Rodapé Simples -->
    <footer class="bg-white border-t border-slate-200 py-6 mt-auto">
      <div class="max-w-7xl mx-auto px-4 text-center">
        <p class="text-sm text-slate-500">Academia Vania Valle &copy; {{ new Date().getFullYear() }} - Todos os direitos reservados.</p>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()
  router.push({ name: 'Login' })
}
</script>
