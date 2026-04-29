<template>
  <div class="min-h-screen bg-white">
    <!-- Navbar Publica -->
    <header class="absolute top-0 w-full z-50 bg-gradient-to-b from-black/60 to-transparent">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-20">
          <div class="flex items-center gap-4">
            <img src="../../assets/logo_academia.png" alt="Academia Vania Valle Logo" class="h-32 md:h-40 w-auto object-contain bg-white/10 rounded-full p-2 shadow-lg" />
            <span class="text-white font-bold text-2xl md:text-3xl tracking-tight hidden sm:block">Academia Vania Valle</span>
          </div>
          <nav class="flex gap-4 items-center">
            <router-link :to="{ name: 'Login' }" class="text-white font-medium hover:text-secondary-light transition-colors hidden sm:block">Painel do Aluno</router-link>
            <router-link :to="{ name: 'Login' }" class="bg-primary hover:bg-primary-dark text-white px-6 py-2 rounded-full font-bold transition-all shadow-lg border border-primary-light/50">
              Acessar
            </router-link>
          </nav>
        </div>
      </div>
    </header>

    <!-- Hero / Carrossel -->
    <section class="relative h-screen bg-slate-900 flex items-center justify-center overflow-hidden">
      <!-- Imagem do Carrossel (Estrutura pronta para carregar fotos na pasta assets) -->
      <img :src="currentImage" alt="Espetáculo" class="absolute inset-0 w-full h-full object-cover opacity-60 transition-opacity duration-1000" />
      
      <!-- Overlay Gradiente -->
      <div class="absolute inset-0 bg-gradient-to-t from-slate-900 via-slate-900/40 to-transparent"></div>

      <div class="relative z-10 text-center px-4 max-w-4xl mx-auto mt-20">
        <h1 class="text-5xl md:text-7xl font-extrabold text-white mb-6 drop-shadow-lg leading-tight">
          A Arte do Movimento em <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary-light to-secondary">Sua Vida</span>
        </h1>
        <p class="text-xl md:text-2xl text-slate-200 mb-10 font-light drop-shadow-md">
          Aulas de ballet clássico, jazz e danças urbanas para todas as idades.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <a href="https://wa.me/5521997416205?text=Olá! Gostaria de agendar uma aula experimental." target="_blank" class="bg-secondary hover:bg-secondary-dark text-white px-8 py-4 rounded-full font-bold text-lg transition-all shadow-xl shadow-secondary/30 transform hover:-translate-y-1 inline-block">
            Agende uma Aula Experimental
          </a>
          <button class="bg-white/10 backdrop-blur-md hover:bg-white/20 text-white border border-white/30 px-8 py-4 rounded-full font-bold text-lg transition-all">
            Ver Modalidades
          </button>
        </div>
      </div>

      <!-- Controles Carrossel -->
      <div class="absolute bottom-10 left-0 right-0 flex justify-center gap-3 z-20">
        <button v-for="(img, index) in images" :key="index" @click="setCarousel(index)" 
                :class="['w-3 h-3 rounded-full transition-all', currentIndex === index ? 'bg-secondary scale-125' : 'bg-white/50 hover:bg-white']">
        </button>
      </div>
    </section>

    <!-- Próximos Shows -->
    <section class="py-24 bg-slate-50">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-4xl font-extrabold text-slate-800 mb-4">Nossos Próximos Espetáculos</h2>
          <p class="text-lg text-slate-500 max-w-2xl mx-auto">Venha prestigiar o talento dos nossos alunos nos palcos mais belos da cidade.</p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <!-- Card de Show 1 -->
          <div class="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden group">
            <div class="h-48 bg-slate-200 relative overflow-hidden">
              <div class="absolute inset-0 bg-primary/20 group-hover:bg-transparent transition-colors z-10"></div>
              <!-- Placeholder de arquitetura pronto, basta trocar o src apontando para os assets -->
              <img src="https://images.unsplash.com/photo-1508700929628-666bc8bd84ea?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" alt="Ballet Clássico" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
            </div>
            <div class="p-6">
              <div class="text-secondary font-bold text-sm mb-2 uppercase tracking-wide">Dezembro 2026</div>
              <h3 class="text-xl font-bold text-slate-800 mb-2">O Quebra-Nozes</h3>
              <p class="text-slate-600 mb-4 line-clamp-2">A tradicional apresentação de fim de ano com a participação de todos os alunos de ballet clássico.</p>
              <button class="text-primary font-bold hover:text-primary-dark transition-colors flex items-center gap-1">
                Comprar Ingressos <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
              </button>
            </div>
          </div>

          <!-- Card de Show 2 -->
          <div class="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden group">
            <div class="h-48 bg-slate-200 relative overflow-hidden">
              <div class="absolute inset-0 bg-secondary/20 group-hover:bg-transparent transition-colors z-10"></div>
              <img src="https://images.unsplash.com/photo-1518834107812-6a36472488a4?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" alt="Danças Urbanas" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
            </div>
            <div class="p-6">
              <div class="text-primary font-bold text-sm mb-2 uppercase tracking-wide">Outubro 2026</div>
              <h3 class="text-xl font-bold text-slate-800 mb-2">Festival Urban Move</h3>
              <p class="text-slate-600 mb-4 line-clamp-2">As turmas de Hip-Hop e Jazz Funk invadem o teatro municipal com coreografias inéditas.</p>
              <button class="text-primary font-bold hover:text-primary-dark transition-colors flex items-center gap-1">
                Comprar Ingressos <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
              </button>
            </div>
          </div>

          <!-- Card de Show 3 -->
          <div class="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden group">
            <div class="h-48 bg-slate-200 relative flex items-center justify-center bg-slate-800">
              <span class="text-slate-400 font-medium">Sua Foto Aqui (assets)</span>
            </div>
            <div class="p-6">
              <div class="text-slate-500 font-bold text-sm mb-2 uppercase tracking-wide">A Definir</div>
              <h3 class="text-xl font-bold text-slate-800 mb-2">Mostra Coreográfica</h3>
              <p class="text-slate-600 mb-4 line-clamp-2">Apresentação especial das turmas iniciantes demonstrando sua evolução.</p>
              <button class="text-primary font-bold hover:text-primary-dark transition-colors flex items-center gap-1">
                Saiba Mais <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

// Array com os nomes das imagens. 
// Atualmente usando imagens provisórias do Unsplash, basta trocar os links 
// para algo como: import img1 from '../../assets/foto-ballet.jpg'
const images = [
  'https://images.unsplash.com/photo-1518609878373-06d740f60d8b?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80',
  'https://images.unsplash.com/photo-1547153760-18fc86324498?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80',
  'https://images.unsplash.com/photo-1504609774528-69352e0084db?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80'
]

const currentIndex = ref(0)
const currentImage = ref(images[0])
let timer = null

const setCarousel = (index) => {
  currentIndex.value = index
  currentImage.value = images[index]
}

const nextImage = () => {
  const next = (currentIndex.value + 1) % images.length
  setCarousel(next)
}

onMounted(() => {
  timer = setInterval(nextImage, 5000)
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>
