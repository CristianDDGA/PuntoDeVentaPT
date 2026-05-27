# 🛒 Manual Completo y Explicación del Proyecto PuntoVenta

Este documento proporciona una explicación **amplia, profunda y detallada** de qué significa este proyecto, para qué sirve, cómo está construido por dentro y cómo funciona cada una de sus piezas.

---

## 1. ¿Qué es este proyecto? (El Concepto)

Este proyecto es un **Sistema de Punto de Venta (POS - Point of Sale)**. Es el software principal que utiliza una tienda física, comercio o negocio para administrar sus ventas diarias y operaciones. 

En lugar de llevar registros en papel, hojas de cálculo o cajas registradoras antiguas, este sistema automatiza y digitaliza todo:
- **Inventario:** Sabe qué productos hay disponibles, cuántos quedan y a qué precio.
- **Operaciones:** Registra a quién se le vende (Clientes).
- **Finanzas:** Calcula instantáneamente los totales, subtotales y el IVA (Impuestos).
- **Auditoría:** Guarda un registro inalterable e histórico de cada venta.
- **Formalidad:** Genera y entrega comprobantes físicos o digitales (Facturas en PDF).

---

## 2. Explicación de los Módulos (Lo que el usuario ve y usa)

### A. El Punto de Venta (POS - Interfaz de Caja)
* **¿Qué significa?** Es la pantalla de "operación rápida" que usa el cajero. Su objetivo es ser ágil para no hacer esperar al cliente.
* **¿Cómo funciona?** 
  - **Carrito de Compras:** El cajero busca productos por nombre o código y los apila en un detalle temporal (carrito). Puede aumentar cantidades o eliminar ítems.
  - **Cobro y Cierre:** Se asocia la venta a un cliente de la base de datos, se elige el método de pago (Efectivo/Tarjeta) y el sistema procesa la transacción.
  - **Prevención de Errores (Control de Concurrencia):** Es una característica de seguridad. Si dos cajeros, en computadoras distintas, intentan vender la *última unidad* de un producto exactamente en el mismo segundo, el sistema permite la venta al primero y bloquea al segundo indicando que el stock se agotó. Esto evita que el negocio venda "inventario fantasma" (productos que ya no tiene físicamente).

### B. Historial de Ventas y Facturas
* **¿Qué significa?** Es la memoria inmutable del negocio. Una "Caja Fuerte" virtual de todas las transacciones realizadas.
* **¿Cómo funciona?**
  - Permite a los administradores o cajeros auditar qué se vendió, a quién y a qué hora exacta.
  - **Filtros Avanzados e Inteligentes:** Puedes buscar facturas usando el número de recibo o escribiendo el nombre del cliente. Además, cuenta con un **toggle (interruptor) visual** que permite ocultar las facturas anuladas, dejando en pantalla únicamente el flujo de dinero real y activo.
  - **Anulaciones Lógicas:** En la vida real ocurren errores (el cliente devuelve el producto o la tarjeta rechaza el cobro). Cuando se "Anula" una venta, el sistema es muy inteligente: **no la borra** (borrar datos es malo para la contabilidad), simplemente la marca como "Anulada" y, de forma automática, **devuelve las cantidades exactas de esos productos al inventario** para que puedan volver a venderse.

### C. Generación de Comprobantes (PDF)
* **¿Qué significa?** La formalización legal o física de la transacción comercial.
* **¿Cómo funciona?** El sistema toma los datos crudos de la base de datos (números, fechas, IDs) y los dibuja ("renderiza") en un documento con diseño profesional (incluyendo logos, tablas organizadas y desglose de impuestos). Este PDF se muestra en una ventana emergente dentro de la misma aplicación y se puede descargar para imprimirlo o enviarlo por correo.

### D. Gestión de Inventario y Clientes
* **¿Qué significa?** Son los catálogos maestros que alimentan el resto del sistema.
  - **Productos:** Administra el catálogo. Controla el nombre, el precio y el **Stock** (las unidades físicas que hay en la bodega o estantería).
  - **Clientes:** Mantiene una base de datos centralizada. Esto sirve para generar facturas a nombre de una persona o empresa específica y mantener un historial de quién compra qué.

---

## 3. Explicación de la Arquitectura (Cómo está construido por dentro)

