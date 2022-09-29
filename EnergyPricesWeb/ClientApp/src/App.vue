<template class="content">
    <q-layout view="hHh lpR fff">

        <q-header elevated class="bg-primary text-white" style="background-color: #004C99 !important ">
            <q-toolbar>
                <q-toolbar-title>
                    <q-avatar>
                        <img src="./assets/outline_ssid_chart_white_36dp.png">
                    </q-avatar>
                    energy-prices.net
                </q-toolbar-title>

                <q-btn dense flat round icon="build" @click="toggleRightDrawer" />
            </q-toolbar>
        </q-header>

        <q-drawer show-if-above v-model="rightDrawerOpen" side="right" bordered style="background-color: #f2f2f2">
            <div class="q-pa-md q-gutter-sm">
                <h3>Tools</h3>
                <small>Pick date for comparison</small>
                <q-btn outline icon="event" :label="compareDateTitle" style="width:200px">
                    <q-popup-proxy>
                        <q-date :options="existingDates" v-model="compareDate">
                            <div class="row items-center justify-end q-gutter-sm">
                                <q-btn label="Clear" color="primary" flat @click="clearCompareDate()" v-close-popup />
                                <q-btn label="OK" color="primary" flat @click="selectCompareDate" v-close-popup />
                            </div>
                        </q-date>
                    </q-popup-proxy>
                </q-btn>
                <div>
                    <small>Set tax percent for prices</small>
                    <q-input outlined v-model.number="taxPercent" @update:model-value="onTaxPercentUpdate" :dense="true" style="width:200px">
                        <template v-slot:prepend>
                            <q-icon name="percent" />
                        </template>
                    </q-input>
                </div>
            </div>
        </q-drawer>

        <q-page-container>
            <div class="q-pa-md">
                <div class="row">
                    <div class="col-12">
                        <h3>{{ $t("subtitle") }} - <b>{{ $t("updated") }} {{utcDate}}</b></h3>
                        <b><p>{{$t("top-description")}}</p></b>
                        <h5>{{displayDate}} {{$t("estimate.first")}} <b>{{addTax(firstPrice)}}</b> eur/kWH in {{firstMonth}} {{$t("estimate.second")}} <b>{{addTax(secondPrice)}}</b> eur/kWH in {{secondMonth}}.</h5>
                    </div>
                </div>
                <div class="row justify-center q-gutter-sm">
                    <q-select outlined v-model="model" :options="options" :dense="true" label="Market" style="width:240px">
                        <template v-slot:prepend>
                            <q-icon name="show_chart" />
                        </template>
                    </q-select>
                    <q-btn outline color="black" icon="event" :label="date" style="width:240px">
                        <q-popup-proxy>
                            <q-date :options="existingDates" v-model="date">
                                <div class="row items-center justify-end q-gutter-sm">
                                    <q-btn label="OK" color="primary" flat @click="selectDate" v-close-popup />
                                </div>
                            </q-date>
                        </q-popup-proxy>
                    </q-btn>

                </div>
                <div class="row">
                    <div class="col-12">
                        <PriceChart ref="priceChart"></PriceChart>
                    </div>
                </div>
                <div class="row q-pt-md">
                    <div class="col-12">
                        <p>
                            <i>Commodity prices are provided by http://www.nasdaqomx.com/. Futures contracts are agreements to buy or sell a specific quantity of a commodity at a specified price on a particular date in the future.</i>
                        </p>
                    </div>
                </div>
            </div>
            <router-view />
        </q-page-container>
        <q-footer class="bg-grey-8 text-white">
            <q-btn-dropdown color="grey-9" label="Language">
                <q-list>
                    <q-item clickable v-close-popup @click="onLanguageClick('de-DE')">
                        <q-item-section>
                            <q-item-label>Deutch</q-item-label>
                        </q-item-section>
                    </q-item>
                    <q-item clickable v-close-popup @click="onLanguageClick('en-US')">
                        <q-item-section>
                            <q-item-label>English</q-item-label>
                        </q-item-section>
                    </q-item>
                    <q-item clickable v-close-popup @click="onLanguageClick('fi-FI')">
                        <q-item-section>
                            <q-item-label>Finnish</q-item-label>
                        </q-item-section>
                    </q-item>
                </q-list>
            </q-btn-dropdown>
            <div class="q-pa-sm float-right"><a href="mailto:feedback@energy-prices.net">feedback</a> | (c) 2022</div>
        </q-footer>
    </q-layout>
