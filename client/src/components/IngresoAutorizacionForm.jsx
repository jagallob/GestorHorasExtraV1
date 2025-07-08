import { useState } from "react";
import axios from "axios";

const initialState = {
  employeeName: "",
  date: "",
  estimatedEntryTime: "",
  estimatedExitTime: "",
  taskDescription: "",
  managerName: "",
  managerEmail: "",
};

export default function IngresoAutorizacionForm() {
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
      await axios.post("/api/IngresoAutorizacion", form);
      setSuccess("Correo de autorización enviado correctamente.");
      setForm(initialState);
    } catch (err) {
      setError(
        "Error al enviar la autorización. Verifique los datos e intente nuevamente."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="autorizacion-form">
      <h2>Autorizar ingreso en día de descanso</h2>
      <label>
        Nombre del empleado
        <input
          name="employeeName"
          value={form.employeeName}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Fecha de ingreso
        <input
          name="date"
          type="date"
          value={form.date}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Hora estimada de ingreso
        <input
          name="estimatedEntryTime"
          type="time"
          value={form.estimatedEntryTime}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Hora estimada de salida
        <input
          name="estimatedExitTime"
          type="time"
          value={form.estimatedExitTime}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Motivo o descripción de la labor
        <input
          name="taskDescription"
          value={form.taskDescription}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Nombre del manager
        <input
          name="managerName"
          value={form.managerName}
          onChange={handleChange}
          required
        />
      </label>
      <label>
        Email del manager
        <input
          name="managerEmail"
          type="email"
          value={form.managerEmail}
          onChange={handleChange}
          required
        />
      </label>
      <button type="submit" disabled={loading}>
        {loading ? "Enviando..." : "Autorizar Ingreso"}
      </button>
      {success && <div className="success-msg">{success}</div>}
      {error && <div className="error-msg">{error}</div>}
    </form>
  );
}