El código del proyecto no está escrito en un solo archivo gigante, sino que está diseñado bajo una filosofía de ingeniería de software llamada **Clean Architecture (Arquitectura Limpia)**. 

**¿Qué significa esto?** Imagina un edificio donde la plomería, la electricidad y la pintura están estrictamente separadas. Si cambias el color de la pintura, no rompes una tubería de agua. Aquí, el código está dividido en **5 capas** o "proyectos" independientes:

1. 🟢 **El Corazón: `PuntoVenta.Domain` (Dominio)**
   - **¿Qué es?** Las reglas puras del negocio. Aquí no hay noción de bases de datos, internet, ni pantallas. 
   - **Contiene:** Las "Entidades" puras. Aquí se programa la realidad: que un "Producto" tiene un "Precio", o que el estado de una "Venta" solo puede ser "Pagada" o "Anulada".

2. 🔵 **El Cerebro: `PuntoVenta.Application` (Aplicación)**
   - **¿Qué es?** Son los "Casos de Uso". Es el director de orquesta que coordina las acciones.
   - **Contiene:** La lógica de proceso. Cuando el usuario hace clic en "Comprar", esta capa dicta el flujo: *"Paso 1: Revisa si hay stock disponible. Paso 2: Crea el registro de la venta. Paso 3: Descuenta el stock. Paso 4: Guarda todo"*.

3. 🟡 **El Almacén: `PuntoVenta.Infrastructure` (Infraestructura)**
   - **¿Qué es?** Es la única capa que sabe cómo interactuar con el mundo exterior físico.
   - **Contiene:** La conexión directa a la Base de Datos (SQL). Su trabajo es tomar los datos que están en la memoria RAM y escribirlos permanentemente en el disco duro. También es la capa que sabe cómo "dibujar" y crear el archivo físico del PDF.

4. 🔴 **El Mensajero: `PuntoVenta.API` (Backend / Servidor)**
   - **¿Qué es?** Funciona como un mesero en un restaurante. La interfaz (el cliente) le pide cosas (ej. "Dame la lista de productos") y la API va a la cocina (Aplicación/Infraestructura) a buscar la información, la empaqueta y se la entrega a la interfaz.
   - **Contiene:** Las rutas de internet (Endpoints REST) que mantienen al sistema seguro y ordenado.

5. 🟣 **El Rostro: `PuntoVenta.Blazor` (Frontend / Interfaz Visual)**
   - **¿Qué es?** Es lo único que el usuario final realmente ve y toca.
   - **Contiene:** Los botones, los colores, las animaciones, las tablas y las ventanas emergentes. Está construido con una tecnología llamada **Blazor WebAssembly**. Esto significa que es una aplicación web, pero funciona con la velocidad y potencia de una aplicación instalada directamente en la computadora, todo corriendo dentro de Google Chrome, Edge o Safari.

---

## 4. Conceptos Técnicos Avanzados (Explicados en lenguaje simple)

* **Patrón "Unit of Work" (Unidad de Trabajo) y Transacciones de Base de Datos:**
  * **Explicación:** Imagina que haces una venta de 10 productos y justo cuando el sistema está guardando el producto número 5, se corta la luz. ¿Qué pasa? ¿Se cobró pero no se descontó el stock? Con este patrón, una transacción es **"Todo o Nada"**. Si ocurre cualquier error microscópico en medio del proceso, el sistema revierte *todos* los cambios y la base de datos queda exactamente igual que antes. Nunca quedan datos "a medias".
* **Entity Framework Core (ORM):**
  * **Explicación:** Funciona como un traductor universal. Los programadores escriben la lógica usando el lenguaje de programación C#. El ORM traduce automáticamente esas órdenes a lenguaje puro de base de datos (SQL). Esto ahorra cientos de horas de escritura manual de código.
* **API RESTful:**
  * **Explicación:** Es un estándar de comunicación global en internet. Al separar la API (Backend) de la pantalla (Blazor), el sistema es infinitamente escalable. Si el día de mañana los dueños del negocio quieren una **App para teléfonos móviles**, no necesitan reprogramar el sistema de ventas; la nueva App de celular simplemente se conectaría a esta misma API para funcionar.

---

## 5. ¿Cómo arrancar el proyecto? (Guía de Ejecución)

Dado que el sistema tiene una arquitectura separada, necesitas encender **dos motores** simultáneamente para que funcione (el Servidor y las Pantallas).

