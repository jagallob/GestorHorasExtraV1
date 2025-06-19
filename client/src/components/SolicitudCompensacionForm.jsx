import { useState } from "react";
import axios from "axios";
import { useAuth } from "../utils/useAuth";

const initialState = {
  workDate: "",
  requestedCompensationDate: "",
  justification: "",
};

export default function SolicitudCompensacionForm() {
  const { getEmployeeIdFromToken } = useAuth();
  const employeeId = getEmployeeIdFromToken();
  const [form, setForm] = useState(initialState);
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [error, setError] = useState("");

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSuccess("");
    setError("");
    try {
      await axios.post("/api/CompensationRequest", {
        employeeId,
        workDate: form.workDate,
        requestedCompensationDate: form.requestedCompensationDate,
        justification: form.justification,
      });
      setSuccess("Solicitud enviada correctamente.");
      setForm(initialState);
    } catch (err) {
      setError(
        "Error al enviar la solicitud. Verifique los datos e intente nuevamente."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="autorizacion-form">
      <h2>Solicitar día de compensación</h2>
      <label>
        Fecha trabajada (domingo/festivo)
        <input
          name="workDate"
          type="date"
          value={form.workDate}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Día solicitado como compensación
        <input
          name="requestedCompensationDate"
          type="date"
          value={form.requestedCompensationDate}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Justificación o comentario (opcional)
        <input
          name="justification"
          value={form.justification}
          onChange={handleChange}
        />
      </label>
      <button type="submit" disabled={loading || !employeeId}>
        {loading ? "Enviando..." : "Solicitar Día de Compensación"}
      </button>
      {success && <div className="success-msg">{success}</div>}
      {error && <div className="error-msg">{error}</div>}
      {!employeeId && (
        <div className="error-msg">
          No se pudo identificar al usuario autenticado.
        </div>
      )}
    </form>
  );
}
