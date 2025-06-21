export const API_CONFIG = {
  BASE_URL:
    (import.meta.env.VITE_API_USE_LOCAL === "true"
      ? import.meta.env.VITE_API_BASE_URL_LOCAL
      : import.meta.env.VITE_API_BASE_URL) ||
    "https://extrahours-api.onrender.com",
};

// Debugging
console.log("=== API CONFIG DEBUG ===");
console.log("VITE_API_USE_LOCAL:", import.meta.env.VITE_API_USE_LOCAL);
console.log("VITE_API_BASE_URL:", import.meta.env.VITE_API_BASE_URL);
console.log(
  "VITE_API_BASE_URL_LOCAL:",
  import.meta.env.VITE_API_BASE_URL_LOCAL
);
console.log("Final BASE_URL:", API_CONFIG.BASE_URL);
