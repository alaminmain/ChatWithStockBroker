import React, { useEffect, useState } from 'react';
import { Chart } from 'react-chartjs-2';
import { TreemapController, TreemapElement } from 'chartjs-chart-treemap';
import { Chart as ChartJS, Tooltip, Legend, LinearScale } from 'chart.js';
import { getHeatmapData } from '../../api';

ChartJS.register(TreemapController, TreemapElement, Tooltip, Legend, LinearScale);

const SectorHeatmap = ({ sectorData }) => {

    const colorFromValue = (value) => {
        if (value > 0) return '#008000'; // green
        if (value < 0) return '#FF0000'; // red
        return '#0000FF'; // blue
    };

    const data = {
        datasets: [
            {
                label: sectorData.sector,
                tree: sectorData.stocks,
                key: 'volume',
                backgroundColor: (ctx) => {
                    if (ctx.type !== 'treemap') return '#808080';
                    const item = ctx.raw.g ? ctx.raw.g : ctx.raw;
                    console.log('BackgroundColor item:', item);
                    if (!item || !item._data) {
                        console.log('BackgroundColor returning gray');
                        return '#808080'; // Grey for safety
                    }
                    const color = colorFromValue(item._data.changePercent);
                    console.log('BackgroundColor value:', item._data.changePercent, 'color:', color);
                    return color;
                },
                labels: {
                    display: true,
                    formatter: (ctx) => {
                        const item = ctx.raw.g ? ctx.raw.g : ctx.raw;
                        const originalItem = item ? item._data : null;
                        if (originalItem && typeof originalItem.changePercent === 'number') {
                            return [originalItem.symbol, `${originalItem.changePercent.toFixed(2)}%`];
                        }
                        return null;
                    },
                    color: '#fff',
                    font: { size: 12, weight: 'bold' },
                },
            },
        ],
    };

    const options = {
        plugins: {
            title: {
                display: true,
                text: sectorData.sector,
            },
            legend: {
                display: false,
            },
            tooltip: {
                callbacks: {
                    label: (context) => {
                        const item = context.raw.g ? context.raw.g : context.raw;
                        const originalItem = item ? item._data : null;
                        if (originalItem && typeof originalItem.changePercent === 'number' && typeof originalItem.volume === 'number') {
                            return `${originalItem.symbol}: ${originalItem.changePercent.toFixed(2)}% (Volume: ${originalItem.volume.toLocaleString()})`;
                        }
                        if (originalItem) {
                            return `${originalItem.symbol || 'N/A'}: Data unavailable`;
                        }
                        return 'Data unavailable';
                    },
                },
            },
        },
        maintainAspectRatio: false,
    };

    return (
        <div className="col-lg-6 col-md-12 mb-4">
            <div className="card">
                <div className="card-body">
                    <div style={{ height: '400px' }}>
                        <Chart type="treemap" data={data} options={options} />
                    </div>
                </div>
            </div>
        </div>
    );
};

const HeatmapPage = () => {
    const [heatmapData, setHeatmapData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const response = await getHeatmapData();
                setHeatmapData(response.data);
            } catch (err) {
                setError('Failed to fetch heatmap data.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    if (loading) return <p>Loading heatmap data...</p>;
    if (error) return <p className="text-danger">{error}</p>;

    return (
        <div className="container-fluid mt-4">
            <h2 className="mb-4">Sector Heatmaps</h2>
            <div className="row">
                {heatmapData.map((sector) => (
                    <SectorHeatmap key={sector.sector} sectorData={sector} />
                ))}
            </div>
        </div>
    );
};

export default HeatmapPage;