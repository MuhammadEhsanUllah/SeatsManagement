// nuxt.config.ts
import { defineNuxtConfig } from 'nuxt/config'

export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  css: [
    '~/assets/scss/main.scss', // Your SCSS file that already imports Bootstrap CSS
  ],
  modules: ['@pinia/nuxt'],
  plugins: [
    '~/plugins/bootstrap.client.ts' // Plugin to handle Bootstrap JS
  ],
  app: {
    head: {
      title: 'Seats Reservation',
    },
  },
  runtimeConfig: {
    public: {
      API_BASE_URL: process.env.API_BASE_URL || 'http://localhost:3000', // Fallback to a default URL
    },
  },
})
