<template>
  <div class="min-h-screen bg-slate-50 flex flex-col">
    <!-- Navbar Superior -->
    <header class="bg-white border-b border-slate-200 sticky top-0 z-30 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 h-16 flex items-center justify-between">
        <div class="flex items-center gap-2">
          <div class="w-8 h-8 bg-primary rounded-lg flex items-center justify-center">
            <span class="text-white font-bold">V</span>
          </div>
          <h1 class="text-lg font-bold text-slate-800 tracking-tight hidden sm:block">Portal do Professor</h1>
        </div>
        
        <div class="flex items-center gap-4">
          <div class="text-right hidden sm:block">
            <p class="text-xs font-bold text-slate-800 leading-none">{{ userName }}</p>
            <p class="text-[10px] text-slate-500">Professor</p>
          </div>
          <button @click="logout" class="p-2 text-slate-400 hover:text-red-500 transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"></path></svg>
          </button>
        </div>
      </div>
    </header>

    <main class="flex-1 max-w-7xl mx-auto w-full p-4">
      <router-view></router-view>
    </main>

    <!-- Menu Inferior (Mobile Friendly) -->
    <nav class="sm:hidden bg-white border-t border-slate-200 h-16 flex items-center justify-around sticky bottom-0 z-30 shadow-[0_-2px_10px_rgba(0,0,0,0.05)]">
      <router-link :to="{ name: 'TeacherDashboard' }" class="flex flex-col items-center gap-1 text-slate-400" active-class="text-primary">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"></path></svg>
        <span class="text-[10px] font-bold">Início</span>
      </router-link>
      <router-link :to="{ name: 'TeacherTurmas' }" class="flex flex-col items-center gap-1 text-slate-400" active-class="text-primary">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path></svg>
        <span class="text-[10px] font-bold">Minhas Turmas</span>
      </router-link>
    </nav>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const userName = ref('')

onMounted(() => {
  const user = JSON.parse(localStorage.getItem('user'))
  if (user) {
    userName.value = user.email.split('@')[0]
  }
})

const logout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  router.push('/login')
}
</script>
