<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8 bg-white p-10 rounded-xl shadow-lg">
      <div>
        <!-- Logo simulado com as cores -->
        <div class="flex justify-center">
            <div class="h-32 w-32 bg-primary rounded-full flex items-center justify-center shadow-md">
              <span class="text-white font-bold text-5xl">VV</span>
            </div>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-slate-800">
          Acesso Restrito
        </h2>
        <p class="mt-2 text-center text-sm text-slate-500">
          Painel de Gestão - Academia Vania Valle
        </p>
      </div>
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="rounded-md shadow-sm -space-y-px">
          <div>
            <label for="email-address" class="sr-only">E-mail</label>
            <input id="email-address" name="email" type="email" autocomplete="email" required v-model="email" class="appearance-none rounded-none relative block w-full px-3 py-3 border border-slate-300 placeholder-slate-400 text-slate-900 rounded-t-md focus:outline-none focus:ring-primary focus:border-primary focus:z-10 sm:text-sm" placeholder="Endereço de E-mail">
          </div>
          <div>
            <label for="password" class="sr-only">Senha</label>
            <input id="password" name="password" type="password" autocomplete="current-password" required v-model="password" class="appearance-none rounded-none relative block w-full px-3 py-3 border border-slate-300 placeholder-slate-400 text-slate-900 rounded-b-md focus:outline-none focus:ring-primary focus:border-primary focus:z-10 sm:text-sm" placeholder="Senha">
          </div>
        </div>

        <div v-if="errorMsg" class="text-secondary text-sm text-center font-medium bg-red-50 p-2 rounded">
          {{ errorMsg }}
        </div>

        <div>
          <button type="submit" :disabled="loading" class="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-primary hover:bg-primary-dark focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary transition-colors shadow-sm">
            <span v-if="loading">Aguarde...</span>
            <span v-else>Entrar no Sistema</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'

const email = ref('')
const password = ref('')
const errorMsg = ref('')
const loading = ref(false)

const router = useRouter()
const authStore = useAuthStore()

const handleLogin = async () => {
  loading.value = true
  errorMsg.value = ''
  
  const success = await authStore.login(email.value, password.value)
  
  if (success) {
    if (authStore.userRole === 'Admin') {
      router.push({ name: 'AdminDashboard' })
    } else {
      router.push({ name: 'StudentDashboard' })
    }
  } else {
    errorMsg.value = 'Email ou senha inválidos. Tente novamente.'
  }
  
  loading.value = false
}
</script>
