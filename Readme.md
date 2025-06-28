# Proyecto de Gestión de Horas Extra

## Tabla de Contenidos

1. [Introducción](#proyecto-de-gestion-de-horas-extra)
2. [Dockerización y despliegue rápido](#dockerización-y-despliegue-rápido)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Requisitos Previos](#requisitos-previos)
5. [Instalación y Configuración Manual](#instalacion-y-configuracion)
   - [Frontend (React)](#frontend-react)
   - [Backend (C#)](#backend-c)
6. [Funcionalidades Principales](#funcionalidades-principales)
   - [Sistema de Horas Extra](#sistema-de-horas-extra)
   - [Cálculo automático categorizado](#calculo-automatico-categorizado)
   - [Sistema de aprobación](#sistema-de-aprobacion)
   - [Gestión y Mantenimiento de Registros](#gestion-y-mantenimiento-de-registros)
   - [Interfaz adaptada por roles](#interfaz-adaptada-por-roles)
   - [Control de Límites y Alertas](#control-de-limites-y-alertas)
   - [Calendario y Gestión de Días Festivos](#calendario-y-gestion-de-dias-festivos)
   - [Cálculo avanzado de horas extra](#calculo-avanzado-de-horas-extra)
   - [Reportes y análisis](#reportes-y-analisis)
   - [Autenticación y seguridad](#autenticacion-y-seguridad)
   - [Gestión de Empleados](#gestion-de-empleados)
   - [Gestión de Managers](#gestion-de-managers)
   - [Panel de Configuración del Sistema](#panel-de-configuracion-del-sistema)
   - [Menú Principal Adaptativo](#menu-principal-adaptativo)
7. [Detalles Técnicos](#detalles-tecnicos)
   - [Frontend](#frontend)
   - [Backend](#backend)
8. [Generación de Migraciones](#generacion-de-migraciones)
9. [Despliegue](#despliegue)
   - [Frontend](#frontend-1)
   - [Backend](#backend-1)
10. [Estructura de Directorios Recomendada](#estructura-de-directorios-recomendada)
11. [Flujo de Trabajo](#flujo-de-trabajo)
12. [Pruebas Unitarias](#pruebas-unitarias)
13. [Solución de Problemas Comunes](#solucion-de-problemas-comunes)
    - [Errores de migración de base de datos](#errores-de-migracion-de-base-de-datos)
    - [Problemas con el frontend](#problemas-con-el-frontend)
    - [Problemas con la autenticación](#problemas-con-la-autenticacion)
14. [Contribución](#contribucion)

---

> **Nota:** Este proyecto puede ejecutarse de dos formas:
>
> - **Dockerización y despliegue rápido:** Recomendado para desarrollo y producción, levanta todo el stack con un solo comando.
> - **Instalación y configuración manual:** Si prefieres instalar dependencias y servicios por separado, sigue la sección correspondiente.

## Dockerización y despliegue rápido

Este proyecto está completamente dockerizado. Puedes levantar el frontend, backend y base de datos con un solo comando usando Docker Compose.

### ¿Qué incluye la configuración Docker?

1. **Dockerización completa:**

   - `Dockerfile` para backend (.NET) y frontend (Vite + React).
   - `docker-compose.yml` para orquestar backend, frontend y PostgreSQL.

2. **Variables de entorno:**

   - Deben ser centralizadas en `.env` para base de datos, JWT y correo.
   - El backend toma la cadena de conexión y configuraciones sensibles desde variables de entorno.

3. **Base de datos y datos iniciales:**

   - Servicio PostgreSQL en Docker.
   - Backup SQL en `db-backup/backup.sql` para poblar la base de datos automáticamente al iniciar el contenedor.
   - Si la base de datos no existe, créala y restaura el backup manualmente con:
     ```bash
     docker exec -i gestorhorasextrav1-db-1 psql -U postgres -c "CREATE DATABASE extrahours;"
     docker exec -i gestorhorasextrav1-db-1 psql -U postgres -d extrahours < db-backup/backup.sql
     ```

4. **Frontend:**
   - Configuración para que la URL de la API se tome de variables de entorno y use HTTP en desarrollo.
   - Sin referencias fijas a URLs en el código fuente.

### ¿Cómo levantar el proyecto?

#### 1. Clona el repositorio y sitúate en la raíz del proyecto

```bash
git clone https://github.com/jagallob/GestorHorasExtraV1.git
cd GestorHorasExtraV1
```

#### 2. Configura las variables de entorno

## Crea y edita el archivo .env con tus valores específicos

**Variables que debes configurar en `.env`:**

```bash
# Configuración de Base de Datos
DB_HOST=db
DB_PORT=5432
DB_NAME=extrahours
DB_USER=postgres
DB_PASSWORD=tu_password_aqui          # ⚠️ CAMBIAR

# Configuración JWT
JWT_SECRET=tu_clave_secreta_jwt_aqui   # ⚠️ CAMBIAR (mínimo 32 caracteres)
JWT_ISSUER=ExtraHours.API
JWT_AUDIENCE=ExtraHours.Client
JWT_EXPIRES_IN_MINUTES=60

# Configuración del entorno
ASPNETCORE_ENVIRONMENT=Development
```

> **⚠️ Importante:**
>
> - Cambia `DB_PASSWORD` por una contraseña segura
> - Genera una `JWT_SECRET` única y segura (puedes usar: `openssl rand -base64 32`)
> - **Nunca compartas tu archivo `.env`** - contiene información sensible

#### 3. Levanta los servicios

```bash
docker-compose up --build
```

#### 4. Accede a la aplicación

Una vez que todos los contenedores estén ejecutándose:

- **Frontend:** http://localhost:5173
- **Backend API:** http://localhost:7086
- **Base de datos PostgreSQL:** localhost:5432

> **Nota:**
> - Si usas Docker Compose, tanto el frontend como el backend estarán disponibles en tu máquina local en los puertos indicados.
> - Si despliegas el backend en la nube (por ejemplo, en Render), la URL será la pública de Render (https://extrahours-api.onrender.com), pero para desarrollo local con Docker, usa la URL local.

### Comandos útiles

```bash
# Levantar en segundo plano
docker-compose up -d --build

# Ver logs de todos los servicios
docker-compose logs

# Ver logs de un servicio específico
docker-compose logs backend
docker-compose logs frontend
docker-compose logs db

# Parar todos los servicios
docker-compose down

# Parar y eliminar volúmenes (⚠️ elimina datos de BD)
docker-compose down -v

# Ejecutar tests del backend
docker-compose run test-backend
```

### Solución de problemas comunes

**Error: "no configuration file provided"**

- Verifica que estés en la raíz del proyecto donde está `docker-compose.yml`

**Error de conexión a base de datos:**

- Verifica que tu archivo `.env` tenga las variables correctas
- Asegúrate de que el puerto 5432 no esté ocupado por otra instancia de PostgreSQL

**El frontend no puede conectar con el backend:**

- Verifica que todos los contenedores estén ejecutándose: `docker-compose ps`
- Revisa los logs: `docker-compose logs backend`

## Estructura del Proyecto

El proyecto está dividido en dos partes principales:

- **Frontend**: Aplicación React
- **Backend**: API en C# con Entity Framework Core

## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- [Node.js](https://nodejs.org/) (versión recomendada: 18.x o superior)
- [npm](https://www.npmjs.com/) o [yarn](https://yarnpkg.com/)
- [.NET SDK](https://dotnet.microsoft.com/download) (versión 7.0 o superior)
- [PostgreSQL](https://www.postgresql.org/) (versión 14 o superior)

## Instalación y Configuración

### Frontend (React)

1. Navega al directorio del frontend:

   ```bash
   cd ExtraHours.Net/client
   ```

2. Instala las dependencias:
   ```bash
   npm install
   ```
3. Principales dependencias:

   - `antd` (v5.19.4): Biblioteca de componentes UI
   - `axios` (v1.7.4): Cliente HTTP para realizar peticiones a la API
   - `exceljs` (v4.4.0): Generación de reportes en Excel
   - `jwt-decode` (v4.0.0): Decodificación de tokens JWT
   - `react-router-dom` (v6.26.0): Enrutamiento en React
   - `recharts` (v2.15.1): Biblioteca de gráficos para React
   - `dayjs`: Manipulación de fechas y horas
   - `prop-types`: Validación de propiedades de componentes

4. Dependencias de desarrollo:

   - Vite (v6.2.3): Herramienta de compilación
   - ESLint: Linting de código
   - SASS: Preprocesador CSS

5. Inicia el servidor de desarrollo:
   ```bash
   npm run dev
   ```

### Backend (C#)

1. Navega al directorio del backend:

   ```bash
   cd ExtraHours.Net/ExtraHours.API
   ```

2. Restaura los paquetes NuGet:

   ```bash
   dotnet restore
   ```

3. Principales paquetes:

   - `Microsoft.EntityFrameworkCore`: ORM para interactuar con la base de datos
   - `Microsoft.EntityFrameworkCore.Design`: Herramientas de diseño para EF Core
   - `Npgsql.EntityFrameworkCore.PostgreSQL`: Proveedor de PostgreSQL para EF Core
   - `BCrypt.Net-Next`: Para el cifrado seguro de contraseñas
   - `Microsoft.AspNetCore.Authentication.JwtBearer`: Para la autenticación mediante tokens JWT

4. Configura la cadena de conexión a PostgreSQL en `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=HorasExtra;Username=TuUsuario;Password=TuContraseña;"
     },
     "JwtSettings": {
       "Secret": "TuClaveSecretaLargaYCompleja",
       "TokenExpiryMinutes": 60,
       "RefreshTokenExpiryDays": 7
     }
   }
   ```

5. Ejecuta las migraciones para crear la base de datos:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

6. Inicia el servidor de desarrollo:
   ```bash
   dotnet run
   ```

## Funcionalidades Principales

### Sistema de Horas Extra

El sistema permite:

1. **Registro de horas extra**:

   - Los empleados pueden registrar sus horas extra especificando fecha, hora de inicio y hora de fin.
   - Los superusuarios pueden registrar horas extra en nombre de cualquier empleado.
   - Campo de observaciones para notas adicionales sobre el registro.

2. **Cálculo automático categorizado**:

   - Cálculo automático basado en la configuración del sistema.
   - Clasificación automática de horas en diferentes categorías:
     - Horas diurnas (día típico laboral)
     - Horas nocturnas (día típico laboral)
     - Horas diurnas en días festivos
     - Horas nocturnas en días festivos
   - Cálculo automático del total de horas extra.
   - Procesamiento hora por hora para mayor precisión.
   - Manejo inteligente de transiciones entre periodos diurnos y nocturnos.
   - Cálculo con precisión de minutos y redondeo a dos decimales.

3. **Sistema de aprobación**:

   - Las horas extra deben ser aprobadas (por defecto se registran como no aprobadas).
   - Diferentes niveles de permisos basados en roles (manager/superusuario).
   - Interfaz para administradores para aprobar solicitudes pendientes.
   - Contador de solicitudes pendientes con badge visual.

4. **Gestión y Mantenimiento de Registros**:

   - Modificación de registros existentes a través de formulario modal.
   - Eliminación de registros con confirmación.
   - Filtrado por rango de fechas.
   - Actualización en tiempo real de la información.

5. **Interfaz adaptada por roles**:

   - Interfaz especial para superusuarios y managers con capacidad de seleccionar empleados.
   - Panel de administración con acciones específicas.

6. **Control de Límites y Alertas**:

   - Monitoreo de límites semanales de horas extra por empleado.
   - Alertas visuales cuando un empleado se acerca o supera el límite configurado.
   - Codificación por colores en la tabla para identificar registros aprobados y pendientes.

7. **Calendario y Gestión de Días Festivos**:

   - Reconocimiento automático de días festivos para cálculos adecuados.
   - Servicio personalizado para cálculo de días festivos en Colombia según la legislación local.
   - Tratamiento automático de domingos como días festivos.

8. **Cálculo avanzado de horas extra**:

   - Servicio especializado `ExtraHourCalculationService` para determinar tipos de horas extra.
   - Validación de coherencia en horas de inicio y fin.
   - Cálculo detallado hora por hora para mayor precisión.
   - Manejo de transiciones entre periodos diurnos y nocturnos dentro del mismo registro.
   - Algoritmos para distribuir correctamente las horas cuando un registro cruza entre diferentes periodos.
   - Integración con configuración centralizada de franjas horarias.

9. **Reportes y análisis**:

   - Generación de informes en Excel con exceljs.
   - Visualización de datos con gráficos utilizando recharts.
   - Acceso directo a sección de reportes desde el panel de gestión.
   - Sistema de consulta con múltiples filtros:
     - Búsqueda por ID de empleado
     - Filtrado por rango de fechas
     - Filtros específicos por estado de aprobación (Todos/Aprobados/Pendientes)
   - Vista en formato tabla y dashboard interactivo con pestañas
   - Cálculo de totales y subtotales por página en las tablas
   - Exportación personalizada de reportes según filtros aplicados

10. **Autenticación y seguridad**:

    - Sistema de autenticación basado en JWT.
    - Identificación automática del empleado a partir del token.
    - Diferentes niveles de acceso según rol de usuario.
    - Rutas protegidas según el rol del usuario.
    - Redirección automática si el usuario no tiene los permisos necesarios.
    - Funcionalidad de cambio de contraseña para usuarios autenticados.
    - Almacenamiento seguro del ID de usuario en localStorage.
    - Implementación de refresh tokens para mantener la sesión activa.
    - Endpoints dedicados para inicio de sesión y cambio de contraseña.
    - Cifrado seguro de contraseñas con BCrypt.
    - Funcionalidad de cierre de sesión (logout) con invalidación de tokens.
    - Modelo de usuario completo con validaciones de datos:
      - Email con validación de formato
      - Contraseñas hasheadas para mayor seguridad
      - Roles definidos para control de acceso
      - Nombre de usuario único para identificación

11. **Gestión de Empleados**:

    - Gestión completa de empleados (crear, modificar, eliminar).
    - Buscador de empleados por ID.
    - Pantalla de administración con dos pestañas principales:
      - Agregar empleado: Formulario completo para agregar nuevos empleados
      - Gestionar empleados: Buscar, editar y eliminar empleados existentes
    - Generación automática de correos electrónicos corporativos.
    - Opciones para cambiar contraseñas de empleados.
    - Asignación de roles (manager, empleado, superusuario).
    - Gestión de relaciones entre empleados y managers.
    - Validación de datos durante la creación y actualización.

12. **Gestión de Managers**:

    - Sistema CRUD completo para gestionar managers en el sistema.
    - Obtención de listado completo de managers.
    - Búsqueda de managers por ID.
    - Creación, actualización y eliminación de managers.
    - Asignación de empleados a managers específicos.
    - Validación de datos al crear o modificar managers.

13. **Panel de Configuración del Sistema**:

    - Configuración de límites de horas extra semanales.
    - Configuración de multiplicadores para diferentes tipos de horas:
    - Configuración de horarios diurnos y nocturnos.
    - Interfaz amigable para la configuración de todos los parámetros del sistema.
    - API dedicada para obtener y actualizar la configuración con permisos de superusuario.

14. **Menú Principal Adaptativo**:
    - Menú principal con opciones específicas según el rol del usuario.
    - Acceso rápido a las funcionalidades permitidas por rol.
    - Diseño visual con iconos descriptivos.
    - Funcionalidad de cierre de sesión integrada.
    - Acceso directo a cambio de contraseña desde el menú principal.

## Detalles Técnicos

### Frontend

- **Componentes principales**:

  - `FormExtraHour`: Formulario para registrar horas extra con cálculo automático.
  - `EmployeeInfo`: Componente para selección de empleados (solo superusuarios).
  - `UpdateDeleteApprove`: Componente para gestionar, aprobar y eliminar registros.
  - `ReportInfo`: Componente para consulta, visualización y exportación de reportes.
  - `ExtraHoursDashboard`: Componente para visualización gráfica de datos.
  - `EmployeeManagement`: Componente para gestión de empleados.
  - `ExtraHoursSettings`: Componente para configuración del sistema.
  - `ChangePasswordModal`: Componente modal para cambio de contraseña.
  - `ProtectedRoute`: Componente para proteger rutas según roles de usuario.
  - `Layout`: Componente contenedor para la estructura común de páginas.
  - `ExtraHoursMenu`: Componente de menú principal adaptativo por rol.
  - `LoginForm`: Componente para autenticación de usuarios.
  - Hooks personalizados:
    - `useAuth`: Manejo de autenticación, roles y cierre de sesión.
    - `useConfig`: Acceso a la configuración del sistema.

- **Servicios**:

  - `addExtraHour`: Envía los datos de horas extra al backend.
  - `calculateExtraHour`: Calcula la distribución de horas basada en la fecha y horario.
  - `findAllExtraHours`: Recupera todas las horas extra (superusuarios).
  - `findExtraHoursByManager`: Recupera horas extra de empleados a cargo de un manager.
  - `findExtraHourByDateRange`: Recupera horas extra dentro de un rango de fechas.
  - `findManagerEmployeesExtraHours`: Recupera horas extra de empleados bajo un manager.
  - `updateExtraHour`: Actualiza un registro existente.
  - `deleteExtraHour`: Elimina un registro.
  - `approveExtraHour`: Aprueba un registro de horas extras.
  - `findEmployee`: Recupera información de un empleado específico.
  - `addEmployee`: Agrega un nuevo empleado al sistema.
  - `updateEmployee`: Actualiza información de un empleado existente.
  - `deleteEmployee`: Elimina un empleado del sistema.
  - `updateConfig`: Actualiza la configuración del sistema.
  - `AuthService`: Manejo de autenticación, incluye login y refresh de tokens.
  - `UserService`: Gestión de usuarios, autenticación y cambio de contraseñas.
  - `LogoutService`: Servicio para cierre de sesión e invalidación de tokens.
  - `ManagerService`: Servicio para gestión completa de managers.

- **Características UI**:
  - Interfaz responsiva con SASS.
  - Validación de formularios.
  - Sistema de pestañas para diferentes secciones.
  - Feedback visual de operaciones (carga, errores, éxito).
  - Reseteo automático de formularios después de envíos exitosos.
  - Tablas con paginación, ordenamiento y filtrado.
  - Modales para confirmación, edición y cambio de contraseña.
  - Sistema de notificaciones con mensajes contextuales.
  - Vistas diferenciadas según el rol del usuario.
  - Panel de dashboard con visualizaciones gráficas interactivas.
  - Exportación de datos en formato Excel.
  - Confirmaciones antes de acciones destructivas.
  - Menú principal con iconos descriptivos.

### Backend

- **Controladores API para**:

  - Gestión de empleados (`EmployeeController`)
  - Registro y aprobación de horas extra (`ExtraHourController`)
  - Gestión de configuración del sistema (`ExtraHoursConfigController`)
  - Autenticación y gestión de usuarios (`AuthController`)
  - Cierre de sesión (`LogoutController`)
  - Gestión de managers (`ManagerController`)
  - Cambio de contraseñas

- **Modelos de datos principales**:

  - `Employee`: Información de empleados
  - `ExtraHour`: Registro de horas extra
  - `ExtraHourCalculation`: Modelo para resultados del cálculo de horas extra
  - `ExtraHoursConfig`: Configuración del sistema (horarios diurnos/nocturnos, multiplicadores)
  - `Holiday`: Gestión de días festivos
  - `User`: Información de usuarios y autenticación
    - Campos principales: id, email, name, passwordHash, role, username
    - Validaciones: Email con formato válido, campos requeridos
    - Mapeo a tabla "users" en la base de datos
  - `Manager`: Información de gestores de equipos

- **Servicios**:

  - `AuthService`: Autenticación, generación de tokens JWT y refresh tokens
  - `EmployeeService`: Gestión completa de empleados
  - `ExtraHourService`: Cálculo y gestión de horas extra
  - `ExtraHourCalculationService`: Servicio especializado para el cálculo detallado y clasificación de horas extra
    - Determinación de tipo de hora extra (diurna, nocturna, festiva)
    - Cálculo hora por hora para mayor precisión
    - Manejo de transiciones entre periodos diurnos y nocturnos
    - Validaciones de coherencia en horas registradas
  - `ExtraHoursConfigService`: Gestión de configuración del sistema
  - `UserService`: Gestión de usuarios y contraseñas
  - `JWTTokenService`: Gestión y validación de tokens JWT, incluyendo invalidación
  - `ManagerService`: Gestión completa de managers
  - `ColombianHolidayService`: Servicio especializado para cálculo de días festivos en Colombia
    - Cálculo de festivos fijos (como Año Nuevo, Navidad)
    - Cálculo de festivos trasladables al lunes siguiente
    - Cálculo de festivos que dependen de la Pascua
    - Implementación del algoritmo de Butcher para calcular el Domingo de Pascua
    - Reconocimiento de domingos como días festivos
  - Validación de datos
  - Gestión de permisos y roles

- **Repositorios**:
  - `EmployeeRepository`: Acceso a datos de empleados
  - `ExtraHourRepository`: Acceso a datos de horas extra
  - `ConfigurationRepository`: Acceso a datos de configuración
  - `UserRepository`: Acceso a datos de usuarios
  - `ManagerRepository`: Acceso a datos de managers

## Generación de Migraciones

Para generar nuevas migraciones después de cambios en el modelo de datos:

```bash
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

## Despliegue
El proyecto puede desplegarse completamente en la nube usando:

🔹 Frontend en Vercel

🔹 Backend y Base de Datos en Render

🔷 **Despliegue del Frontend en Vercel**
1. **Requisitos**
   - Tener una cuenta en Vercel
   - Repositorio conectado con GitHub
   - Frontend ubicado en la carpeta `client/` o similar

2. **Pasos**
   - Ve a Vercel, crea un nuevo proyecto desde GitHub.
   - Selecciona el repositorio del proyecto.
   - En Root Directory, elige `client` (si el frontend está en `/client`).
   - Configura las siguientes variables de entorno:

     | Clave                    | Valor                                             |
     |--------------------------|---------------------------------------------------|
     | VITE_API_USE_LOCAL       | false                                             |
     | VITE_API_BASE_URL        | https://<TU-BACKEND>.onrender.com (tu URL Render) |
     | VITE_API_BASE_URL_LOCAL  | http://localhost:7086                             |

   - Haz clic en Deploy.

3. **Resultado**
   - Vercel generará una URL como `https://gestor-horas-frontend.vercel.app`
   - Esta URL debe ser configurada como origen CORS en el backend.

🔷 **Despliegue del Backend en Render**
1. **Requisitos**
   - Cuenta en Render
   - Dockerfile en el backend
   - Proyecto con .NET y PostgreSQL configurado

2. **Pasos**
   - En Render, selecciona "New Web Service".
   - Conecta tu cuenta de GitHub y elige el repo.
   - Define los siguientes parámetros:
     - Environment: Docker
     - Start Command: Render detecta automáticamente el CMD del Dockerfile.
   - Configura las variables de entorno en Render:

     | Clave           | Valor                                 |
     |-----------------|---------------------------------------|
     | DB_HOST         | dpg-xxxxxx.oregon-postgres.render.com |
     | DB_NAME         | Nombre de la base de datos en Render  |
     | DB_USER         | Usuario de Render                     |
     | DB_PASSWORD     | Contraseña de Render                   |
     | JWT_SECRET      | Clave secreta segura (mínimo 32 chars)|
     | JWT_ISSUER      | ExtraHours.API                        |
     | JWT_AUDIENCE    | ExtraHours.Client                     |
     | ASPNETCORE_ENVIRONMENT | Production                      |

   - Render generará una URL como: `https://gestor-horas-api.onrender.com`

🔷 **Despliegue de Base de Datos en Render**
- En Render, crea un nuevo recurso de tipo PostgreSQL.
- Copia el Database URL y configura las variables DB_HOST, DB_NAME, etc. en tu backend.
- Restaura el backup SQL desde tu equipo local con el comando:

```bash
PGPASSWORD=tu_password psql -h tu_host -U tu_usuario -d tu_base -p 5432 -f backup.sql
```

🔁 **Conexión Frontend ↔ Backend**
En tu `api.config.js` del frontend:

```js
export const API_CONFIG = {
  BASE_URL:
    (import.meta.env.VITE_API_USE_LOCAL === "true"
      ? import.meta.env.VITE_API_BASE_URL_LOCAL
      : import.meta.env.VITE_API_BASE_URL) || "http://localhost:7086",
};
```
Asegúrate de que las variables estén correctamente definidas en Vercel.

✅ **Recomendaciones finales**
- Verifica que en `Program.cs` del backend estés usando CORS correctamente:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://gestor-horas-frontend.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("CorsPolicy");
```
- Usa https siempre en producción.
- Protege tus variables sensibles: nunca subas `.env` al repo.


## Estructura de Directorios Recomendada

```
proyecto-horas-extra/
├── frontend/                # Aplicación React
│   ├── public/              # Archivos estáticos
│   ├── src/                 # Código fuente React
│   │   ├── components/      # Componentes React
│   │   │   ├── FormExtraHour/    # Formulario de registro de horas extra
│   │   │   ├── EmployeeInfo/     # Selector de empleados
│   │   │   ├── UpdateDeleteApprove/  # Componente de gestión y aprobación
│   │   │   ├── ReportInfo/      # Componente de reportes y consultas
│   │   │   ├── ExtraHoursDashboard/ # Visualización gráfica de datos
│   │   │   ├── EmployeeManagement/ # Gestión de empleados
│   │   │   ├── ExtraHoursSettings/ # Configuración del sistema
│   │   │   ├── ChangePasswordModal/ # Modal para cambio de contraseña
│   │   │   ├── ProtectedRoute/   # Componente para proteger rutas
│   │   │   ├── Layout/          # Componente layout común
│   │   │   ├── LoginForm/       # Formulario de inicio de sesión
│   │   │   └── ...
│   │   ├── pages/           # Páginas de la aplicación
│   │   │   ├── LoginPage/       # Página de inicio de sesión
│   │   │   ├── ReportsPage/     # Página de reportes
│   │   │   ├── AddExtrahour/    # Página para agregar horas extra
│   │   │   ├── UpdateDeleteApprovePage/ # Página para gestión y aprobación
│   │   │   ├── Settings/        # Páginas de configuración
│   │   │   └── ...
│   │   ├── services/        # Servicios de API
│   │   │   ├── addExtraHour.js   # Servicio para agregar horas extra
│   │   │   ├── calculateExtraHour.js # Servicio para calcular horas
│   │   │   ├── updateExtraHour.js # Servicio para actualizar registros
│   │   │   ├── deleteExtraHour.js # Servicio para eliminar registros
│   │   │   ├── approveExtraHour.js # Servicio para aprobar registros
│   │   │   ├── findAllExtraHours.js # Servicio para listar todos los registros
│   │   │   ├── findExtraHoursByManager.js # Servicio para listar registros por manager
│   │   │   ├── findExtraHourByDateRange.js # Servicio para buscar por rango de fechas
│   │   │   ├── findManagerEmployeesExtraHours.js # Servicio para listar registros de empleados bajo manager
│   │   │   ├── addEmployee.js # Servicio para agregar empleados
│   │   │   ├── findEmployee.js # Servicio para buscar empleados
│   │   │   ├── updateEmployee.js # Servicio para actualizar empleados
│   │   │   ├── deleteEmployee.js # Servicio para eliminar empleados
│   │   │   ├── updateConfig.js # Servicio para actualizar configuración
│   │   │   ├── authService.js # Servicio para autenticación
│   │   │   ├── UserService.js # Servicio para gestión de usuarios
│   │   │   ├── logoutService.js # Servicio para cierre de sesión
│   │   │   ├── managerService.js # Servicio para gestión de managers
│   │   │   └── ...
│   │   ├── utils/           # Utilidades
│   │   │   ├── useAuth.js   # Hook para autenticación
│   │   │   ├── AuthProvider.js # Proveedor de contexto de autenticación
│   │   │   ├── useConfig.js # Hook para configuración
│   │   │   ├── ConfigProvider.js # Proveedor de contexto de configuración
│   │   │   ├── tableColumns.js # Configuración de columnas para tablas
│   │   │   ├── generateXLSReport.js # Generación de reportes Excel
│   │   │   ├── tokenUtils.js # Utilidades para manejo de tokens JWT
│   │   │   └── ...
│   │   ├── assets/          # Recursos estáticos
│   │   │   ├── images/      # Imágenes e iconos
│   │   │   └── ...
│   │   └── App.jsx          # Componente principal
│   ├── package.json         # Dependencias de npm
│   └── vite.config.js       # Configuración de Vite
│
└── backend/                 # API en C#
    ├── Controllers/         # Controladores API
    │   ├── ExtraHourController.cs # Controlador de horas extra
    │   ├── EmployeeController.cs  # Controlador de empleados
    │   ├── ExtraHoursConfigController.cs # Controlador de configuración
    │   ├── AuthController.cs     # Controlador de autenticación
    │   ├── LogoutController.cs   # Controlador de cierre de sesión
    │   ├── ManagerController.cs  # Controlador de gestión de managers
    │   └── ...
    ├── Models/              # Modelos de datos
    │   ├── ExtraHour.cs     # Modelo de horas extra
    │   ├── ExtraHourCalculation.cs # Modelo para resultados de cálculo
    │   ├── Employee.cs      # Modelo de empleado
    │   ├── Manager.cs       # Modelo de manager
    │   ├── ExtraHoursConfig.cs # Modelo de configuración
    │   ├── User.cs          # Modelo de usuario con validaciones
    │   └── ...
    ├── Services/            # Servicios de negocio
    │   ├── Implementations/ # Implementaciones de servicios
    │   │   ├── AuthService.cs  # Servicio de autenticación
    │   │   ├── EmployeeService.cs # Servicio de gestión de empleados
    │   │   ├── ExtraHourCalculationService.cs # Servicio para cálculo detallado de horas extra
    │   │   ├── ColombianHolidayService.cs # Servicio para cálculo de festivos colombianos
    │   │   ├── UserService.cs  # Servicio de gestión de usuarios
    │   │   ├── JWTTokenService.cs # Servicio de gestión de tokens JWT
    │   │   ├── ExtraHoursConfigService.cs # Servicio de configuración
    │   │   └── ...
    │   └── Interface/       # Interfaces de servicios
    │       ├── IAuthService.cs # Interfaz del servicio de autenticación
    │       ├── IEmployeeService.cs # Interfaz del servicio de empleados
    │       ├── IExtraHourCalculationService.cs # Interfaz del servicio de cálculo
    │       ├── IUserService.cs # Interfaz del servicio de usuarios
    │       ├── IJWTTokenService.cs # Interfaz del servicio de tokens JWT
    │       ├── IExtraHoursConfigService.cs # Interfaz del servicio de configuración
    │       └── ...
    ├── Repositories/        # Repositorios para acceso a datos
    │   ├── Implementations/ # Implementaciones de repositorios
    │   │   ├── UserRepository.cs # Repositorio de usuarios
    │   │   ├── ManagerRepository.cs # Repositorio de managers
    │   │   └── ...
    │   └── Interfaces/      # Interfaces de repositorios
    │       ├── IUserRepository.cs # Interfaz del repositorio de usuarios
    │       ├── IManagerRepository.cs # Interfaz del repositorio de managers
    │       └── ...
    ├── Dto/                 # Objetos de transferencia de datos
    │   ├── UserLoginRequest.cs # DTO para login
    │   ├── ChangePasswordRequest.cs # DTO para cambio de contraseña
    │   ├── EmployeeWithUserDTO.cs # DTO para creación de empleados
    │   ├── UpdateEmployeeDTO.cs # DTO para actualización de empleados
    │   └── ...
    ├── Data/                # Contexto de base de datos
    ├── Migrations/          # Migraciones de EF Core
    ├── appsettings.json     # Configuración
    └── Program.cs           # Punto de entrada
```

## Flujo de Trabajo

1. **Autenticación de usuario**:

   - El usuario accede a la página de inicio de sesión
   - Introduce credenciales (email y contraseña)
   - El sistema valida las credenciales y devuelve un token JWT y un refresh token
   - El token se almacena en localStorage para futuras peticiones
   - El usuario es redirigido al menú principal según su rol

2. **Menú principal adaptativo**:

   - Se muestra un menú con opciones específicas según el rol del usuario:
     - Empleado: Registrar horas extra, Informes
     - Manager: Informes, Gestionar horas extra
     - Superusuario: Registrar horas extra, Gestionar horas extra, Informes, Configuración
   - El usuario puede cambiar su contraseña o cerrar sesión desde el menú

3. **Registro de horas extra**:

   - El empleado o superusuario accede al formulario de registro
   - Selecciona fecha, hora de inicio y fin
   - El sistema calcula automáticamente la distribución de horas usando el servicio `ExtraHourCalculationService`:

     - Determina si la fecha corresponde a un día festivo usando `ColombianHolidayService`
     - Distribuye las horas en las categorías correspondientes (diurna, nocturna, diurna festiva, nocturna festiva)
     - Maneja correctamente las transiciones entre periodos diurnos y nocturnos
     - Calcula con precisión hora por hora incluso cuando el registro cruza diferentes categorías

   - Se agregan observaciones si es necesario
   - Se envía el formulario

4. **Aprobación de horas extra**:

   - Los gerentes o superusuarios acceden al panel de gestión
   - Los gerentes pueden ver todos los registros pendientes de los empelados a su cargo (indicados con badge)
   - Filtran por fecha si es necesario
   - Revisan y aprueban/rechazan cada solicitud usando los botones de acción
   - El sistema actualiza el estado y muestra notificaciones de éxito

5. **Mantenimiento de registros**:

   - Selección del registro a modificar/eliminar
   - Para edición: se abre modal con formulario pre-cargado
   - Para eliminación: se confirma la acción antes de proceder
   - El sistema verifica límites y muestra alertas si es necesario

6. **Cambio de contraseña**:

   - El usuario accede al modal de cambio de contraseña desde el menú principal
   - Introduce su contraseña actual y la nueva contraseña
   - El sistema valida los datos y actualiza la contraseña
   - Se muestra un mensaje de éxito o error según corresponda

7. **Gestión de empleados**:

   - Acceso a la sección de gestión de empleados
   - Uso de la pestaña "Agregar Empleado" para crear nuevos registros
   - Uso de la pestaña "Gestionar Empleados" para buscar, editar y eliminar
   - Generación automática de correos electrónicos corporativos
   - Confirmación antes de eliminar empleados
   - Posibilidad de cambiar contraseñas de empleados durante la edición

8. **Gestión de managers**:

   - Acceso a la sección de gestión de managers
   - Listar todos los managers disponibles en el sistema
   - Buscar managers específicos por ID
   - Crear nuevos managers con validación de datos
   - Actualizar información de managers existentes
   - Eliminar managers con confirmación previa
   - Asignar managers a empleados durante la gestión de empleados

9. **Configuración del sistema**:

   - Acceso a la sección de configuración (solo superusuarios)
   - Establecimiento de límites semanales de horas extra
   - Configuración de multiplicadores para diferentes tipos de horas
   - Definición de horarios diurnos y nocturnos
   - Guardado de cambios con confirmación visual

10. **Generación de reportes**:

    - Acceso a la sección de consulta de horas extras
    - Si es empleado: visualiza automáticamente sus registros
    - Si es manager o superusuario:
      - Búsqueda por ID de empleado o por rango de fechas
      - Aplicación de filtros por estado de aprobación
      - Visualización de datos en formato tabla con totales calculados
      - Cambio a vista de dashboard para análisis gráfico
      - Exportación a Excel de los datos filtrados
    - Navegación entre vistas de tabla y dashboard mediante pestañas

11. **Cierre de sesión**:
    - El usuario puede cerrar sesión desde el menú principal
    - El sistema invalida el token JWT actual
    - Se eliminan tokens y datos de autenticación del localStorage
    - El usuario es redirigido a la página de inicio de sesión
    - Se previene el acceso a rutas protegidas después del cierre de sesión

## Solución de Problemas Comunes

### Errores de migración de base de datos

Si encuentras errores al ejecutar las migraciones:

1. Verifica que PostgreSQL esté en ejecución
2. Comprueba la cadena de conexión en `appsettings.json`
3. Asegúrate de que el usuario tenga permisos para crear bases de datos

### Problemas con el frontend

- Limpia la caché de npm: `npm cache clean --force`
- Elimina `node_modules` y reinstala: `rm -rf node_modules && npm install`
- Si hay problemas con el cálculo de horas, verifica la configuración del sistema
- Asegúrate de que se han instalado todas las dependencias

### Problemas con la autenticación

- Verifica que el token JWT no haya expirado
- Comprueba que el usuario tenga los permisos adecuados para la operación
- Si hay problemas de redirección, verifica el componente `ProtectedRoute`
- En caso de error al cambiar la contraseña, verifica que la contraseña actual sea correcta
- Asegúrate de que el token se envía correctamente en el encabezado Authorization
- Si el token expira continuamente, verifica la configuración de refresh token
- Comprueba que el email utilizado en el login existe en el sistema

> Nota: Todas las dependencias necesarias para las pruebas (como xUnit y NSubstitute) se restauran automáticamente al ejecutar `dotnet restore`. No es necesario instalarlas manualmente.

## Contribución

Para contribuir al proyecto:

1. Crea una rama para tu funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
2. Realiza tus cambios y haz commit (`git commit -m 'Añadir nueva funcionalidad'`)
3. Envía tu rama (`git push origin feature/nueva-funcionalidad`)
4. Abre un Pull Request

## Pruebas Unitarias

El proyecto incluye pruebas unitarias para la lógica de cálculo de horas extra, ubicadas en `ExtraHours.API.Tests`.

### Ejecución de tests

Puedes ejecutar todas las pruebas con:

```bash
dotnet test
```

O desde el panel de Testing de Visual Studio Code si tienes instalada la extensión C# Dev Kit.

### Requisitos para ejecutar las pruebas

- Tener instalado [.NET SDK 9.0](https://dotnet.microsoft.com/download) o superior.
- (Opcional) Para una mejor experiencia en Visual Studio Code, instalar la extensión **C# Dev Kit**.

### Escenarios cubiertos

- Cálculo de horas extra diurnas y nocturnas en días laborales, domingos y festivos.
- Casos donde el rango horario cruza la medianoche, dividiendo correctamente el cálculo en dos días distintos.
- Validación de la lógica de negocio para reportes diarios, asegurando que las horas se asignen al día correspondiente (festivo o no festivo).
- Pruebas de casos límite, como rangos fraccionarios y validaciones de configuración nula o rangos inválidos.

Estas pruebas garantizan la robustez y exactitud del sistema de cálculo de horas extra.
