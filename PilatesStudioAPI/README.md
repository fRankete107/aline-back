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
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

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
https://localhost:5001/swagger
```

### Ejemplos de Uso

#### Registrar un nuevo usuario
```bash
curl -X POST "https://localhost:5001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "estudiante@example.com",
    "password": "MiPassword123!",
    "firstName": "Juan",
    "lastName": "Pérez",
    "role": "student"
  }'
```

#### Login
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "estudiante@example.com",
    "password": "MiPassword123!"
  }'
```

## 🏗️ Estructura del Proyecto

```
PilatesStudioAPI/
├── 📁 Configuration/          # Clases de configuración
│   ├── CorsSettings.cs
│   ├── EmailSettings.cs
│   └── JwtSettings.cs
├── 📁 Controllers/            # Controladores de API
│   └── WeatherForecastController.cs
├── 📁 Data/                   # Contexto de base de datos
│   ├── Context/
│   │   ├── PilatesStudioDbContext.cs
│   │   └── PilatesStudioDbContextFactory.cs
│   └── Configurations/        # Configuraciones EF Core
├── 📁 Models/                 # Modelos de dominio
│   ├── Entities/             # Entidades de EF Core
│   │   ├── User.cs
│   │   ├── Instructor.cs
│   │   ├── Student.cs
│   │   ├── Plan.cs
│   │   ├── Subscription.cs
│   │   ├── Zone.cs
│   │   ├── Class.cs
│   │   ├── Reservation.cs
│   │   ├── Payment.cs
│   │   ├── Contact.cs
│   │   └── AuditLog.cs
│   ├── DTOs/                 # Data Transfer Objects
│   └── Requests/             # Modelos de request
├── 📁 Services/              # Lógica de negocio
│   ├── Interfaces/
│   └── Implementations/
├── 📁 Repositories/          # Patrón Repository
│   ├── Interfaces/
│   └── Implementations/
├── 📁 Middleware/            # Middlewares personalizados
│   └── GlobalExceptionMiddleware.cs
├── 📁 Extensions/            # Métodos de extensión
├── 📁 Utils/                 # Utilidades y helpers
├── 📁 Migrations/            # Migraciones EF Core
├── Program.cs                # Punto de entrada
├── appsettings.json          # Configuración
└── README.md                 # Este archivo
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

### Autenticación
- `POST /api/auth/register` - Registrar usuario
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/refresh` - Renovar token
- `POST /api/auth/logout` - Cerrar sesión

### Usuarios
- `GET /api/users` - Listar usuarios
- `GET /api/users/{id}` - Obtener usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

### Instructores
- `GET /api/instructors` - Listar instructores
- `POST /api/instructors` - Crear instructor
- `GET /api/instructors/{id}` - Obtener instructor
- `PUT /api/instructors/{id}` - Actualizar instructor

### Estudiantes
- `GET /api/students` - Listar estudiantes
- `POST /api/students` - Crear estudiante
- `GET /api/students/{id}` - Obtener estudiante
- `PUT /api/students/{id}` - Actualizar estudiante

### Planes
- `GET /api/plans` - Listar planes
- `POST /api/plans` - Crear plan
- `GET /api/plans/{id}` - Obtener plan
- `PUT /api/plans/{id}` - Actualizar plan

### Clases
- `GET /api/classes` - Listar clases
- `POST /api/classes` - Crear clase
- `GET /api/classes/{id}` - Obtener clase
- `PUT /api/classes/{id}` - Actualizar clase
- `GET /api/classes/available` - Clases disponibles

### Reservas
- `GET /api/reservations` - Listar reservas
- `POST /api/reservations` - Crear reserva
- `PUT /api/reservations/{id}/cancel` - Cancelar reserva

### Pagos
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

### Fase 2: 🔄 Autenticación y Usuarios (En Progreso)
- [ ] Endpoints de autenticación
- [ ] Gestión de usuarios
- [ ] Perfiles de instructor/estudiante
- [ ] Validaciones y DTOs

### Fase 3: 📅 Gestión de Clases
- [ ] CRUD de clases
- [ ] Sistema de horarios
- [ ] Gestión de zonas
- [ ] Validaciones de negocio

### Fase 4: 💳 Planes y Suscripciones
- [ ] CRUD de planes
- [ ] Sistema de suscripciones
- [ ] Lógica de expiración
- [ ] Renovaciones automáticas

### Fase 5: 📝 Sistema de Reservas
- [ ] Booking de clases
- [ ] Validaciones de capacidad
- [ ] Cancelaciones
- [ ] Notificaciones

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

### [1.0.0] - 2024-09-10
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
- 📚 Documentación: [Swagger UI](https://localhost:5001/swagger)

---

**⭐ Si este proyecto te resulta útil, considera darle una estrella en GitHub!**