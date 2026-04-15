import { defineConfig } from 'vite';

export default defineConfig({
    root: "wwwroot",
    envDir: "../",
    base: '/dist/',


    build: {
        outDir: "dist",
        sourcemap: false
    }
});