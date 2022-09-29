import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import mkcert from 'vite-plugin-mkcert'
import vueI18n from '@intlify/vite-plugin-vue-i18n'
import { quasar, transformAssetUrls } from '@quasar/vite-plugin'

import fs from "node:fs";
import type { ServerOptions } from "https";
import path from 'path';

// https://vitejs.dev/config/
export default defineConfig({

    plugins: [vue({
        template: { transformAssetUrls }
    }), mkcert(),
        quasar({
            sassVariables: 'src/quasar-variables.sass'
        }),
        vueI18n({
            // if you want to use Vue I18n Legacy API, you need to set `compositionOnly: false`
            // compositionOnly: false,

            // you need to set i18n resource including paths !
            include: path.resolve(__dirname, './src/locales/**')
        })
    ],
    build: {
        rollupOptions: {
            output: {
                entryFileNames: `assets/[name].js`,
                chunkFileNames: `assets/[name].js`,
                assetFileNames: `assets/[name].[ext]`
            }
        }
    },
  server: {
    https: true,
  },
});
