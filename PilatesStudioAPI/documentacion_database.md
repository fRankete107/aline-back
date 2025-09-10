Estoy trabajando en clonar un proyecto de un cliente y quiero diagramar la base de datos.

El proyecto es un a página para el negocio de mi cliente. Mi cliente imparte clases de pilates y mediante este proyecto vende sus sesiones y planes para éstas mismas.

El proyecto tendrá también un sector de alumnos donde se guardará la información de los clientes. Obviamente, esta sección estará directamente relacionada con los planes y clases.

TAREA: Necesito que analices el caso con mucha cautela y crees un markdown acerca de como crear esta base de datos modificando las tablas a conveniencia para seguir las mejores prácticas, llegar al objetivo del proyecto y agregando metadata.

Las tablas y campos particulares son:

CLASES
- Fecha
- Hora inicio
- Hora fin
- Zona [número] (Aún a descubrir de qué se trata esta variable, solo sé que tiene "Zona" y luego un número)
- Profesor
- Límite
- Array de alumnos inscriptos

RESERVAS
- ID Clase
- ID Alumno

PAGO
- ID Plan
- Fecha y hora
- Precio
- ID Comprobante de pago

PLANES
- Título
- Subtítulo
- Descripción
- Precio
- Cantidad de clases mensuales

ALUMNO
- Nombre
- Apellido
- Teléfono
- Correo
- Cumpleaños
- NIT (opcional)
- Plan {
    - ID Plan
    - Clases disponibles
}
- ID Reserva (arr) (opcional)
- ID Pago (arr) (opcional)

CONTACTO
- Nombre
- Apellido
- Teléfono
- Correo
- Mensaje