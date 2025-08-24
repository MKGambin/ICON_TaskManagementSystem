import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: 63443,
        proxy: {
            '/api': {
                target: 'https://localhost:7065',
                changeOrigin: true,
                secure: false,
            }
        },
    }
})