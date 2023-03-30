
import { useState, useEffect } from 'react';
import axios from 'axios';

const useWeatherData = (url, chartType, country, city) => {
    const [data, setData] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            if (chartType === 'bar') {
                const response = await axios.get(url + '/GetWeatherForecast');
                const weatherData = formatWeatherData(response.data);

                setData(weatherData);
            }
            else {
                const utcNow = new Date();
                const twoHoursAgo = new Date(utcNow.getTime() - 2 * 60 * 60 * 1000);

                const response = await axios.get(url + '/GetWeatherByTimeRange', {
                    params: {
                        country: country,
                        city: city,
                        from: twoHoursAgo.toISOString(),
                        to: utcNow.toISOString(),
                    }
                });

                setData(response.data);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 60 * 1000);
        return () => clearInterval(interval);
    }, [url, chartType, country, city]);

    const formatWeatherData = (jsonData) => {
        const groupedData = jsonData.reduce((acc, country) => {
            country.cities.forEach((city) => {
                const weather = city.weatherDescriptions[0];
                const cityData = {
                    cityName: city.cityName,
                    countryName: country.countryName,
                    temperature: weather.temperature,
                    wind: weather.wind,
                    lastUpdateDateTime: weather.lastUpdateDateTime,
                };

                acc.push(cityData);
            });
            return acc;
        }, []);

        return groupedData;
    };

    return data;
};

export default useWeatherData;