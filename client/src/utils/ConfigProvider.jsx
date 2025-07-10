
import { useEffect, useState, useCallback } from "react";
import PropTypes from "prop-types";
import axios from "axios";
import ConfigContext from "./ConfigContext";
import { API_CONFIG } from "../environments/api.config";

export const ConfigProvider = ({ children }) => {
  const [config, setConfig] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  // Permite recargar la configuración manualmente
  const fetchConfig = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        throw new Error("No token found in localStorage.");
      }
      const response = await axios.get(`${API_CONFIG.BASE_URL}/api/config`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
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
    } catch (err) {
      setError(err.message || "Error fetching configuration");
      setConfig(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Carga inicial y recarga cuando cambia el token
  useEffect(() => {
    fetchConfig();
    // También puedes escuchar eventos de login/logout para recargar
    // window.addEventListener('storage', fetchConfig);
    // return () => window.removeEventListener('storage', fetchConfig);
  }, [fetchConfig]);

  if (isLoading) {
    return <div>Loading configuration...</div>;
  }
  if (error) {
    return <div>Error cargando configuración: {error} <button onClick={fetchConfig}>Reintentar</button></div>;
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
};

ConfigProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
