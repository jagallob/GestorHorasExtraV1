# 🧾 Procedimiento: Ingreso laboral en días de descanso (Domingos/Festivos) y solicitud de día de compensación

## 🧑‍🤝‍🧑 Roles involucrados

### 👷 Empleado

Persona que debe asistir a laborar durante un domingo o festivo por requerimiento de sus superiores.

### 👨‍💼 Manager

Superiores jerárquicos del colaborador, quienes realizan la solicitud formal de ingreso y aprueban o gestionan la compensación. Tiene autoridad para aprobar o negar la solicitud del día de compensación, según criterios operativos.

### 🧑‍✈️ Superusuario

Tiene acceso a todas las funcionalidades.

---

## 📌 Etapas del Procedimiento

### 1️⃣ Solicitud previa para ingreso en día de descanso

**Responsables:** Manager

- Los manager deben enviar un correo electrónico a:
  - Seguridad física de empresa o instalaciones
  - Monitoreo
  - **Con copia (CC)** a la Coordinadora de Talento Humano

#### 📧 El correo debe incluir:

- Nombre completo del empleado que va a ingresar
- Fecha del ingreso (ejemplo: "domingo 25 de junio")
- Hora estimada de ingreso y salida
- Motivo o descripción de la labor a realizar

#### 🎯 Propósito:

- Garantizar el ingreso autorizado del empleado a las instalaciones en día de descanso.
- Dejar evidencia formal de que se trata de una actividad **autorizada oficialmente**.

---

### 2️⃣ Ingreso del empleado

**Responsables:** Seguridad y Monitoreo

- Verifican la existencia del correo de autorización enviado previamente.
- Si no hay autorización oficial, **NO se permite el ingreso**.

**Responsables:** Colaborador

- Asiste a las instalaciones y cumple con la jornada laboral en el horario previamente establecido.
- Realiza las tareas asignadas que justificaron su asistencia.

---

### 3️⃣ Solicitud del día de compensación

**Responsables:** Empleado

- Una vez realizado el trabajo en día de descanso, el colaborador debe enviar un **correo electrónico** a su manager,  
  con **copia (CC)** a la Coordinadora de Talento Humano.

#### 📧 El correo debe incluir:

- La fecha del día trabajado (ejemplo: “domingo 25 de junio”)
- El día solicitado como compensación (ejemplo: “lunes 1 de julio”)
- Una petición clara, por ejemplo:
  > "Solicito como día de compensación el lunes 1 de julio por el trabajo realizado el domingo 25 de junio."

---

### 4️⃣ Revisión y aprobación de la compensación

**Responsables:** Manager

- Evalúan la solicitud del colaborador.
  - Programación interna
  - Eventos
  - Carga operativa del área

**Responsables:** Manager - Superusuario

- Aprueba o niega la solicitud según disponibilidad y necesidades del servicio.

---

### 5️⃣ Respuesta formal

**Responsables:** Manager

- Responden al correo del colaborador con la decisión,  
  **con copia (CC)** a la Coordinadora de Talento Humano.

#### 📢 En la respuesta se debe indicar:

- ✅ **Aprobación** del día compensatorio
- ❌ **Negación**, explicando el motivo (ejemplo: “Ese día hay evento programado”)

---

## 📍 Observaciones importantes

- Si **no se realiza el paso 1** (correo de autorización previo), **el colaborador NO podrá ingresar** a las instalaciones en domingo o festivo.
- Si **el día solicitado para compensación no es viable**, el colaborador sigue teniendo derecho a su día de descanso,  
  pero deberá **reprogramarlo** según disponibilidad del área.
- Todos los pasos deben dejar trazabilidad mediante **correo electrónico**,  
  lo cual permite verificación y gestión adecuada por parte de Talento Humano.

---

# 🧾 Procedimiento de Implantación de Nuevas Funcionalidades

## 🎯 Objetivo

Adaptar el sistema actual de gestión de horas extra para permitir:

1. La **autorización formal del ingreso de colaboradores** en días de descanso (domingos/festivos), enviada por los managers vía correo electrónico a Seguridad y Talento Humano.
2. La **solicitud, aprobación y trazabilidad de días compensatorios** por parte de los empleados, managers y el superusuario.

---

