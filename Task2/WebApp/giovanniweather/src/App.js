import React from 'react';
import { ResponsiveBar } from '@nivo/bar';
import useWeatherData from './data/useWeatherData';
import { scaleLinear } from 'd3-scale';
import { interpolateBlues, interpolateOranges } from 'd3-scale-chromatic';

const App = () => {
  const data = useWeatherData('https://localhost:7072/WeatherForecast');

  const getLatestUpdateTime = () => {
    const latestUpdate = data.reduce((latest, cityData) => {
      const cityUpdate = new Date(cityData.lastUpdateDateTime);
      return latest < cityUpdate ? cityUpdate : latest;
    }, new Date(0));
    return latestUpdate.toLocaleString();
  };

  const temperatureScale = scaleLinear()
    .domain([-20, 40])
    .range([interpolateBlues(0.8), interpolateOranges(0.8)]);

  return (
    <div style={{ height: '600px', width: '100%' }}>
      <ResponsiveBar
        data={data}
        keys={['temperature', 'wind']}
        indexBy="cityName"
        margin={{ top: 50, right: 130, bottom: 150, left: 60 }}
        padding={0.3}
        groupMode="grouped"
        colors={({ id, value }) =>
          id === 'temperature' ? temperatureScale(value) : 'steelblue'
        }
        borderColor={{ from: 'color', modifiers: [['darker', 1.6]] }}
        axisTop={null}
        axisRight={null}
        axisBottom={{
          tickSize: 5,
          tickPadding: 5,
          tickRotation: -45,
          legend: 'City',
          legendPosition: 'middle',
          legendOffset: 100,
          format: (value, context) => {
            const cityData = data.find((d) => d.cityName === value);
            return `${cityData.cityName}, ${cityData.countryName}`;
          },
        }}
        axisLeft={{
          tickSize: 5,
          tickPadding: 5,
          tickRotation: 0,
          legend: 'Value',
          legendOffset: -40,
          legendPosition: 'middle',
        }}
        labelSkipWidth={15}
        labelSkipHeight={15}
        animate={true}
        motionStiffness={90}
        motionDamping={15}
        tooltip={({ id, value, data, color }) => (
          <div
            style={{
              background: 'rgba(0, 0, 0, 0.8)',
              color: '#fff',
              border: '1px solid #000',
              padding: '10px',
              borderRadius: '5px',
            }}
          >
            <strong style={{ color }}>{data.cityName}, {data.countryName}</strong>
            <div>{id}: {value}{id === 'temperature' ? '°C' : ' m/s'}</div>
          </div>
        )}
        onClick={(data, event) => {
          console.log('Clicked bar data:', data);
        }}
      />
      <div style={{ textAlign: 'center', marginTop: '10px' }}>
        Last updated: {getLatestUpdateTime()}
      </div>
    </div>
  );
}

export default App;