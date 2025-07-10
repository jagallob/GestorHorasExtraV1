import { useEffect, useState, useCallback } from "react";
import PropTypes from "prop-types";
import axios from "axios";
import ConfigContext from "./ConfigContext";
import { API_CONFIG } from "../environments/api.config";

export const ConfigProvider = ({ children }) => {
  const [config, setConfig] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [retryCount, setRetryCount] = useState(0);
  const MAX_RETRIES = 2;

  // Permite recargar la configuración manualmente
  const fetchConfig = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        throw new Error(
          "No se encontró el token de autenticación. Por favor, inicia sesión nuevamente."
        );
      }
      const response = await axios.get(`${API_CONFIG.BASE_URL}/api/config`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        timeout: 7000,
      });
      const data = response.data;
      const transformedData = {
        weeklyExtraHoursLimit: data.weeklyExtraHoursLimit,
        // diurnalMultiplier: data.diurnalMultiplier,
        // nocturnalMultiplier: data.nocturnalMultiplier,
        // diurnalHolidayMultiplier: data.diurnalHolidayMultiplier,
        // nocturnalHolidayMultiplier: data.nocturnalHolidayMultiplier,
        diurnalStart: data.diurnalStart,
        diurnalEnd: data.diurnalEnd,
        id: data.id,
      };
      setConfig(transformedData);
      setError(null);
    } catch (err) {
      let msg = "Error cargando configuración.";
      if (err.code === "ECONNABORTED") {
        msg = "La solicitud está tardando demasiado. Intenta de nuevo.";
      } else if (err.message && err.message.includes("Network Error")) {
        msg =
          "No se pudo conectar con el servidor. Verifica tu conexión o CORS.";
      } else if (err.response && err.response.status === 401) {
        msg = "No autorizado. Por favor, inicia sesión nuevamente.";
      } else if (err.response && err.response.status === 403) {
        msg = "Acceso denegado. No tienes permisos suficientes.";
      } else if (err.response && err.response.status === 500) {
        msg = "Error interno del servidor. Intenta más tarde.";
      } else if (err.message) {
        msg = err.message;
      }
      setError(msg);
      setConfig(null);
      // Reintento automático limitado
      if (retryCount < MAX_RETRIES) {
        setTimeout(() => setRetryCount((c) => c + 1), 1500);
      }
    } finally {
      setIsLoading(false);
    }
  }, [retryCount]);

  // Carga inicial y recarga cuando cambia el token o retryCount
  useEffect(() => {
    fetchConfig();
    // También puedes escuchar eventos de login/logout para recargar
    // window.addEventListener('storage', fetchConfig);
    // return () => window.removeEventListener('storage', fetchConfig);
  }, [fetchConfig]);

  // Resetear retryCount cuando se reintenta manualmente
  const handleRetry = () => {
    setRetryCount(0);
    fetchConfig();
  };

  if (isLoading) {
    return <div>Loading configuration...</div>;
  }
  if (error) {
    return (
      <div style={{ color: "red", padding: 16 }}>
        <b>{error}</b>
        <br />
        <button onClick={handleRetry} style={{ marginTop: 8 }}>
          Reintentar
        </button>
      </div>
    );
  }
  return (
    <ConfigContext.Provider value={{ config, setConfig, fetchConfig }}>
      {children}
    </ConfigContext.Provider>
  );
};

ConfigProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

ConfigProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
