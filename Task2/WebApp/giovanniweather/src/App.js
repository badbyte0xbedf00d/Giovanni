import React, { useState } from 'react';
import { ResponsiveBar } from '@nivo/bar';
import { ResponsiveLine } from '@nivo/line';
import { scaleLinear } from 'd3-scale';
import { interpolateBlues, interpolateOranges } from 'd3-scale-chromatic';
import useWeatherData from './data/useWeatherData';
import './App.css';

const App = () => {
  const [chartType, setChartType] = useState('bar');
  const [cityName, setCityName] = useState('');
  const [countryName, setCountryName] = useState('');

  const data = useWeatherData('https://localhost:7072/WeatherForecast', chartType, countryName, cityName);

  const lineChartData = () => {
    return {
      data: [
        {
          id: 'wind',
          data: data.map((weatherData) => ({
            x: `${new Date(weatherData.avgUpdateDateTime).toLocaleString()}`,
            y: Math.round(weatherData.avgWind * 10) / 10,
          })),
        },
        {
          id: 'temperature',
          data: data.map((weatherData) => ({
            x: `${new Date(weatherData.avgUpdateDateTime).toLocaleString()}`,
            y: Math.round(weatherData.avgTemperature * 10) / 10,
          })),
        },
      ],
    };
  };

  const toggleChartType = () => {
    setChartType(chartType === 'bar' ? 'line' : 'bar');
  };

  const getLatestUpdateTime = () => {
    if (chartType === 'bar') {
      const latestUpdate = data.reduce((latest, cityData) => {
        const cityUpdate = new Date(cityData.lastUpdateDateTime);
        return latest < cityUpdate ? cityUpdate : latest;
      }, new Date(0));
      return latestUpdate.toLocaleString();
    }
    return new Date(data[data.length - 1].avgUpdateDateTime).toLocaleString();
  };

  const temperatureScale = scaleLinear()
    .domain([-20, 40])
    .range([interpolateBlues(0.8), interpolateOranges(0.8)]);

  return (
    <div style={{ height: '600px', width: '100%' }}>
      {chartType === 'bar' ? (
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
            setCityName(data.data.cityName);
            setCountryName(data.data.countryName);
            toggleChartType();
          }}
        />
      ) : (

        <div style={{width: '100%' , height: '700px'}}>
          <div style={{ textAlign: 'center', marginTop: '10px' }}>
            Location: {cityName}, {countryName}
          </div>
          <ResponsiveLine
            data={lineChartData().data}
            isInteractive={true}
            useMesh={true}
            margin={{ top: 50, right: 130, bottom: 150, left: 60 }}
            xScale={{ type: 'point' }}
            yScale={{
              type: 'linear',
              min: 'auto',
              max: 'auto',
              stacked: false,
              reverse: false,
            }}
            axisTop={null}
            axisRight={null}
            axisBottom={{
              orient: 'bottom',
              tickSize: 5,
              tickPadding: 5,
              tickRotation: -45,
              legend: 'Update time',
              legendOffset: 100,
              legendPosition: 'middle',
            }}
            axisLeft={{
              orient: 'left',
              tickSize: 5,
              tickPadding: 5,
              tickRotation: 0,
              legend: 'Value',
              legendOffset: -40,
              legendPosition: 'middle',
            }}
            colors={({ id, value }) =>
              id === 'temperature' ? 'orange' : 'steelblue'
            }
            pointSize={10}
            pointColor={{ theme: 'background' }}
            pointBorderWidth={2}
            pointBorderColor={{ from: 'serieColor' }}
            pointLabel="y"
            pointLabelYOffset={-12}
            enablePointLabel={true}
            enableArea={false}
            animate={true}
            motionStiffness={90}
            motionDamping={15}
            onClick={(point, event) => {
              toggleChartType();
            }}
            tooltip={({ point }) => {
              const { serieId, data } = point;
              const value = serieId === "temperature" ? data.y : data.y;
          
              return (
                <div
                  style={{
                    background: "rgba(0, 0, 0, 0.8)",
                    color: "#fff",
                    border: "1px solid #000",
                    padding: "10px",
                    borderRadius: "5px",
                  }}
                >
                  <strong>{serieId === "temperature" ? "Temperature" : "Wind"}</strong>
                  <div>
                    {value}
                    {serieId === "temperature" ? "°C" : " m/s"}
                  </div>
                  <div>Updated: {data.x}</div>
                </div>
              );
            }}
          />

        </div>
      )}

      <div style={{ textAlign: 'center', marginTop: '10px' }}>
        Last updated: {getLatestUpdateTime()}
      </div>
    </div>
  );
}

export default App;