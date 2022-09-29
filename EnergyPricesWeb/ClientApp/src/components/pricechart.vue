<template>
    <div style="position: relative; text-align: center">
        <div v-if="loading" style="font-size:2.0em; color:grey; position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);" >Loading</div>
        <LineChart style="height:600px" v-bind="lineChartProps" />
    </div>
</template>



<script lang='ts'>


    import { computed, defineComponent, ref } from "vue";
    import { shuffle } from "lodash";
    import { LineChart, useLineChart } from "vue-chart-3";
    import ChartDataLabels from 'chartjs-plugin-datalabels';
    import { Chart, ChartData, ChartOptions, registerables } from "chart.js";

    Chart.register(ChartDataLabels);
    Chart.register(...registerables);
    export default defineComponent({
        name: "App",
        components: { LineChart },
        setup() {
            const loading = ref(true);

            const compareDate = ref('')
            const compareDataValues = ref([] as string[]);

            const date = ref('')
            const dataValues = ref([] as string[]);
            const dataLabels = ref([] as string[]);
            var dataProducts = [];

            //@ts-ignore
            const testData = computed<ChartData<"line">>(() => ({
                labels: dataLabels.value,
                datasets: [
                    {
                        label: compareDate.value,
                        data: compareDataValues.value,
                        backgroundColor: '#99CCFFAA',
                        borderColor: '#99CCFFAA',
                    },
                    {
                        label: date.value,
                        data: dataValues.value,
                        backgroundColor: '#004C99',
                        borderColor: '#004C99',
                    }
                ],
            }));

            //@ts-ignore
            const options = computed<ChartOptions<"line">>(() => ({
                plugins: {
                    legend: {
                        labels: {
                            filter: function (item, chart) {
                                if (item.datasetIndex == 1 && dataValues.value.length > 0) {
                                    return true
                                }
                                if (item.datasetIndex == 0 && compareDataValues.value.length > 0) {
                                    return true
                                }
                                return false;
                            }
                        }
                    },
                    datalabels: {
                        backgroundColor: function (context) {
                            return context.dataset.backgroundColor;
                        },
                        display: function (context) {
                            if (context.datasetIndex == 0) {
                                return false
                            }
                            return true
                        },
                        borderRadius: 4,
                        color: 'white',
                        font: {
                            weight: 'bold'
                        },
                        formatter: function (value, ctx) {
                            return value.toFixed(2) + ' eur';
                        },
                        padding: 6
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                let label = context.dataset.label + ' ' +  context.parsed.y.toFixed(3) + ' eur/kWH';
                                return label;
                            },
                            footer: (context) => {
                                const arrayLines = ['Product: ' + dataProducts[context[0].dataIndex]]
                                return arrayLines
                            }
                        }
                    }
                },
                scales: {
                    yAxes: {
                        title: {
                            display: true,
                            text: 'Eur/kWH',
                            font: {
                                size: 15
                            }
                        },
                        ticks: {
                            stepSize: 0.05
                        }
                    },
                    xAxes: {
                        title: {
                            display: true,
                            text: 'Time',
                            font: {
                                size: 15
                            }
                        },
                        ticks: {
                            autoSkip: true,
                            maxTicksLimit: 20
                        }
                    }

                },
                interaction: {
                    intersect: false,
                    mode: 'index',
                },
                maintainAspectRatio:false
            }));

            const { lineChartProps, lineChartRef } = useLineChart({
                chartData: testData,
                options,
            });

            function loadData(newData, newDate) {
                date.value = newDate
                dataValues.value.splice(0, dataValues.value.length);
                dataValues.value.push(...newData.data)
                dataLabels.value.splice(0, dataLabels.value.length);
                dataLabels.value.push(...newData.labels)
                dataProducts = newData.products
                loading.value = false
            }

            function loadCompareData(newData, newDate) {
                compareDate.value = newDate
                compareDataValues.value.splice(0, compareDataValues.value.length);
                compareDataValues.value.push(...newData.data)
            }

            return {
                loading,
                loadData,
                loadCompareData,
                testData,
                options,
                lineChartRef,
                lineChartProps,
            };
        },
    });
</script>