### Requisitos Previos:
* Tener instalado el SDK de **.NET 8**.
* Tener una instancia de **SQL Server** activa (local o remota).

### Paso 0: Configurar e Inicializar la Base de Datos (Migraciones)
Antes de ejecutar el sistema por primera vez, debes configurar la base de datos y aplicar las migraciones para generar las tablas correspondientes:

1. **Configurar la cadena de conexión:**
   Abre el archivo [PuntoVenta.API/appsettings.json](PuntoVenta.API/appsettings.json) y modifica la propiedad `DefaultConnection` para que apunte a tu servidor de base de datos SQL Server:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=TU_SERVIDOR_SQL;Database=PuntoVentaDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
2. **Instalar la herramienta CLI de Entity Framework Core (si aún no la tienes):**
   ```bash
   dotnet tool install --global dotnet-ef
   ```
3. **Aplicar las migraciones para crear la base de datos y las tablas:**
   Abre una terminal en la raíz del proyecto y ejecuta:
   ```bash
   dotnet ef database update --project PuntoVenta.Infrastructure --startup-project PuntoVenta.API
   ```
   *(Alternativa en Visual Studio: Ejecuta `Update-Database -Project PuntoVenta.Infrastructure -StartupProject PuntoVenta.API` desde la Consola del Administrador de Paquetes).*

### Paso 1: Encender el Servidor Backend (API)
Este es el cerebro central. Sin él, las pantallas no tienen datos.
1. Abre una terminal (consola de comandos).
2. Entra a la carpeta del servidor: `cd PuntoVenta.API`
3. Ejecuta el comando de encendido: `dotnet run`
   - *Resultado:* El servidor se enciende, se conecta a la base de datos en segundo plano y se queda esperando órdenes en el puerto `5291`.

### Paso 2: Encender la Interfaz Gráfica (Blazor)
1. Abre una **nueva** ventana de terminal (es crucial no cerrar la del paso anterior).
2. Entra a la carpeta de las pantallas: `cd PuntoVenta.Blazor`
3. Ejecuta el comando de encendido: `dotnet run`
   - *Resultado:* El sistema procesa todos los botones y colores, y automáticamente abre tu navegador web. A partir de este momento, cada vez que hagas clic en la pantalla, esta se comunicará por detrás con el servidor que encendiste en el Paso 1.
4. Los usuarios que se crean son los siguientes: ADMIN:admin@puntoventa.local y la contraseña es: Admin123* y para SELLER:admin@puntoventa.local y la contraseña es: Seller123*
---

## 6. Preparar y subir el proyecto a GitHub

Sigue estos pasos para preparar el repositorio y subirlo a GitHub de forma limpia.

1) Añade un `.gitignore` (está en la raíz del repositorio) para evitar subir binarios, carpetas de compilación, archivos de IDE y secretos. Esto mantiene el repositorio ligero.

2) Inicializar y subir al remoto (opciones):

   - Usando la interfaz web de GitHub: crea un nuevo repositorio y copia la URL.
   - En tu máquina (desde la raíz del proyecto):

```bash
git init
git add .
git commit -m "Initial commit — PuntoVenta"
git branch -M main
git remote add origin <URL-del-repositorio>
git push -u origin main
```

3) Si tienes archivos grandes ya (carpetas `bin/`, `obj/`, `packages/` u otros), elimínalos del índice antes de commitear con:

```bash
git rm -r --cached bin obj **/bin/** **/obj/**
git commit -m "Remove build outputs from repo"
```

4) Para históricos con archivos grandes ya cometidos, usa `git filter-repo` o BFG Repo-Cleaner para limpiar el historial, o considera `git lfs` para manejar binarios pesados.

5) Recomendaciones adicionales:
   - No subir `appsettings.Development.json` ni archivos con credenciales.
   - Añade documentación en el README sobre cómo ejecutar, variables de entorno y pasos de despliegue (ya incluído arriba).

6) Flujo típico para colaborar:
   - Crea ramas por feature: `git checkout -b feature/nombre-funcionalidad`
   - Haz PRs hacia `main` y revisa antes de mergear.

---

Si quieres, puedo:
- crear el `.gitignore` ahora (ya añadido)
- actualizar el README con instrucciones más detalladas sobre la base de datos o CI/CD
- configurar un archivo `deploy` o `launch` para GitHub Actions

