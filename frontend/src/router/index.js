import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/public/Home.vue')
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/auth/Login.vue')
  },
  {
    path: '/admin',
    component: () => import('../layouts/AdminLayout.vue'),
    meta: { requiresAuth: true, role: 'Admin' },
    children: [
      {
        path: 'dashboard',
        name: 'AdminDashboard',
        component: () => import('../views/admin/Dashboard.vue')
      },
      {
        path: 'alunos',
        name: 'AdminAlunos',
        component: () => import('../views/admin/alunos/AlunosList.vue')
      },
      {
        path: 'alunos/novo',
        name: 'AdminAlunosNovo',
        component: () => import('../views/admin/alunos/AlunoForm.vue')
      },
      {
        path: 'alunos/:id/editar',
        name: 'AdminAlunosEditar',
        component: () => import('../views/admin/alunos/AlunoForm.vue')
      },
      {
        path: 'turmas',
        name: 'AdminTurmas',
        component: () => import('../views/admin/turmas/TurmasList.vue')
      },
      {
        path: 'turmas/nova',
        name: 'AdminTurmasNova',
        component: () => import('../views/admin/turmas/TurmaForm.vue')
      },
      {
        path: 'turmas/:id/editar',
        name: 'AdminTurmasEditar',
        component: () => import('../views/admin/turmas/TurmaForm.vue')
      },
      {
        path: 'financeiro',
        name: 'AdminFinanceiro',
        component: () => import('../views/admin/financeiro/FaturasList.vue')
      },
      {
        path: 'professores',
        name: 'AdminProfessores',
        component: () => import('../views/admin/professores/ProfessoresList.vue')
      },
      {
        path: 'modalidades',
        name: 'AdminModalidades',
        component: () => import('../views/admin/modalidades/ModalidadesList.vue')
      },
      {
        path: 'leads',
        name: 'AdminLeads',
        component: () => import('../views/admin/leads/LeadsList.vue')
      }
    ]
  },
  {
    path: '/student',
    component: () => import('../layouts/StudentLayout.vue'),
    meta: { requiresAuth: true, role: 'Student' },
    children: [
      {
        path: 'dashboard',
        name: 'StudentDashboard',
        component: () => import('../views/student/Dashboard.vue')
      }
    ]
  },
  {
    path: '/teacher',
    component: () => import('../layouts/TeacherLayout.vue'),
    meta: { requiresAuth: true, role: 'Teacher' },
    children: [
      {
        path: 'dashboard',
        name: 'TeacherDashboard',
        component: () => import('../views/teacher/TeacherDashboard.vue')
      },
      {
        path: 'turmas',
        name: 'TeacherTurmas',
        component: () => import('../views/teacher/TeacherTurmas.vue')
      },
      {
        path: 'chamada/:id',
        name: 'TeacherChamada',
        component: () => import('../views/teacher/TeacherChamada.vue')
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth) {
    if (!authStore.isAuthenticated) {
      return next({ name: 'Login' })
    }
    
    // Simple role check
    if (to.meta.role && to.meta.role !== authStore.userRole) {
      if (authStore.userRole === 'Admin') return next({ name: 'AdminDashboard' })
      if (authStore.userRole === 'Teacher') return next({ name: 'TeacherDashboard' })
      if (authStore.userRole === 'Student') return next({ name: 'StudentDashboard' })
      return next({ name: 'Home' })
    }
  }
  
  next()
})

export default router
