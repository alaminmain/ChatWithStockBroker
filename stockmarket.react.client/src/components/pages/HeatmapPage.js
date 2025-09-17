import React, { useEffect, useState } from 'react';
import { Chart } from 'react-chartjs-2';
import { TreemapController, TreemapElement } from 'chartjs-chart-treemap';
import { Chart as ChartJS, Tooltip, Legend } from 'chart.js';
import { getHeatmapData } from '../../api';

ChartJS.register(TreemapController, TreemapElement, Tooltip, Legend);

const SectorHeatmap = ({ sectorData }) => {

    const colorFromValue = (value) => {
        if (value > 2) return '#008000'; // Strong green
        if (value > 0) return '#90EE90'; // Light green
        if (value < -2) return '#FF0000'; // Strong red
        if (value < 0) return '#F08080'; // Light red
        return '#D3D3D3'; // Grey for unchanged
    };

    const data = {
        datasets: [
            {
                label: sectorData.sector,
                tree: sectorData.stocks,
                key: 'volume',
                groups: ['symbol'],
                backgroundColor: (ctx) => {
                    if (ctx.type !== 'treemap') return '#808080';
                    const item = ctx.raw.g ? ctx.raw.g : ctx.raw;
                    return colorFromValue(item.changePercent);
                },
                labels: {
                    display: true,
                    formatter: (ctx) => {
                        const item = ctx.raw.g ? ctx.raw.g : ctx.raw;
                        return [item.symbol, `${item.changePercent.toFixed(2)}%`];
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
                        return `${item.symbol}: ${item.changePercent.toFixed(2)}% (Volume: ${item.volume.toLocaleString()})`;
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
