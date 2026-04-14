import { defineConfig } from 'vite';

export default defineConfig({
    root: "wwwroot",   // your index.html location
    envDir: "../",


    build: {
        outDir: "dist",  // output inside wwwroot/dist
        sourcemap: false // disable source maps (your goal)
    }
});