import axios from 'axios';
import { clearSession, getSession } from '../auth';
const instance = axios.create({
  baseURL: 'https://secure-webapp-api.azurewebsites.net/api',
});

instance.interceptors.request.use(
  function (config) {
    // Do something before request is sent

    const token = getSession();
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

    return config;
  },
  function (error) {
    // Do something with request error
    return Promise.reject(error);
  }
);

// Add a response interceptor
instance.interceptors.response.use(
  function (response) {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response;
  },
  function (error) {
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error

    if (error.response?.status === 401) {
      clearSession();
      document.location = '/signin';
    }

    if (error.response?.data?.message) {
      return Promise.reject(error.response.data.message);
    }

    return Promise.reject(error);
  }
);

export default instance;
