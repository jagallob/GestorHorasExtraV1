import { useEffect, useState } from "react";
import axios from "axios";
import { useAuth } from "../utils/useAuth";
import dayjs from "dayjs";

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
      setSolicitudes(res.data);
    } catch (err) {
      setError("Error al cargar las solicitudes.");
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
      await axios.put(`/api/CompensationRequest/${id}/status`, {
        status,
        justification:
          status === "Rejected" ? prompt("Motivo de rechazo:") : "",
        approvedById: null, // El backend lo puede obtener del token si es necesario
      });
      setSuccess("Solicitud actualizada correctamente.");
      fetchSolicitudes();
    } catch (err) {
      setError("Error al actualizar la solicitud.");
    }
  };

  return (
    <div className="autorizacion-form">
      <h2>Gestión de Solicitudes de Compensación</h2>
      {loading && <p>Cargando...</p>}
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
              <th>Fecha trabajada</th>
              <th>Día solicitado</th>
              <th>Estado</th>
              <th>Justificación</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {solicitudes.map((s) => (
              <tr key={s.id}>
                <td>{s.employee?.name || s.employeeId}</td>
                <td>{dayjs(s.workDate).format("YYYY-MM-DD")}</td>
                <td>
                  {dayjs(s.requestedCompensationDate).format("YYYY-MM-DD")}
                </td>
                <td>{s.status}</td>
                <td>{s.justification || "-"}</td>
                <td>
                  {s.status === "Pending" &&
                    (userRole === "manager" || userRole === "superusuario") && (
                      <>
                        <button
                          onClick={() => handleDecision(s.id, "Approved")}
                        >
                          Aprobar
                        </button>
                        <button
                          onClick={() => handleDecision(s.id, "Rejected")}
                        >
                          Rechazar
                        </button>
                      </>
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
