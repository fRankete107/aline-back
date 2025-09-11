# 🧘‍♀️ Aline - Pilates Studio API

Una API completa y robusta para la gestión integral de un estudio de pilates, desarrollada con ASP.NET Core 8 y Entity Framework Core.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Características](#características)
- [Tecnologías](#tecnologías)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Uso](#uso)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Base de Datos](#base-de-datos)
- [API Endpoints](#api-endpoints)
- [Autenticación](#autenticación)
- [Desarrollo](#desarrollo)
- [Deployment](#deployment)
- [Contribución](#contribución)
- [Licencia](#licencia)

## 🎯 Descripción

**Aline** es una API REST completa diseñada para automatizar y optimizar la gestión de un estudio de pilates. Permite administrar clases, instructores, estudiantes, planes de suscripción, reservas y pagos de manera eficiente y segura.

### Funcionalidades Principales

- 👥 **Gestión de Usuarios**: Administradores, instructores y estudiantes
- 🏃‍♀️ **Gestión de Clases**: Programación, horarios y capacidad
- 💳 **Planes y Suscripciones**: Diferentes tipos de membresías
- 📅 **Sistema de Reservas**: Booking inteligente con validaciones
- 💰 **Gestión de Pagos**: Múltiples métodos de pago y facturación
- 🏢 **Gestión de Zonas**: Diferentes salas y espacios
- 📞 **Sistema de Contacto**: Gestión de consultas y soporte
- 📊 **Reportes y Analytics**: Métricas de negocio en tiempo real

## ✨ Características

### 🔒 Seguridad
- **JWT Authentication** con refresh tokens
- **Autorización por roles** (Admin, Instructor, Student)
- **Validación de entrada** con FluentValidation
- **Encriptación de contraseñas** con Identity Framework
- **Middleware de manejo de errores** global

### 🚀 Performance
- **Entity Framework Core** con optimizaciones
- **Logging estructurado** con Serilog
- **Caching** preparado para implementar
- **Paginación** en endpoints de listado
- **Índices de base de datos** optimizados

### 📖 Documentación
- **Swagger/OpenAPI** integrado
- **Autenticación en Swagger** para testing
- **Documentación de endpoints** completa
- **Ejemplos de uso** incluidos

### 🔧 Arquitectura
- **Clean Architecture** con separación de concerns
- **Patrón Repository** preparado
- **Dependency Injection** configurado
- **CORS** habilitado para frontend
- **Health checks** implementados

## 🛠️ Tecnologías

### Backend
- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 8.0** - ORM
- **MySQL/SQLite** - Base de datos
- **Identity Framework** - Gestión de usuarios
- **JWT Bearer** - Autenticación
- **AutoMapper** - Mapeo de objetos
- **FluentValidation** - Validación de modelos
- **Serilog** - Logging estructurado

### Herramientas de Desarrollo
- **.NET CLI** - Herramientas de línea de comandos
- **Entity Framework Tools** - Migraciones
- **Swagger/OpenAPI** - Documentación de API
- **Git** - Control de versiones

### Base de Datos
- **MySQL 8.0+** (Producción)
- **SQLite** (Desarrollo)
- **Pomelo MySQL Provider** - Driver MySQL para EF Core

## 🚀 Instalación

### Prerrequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0+](https://dev.mysql.com/downloads/) (para producción)
- [Git](https://git-scm.com/)

### 1. Clonar el Repositorio

```bash
git clone https://github.com/fRankete107/aline-back.git
cd aline-back/PilatesStudioAPI
```

### 2. Instalar Dependencias

```bash
dotnet restore
```

### 3. Configurar Base de Datos

#### Para Desarrollo (SQLite)
```bash
# La configuración por defecto usa SQLite
dotnet ef database update
```

#### Para Producción (MySQL)
```bash
# 1. Crear base de datos en MySQL
mysql -u root -p
CREATE DATABASE aline CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# 2. Actualizar connection string en appsettings.json
# 3. Aplicar migraciones
dotnet ef database update
```

### 4. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- **HTTP**: http://localhost:5121
- **Swagger**: http://localhost:5121/swagger

## ⚙️ Configuración

### Variables de Entorno

Configura las siguientes variables en `appsettings.json` o `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Tu cadena de conexión"
  },
  "JwtSettings": {
    "Secret": "tu-clave-secreta-jwt-minimo-32-caracteres",
    "Issuer": "PilatesStudioAPI",
    "Audience": "PilatesStudioClient",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-contraseña-app",
    "FromEmail": "noreply@pilatesstudio.com",
    "FromName": "Pilates Studio"
  }
}
```

### Configuración de CORS

```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "https://tu-frontend.com"
    ],
    "AllowCredentials": true
  }
}
```

## 📖 Uso

### Autenticación

1. **Registro/Login**: Usar endpoints de autenticación para obtener JWT token
2. **Autorización**: Incluir token en header: `Authorization: Bearer {token}`
3. **Roles**: Los endpoints están protegidos por roles específicos

### Swagger UI

Visita `/swagger` para explorar y probar todos los endpoints de la API:

```
http://localhost:5121/swagger
```

### Ejemplos de Uso

#### Registrar un nuevo usuario
```bash
curl -X POST "http://localhost:5121/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "estudiante@example.com",
    "password": "MiPassword123$",
    "confirmPassword": "MiPassword123$",
    "firstName": "Juan",
    "lastName": "Pérez",
    "role": "student"
  }'
```

#### Login
```bash
curl -X POST "http://localhost:5121/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "estudiante@example.com",
    "password": "MiPassword123$"
  }'
```

#### Usar endpoints autenticados
```bash
# Guardar token recibido del login
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# Obtener perfil del usuario
curl -X GET "http://localhost:5121/api/auth/me" \
  -H "Authorization: Bearer $TOKEN"

# Listar instructores activos
curl -X GET "http://localhost:5121/api/instructors/active" \
  -H "Authorization: Bearer $TOKEN"
```

## 🏗️ Estructura del Proyecto

```
PilatesStudioAPI/
├── 📁 Configuration/          # Clases de configuración
│   ├── CorsSettings.cs
│   ├── EmailSettings.cs
│   └── JwtSettings.cs
├── 📁 Controllers/            # Controladores de API
│   ├── AuthController.cs      # ✅ Autenticación
│   ├── InstructorsController.cs # ✅ Gestión de instructores
│   ├── StudentsController.cs  # ✅ Gestión de estudiantes
│   └── WeatherForecastController.cs
├── 📁 Data/                   # Contexto de base de datos
│   ├── Context/
│   │   ├── PilatesStudioDbContext.cs
│   │   └── PilatesStudioDbContextFactory.cs
│   └── Configurations/        # Configuraciones EF Core
├── 📁 Models/                 # Modelos de dominio
│   ├── Entities/             # Entidades de EF Core
│   │   ├── User.cs            # ✅ Entidad usuario
│   │   ├── Instructor.cs      # ✅ Entidad instructor
│   │   ├── Student.cs         # ✅ Entidad estudiante
│   │   ├── Zone.cs            # ✅ Entidad zona/sala
│   │   ├── Class.cs           # ✅ Entidad clase
│   │   ├── Plan.cs            # Entidad plan
│   │   ├── Subscription.cs    # Entidad suscripción
│   │   ├── Reservation.cs     # Entidad reserva
│   │   ├── Payment.cs         # Entidad pago
│   │   ├── Contact.cs         # Entidad contacto
│   │   └── AuditLog.cs        # Entidad auditoría
│   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Auth/             # ✅ DTOs de autenticación
│   │   ├── Users/            # ✅ DTOs de usuarios
│   │   ├── ZoneDto.cs        # ✅ DTOs de zonas
│   │   ├── ClassDto.cs       # ✅ DTOs de clases
│   │   ├── PlanDto.cs        # ✅ DTOs de planes
│   │   ├── SubscriptionDto.cs# ✅ DTOs de suscripciones
│   │   ├── ReservationDto.cs # ✅ DTOs de reservas
│   │   ├── Payments/         # DTOs de pagos
│   │   └── Contacts/         # DTOs de contactos
│   └── Validators/           # Validadores FluentValidation
│       ├── Auth/             # ✅ Validadores de autenticación
│       ├── ZoneValidators.cs # ✅ Validadores de zonas
│       ├── ClassValidators.cs# ✅ Validadores de clases
│       ├── PlanValidators.cs # ✅ Validadores de planes
│       ├── SubscriptionValidators.cs# ✅ Validadores de suscripciones
│       └── ReservationValidators.cs# ✅ Validadores de reservas
├── 📁 Services/              # Lógica de negocio
│   ├── Interfaces/
│   │   ├── IJwtService.cs    # ✅ Servicio JWT
│   │   ├── IAuthService.cs   # ✅ Servicio autenticación
│   │   ├── IInstructorService.cs # ✅ Servicio instructores
│   │   ├── IStudentService.cs # ✅ Servicio estudiantes
│   │   ├── IZoneService.cs   # ✅ Servicio zonas
│   │   ├── IClassService.cs  # ✅ Servicio clases
│   │   ├── IPlanService.cs   # ✅ Servicio planes
│   │   ├── ISubscriptionService.cs# ✅ Servicio suscripciones
│   │   └── IReservationService.cs# ✅ Servicio reservas
│   └── Implementations/
│       ├── JwtService.cs     # ✅ Implementación JWT
│       ├── AuthService.cs    # ✅ Implementación auth
│       ├── InstructorService.cs # ✅ Implementación instructores
│       ├── StudentService.cs # ✅ Implementación estudiantes
│       ├── ZoneService.cs    # ✅ Implementación zonas
│       ├── ClassService.cs   # ✅ Implementación clases
│       ├── PlanService.cs    # ✅ Implementación planes
│       ├── SubscriptionService.cs# ✅ Implementación suscripciones
│       └── ReservationService.cs# ✅ Implementación reservas
├── 📁 Repositories/          # Patrón Repository
│   ├── Interfaces/
│   │   ├── IInstructorRepository.cs # ✅ Repositorio instructores
│   │   ├── IStudentRepository.cs# ✅ Repositorio estudiantes
│   │   ├── IZoneRepository.cs # ✅ Repositorio zonas
│   │   ├── IClassRepository.cs# ✅ Repositorio clases
│   │   ├── IPlanRepository.cs # ✅ Repositorio planes
│   │   ├── ISubscriptionRepository.cs# ✅ Repositorio suscripciones
│   │   └── IReservationRepository.cs# ✅ Repositorio reservas
│   └── Implementations/
│       ├── InstructorRepository.cs # ✅ Implementación instructores
│       ├── StudentRepository.cs# ✅ Implementación estudiantes
│       ├── ZoneRepository.cs # ✅ Implementación zonas
│       ├── ClassRepository.cs# ✅ Implementación clases
│       ├── PlanRepository.cs # ✅ Implementación planes
│       ├── SubscriptionRepository.cs# ✅ Implementación suscripciones
│       └── ReservationRepository.cs# ✅ Implementación reservas
├── 📁 Middleware/            # Middlewares personalizados
│   └── GlobalExceptionMiddleware.cs # ✅ Manejo de errores
├── 📁 Mapping/               # Perfiles de AutoMapper
│   └── MappingProfile.cs     # ✅ Configuración de mapeos (actualizado)
├── 📁 Extensions/            # Métodos de extensión
├── 📁 Utils/                 # Utilidades y helpers
├── 📁 Migrations/            # Migraciones EF Core
├── Program.cs                # ✅ Punto de entrada configurado
├── appsettings.json          # ✅ Configuración completa
└── README.md                 # Este archivo actualizado
```

## 🗄️ Base de Datos

### Modelo de Datos

La base de datos está diseñada siguiendo las mejores prácticas de normalización:

#### Entidades Principales

1. **Users** - Usuarios del sistema (Admin, Instructor, Student)
2. **Instructors** - Información específica de instructores
3. **Students** - Información específica de estudiantes
4. **Plans** - Planes de suscripción disponibles
5. **Subscriptions** - Suscripciones activas de estudiantes
6. **Zones** - Salas/espacios del estudio
7. **Classes** - Clases programadas
8. **Reservations** - Reservas de estudiantes a clases
9. **Payments** - Registros de pagos
10. **Contacts** - Consultas de contacto
11. **AuditLog** - Auditoría de cambios importantes

### Relaciones

- **User** ↔ **Instructor/Student** (1:1)
- **Student** ↔ **Subscription** (1:N)
- **Plan** ↔ **Subscription** (1:N)
- **Class** ↔ **Reservation** (1:N)
- **Student** ↔ **Reservation** (1:N)
- Y más relaciones detalladas...

### Migraciones

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir migración
dotnet ef database update MigracionAnterior
```

## 🔌 API Endpoints

### 🔐 Autenticación (✅ Implementados)
- `POST /api/auth/register` - Registrar nuevo usuario
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/refresh-token` - Renovar access token
- `POST /api/auth/logout` - Cerrar sesión
- `POST /api/auth/change-password` - Cambiar contraseña
- `GET /api/auth/me` - Obtener información del usuario actual
- `POST /api/auth/forgot-password` - Solicitar reset de contraseña (preparado)
- `POST /api/auth/reset-password` - Restablecer contraseña (preparado)
- `POST /api/auth/verify-email` - Verificar email (preparado)

### 👨‍🏫 Instructores (✅ Implementados)
- `GET /api/instructors` - Listar todos los instructores (Solo Admin)
- `GET /api/instructors/active` - Listar instructores activos
- `GET /api/instructors/{id}` - Obtener instructor por ID
- `GET /api/instructors/me` - Obtener perfil del instructor actual
- `POST /api/instructors` - Crear nuevo instructor (Solo Admin)
- `PUT /api/instructors/{id}` - Actualizar instructor
- `DELETE /api/instructors/{id}` - Eliminar instructor (Solo Admin)
- `POST /api/instructors/{id}/activate` - Activar instructor (Solo Admin)
- `POST /api/instructors/{id}/deactivate` - Desactivar instructor (Solo Admin)

### 👥 Estudiantes (✅ Implementados)
- `GET /api/students` - Listar todos los estudiantes
- `GET /api/students/search?q={term}` - Buscar estudiantes
- `GET /api/students/{id}` - Obtener estudiante por ID
- `GET /api/students/me` - Obtener perfil del estudiante actual
- `POST /api/students` - Crear nuevo estudiante
- `PUT /api/students/{id}` - Actualizar estudiante
- `DELETE /api/students/{id}` - Eliminar estudiante (Solo Admin)

### 🏢 Zonas/Salas (✅ Implementados)
- `GET /api/zones` - Listar todas las zonas (Admin/Instructor)
- `GET /api/zones/active` - Listar zonas activas
- `GET /api/zones/{id}` - Obtener zona por ID
- `POST /api/zones` - Crear nueva zona (Solo Admin)
- `PUT /api/zones/{id}` - Actualizar zona (Solo Admin)
- `DELETE /api/zones/{id}` - Eliminar zona (Solo Admin)

### 🏃‍♀️ Clases (✅ Implementados)
- `GET /api/classes` - Listar clases con filtros opcionales
- `GET /api/classes/available` - Listar clases con espacios disponibles
- `GET /api/classes/date-range?startDate={date}&endDate={date}` - Clases por rango de fechas
- `GET /api/classes/instructor/{id}` - Clases por instructor
- `GET /api/classes/zone/{id}` - Clases por zona
- `GET /api/classes/{id}` - Obtener clase por ID
- `POST /api/classes` - Crear nueva clase (Admin/Instructor)
- `PUT /api/classes/{id}` - Actualizar clase (Admin/Instructor)
- `DELETE /api/classes/{id}` - Eliminar clase (Solo Admin)
- `GET /api/classes/instructor/{id}/conflicts` - Verificar conflictos de instructor
- `GET /api/classes/zone/{id}/conflicts` - Verificar conflictos de zona

### 💳 Planes (✅ Implementados)
- `GET /api/plans` - Listar todos los planes (Admin/Instructor)
- `GET /api/plans/active` - Listar planes activos
- `GET /api/plans/{id}` - Obtener plan por ID
- `POST /api/plans` - Crear nuevo plan (Solo Admin)
- `PUT /api/plans/{id}` - Actualizar plan (Solo Admin)
- `DELETE /api/plans/{id}` - Eliminar plan (Solo Admin)
- `GET /api/plans/{id}/active-subscriptions` - Verificar suscripciones activas (Solo Admin)

### 📋 Suscripciones (✅ Implementados)
- `GET /api/subscriptions` - Listar suscripciones con filtros (Admin/Instructor)
- `GET /api/subscriptions/active` - Listar suscripciones activas (Admin/Instructor)
- `GET /api/subscriptions/expired` - Listar suscripciones vencidas (Solo Admin)
- `GET /api/subscriptions/expiring-soon?daysThreshold={days}` - Suscripciones por vencer
- `GET /api/subscriptions/{id}` - Obtener suscripción por ID
- `GET /api/subscriptions/student/{id}` - Obtener suscripciones de un estudiante
- `GET /api/subscriptions/student/{id}/active` - Obtener suscripción activa de estudiante
- `GET /api/subscriptions/plan/{id}` - Obtener suscripciones de un plan
- `POST /api/subscriptions` - Crear nueva suscripción (Solo Admin)
- `PUT /api/subscriptions/{id}` - Actualizar suscripción (Solo Admin)
- `POST /api/subscriptions/{id}/renew` - Renovar suscripción (Solo Admin)
- `DELETE /api/subscriptions/{id}` - Eliminar suscripción (Solo Admin)
- `GET /api/subscriptions/student/{id}/can-reserve` - Verificar si puede reservar clases
- `POST /api/subscriptions/process-expired` - Procesar suscripciones vencidas (Solo Admin)

### 📅 Reservas (✅ Implementados)
- `GET /api/reservations` - Listar reservas con filtros avanzados (Admin/Instructor)
- `GET /api/reservations/{id}` - Obtener reserva por ID
- `GET /api/reservations/student/{id}` - Obtener reservas de un estudiante
- `GET /api/reservations/student/{id}/upcoming` - Obtener reservas próximas de estudiante
- `GET /api/reservations/class/{id}` - Obtener reservas de una clase (Admin/Instructor)
- `GET /api/reservations/instructor/{id}` - Obtener reservas de un instructor (Admin/Instructor)
- `POST /api/reservations` - Crear nueva reserva con validaciones automáticas
- `PUT /api/reservations/{id}` - Actualizar reserva (Solo Admin)
- `POST /api/reservations/{id}/cancel` - Cancelar reserva con políticas
- `POST /api/reservations/{id}/complete` - Marcar reserva como completada (Admin/Instructor)
- `POST /api/reservations/{id}/no-show` - Marcar como no presentado (Admin/Instructor)
- `DELETE /api/reservations/{id}` - Eliminar reserva (Solo Admin)
- `GET /api/reservations/student/{studentId}/can-reserve/{classId}` - Verificar elegibilidad
- `GET /api/reservations/{id}/can-cancel` - Verificar si puede cancelar
- `POST /api/reservations/process-completed` - Procesar reservas completadas (Solo Admin)

### 💰 Pagos (🚧 Próximamente)
- `GET /api/payments` - Listar pagos
- `POST /api/payments` - Procesar pago
- `GET /api/payments/{id}` - Obtener pago

*Documentación completa disponible en `/swagger`*

## 🔐 Autenticación

### JWT (JSON Web Tokens)

La API utiliza JWT para autenticación y autorización:

1. **Login**: Envía credenciales para recibir access token y refresh token
2. **Autorización**: Incluye access token en headers de requests
3. **Refresh**: Usa refresh token para obtener nuevo access token
4. **Roles**: Los tokens incluyen información de roles para autorización

### Roles del Sistema

- **Admin**: Acceso completo al sistema
- **Instructor**: Gestión de sus clases y estudiantes
- **Student**: Acceso a reservas y perfil personal

### Políticas de Autorización

```csharp
[Authorize(Policy = "AdminOnly")]        // Solo administradores
[Authorize(Policy = "InstructorOnly")]   // Instructores y admins
[Authorize(Policy = "StudentOnly")]      // Todos los usuarios autenticados
```

## 💻 Desarrollo

### Comandos Útiles

```bash
# Ejecutar en modo desarrollo
dotnet run

# Ejecutar con hot reload
dotnet watch run

# Ejecutar tests
dotnet test

# Compilar para producción
dotnet publish -c Release

# Verificar formato de código
dotnet format

# Restaurar paquetes
dotnet restore
```

### Configuración de Desarrollo

1. **Entorno**: Usa SQLite para desarrollo local
2. **Hot Reload**: Configurado para desarrollo ágil
3. **Logging**: Nivel Debug en desarrollo
4. **CORS**: Configurado para localhost
5. **Swagger**: Habilitado solo en desarrollo

### Base de Datos de Desarrollo

```bash
# Resetear base de datos de desarrollo
rm aline.db
dotnet ef database update
```

### Estructura de Branches

- `main` - Rama principal (producción)
- `develop` - Rama de desarrollo
- `feature/*` - Ramas de características
- `hotfix/*` - Ramas de correcciones urgentes

## 🚀 Deployment

### Producción

1. **Compilar para producción**:
```bash
dotnet publish -c Release -o ./publish
```

2. **Configurar variables de entorno**:
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="tu-connection-string-mysql"
```

3. **Ejecutar**:
```bash
dotnet PilatesStudioAPI.dll
```

### Docker (Opcional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PilatesStudioAPI.dll"]
```

### Variables de Entorno Producción

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `EmailSettings__Username`
- `EmailSettings__Password`

## 📈 Roadmap

### Fase 1: ✅ Configuración Base (Completada)
- [x] Infraestructura base
- [x] Configuración de servicios
- [x] Base de datos y migraciones
- [x] Autenticación JWT

### Fase 2: ✅ Autenticación y Usuarios (Completada)
- [x] Endpoints de autenticación completos
- [x] Gestión completa de usuarios
- [x] Perfiles de instructor/estudiante
- [x] Validaciones exhaustivas y DTOs
- [x] JWT Service con access/refresh tokens
- [x] Controladores con autorización por roles
- [x] AutoMapper profiles configurados
- [x] FluentValidation implementado

### Fase 3: ✅ Gestión de Clases y Horarios (Completada)
- [x] CRUD completo de zonas/salas con validaciones
- [x] CRUD completo de clases con horarios
- [x] Sistema de programación y asignación de instructores
- [x] Control de conflictos de horario automático
- [x] Consultas y filtros avanzados para clases
- [x] Validaciones de capacidad y lógica de negocio

### Fase 4: ✅ Planes y Suscripciones (Completada)
- [x] CRUD completo de planes con validaciones avanzadas
- [x] Sistema integral de suscripciones con estados
- [x] Lógica de expiración y control automático
- [x] Sistema de renovaciones y gestión de clases restantes
- [x] Validaciones de suscripciones activas para reservas
- [x] Filtros avanzados y consultas especializadas

### Fase 5: ✅ Sistema de Reservas (Completada)
- [x] CRUD completo de reservas con validaciones automáticas
- [x] Control inteligente de capacidad de clases
- [x] Sistema de cancelaciones con políticas de tiempo
- [x] Estados de reserva (confirmada, cancelada, completada, no asistió)
- [x] Validaciones de suscripciones activas para reservar
- [x] Control automático de descuento de clases restantes
- [x] Verificación de elegibilidad en tiempo real

### Fase 6: 💰 Gestión de Pagos
- [ ] Procesamiento de pagos
- [ ] Múltiples métodos de pago
- [ ] Facturación
- [ ] Reportes financieros

### Fase 7: 📊 Reportes y Analytics
- [ ] Dashboard administrativo
- [ ] Métricas de negocio
- [ ] Reportes de uso
- [ ] Analytics en tiempo real

### Fase 8: 🔧 Optimización y Testing
- [ ] Tests unitarios
- [ ] Tests de integración
- [ ] Optimización de performance
- [ ] Documentación completa

## 🤝 Contribución

### Cómo Contribuir

1. **Fork** el proyecto
2. **Crea** una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. **Push** a la rama (`git push origin feature/AmazingFeature`)
5. **Abre** un Pull Request

### Estándares de Código

- Seguir convenciones de nomenclatura de C#
- Comentar código complejo
- Escribir tests para nuevas funcionalidades
- Actualizar documentación cuando sea necesario

### Reportar Bugs

Abre un issue en GitHub con:
- Descripción del problema
- Pasos para reproducir
- Comportamiento esperado vs actual
- Información del entorno

## 📝 Changelog

### [5.0.0] - 2025-09-11
#### 🎉 Fase 5 Completada: Sistema de Reservas
- ✅ **Sistema Completo de Reservas**
  - CRUD completo con validaciones automáticas de elegibilidad
  - Control inteligente de capacidad de clases en tiempo real
  - Validación automática de suscripciones activas antes de reservar
  - Prevención de reservas duplicadas para la misma clase
- ✅ **Gestión Avanzada de Estados**
  - Estados completos (confirmada, cancelada, completada, no asistió)
  - Transiciones automáticas con validaciones de negocio
  - Procesamiento batch de reservas completadas
  - Control de tiempo para cambios de estado
- ✅ **Sistema de Cancelaciones con Políticas**
  - Política configurable de cancelación (2 horas antes por defecto)
  - Validación de elegibilidad para cancelar en tiempo real
  - Motivos de cancelación opcionales para auditoría
  - Integración con sistema de suscripciones para recuperar clases
- ✅ **Validaciones Integrales de Negocio**
  - Verificación automática de capacidad disponible
  - Control de fechas y horarios (no reservar en el pasado)
  - Validación de suscripciones con clases restantes
  - Descuento automático de clases al confirmar reserva
- ✅ **Consultas Especializadas y Filtros**
  - Filtros avanzados por estudiante, clase, instructor, zona, fechas
  - Consultas de reservas próximas con cálculos de tiempo
  - Dashboard para instructores con sus reservas
  - Verificaciones de elegibilidad en tiempo real

#### 🔧 Mejoras Arquitectónicas
- AutoMapper con lógica compleja para cálculos de tiempo y estados
- Repositorio con consultas optimizadas para múltiples escenarios
- Servicios con validaciones multicapa y lógica de negocio robusta
- Controladores con autorización granular según funcionalidad

### [4.0.0] - 2025-09-11
#### 🎉 Fase 4 Completada: Planes y Suscripciones
- ✅ **Sistema Completo de Planes**
  - CRUD completo con validaciones de precios y capacidades
  - Control de títulos únicos y estados activo/inactivo
  - Gestión de validez en días y clases mensuales incluidas
  - Soft delete inteligente cuando hay suscripciones activas
- ✅ **Sistema Avanzado de Suscripciones**
  - CRUD completo con control de estados (activa, vencida, cancelada)
  - Gestión automática de fechas de expiración y clases restantes
  - Sistema de renovación de suscripciones con cambio de plan
  - Control de suscripciones activas por estudiante (una por vez)
- ✅ **Lógica de Negocio Robusta**
  - Validaciones de suscripciones activas para permitir reservas
  - Descuento automático de clases al usar reservas
  - Procesamiento masivo de suscripciones vencidas
  - Control de expiración con alertas tempranas configurables
- ✅ **Consultas Especializadas**
  - Filtrado avanzado por estudiante, plan, estado y fechas
  - Consultas de suscripciones por vencer con umbral configurable
  - Verificación de elegibilidad para reservas de clases
  - Dashboard de suscripciones activas y vencidas
- ✅ **Validaciones y DTOs Exhaustivos**
  - FluentValidation con reglas de negocio específicas
  - DTOs con información calculada (días restantes, clases usadas)
  - Mapeo automático con AutoMapper incluyendo datos derivados
  - Estados de expiración con alertas visuales

#### 🔧 Mejoras Arquitectónicas
- Repositorio Student completado para integridad de dependencias
- AutoMapper configurado con lógica compleja de mapeo para suscripciones
- Controladores con autorización granular por funcionalidad
- Servicios con validaciones de lógica de negocio específicas del dominio

### [3.0.0] - 2025-09-11
#### 🎉 Fase 3 Completada: Gestión de Clases y Horarios
- ✅ **Sistema Completo de Zonas/Salas**
  - CRUD completo con validaciones de capacidad
  - Control de estados activo/inactivo con soft delete inteligente
  - Validación de nombres únicos y capacidades
  - Gestión de equipamiento disponible
- ✅ **Sistema Avanzado de Clases**
  - CRUD completo con programación de horarios
  - Asignación automática de instructores y zonas
  - Control de conflictos de horario en tiempo real
  - Sistema de estados (programada, en curso, completada, cancelada)
  - Validaciones de duración mínima y máxima de clase
- ✅ **Consultas y Filtros Inteligentes**
  - Filtrado avanzado por fecha, instructor, zona, nivel y disponibilidad
  - Endpoints para verificación de conflictos de programación
  - Consultas de clases disponibles con espacios libres
  - Optimización de queries con Include para navegación
- ✅ **Validaciones de Lógica de Negocio**
  - Prevención de doble booking de instructores y zonas
  - Control automático de capacidad vs reservas
  - Validaciones de horarios coherentes y restricciones temporales
  - Soft delete inteligente cuando hay dependencias
- ✅ **Repositorios y Servicios Robustos**
  - Implementación del patrón Repository completo
  - Servicios de negocio con validaciones exhaustivas
  - Mapeo automático entre entidades y DTOs
  - Manejo de errores específicos por tipo de operación

#### 🔧 Mejoras Arquitectónicas
- Patrón Repository implementado para mejor separación de concerns
- AutoMapper configurado para entidades Zone y Class
- FluentValidation con reglas de negocio específicas
- Controladores con autorización granular por rol
- Logging estructurado en todas las operaciones de clase y zona

### [2.0.0] - 2025-09-10
#### 🎉 Fase 2 Completada: Autenticación y Gestión de Usuarios
- ✅ **Sistema de Autenticación JWT Completo**
  - Registro de usuarios con validaciones exhaustivas
  - Login con generación de access y refresh tokens  
  - Endpoints para cambio de contraseña y logout
  - Preparado para verificación de email y reset de contraseña
- ✅ **Gestión Completa de Usuarios**
  - CRUD completo para instructores con autorización por roles
  - CRUD completo para estudiantes con búsqueda
  - Perfiles específicos para cada tipo de usuario
  - Activación/desactivación de usuarios
- ✅ **Validaciones y DTOs Robustos** 
  - FluentValidation implementado en todos los endpoints
  - DTOs específicos para cada operación
  - Validaciones de contraseña con regex
  - Mapeos automáticos con AutoMapper
- ✅ **Seguridad Avanzada**
  - Creación automática de roles en base de datos
  - Políticas de autorización granulares
  - JWT con claims personalizados
  - Middleware de manejo global de errores

#### 🔧 Mejoras Técnicas
- AutoMapper profiles configurados para todas las entidades
- Servicios con patrón de inyección de dependencias
- Logging estructurado en todas las operaciones
- Validación de timestamps mejorada para Identity
- Arquitectura limpia con separación de responsabilidades

### [1.0.0] - 2025-09-09
#### Agregado
- ✅ Configuración base del proyecto ASP.NET Core 8
- ✅ Integración completa de Entity Framework Core
- ✅ Sistema de autenticación JWT con Identity Framework
- ✅ 11 entidades del dominio implementadas
- ✅ Migraciones iniciales de base de datos
- ✅ Middleware de manejo global de errores
- ✅ Configuración de Swagger/OpenAPI
- ✅ Logging estructurado con Serilog
- ✅ Configuración CORS para frontend
- ✅ Validaciones con FluentValidation
- ✅ Mapeo de objetos con AutoMapper
- ✅ Arquitectura preparada para patrón Repository

#### Configurado
- 🔒 Autorización por roles (Admin, Instructor, Student)
- 🗄️ Soporte para MySQL (producción) y SQLite (desarrollo)
- ⚙️ Variables de configuración externalizadas
- 📖 Documentación completa de API

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autores

- **Franco** - *Desarrollo inicial* - [@fRankete107](https://github.com/fRankete107)
- **Claude** - *Asistente de desarrollo* - Anthropic

## 🙏 Agradecimientos

- ASP.NET Core Team por el excelente framework
- Entity Framework Core por el ORM robusto
- Comunidad .NET por las mejores prácticas
- Swagger/OpenAPI por la documentación automática

---

## 📞 Soporte

¿Necesitas ayuda? Contacta:

- 📧 Email: francogames107@gmail.com
- 🐛 Issues: [GitHub Issues](https://github.com/fRankete107/aline-back/issues)
- 📚 Documentación: [Swagger UI](http://localhost:5121/swagger)

---

**⭐ Si este proyecto te resulta útil, considera darle una estrella en GitHub!**