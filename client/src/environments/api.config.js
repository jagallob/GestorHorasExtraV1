export const API_CONFIG = {
  BASE_URL:
    (import.meta.env.VITE_API_USE_LOCAL === "true"
      ? import.meta.env.VITE_API_BASE_URL_LOCAL
      : import.meta.env.VITE_API_BASE_URL) || "http://localhost:7086",
};
