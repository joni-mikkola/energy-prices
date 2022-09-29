import { createApp } from 'vue'
import { Quasar } from 'quasar'
import App from './App.vue'

import deDE from './locales/de-DE.json'
import enUS from './locales/en-US.json'
import fiFI from './locales/fi-FI.json'

import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/src/css/index.sass'

import Adsense from 'vue-google-adsense/dist/Adsense.min.js'
import ScriptX from 'vue-scriptx'

import { createI18n } from "vue-i18n";

type MountEl = {
    dataset: {
        date: string
    }
}

const mountEl = document.querySelector("#app") as MountEl | null;
var dataset = (mountEl as any).dataset

const i18n = createI18n({
    legacy: false,
    fallbackLocale: 'en-US',
    locale: dataset.culture,
    globalInjection: true,
    messages: {
        'de-DE': deDE,
        'en-US': enUS,
        'fi-FI': fiFI
    }
});

let utcDate = ''
if (mountEl != null) {
    utcDate = mountEl.dataset.date
}

const myApp = createApp(App, { utcDate: utcDate })
myApp.provide('BASE_URL', dataset.baseurl)

myApp.use(Quasar, {
    plugins: {}, // import Quasar plugins and add here
})

myApp.use(i18n)

myApp.use(ScriptX)
myApp.use(Adsense)

myApp.mount('#app')
