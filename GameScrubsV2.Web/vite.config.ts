import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import vueDevTools from 'vite-plugin-vue-devtools';

// https://vite.dev/config/
export default defineConfig({
  define: {
    'process.env': process.env,
  },
  plugins: [vue(), vueDevTools()],
  server: {
    port: 5176, // or whatever port you want
    strictPort: true, // fail if port is already in use
    host: true, // expose to network if needed
    proxy: {
      '/api': {
        target: 'https://localhost:7291',
        changeOrigin: true,
        secure: false, // ignore SSL certificate errors
      },
    },
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
});
