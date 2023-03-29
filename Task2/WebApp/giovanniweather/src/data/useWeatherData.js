
import { useState, useEffect } from 'react';
import axios from 'axios';

const useWeatherData = (url) => {
  const [data, setData] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      const response = await axios.get(url);
      setData([]);
      const weatherData = formatWeatherData(response.data);
      setData(weatherData);
    };

    fetchData();
    const interval = setInterval(fetchData, 1000);
    return () => clearInterval(interval);
  }, [url]);

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