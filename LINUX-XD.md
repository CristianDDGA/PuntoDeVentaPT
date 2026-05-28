# 🚀 Guía de Despliegue — Sistema Punto de Venta
### Grupo 4

> Arquitectura: **Oracle AI Database 23ai Free** · **Docker** · **Arch Linux** · **WSL 2** · **.NET Core / C#**

---

## 📋 Tabla de Contenido

1. [Prerrequisitos en Windows](#-prerrequisitos-en-windows)
2. [Instalación de Arch Linux](#-1-instalación-de-arch-linux-doble-clic)
3. [Inicialización de Docker](#-2-conexión-e-inicialización-de-docker)
4. [Despliegue de Oracle Database](#-3-despliegue-de-oracle-database-23ai)
5. [Configuración del Esquema](#-4-configuración-del-esquema-y-permisos)
6. [Conexión del Backend](#-5-conexión-del-backend-net-core--c)

---

## 🛠️ Prerrequisitos en Windows

Antes de iniciar, active las características de virtualización y WSL 2:

1. Abra **PowerShell como Administrador** y ejecute:

```powershell
wsl --install
```

2. Reinicie el equipo si el sistema lo solicita.

---

## 📦 1. Instalación de Arch Linux (doble clic)

El entorno viene empaquetado como inicializador nativo de WSL, sin necesidad de comandos de importación manuales.

1. Extraiga el contenido de `Arch.zip` en la carpeta de su preferencia (ej: `C:\WSL\Arch`).
2. Ingrese a la carpeta extraída y haga **doble clic** en `Arch.exe`.
3. Se abrirá una ventana de comandos mientras se desempaca y registra la distribución.
4. Cuando aparezca el prompt `[root@...]#`, el proceso ha finalizado.
5. Cierre la ventana. La distribución queda registrada permanentemente en WSL 2.

---

## 🐧 2. Conexión e Inicialización de Docker

> ⚠️ **Importante:** Cada vez que Windows se reinicie, el demonio de Docker debe iniciarse manualmente dentro de Arch Linux.

Conéctese a la instancia desde PowerShell o CMD:

```powershell
wsl -d Arch
```

Una vez dentro de Arch Linux, levante el demonio de Docker:

```bash
# Asegurar la existencia de los directorios del socket
mkdir -p /run /run/lock /var/run

# Levantar Docker en segundo plano
/usr/bin/dockerd --init &
```

> 💡 Si los logs bloquean la pantalla, presione **Enter** una vez para recuperar la consola.

Verifique que Docker responda correctamente:

```bash
docker ps
```

---

## 🛢️ 3. Despliegue de Oracle Database 23ai

Se utiliza la imagen `gvenzl/oracle-free:23-faststart`, optimizada para arranque rápido y bajo consumo de recursos.

### Paso A — Crear el contenedor

El puerto alterno **1522** se mapea al puerto nativo **1521** de Oracle:

```bash
docker run -d \
  --name PUNTOVENTA \
  -p 1522:1521 \
  -e ORACLE_PASSWORD=oracle \
  gvenzl/oracle-free:23-faststart
```

### Paso B — Verificar el estado

```bash
docker ps
# Estado esperado: Up (healthy)
```

---

## 🔑 4. Configuración del Esquema y Permisos

Es necesario provisionar el usuario administrador del esquema para que Entity Framework Core pueda aplicar migraciones y operar sobre los datos.

Conéctese a la consola interactiva de SQL\*Plus:

```bash
docker exec -it PUNTOVENTA sqlplus sys/oracle as sysdba
```

Dentro del indicador `SQL>`, ejecute:

```sql
-- Cambiar al Pluggable Database por defecto
ALTER SESSION SET CONTAINER = FREEPDB1;

-- Crear usuario con su clave de acceso
CREATE USER PUNTOVENTA_USER IDENTIFIED BY PUNTOVENTA;

-- Otorgar permisos esenciales de desarrollo
GRANT CONNECT, RESOURCE, CREATE VIEW TO PUNTOVENTA_USER;

-- Asignar cuota ilimitada de almacenamiento
ALTER USER PUNTOVENTA_USER QUOTA UNLIMITED ON USERS;

COMMIT;
exit;
```

---

## ⚡ 5. Conexión del Backend (.NET Core / C#)

Configure el archivo `appsettings.json` en el proyecto `PuntoVenta.API`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1522))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=FREEPDB1)));User Id=PUNTOVENTA_USER;Password=PUNTOVENTA;"
  }
}
```

> 💡 El puerto `1522` corresponde al mapeo externo del contenedor Docker. Entity Framework Core aplicará las migraciones automáticamente al iniciar la aplicación.

---

## 📌 Resumen de Credenciales

| Parámetro        | Valor            |
|------------------|------------------|
| Usuario Oracle   | `PUNTOVENTA_USER`|
| Contraseña       | `PUNTOVENTA`     |
| SYS Password     | `oracle`         |
| Puerto externo   | `1522`           |
| Puerto interno   | `1521`           |
| Service Name     | `FREEPDB1`       |
| Contenedor       | `PUNTOVENTA`     |

---

*Grupo 4 — Sistema de Punto de Venta*
