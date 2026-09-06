# TicketProcessor

API backend para registrar, clasificar y procesar tickets de soporte en segundo plano.

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET 10
* **API:** ASP.NET Core Minimal API
* **Clasificación con IA:** Gemini
* **Procesamiento en segundo plano:** Hangfire
* **Acceso a Datos:** Dapper
* **Base de datos:** SQL Server

---

## ⚙️ Requisitos y Configuración Inicial

Para ejecutar el proyecto localmente necesitas el SDK de .NET 10, una instancia de SQL Server y una clave de API de Gemini.

1. **Preparar la base de datos:**
   * Conéctate a tu servidor local de SQL Server.
   * Ejecuta el script `query.sql` incluido en la raíz del repositorio. Creará la base de datos `TicketProcessorDb` y la tabla `TicketsSoporte`.

2. **Configurar la aplicación:**
   * Define la cadena de conexión bajo `ConnectionStrings:BdConexion`.
   * Define la clave de Gemini bajo `GeminiApiKey`.
   > **Nota:** Si usas contenedores locales (especialmente en macOS), asegúrate de incluir `TrustServerCertificate=True`.

3. **Arrancar la API:**
   * Abre tu terminal en la carpeta del proyecto y ejecuta el comando `dotnet run`.

4. **Acceder al panel de Hangfire:**
   * Navega a la URL que devuelve la consola añadiendo la ruta `/hangfire` (ejemplo: `http://localhost:5xxx/hangfire`). 
   * El proyecto registra un trabajo recurrente para procesar los tickets pendientes.
