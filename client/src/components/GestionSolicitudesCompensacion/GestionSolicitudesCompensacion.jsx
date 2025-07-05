import { useEffect, useState } from "react";
import axios from "axios";
import { useAuth } from "../../utils/useAuth";
import dayjs from "dayjs";
import "./GestionSolicitudesCompensacion.scss";

export default function GestionSolicitudesCompensacion() {
  const { userRole } = useAuth();
  const [solicitudes, setSolicitudes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const fetchSolicitudes = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get("/api/CompensationRequest");

      // Verificar la estructura de la respuesta y manejar diferentes formatos
      let data = res.data;

      // Si la respuesta es un objeto con una propiedad que contiene el array
      if (data && typeof data === "object" && !Array.isArray(data)) {
        // Casos comunes: { data: [...] }, { items: [...] }, { solicitudes: [...] }
        data =
          data.data || data.items || data.solicitudes || data.results || [];
      }

      // Asegurar que siempre sea un array
      const solicitudesArray = Array.isArray(data) ? data : [];

      setSolicitudes(solicitudesArray);

      // Debug: Mostrar en consola la estructura de la respuesta
      console.log("Respuesta de la API:", res.data);
      console.log("Array procesado:", solicitudesArray);
    } catch (err) {
      console.error("Error al cargar solicitudes:", err);
      setError("Error al cargar las solicitudes.");
      setSolicitudes([]); // Asegurar que siempre sea un array
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSolicitudes();
  }, []);

  const handleDecision = async (id, status) => {
    setError("");
    setSuccess("");
    try {
      const justification =
        status === "Rejected" ? prompt("Motivo de rechazo:") : "";

      // Si el usuario cancela el prompt para rechazo, no continuar
      if (status === "Rejected" && justification === null) {
        return;
      }

      await axios.put(`/api/CompensationRequest/${id}/status`, {
        status,
        justification,
        approvedById: null, // El backend lo puede obtener del token si es necesario
      });

      setSuccess(
        `Solicitud ${
          status === "Approved" ? "aprobada" : "rechazada"
        } correctamente.`
      );
      fetchSolicitudes();
    } catch (err) {
      console.error("Error al actualizar solicitud:", err);
      setError("Error al actualizar la solicitud.");
    }
  };

  // Función para obtener el indicador de estado
  const getStatusIndicator = (status) => {
    const statusClass = status.toLowerCase();
    const statusText = {
      pending: "Pendiente",
      approved: "Aprobada",
      rejected: "Rechazada",
    };

    return (
      <span className={`status-indicator ${statusClass}`}>
        {statusText[statusClass] || status}
      </span>
    );
  };

  return (
    <div className="autorizacion-form gestion-solicitudes-component">
      <div className="component-header">
        <h2>Gestión de Solicitudes de Compensación</h2>
      </div>

      {loading && <p>Cargando solicitudes...</p>}
      {error && <div className="error-msg">{error}</div>}
      {success && <div className="success-msg">{success}</div>}

      {!loading && solicitudes.length === 0 && (
        <p>No hay solicitudes pendientes.</p>
      )}

      {!loading && solicitudes.length > 0 && (
        <table className="solicitudes-table">
          <thead>
            <tr>
              <th>Empleado</th>
              <th>Fecha Trabajada</th>
              <th>Día Solicitado</th>
              <th>Estado</th>
              <th>Justificación</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {solicitudes.map((s) => (
              <tr key={s.id} data-status={s.status}>
                <td data-label="Empleado">
                  {s.employee?.name || s.employeeId}
                </td>
                <td data-label="Fecha Trabajada">
                  {dayjs(s.workDate).format("DD/MM/YYYY")}
                </td>
                <td data-label="Día Solicitado">
                  {dayjs(s.requestedCompensationDate).format("DD/MM/YYYY")}
                </td>
                <td data-label="Estado">{getStatusIndicator(s.status)}</td>
                <td data-label="Justificación">{s.justification || "-"}</td>
                <td data-label="Acciones">
                  {s.status === "Pending" &&
                    (userRole === "manager" || userRole === "superusuario") && (
                      <>
                        <button
                          onClick={() => handleDecision(s.id, "Approved")}
                          title="Aprobar solicitud"
                        >
                          Aprobar
                        </button>
                        <button
                          onClick={() => handleDecision(s.id, "Rejected")}
                          title="Rechazar solicitud"
                        >
                          Rechazar
                        </button>
                      </>
                    )}
                  {s.status !== "Pending" && (
                    <span style={{ color: "#6b7280", fontStyle: "italic" }}>
                      Procesada
                    </span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
