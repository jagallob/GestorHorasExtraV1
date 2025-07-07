export const API_CONFIG = {
  BASE_URL: (() => {
    // En producción (Vercel)
    if (import.meta.env.PROD) {
      return (
        import.meta.env.VITE_API_BASE_URL ||
        "https://extrahours-api.onrender.com"
      );
    }

    // En desarrollo (Docker local o npm run dev)
    return import.meta.env.VITE_API_BASE_URL_LOCAL || "http://localhost:7086";
  })(),
};
