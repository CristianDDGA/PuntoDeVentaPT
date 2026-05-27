# Reporte de Cumplimiento de Requerimientos - Punto de Venta

Tras revisar el documento de especificaciones de la Universidad Técnica de Ambato para la asignatura "Patrones de Software", el proyecto cumple con la gran mayoría de los requisitos obligatorios, incluyendo la arquitectura Clean Architecture, seguridad con JWT, bloqueo de usuarios por intentos fallidos, ciclo de vida de la venta sin afectar stock en borrador, y más.

Sin embargo, para alcanzar el **100% de los requisitos obligatorios**, hacen falta los siguientes puntos específicos que aún no se han implementado:

### 1. Paginación con Selector de Cantidad (Requisito 15)
* **Requisito:** *"permitir mediante un combobox seleccionar la cantidad de registros de la paginación (10, 15, 20, 30)."*
* **Estado Actual:** Tenemos paginación y ventanas emergentes (modales) para buscar clientes y productos, pero la cantidad de elementos por página está fija en el backend. 
* **Falta:** Agregar un selector desplegable (combobox) en la UI de Blazor y enviar el parámetro `pageSize` dinámico a la API.

### 2. Visor de Log de Errores en Frontend (Requisito 26)
* **Requisito:** *"Crear un reporte en el front-end para visualizar estos errores que deben ser accesibles solo a los administradores."*
* **Estado Actual:** El backend ya intercepta excepciones y las guarda en la base de datos en la tabla `ErrorLog`.
* **Falta:** Crear una vista en Blazor (`/error-logs`) exclusiva para el rol `Admin` que consulte y muestre esta información en una tabla paginada.

### 3. Eliminación Física Condicional (Requisitos 30 y 31)
* **Requisito:** *"El sistema debe permitir eliminar productos y clientes solo si no tienen ventas/pedidos asociados. Si ya tienen historial, no deben eliminarse físicamente; deben marcarse como inactivos o eliminados lógicamente."*
* **Estado Actual:** Actualmente solo disponemos de desactivación (Soft Delete) genérica. No existen endpoints `DELETE` físicos en la API.
* **Falta:** Implementar la lógica en `ProductService` y `CustomerService` para intentar borrar de la base de datos, y si se detecta que existen claves foráneas en la tabla `SaleDetail` o `Sale`, atrapar la excepción o verificar la relación y proceder con la desactivación (`IsActive = false`).

### 4. Transacciones Explícitas / Unit Of Work (Requisito 17)
* **Requisito:** *"Utilizar transacciones (Se puede usar el patrón Unit Of Work) y concurrencia donde sea necesario según análisis."*
* **Estado Actual:** El guardado se hace mediante Entity Framework Core, pero al momento de confirmar una venta, estamos ejecutando el descuento de inventario, el registro en la tabla `StockMovement` y la actualización del estado de `Sale` de forma secuencial.
* **Falta:** Envolver explícitamente estas operaciones dentro de una transacción (`_dbContext.Database.BeginTransactionAsync()`) en la capa de persistencia para garantizar la integridad ACID en caso de fallos intermedios.

---

> El resto de los requerimientos base obligatorios (modelado, validaciones estrictas de contraseña, despliegue desacoplado, estados de venta, restricciones de rol, etc.) están **completamente cumplidos**.