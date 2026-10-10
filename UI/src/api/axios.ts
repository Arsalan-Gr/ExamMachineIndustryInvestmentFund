import axios from "axios";
import { authStorage } from "../utils/authStorage";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use((config) => {
  const token = authStorage.getToken();

  if (token) {
    config.headers.set("Authorization", `Bearer ${token}`);
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401 && authStorage.getToken()) {
      authStorage.clear();
    }

    return Promise.reject(error);
  },
);

export default api;