</template>



<script lang="ts">
    import axios from 'axios'
    import PriceChart from './components/pricechart.vue'
    import { defineComponent, inject, ref } from 'vue'

    type DataObject = {
        data: Array<number>;
        labels: Array<string>;
        products: Array<string>;
    }

    type Nullable<T> = T | null

    export default defineComponent({
        components: {
            PriceChart
        },
        props: {
            utcDate: String,
        },
        setup(props) {
            const rightDrawerOpen = ref(false)

            const existingDates = ref([] as string[]);

            const responseData = ref({} as DataObject)
            const responseCompareData = ref({} as DataObject)

            var displayDate = ref('' as string | undefined)

            return {
                baseUrl: inject('BASE_URL'),
                responseData,
                responseCompareData,
                existingDates,
                displayDate,    
                firstDate: '',
                firstMonth: '',
                firstPrice: ref(0),
                secondMonth: '',
                secondPrice: ref(0),
                taxPercent: ref(0),
                date: ref(props.utcDate),
                compareDate: ref(null),
                rightDrawerOpen,
                toggleRightDrawer() {
                    rightDrawerOpen.value = !rightDrawerOpen.value
                },
                model: ref('European electricity'),
                options: [
                    'European electricity'
                ]
            }
        },
        computed: {
            compareDateTitle() {
                if (this.compareDate == null) {
                    return "Not selected"
                } else {
                    return this.compareDate
                }
            }
        },
        methods: {
            onLanguageClick(val) {
                window.location.href = '/' + val;
            },
            addTax(value) {
                return (value * (1 + this.taxPercent / 100.0)).toFixed(3)
            },
            onTaxPercentUpdate() {
                var copy = JSON.parse(JSON.stringify(this.responseData));
                for (var i in copy.data) {
                    copy.data[i] = copy.data[i] * (1 + this.taxPercent/100.0)
                }
                (this.$refs.priceChart as typeof PriceChart).loadData(copy, this.date)

                var compareCopy = JSON.parse(JSON.stringify(this.responseCompareData));
                for (var i in compareCopy.data) {
                    compareCopy.data[i] = this.addTax(compareCopy.data[i])
                }
                (this.$refs.priceChart as typeof PriceChart).loadCompareData(compareCopy, this.compareDate)
            },
            async fetchData(date) {
                const { data, status } = await axios.get(
                    `${this.baseUrl}/api/chartdata?date=${date}`,
                    {
                        headers: {
                            Accept: 'application/json',
                        },
                    },
                );
                return data
            },
            async fetchAllowedDates() {
                const { data, status } = await axios.get(
                    `${this.baseUrl}/api/existing-dates`,
                    {
                        headers: {
                            Accept: 'application/json',
                        },
                    },
                );
                return data
            },
            clearCompareDate() {
                this.compareDate = null
                this.responseCompareData = { data: [], labels: [], products: []} as DataObject
                (this.$refs.priceChart as typeof PriceChart).loadCompareData(this.responseCompareData)
            },
            selectCompareDate(event) {
                this.loadCompareData(this.compareDate)
            },
            selectDate(event) {
                this.loadData(this.date)
                this.displayDate = this.date
            },
            async loadCompareData(date) {
                var data = await this.fetchData(date)
                this.responseCompareData = JSON.parse(JSON.stringify(data));
                this.onTaxPercentUpdate()
            },
            async loadData(date) {
                this.taxPercent = 0
                var data = await this.fetchData(date)
                this.responseData = JSON.parse(JSON.stringify(data));
                (this.$refs.priceChart as typeof PriceChart).loadData(this.responseData, this.date)

                if (this.responseData != null) {
                    this.firstPrice = this.responseData.data[0]
                    this.firstMonth = this.responseData.labels[0]

                    this.secondPrice = this.responseData.data[1]
                    this.secondMonth = this.responseData.labels[1]
                }
            },
            async loadExistingDates() {
                var data = await this.fetchAllowedDates() as string[]
                this.existingDates.splice(0, this.existingDates.length);
                this.existingDates.push(...data)
            }
        },
        mounted() {
            this.selectDate(this.date)
            this.loadExistingDates()
        }
    })

</script>

<style>
    body {
        background-color: #f2f2f2;
    }
    h3 {
        padding: 0;
        margin: 0;
        font-size: 1.5em !important;
    }
</style>