# Diseño de Base de Datos - Sistema de Clases de Pilates

## Análisis del Proyecto

Este proyecto consiste en un sistema web para un negocio de clases de pilates que permite:
- Gestión de clases y horarios
- Venta de planes y sesiones
- Administración de alumnos y sus reservas
- Control de pagos y facturación
- Manejo de consultas de contacto

## Estructura de Tablas Optimizada

### 1. USUARIOS (users)
**Propósito**: Tabla principal para la autenticación y datos básicos de usuarios del sistema.

```sql
CREATE TABLE users (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('admin', 'instructor', 'student') NOT NULL DEFAULT 'student',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    email_verified_at TIMESTAMP NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### 2. INSTRUCTORES (instructors)
**Propósito**: Información específica de los profesores de pilates.

```sql
CREATE TABLE instructors (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id BIGINT NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    specializations TEXT,
    bio TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_instructor_active (is_active),
    INDEX idx_instructor_name (first_name, last_name)
);
```

### 3. ALUMNOS (students)
**Propósito**: Información de los estudiantes/clientes.

```sql
CREATE TABLE students (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    user_id BIGINT NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    birth_date DATE,
    nit VARCHAR(50) NULL,
    emergency_contact VARCHAR(255),
    medical_notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_student_name (first_name, last_name),
    INDEX idx_student_phone (phone)
);
```

### 4. PLANES (plans)
**Propósito**: Tipos de planes disponibles para los estudiantes.

```sql
CREATE TABLE plans (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    title VARCHAR(255) NOT NULL,
    subtitle VARCHAR(255),
    description TEXT,
    price DECIMAL(10,2) NOT NULL,
    monthly_classes INT NOT NULL,
    validity_days INT NOT NULL DEFAULT 30,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_plan_active (is_active),
    INDEX idx_plan_price (price)
);
```

### 5. SUSCRIPCIONES (subscriptions)
**Propósito**: Registro de planes activos de cada alumno.

```sql
CREATE TABLE subscriptions (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    student_id BIGINT NOT NULL,
    plan_id BIGINT NOT NULL,
    classes_remaining INT NOT NULL,
    start_date DATE NOT NULL,
    expiry_date DATE NOT NULL,
    status ENUM('active', 'expired', 'cancelled') NOT NULL DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE CASCADE,
    FOREIGN KEY (plan_id) REFERENCES plans(id),
    INDEX idx_subscription_student (student_id),
    INDEX idx_subscription_status (status),
    INDEX idx_subscription_expiry (expiry_date)
);
```

### 6. ZONAS (zones)
**Propósito**: Definir las diferentes áreas o salas donde se imparten las clases.

```sql
CREATE TABLE zones (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    capacity INT NOT NULL,
    equipment_available TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_zone_active (is_active)
);
```

### 7. CLASES (classes)
**Propósito**: Programación de clases individuales.

```sql
CREATE TABLE classes (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    instructor_id BIGINT NOT NULL,
    zone_id BIGINT NOT NULL,
    class_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    capacity_limit INT NOT NULL,
    class_type VARCHAR(100),
    difficulty_level ENUM('beginner', 'intermediate', 'advanced') DEFAULT 'beginner',
    description TEXT,
    status ENUM('scheduled', 'ongoing', 'completed', 'cancelled') NOT NULL DEFAULT 'scheduled',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (instructor_id) REFERENCES instructors(id),
    FOREIGN KEY (zone_id) REFERENCES zones(id),
    INDEX idx_class_date (class_date),
    INDEX idx_class_instructor (instructor_id),
    INDEX idx_class_zone (zone_id),
    INDEX idx_class_status (status),
    UNIQUE KEY unique_class_schedule (instructor_id, class_date, start_time)
);
```

### 8. RESERVAS (reservations)
**Propósito**: Registro de inscripciones de alumnos a clases específicas.

```sql
CREATE TABLE reservations (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    class_id BIGINT NOT NULL,
    student_id BIGINT NOT NULL,
    subscription_id BIGINT NOT NULL,
    reservation_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status ENUM('confirmed', 'cancelled', 'completed', 'no_show') NOT NULL DEFAULT 'confirmed',
    cancellation_reason TEXT,
    cancelled_at TIMESTAMP NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (class_id) REFERENCES classes(id) ON DELETE CASCADE,
    FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE CASCADE,
    FOREIGN KEY (subscription_id) REFERENCES subscriptions(id),
    UNIQUE KEY unique_student_class (class_id, student_id),
    INDEX idx_reservation_class (class_id),
    INDEX idx_reservation_student (student_id),
    INDEX idx_reservation_status (status)
);
```

### 9. PAGOS (payments)
**Propósito**: Registro de todos los pagos realizados.

```sql
CREATE TABLE payments (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    student_id BIGINT NOT NULL,
    plan_id BIGINT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    payment_method ENUM('cash', 'credit_card', 'debit_card', 'bank_transfer', 'digital_wallet') NOT NULL,
    payment_reference VARCHAR(255),
    receipt_number VARCHAR(100) UNIQUE,
    payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status ENUM('pending', 'completed', 'failed', 'refunded') NOT NULL DEFAULT 'pending',
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (student_id) REFERENCES students(id),
    FOREIGN KEY (plan_id) REFERENCES plans(id),
    INDEX idx_payment_student (student_id),
    INDEX idx_payment_date (payment_date),
    INDEX idx_payment_status (status),
    INDEX idx_payment_receipt (receipt_number)
);
```

### 10. CONTACTOS (contacts)
**Propósito**: Registro de consultas y mensajes de contacto.

```sql
CREATE TABLE contacts (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    subject VARCHAR(255),
    message TEXT NOT NULL,
    status ENUM('new', 'in_progress', 'resolved', 'closed') NOT NULL DEFAULT 'new',
    assigned_to BIGINT NULL,
    response TEXT,
    responded_at TIMESTAMP NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (assigned_to) REFERENCES users(id),
    INDEX idx_contact_status (status),
    INDEX idx_contact_date (created_at),
    INDEX idx_contact_email (email)
);
```

### 11. AUDITORÍA (audit_log)
**Propósito**: Registro de cambios importantes para trazabilidad.

```sql
CREATE TABLE audit_log (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    table_name VARCHAR(100) NOT NULL,
    record_id BIGINT NOT NULL,
    action ENUM('INSERT', 'UPDATE', 'DELETE') NOT NULL,
    user_id BIGINT,
    old_values JSON,
    new_values JSON,
    ip_address VARCHAR(45),
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id),
    INDEX idx_audit_table (table_name),
    INDEX idx_audit_date (created_at),
    INDEX idx_audit_user (user_id)
);
```

## Mejoras y Optimizaciones Implementadas

### 1. Normalización Mejorada
- **Separación de usuarios y estudiantes**: Permite que un usuario pueda tener diferentes roles
- **Tabla de zonas independiente**: Facilita la gestión de espacios físicos
- **Suscripciones como entidad separada**: Mejor control de planes activos

### 2. Integridad de Datos
- **Claves foráneas con restricciones**: Mantienen la consistencia referencial
- **Constraints únicos**: Previenen duplicados no deseados
- **Validaciones a nivel de base de datos**: Estados enum predefinidos

### 3. Índices Estratégicos
- **Índices compuestos**: Para consultas frecuentes
- **Índices por fechas**: Optimizan búsquedas temporales
- **Índices por estado**: Mejoran filtros comunes

### 4. Metadata Agregada
- **Timestamps automáticos**: created_at, updated_at
- **Estados de entidades**: Para control de flujo
- **Campos de trazabilidad**: IP, user agent en auditoría
- **Campos de configuración**: is_active, validity_days

### 5. Escalabilidad
- **BIGINT para IDs**: Soporta alto volumen de registros
- **Particionamiento preparado**: Índices optimizados por fecha
- **Diseño modular**: Fácil extensión de funcionalidades

## Consideraciones Adicionales

### Triggers Recomendados
```sql
-- Actualizar clases disponibles después de reserva
DELIMITER $$
CREATE TRIGGER update_classes_remaining 
AFTER INSERT ON reservations
FOR EACH ROW
BEGIN
    UPDATE subscriptions 
    SET classes_remaining = classes_remaining - 1 
    WHERE id = NEW.subscription_id;
END$$
DELIMITER ;
```

### Vistas Útiles
```sql
-- Vista de clases con información completa
CREATE VIEW class_details AS
SELECT 
    c.id,
    c.class_date,
    c.start_time,
    c.end_time,
    CONCAT(i.first_name, ' ', i.last_name) as instructor_name,
    z.name as zone_name,
    c.capacity_limit,
    COUNT(r.id) as enrolled_students,
    c.status
FROM classes c
LEFT JOIN instructors i ON c.instructor_id = i.id
LEFT JOIN zones z ON c.zone_id = z.id
LEFT JOIN reservations r ON c.id = r.class_id AND r.status = 'confirmed'
GROUP BY c.id;
```

Esta estructura proporciona una base sólida, escalable y mantenible para el sistema de gestión de clases de pilates, siguiendo las mejores prácticas de diseño de bases de datos relacionales.