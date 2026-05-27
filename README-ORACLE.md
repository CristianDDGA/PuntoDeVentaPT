# 🛒 Sistema Punto de Venta
### Oracle 26ai + Docker + Entity Framework Core

> Guía de configuración para levantar el entorno de base de datos desde cero.

---

## 📋 Requisitos Previos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y en ejecución
- Visual Studio con soporte para .NET
- Acceso a NuGet (conexión a internet)

---

## 🚀 Pasos de Configuración

### Paso 1 — Levantar el contenedor de Oracle

Abre **PowerShell** y ejecuta:

```bash
docker run -d --name PUNTOVENTA -p 1522:1521 -e ORACLE_PASSWORD=PUNTOVENTA container-registry.oracle.com/database/free:latest
```

> ⏳ **Importante:** Oracle tarda entre **3 y 5 minutos** en inicializar internamente. Verifica el estado con `docker ps` y espera a que el `Status` muestre **(healthy)** antes de continuar.

---

### Paso 2 — Crear el usuario de la aplicación en Oracle

Entra a la terminal del contenedor:

```bash
docker exec -it PUNTOVENTA bash
```

Conéctate como superadministrador:

```bash
sqlplus / as sysdba
```

Cambia a la base de datos conectable:

```sql
ALTER SESSION SET CONTAINER = FREEPDB1;
```

Crea el usuario y otorga permisos:

```sql
CREATE USER PUNTOVENTA_USER IDENTIFIED BY PUNTOVENTA;
GRANT CONNECT, RESOURCE, DBA TO PUNTOVENTA_USER;
ALTER USER PUNTOVENTA_USER QUOTA UNLIMITED ON USERS;
```

Sal de la consola:

```bash
exit;
exit
```

---

### Paso 3 — Verificar `appsettings.json`

En el proyecto **`PuntoVenta.API`**, asegúrate de que la cadena de conexión sea la siguiente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1522/FREEPDB1;User Id=PUNTOVENTA_USER;Password=PUNTOVENTA;"
  }
}
```

---

### Paso 4 — Aplicar las migraciones

> ✅ **No es necesario ejecutar `Add-Migration`.** Las migraciones ya están incluidas en el repositorio.

1. Abre **Visual Studio**
2. Ve a: `Herramientas` → `Administrador de paquetes NuGet` → `Consola del Administrador de paquetes`
3. En el menú **Proyecto predeterminado**, selecciona obligatoriamente: **`PuntoVenta.Infrastructure`**
4. Ejecuta:

```powershell
Update-Database
```

Entity Framework compilará la solución, se conectará a Docker por el puerto `1522` y creará las tablas (`Customers`, `Products`, `Sales`, etc.) junto con los datos de prueba automáticamente.

---

## 🔍 Verificación Rápida

Para confirmar que las tablas y los datos se crearon correctamente, conéctate al contenedor con el usuario de la aplicación:

```bash
docker exec -it PUNTOVENTA bash
sqlplus PUNTOVENTA_USER/PUNTOVENTA@//localhost:1521/FREEPDB1
```

Ejecuta las siguientes consultas (**con comillas dobles**, requerido por Oracle para nombres generados desde C#):

```sql
SELECT * FROM "Customers";
SELECT * FROM "Products";
```

---

## ▶️ Ejecutar el Proyecto

Una vez completados los pasos anteriores, presiona **Play** en Visual Studio.  
La API estará disponible con documentación interactiva en **Swagger**.

---

## 🗂️ Estructura del Proyecto

```
PuntoVenta/
├── PuntoVenta.API/           → Capa de presentación (Controllers, Swagger)
├── PuntoVenta.Application/   → Lógica de negocio (Use Cases)
├── PuntoVenta.Domain/        → Entidades y contratos
└── PuntoVenta.Infrastructure/→ Acceso a datos (EF Core, Migraciones)
```

---

## ⚙️ Configuración Rápida (Resumen)

| Parámetro        | Valor              |
|------------------|--------------------|
| Nombre contenedor| `PUNTOVENTA`       |
| Puerto externo   | `1522`             |
| Puerto interno   | `1521`             |
| Base de datos    | `FREEPDB1`         |
| Usuario          | `PUNTOVENTA_USER`  |
| Contraseña       | `PUNTOVENTA`       |

---

> 💡 **¿Por qué el puerto 1522?** Se usa para evitar conflictos con instalaciones locales previas de Oracle (como Oracle 10g) que ya ocupan el puerto `1521`.
