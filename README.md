# 🚀 Motor Automático de Procesamiento de Tickets

Un servicio backend robusto construido para la clasificación y el procesamiento de tickets de soporte técnico en segundo plano. Diseñado con una arquitectura limpia, tolerante a fallos y optimizado para ejecutar operaciones de alto rendimiento en base de datos.

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET 8 (Web API)
* **Orquestación de Tareas:** Hangfire (Background Jobs)
* **Acceso a Datos:** Dapper (Micro-ORM para máximo rendimiento)
* **Base de Datos:** SQL Server 2022

---

## ⚙️ Requisitos y Configuración Inicial

Para levantar este proyecto en tu entorno local, necesitarás tener instalado el SDK de .NET 8 y una instancia de SQL Server (se recomienda Docker). Sigue estos pasos para arrancar el motor:

1. **Preparar la Base de Datos:**
   * Conéctate a tu servidor local de SQL Server.
   * Ejecuta el script `query.sql` incluido en la raíz del repositorio. Esto creará automáticamente la base de datos `SistemaPedidosCorp`, las tablas necesarias y algunos datos de prueba.

2. **Configurar las Credenciales (Importante):**
   * Por seguridad, la cadena de conexión real no está subida al repositorio.
   * Abre o crea el archivo `appsettings.json` y define tu propia conexión bajo la clave `BdConexion`. 
   > **Nota:** Si usas contenedores locales (especialmente en macOS), asegúrate de incluir `TrustServerCertificate=True`.

3. **Arrancar la API:**
   * Abre tu terminal en la carpeta del proyecto y ejecuta el comando `dotnet run`.

4. **Acceder al Panel de Control:**
   * Navega a la URL que devuelve la consola añadiendo la ruta `/hangfire` (ejemplo: `http://localhost:5xxx/hangfire`). 
   * Desde este panel podrás monitorizar el procesamiento automático que se ejecuta cada minuto.

---

## 🗺️ Roadmap (Próximos Pasos)

* [x] Arquitectura base e inyección de dependencias.
* [x] Implementación de Hangfire para tareas recurrentes.
* [x] Clasificador de tickets simulado (Dummy Service) usando Dapper.
* [ ] **Integración de IA:** Sustituir el servicio simulado por una conexión real a la API de OpenAI para entender el contexto semántico de cada ticket.