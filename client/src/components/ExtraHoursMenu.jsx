import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../utils/useAuth";
import ChangePasswordModal from "../components/ChangePasswordModal/ChangePasswordModal";
import "./ExtraHoursMenu.scss";
import {
  Plus,
  Settings,
  BarChart3,
  FileText,
  Users,
  Shield,
} from "lucide-react";

const ExtraHoursMenu = () => {
  const navigate = useNavigate();
  const { auth } = useAuth();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [hoveredItem, setHoveredItem] = useState(null);

  const closeModal = () => setIsModalOpen(false);

  const menuItems = {
    empleado: [
      {
        id: "register",
        title: "Registrar Horas Extra",
        subtitle: "Registro de tiempo adicional trabajado",
        icon: Plus,
        path: "/add",
        color: "blue",
        delay: "0ms",
      },
      {
        id: "compensacion",
        title: "Solicitar Día de Compensación",
        subtitle: "Solicita tu día compensatorio",
        icon: FileText,
        path: "/solicitud-compensacion",
        color: "indigo",
        delay: "50ms",
      },
      {
        id: "reports",
        title: "Mis Informes",
        subtitle: "Consulta y seguimiento personal",
        icon: BarChart3,
        path: "/reports",
        color: "slate",
        delay: "100ms",
      },
    ],
    manager: [
      {
        id: "autorizacion",
        title: "Autorizar Ingreso Día Descanso",
        subtitle: "Envía autorización a Seguridad y Talento Humano",
        icon: Shield,
        path: "/autorizacion-ingreso",
        color: "blue",
        delay: "0ms",
      },
      {
        id: "gestion-compensacion",
        title: "Gestión de Compensaciones",
        subtitle: "Aprueba o rechaza solicitudes de compensación",
        icon: Users,
        path: "/gestion-compensacion",
        color: "indigo",
        delay: "50ms",
      },
      {
        id: "management",
        title: "Gestión de Horas Extra",
        subtitle: "Aprobar, editar y eliminar registros",
        icon: Users,
        path: "/ManagementExtraHour",
        color: "indigo",
        delay: "100ms",
      },
      {
        id: "reports",
        title: "Informes de Equipo",
        subtitle: "Análisis y métricas del equipo",
        icon: BarChart3,
        path: "/reports",
        color: "slate",
        delay: "0ms",
      },
    ],
    superusuario: [
      {
        id: "register",
        title: "Registrar Horas Extra",
        subtitle: "Crear nuevos registros",
        icon: Plus,
        path: "/add",
        color: "blue",
        delay: "0ms",
      },
      {
        id: "autorizacion",
        title: "Autorizar Ingreso Día Descanso",
        subtitle: "Envía autorización a Seguridad y Talento Humano",
        icon: Shield,
        path: "/autorizacion-ingreso",
        color: "blue",
        delay: "50ms",
      },
      {
        id: "gestion-compensacion",
        title: "Gestión de Compensaciones",
        subtitle: "Aprueba o rechaza solicitudes de compensación",
        icon: Users,
        path: "/gestion-compensacion",
        color: "indigo",
        delay: "100ms",
      },
      {
        id: "management",
        title: "Gestión de Horas Extra",
        subtitle: "Aprobar, editar y eliminar registros",
        icon: Shield,
        path: "/ManagementExtraHour",
        color: "indigo",
        delay: "150ms",
      },
      {
        id: "reports",
        title: "Reportes Generales",
        subtitle: "Análisis y métricas del sistema",
        icon: FileText,
        path: "/reports",
        color: "slate",
        delay: "200ms",
      },
      {
        id: "settings",
        title: "Configuración",
        subtitle: "Parámetros del sistema y Gestión de Usuarios",
        icon: Settings,
        path: "/settings",
        color: "gray",
        delay: "300ms",
      },
    ],
  };

  const currentMenuItems = menuItems[auth?.role] || [];

  return (
    <div className="extra-hours-menu-container">
      <main className="dashboard-content">
        <div className="dashboard-header">
          <div className="menu-title-card">
            <h2 className="menu-title">Panel de Control</h2>
          </div>
        </div>

        <div className="menu-grid">
          {currentMenuItems.map((item) => {
            const IconComponent = item.icon;
            return (
              <div
                key={item.id}
                className={`menu-card ${item.color} ${
                  hoveredItem === item.id ? "hovered" : ""
                }`}
                onClick={() => navigate(item.path)}
                onMouseEnter={() => setHoveredItem(item.id)}
                onMouseLeave={() => setHoveredItem(null)}
                style={{
                  animationDelay: item.delay,
                }}
              >
                <div className="card-content">
                  <div className="icon-section">
                    <div className="icon-container">
                      <IconComponent size={24} className="menu-icon" />
                    </div>
                  </div>
                  <div className="text-section">
                    <h3 className="card-title">{item.title}</h3>
                    <p className="card-subtitle">{item.subtitle}</p>
                  </div>
                  <div className="card-arrow">
                    <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
                      <path
                        d="M7.5 15L12.5 10L7.5 5"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </main>

      {isModalOpen && <ChangePasswordModal onClose={closeModal} />}
    </div>
  );
};

export default ExtraHoursMenu;
