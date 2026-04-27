import { defineStore } from 'pinia';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('dance_token') || null,
    userRole: localStorage.getItem('dance_role') || null,
    userId: localStorage.getItem('dance_userId') || null,
    isAuthenticated: !!localStorage.getItem('dance_token'),
  }),
  actions: {
    async login(email, password) {
      try {
        const response = await api.post('/auth/login', { email, password });
        const data = response.data;
        
        this.token = data.token;
        this.userRole = Array.isArray(data.roles) ? data.roles[0] : data.roles;
        this.userId = data.userId;
        this.isAuthenticated = true;

        localStorage.setItem('dance_token', this.token);
        localStorage.setItem('dance_role', this.userRole);
        localStorage.setItem('dance_userId', this.userId);
        
        return true;
      } catch (error) {
        console.error('Login failed', error);
        return false;
      }
    },
    logout() {
      this.token = null;
      this.userRole = null;
      this.userId = null;
      this.isAuthenticated = false;
      
      localStorage.removeItem('dance_token');
      localStorage.removeItem('dance_role');
      localStorage.removeItem('dance_userId');
    }
  }
});