## 🧑‍💻 Etapas del Desarrollo e Implantación

### 1. **Análisis y diseño funcional**

- Revisar los requerimientos de negocio definidos en el procedimiento institucional.
- Determinar qué roles realizarán qué acciones: manager, empleado, superusuario.
- Validar que el modelo de datos actual permite la extensión sin conflictos.
- Definir qué vistas o formularios nuevos se deben crear.

---

### 2. **Extensión del modelo de datos**

- Crear una nueva entidad para representar las solicitudes de días compensatorios.
- Evaluar si es necesario registrar las autorizaciones de ingreso también como entidades o solo como eventos auditables.
- Actualizar el esquema de la base de datos para incluir las nuevas tablas (mediante migraciones).

---

### 3. **Creación de servicios en backend**

- Crear controladores para gestionar:
  - El envío de correos de autorización de ingreso.
  - El CRUD de solicitudes compensatorias.
- Implementar servicios para:
  - Validar, registrar y procesar la solicitud de compensación.
  - Aprobar o rechazar solicitudes según los roles asignados.
  - Enviar correos con las autorizaciones desde el backend.
- Garantizar trazabilidad con timestamps y campos como "aprobado por", "fecha de decisión", etc.

---

### 4. **Integración con autenticación y roles**

- Asegurar que las nuevas rutas estén protegidas mediante `roles`:
  - Solo los **managers** pueden enviar autorizaciones de ingreso.
  - Solo los **empleados** pueden enviar solicitudes de compensación.
  - Solo **managers** y el **superusuario** pueden aprobar o rechazar las solicitudes.
- Reutilizar el sistema de autenticación basado en JWT existente.

---

### 5. **Actualización del frontend (React)**

- Crear un formulario para que los managers autoricen ingresos por correo.
- Crear un formulario para que los colaboradores soliciten días compensatorios.
- Crear una vista para que los jefes y gerentes vean y gestionen las solicitudes pendientes.
- Mostrar alertas visuales de éxito, error o decisiones tomadas.
- Aplicar validaciones en los formularios para garantizar la integridad de los datos.

---

### 6. **Integración con el sistema de correo**

- Configurar correctamente el servicio SMTP (si no está hecho).
- Utilizar un formato HTML en el correo con los datos claves del colaborador, fecha y motivo del ingreso.
- Enviar correos a Seguridad y Monitoreo, con copia a Talento Humano, desde el backend.
- Validar que los correos lleguen correctamente y mantengan formato y trazabilidad.

---

### 7. **Validación funcional y pruebas**

- Realizar pruebas unitarias para servicios nuevos.
- Ejecutar pruebas de flujo end-to-end para verificar:
  - Que los managers pueden enviar autorizaciones correctamente.
  - Que los colaboradores pueden hacer solicitudes.
  - Que las solicitudes se registran y cambian de estado correctamente.
  - Que los jefes y el gerente pueden aprobar o rechazar con justificación.
- Verificar restricciones de acceso según el rol.

---

### 8. **Despliegue**

- Integrar los cambios en la rama principal del proyecto.
- Generar nuevas migraciones y aplicarlas a la base de datos de producción.
- Desplegar el backend y frontend actualizados.
- Validar los flujos en el entorno real.

---

### 9. **Capacitación y documentación**

- Documentar el uso de las nuevas funcionalidades.
- Crear manuales rápidos o videos para:
  - Managers que autorizan ingresos.
  - Colaboradores que solicitan compensación.
  - Jefes directos que aprueban o rechazan solicitudes.
- Capacitar a los usuarios clave del sistema.

---

### 10. **Mantenimiento y seguimiento**

- Monitorear el funcionamiento del sistema tras la puesta en marcha.
- Recoger feedback de los usuarios.
- Corregir posibles errores o ajustar flujos según observaciones.
- Evaluar si se requiere un módulo de auditoría para mejorar la trazabilidad.

---

## ✅ Resultados Esperados

- Flujo digitalizado y trazable de autorizaciones y compensaciones.
- Aumento en la formalización y transparencia de los ingresos en días no laborales.
- Reducción del riesgo de ingresos no autorizados o sin respaldo documental.
- Mejor control por parte de Talento Humano y del Gerente sobre estos procesos.
