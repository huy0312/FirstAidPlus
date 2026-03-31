import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: '../wwwroot/react/qa',
    emptyOutDir: true,
    lib: {
      entry: './src/main.jsx',
      name: 'QAApp',
      fileName: (format) => `qa-app.js`,
      formats: ['iife']
    },
    rollupOptions: {
      output: {
        extend: true,
      }
    }
  },
  define: {
    'process.env': {}
  }
})
