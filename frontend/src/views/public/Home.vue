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
            <router-link :to="{ name: 'Login' }" class="bg-primary hover:bg-primary-dark text-white px-6 py-2 rounded-full font-bold transition-all shadow-lg border border-primary-light/50 flex items-center gap-2 group">
              <span>Acessar Portal</span>
              <svg class="w-4 h-4 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 5l7 7-7 7M3 12h18"></path></svg>
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
          <a href="https://wa.me/5521964329428?text=Olá! Gostaria de agendar uma aula experimental." target="_blank" class="bg-secondary hover:bg-secondary-dark text-white px-8 py-4 rounded-full font-bold text-lg transition-all shadow-xl shadow-secondary/30 transform hover:-translate-y-1 inline-block">
            Agende uma Aula Experimental
          </a>
          <button @click="scrollToModalidades" class="bg-white/10 backdrop-blur-md hover:bg-white/20 text-white border border-white/30 px-8 py-4 rounded-full font-bold text-lg transition-all">
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

    <!-- Modalidades Dinâmicas -->
    <section id="modalidades" class="py-24 bg-white scroll-mt-20">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex flex-col md:flex-row justify-between items-end mb-12 gap-4">
          <div>
            <h2 class="text-4xl font-extrabold text-slate-800 mb-2 italic uppercase tracking-tighter">Nossas Modalidades</h2>
            <div class="h-1.5 w-24 bg-primary rounded-full"></div>
          </div>
          <p class="text-slate-500 max-w-md">Do clássico ao contemporâneo, oferecemos o melhor ensino de dança para todos os níveis e idades.</p>
        </div>

        <div v-if="modalidades.length > 0" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          <div v-for="m in modalidades" :key="m.id" class="bg-slate-50 p-8 rounded-3xl border border-slate-100 hover:border-primary/30 transition-all group hover:shadow-xl hover:shadow-primary/5 hover:-translate-y-2">
            <div class="w-14 h-14 bg-primary/10 rounded-2xl flex items-center justify-center text-primary mb-6 group-hover:bg-primary group-hover:text-white transition-all">
              <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
            </div>
            <h3 class="text-xl font-bold text-slate-800 mb-3">{{ m.nome }}</h3>
            <p class="text-slate-600 text-sm leading-relaxed mb-6">{{ m.descricao || 'Venha descobrir a beleza desta modalidade com nossos professores especializados.' }}</p>
            <a href="https://wa.me/5521964329428" target="_blank" class="text-primary font-bold text-sm flex items-center gap-2 group/link">
              Quero saber mais
              <svg class="w-4 h-4 group-hover/link:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4-4H3"></path></svg>
            </a>
          </div>
        </div>
        <div v-else class="text-center py-12 text-slate-400 italic">
          Carregando nossas modalidades...
        </div>
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

    <!-- Instagram Feed Simulation -->
    <section class="py-24 bg-slate-900 overflow-hidden">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex flex-col md:flex-row items-center justify-between mb-12 gap-8">
          <div class="text-center md:text-left">
            <h2 class="text-3xl md:text-4xl font-bold text-white mb-2 italic">Acompanhe nossa arte <span class="text-secondary">@academiavaniavalle</span></h2>
            <p class="text-slate-400">Siga-nos no Instagram para ver os bastidores e novidades.</p>
          </div>
          <a href="https://www.instagram.com/academiavaniavalle/" target="_blank" class="bg-gradient-to-tr from-yellow-400 via-red-500 to-purple-600 text-white px-8 py-3 rounded-full font-bold shadow-lg hover:scale-105 transition-transform flex items-center gap-2">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24"><path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/></svg>
            Visitar Perfil
          </a>
        </div>

        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div v-for="i in 8" :key="i" class="aspect-square bg-slate-800 rounded-2xl overflow-hidden relative group cursor-pointer shadow-2xl">
            <div class="absolute inset-0 bg-primary/40 opacity-0 group-hover:opacity-100 transition-opacity z-10 flex items-center justify-center">
               <svg class="w-12 h-12 text-white" fill="currentColor" viewBox="0 0 24 24"><path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/></svg>
            </div>
            <img :src="`https://images.unsplash.com/photo-1518834107812-6a36472488a4?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80&sig=${i}`" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-1000" />
          </div>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="bg-slate-50 py-12 border-t border-slate-100">
      <div class="max-w-7xl mx-auto px-4 text-center">
        <img src="../../assets/logo_academia.png" alt="Logo" class="h-20 mx-auto mb-6 opacity-50 grayscale hover:grayscale-0 transition-all" />
        <p class="text-slate-500 text-sm">© 2026 Academia Vania Valle. Todos os direitos reservados.</p>
        <div class="flex justify-center gap-6 mt-6">
          <a href="#" class="text-slate-400 hover:text-primary transition-colors">Instagram</a>
          <a href="#" class="text-slate-400 hover:text-primary transition-colors">Facebook</a>
          <a href="#" class="text-slate-400 hover:text-primary transition-colors">WhatsApp</a>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import api from '../../services/api'

const images = [
  'https://images.unsplash.com/photo-1518609878373-06d740f60d8b?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80',
  'https://images.unsplash.com/photo-1547153760-18fc86324498?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80',
  'https://images.unsplash.com/photo-1504609774528-69352e0084db?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80'
]

const modalidades = ref([])
const currentIndex = ref(0)
const currentImage = ref('')
let timer = null

const scrollToModalidades = () => {
  document.getElementById('modalidades').scrollIntoView({ behavior: 'smooth' })
}

const setCarousel = (index) => {
  currentIndex.value = index
  currentImage.value = images[index]
}

const nextImage = () => {
  const next = (currentIndex.value + 1) % images.length
  setCarousel(next)
}


onMounted(async () => {
  currentImage.value = images[0]
  timer = setInterval(nextImage, 5000)
  
  try {
    const res = await api.get('/modalidades')
    modalidades.value = res.data
  } catch (err) {
    console.error('Erro ao carregar modalidades', err)
  }
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>
