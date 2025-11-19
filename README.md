# BaseComputadoras-_SQLITE

Repo: [gtshadow33/BaseComputadoras-_SQLITE](https://github.com/gtshadow33/BaseComputadoras-_SQLITE)

---

## Descripción General

**BaseComputadoras-_SQLITE** es una aplicación de escritorio escrita en **C#**, que tiene como objetivo facilitar el registro, consulta y gestión de información sobre computadoras en una base de datos local **SQLite**. Aprovecha el framework **Avalonia** para la interfaz gráfica multiplataforma, lo que permite ejecutarla en Windows, Linux o macOS.

La aplicación está pensada para inventarios, mantenimiento de laboratorios, aulas, empresas o cualquier entorno donde se requiere llevar gestión ordenada de las computadoras.

---

## Estructura del Proyecto

El proyecto cuenta con los siguientes archivos y carpetas principales:

- `MainWindow.axaml`: Diseño de la ventana principal en XAML.
- `MainWindow.axaml.cs`: Código C# para la lógica de interfaz y eventos.
- `Program.cs`: Punto de entrada de la aplicación.
- `App.axaml` y `App.axaml.cs`: Configuración y arranque de la aplicación.
- `.vscode/`, `bin/`, `obj/`, `publish/`: carpetas de configuración e intermediarios de compilación.
- `computadoras.db`: Base de datos SQLite, creada automáticamente.

---

## Instalación y Ejecución

### Prerrequisitos

- Tener instalado **.NET 8.0 o superior**.
- Tener acceso a un sistema operativo compatible (Windows, Linux, macOS).
- Tener instalado **git** si deseas clonar el repositorio.

### Clonado y Build

```sh
git clone https://github.com/gtshadow33/BaseComputadoras-_SQLITE.git
cd BaseComputadoras-_SQLITE

# Si tienes dotnet instalado:
dotnet restore
dotnet build
dotnet run
```

También puedes abrir el `.sln` (solución) en Visual Studio o JetBrains Rider, restaurar paquetes y ejecutar allí.

Al iniciar la app, se creará el archivo `computadoras.db` en el mismo directorio (no necesitas instalar nada más).

---

## Explicación de la Funcionalidad

### 1. Estructura de la Base de Datos

La tabla central es:

```sql
CREATE TABLE IF NOT EXISTS computadoras (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    ram INTEGER NOT NULL,
    disco INTEGER NOT NULL,
    funciona INTEGER NOT NULL
);
```

- **id**: identificador único de cada computadora.
- **nombre**: nombre/comentario para identificar la PC (puede incluir etiqueta, usuario o ubicación).
- **ram**: cantidad de memoria RAM (en MB).
- **disco**: capacidad de almacenamiento (en GB).
- **funciona**: `1` si el equipo funciona, `0` si no (la app acepta varios formatos en el input).

### 2. Interfaz Gráfica: Elementos y Uso

La interfaz consta de:

- Selector de acción:  
  Permite elegir entre "Agregar", "Buscar por ID", "Buscar por Nombre", "Buscar por RAM", "Buscar por Disco", "Buscar por Funciona", "Actualizar".
- Campos dinámicos:  
  Según la acción elegida, la visibilidad de los campos se ajusta automáticamente (ID, Nombre, RAM, Disco, Funciona).
- Botón Ejecutar:  
  Realiza la acción seleccionada tras verificar que los campos necesarios tengan información válida.
- Resultados:  
  Muestra las computadoras que cumplen el criterio de búsqueda.

### 3. Acciones Soportadas

#### Agregar

Agrega una nueva computadora.  
**Requiere**: Nombre, RAM, Disco, Funciona.  
Realiza validación de campos antes de guardar.

#### Buscar

- **Por ID:** Busca equipos según el identificador único.
- **Por Nombre:** Busca equipos cuyos nombres comiencen con el texto ingresado.
- **Por RAM / Disco / Funciona:** Busca por el valor concreto en esos campos.

#### Actualizar

Permite modificar los datos de una computadora existente ingresando el ID y los nuevos datos.

#### Validaciones

La aplicación informa mediante ventanas si falta algún dato, si el ID es requerido, etc.

### 4. Ejemplo de uso

**Ejemplo 1: Agregar una computadora**
- Selecciona "Agregar".
- Completa Nombre, RAM, Disco y Funciona.
- Haz clic en "Ejecutar".
- Se guardará en la base de datos.

**Ejemplo 2: Buscar una computadora por Nombre**
- Selecciona "Buscar por Nombre".
- Escribe parte o todo el nombre en el campo correspondiente.
- Haz clic en "Ejecutar".
- Se mostrarán los resultados que cumplen el criterio.

**Ejemplo 3: Actualizar datos**
- Selecciona "Actualizar".
- Completa el ID y los nuevos valores.
- Ejecuta.

---

## Explicación del Código Principal

El archivo clave es [`MainWindow.axaml.cs`](https://github.com/gtshadow33/BaseComputadoras-_SQLITE/blob/master/MainWindow.axaml.cs).  
Al iniciar la app, llama a `InicializarBaseDatos()` que crea la tabla si no existe.

Los eventos principales son:

- `AccionBox.SelectionChanged`: Cambia qué campos son visibles según la acción seleccionada.
- `EjecutarBtn.Click`: Ejecuta la acción. Dependiendo del tipo, valida los campos, conecta a la base de datos, y realiza el comando (`INSERT`, `SELECT`, o `UPDATE`).

**Conexión a la base de datos:**
```csharp
string connStr = "Data Source=computadoras.db";
using var con = new SqliteConnection(connStr);
con.Open();
```

**Inserción:**
```csharp
add.CommandText = "INSERT INTO computadoras (nombre, ram, disco, funciona) VALUES (@n,@r,@d,@f)";
add.Parameters.AddWithValue("@n", NombreBox.Text);
...
add.ExecuteNonQuery();
```

**Búsqueda:**
```csharp
Buscar(con, "id", id);         // Por ID
BuscarLike(con, "nombre", txt);// Por Nombre inicial
```

**Actualización:**
```csharp
upd.CommandText = "UPDATE computadoras SET nombre=@n, ram=@r, disco=@d, funciona=@f WHERE id=@id";
uci.Parameters.AddWithValue("@id", id); // etc.
```

**Mostrar resultados:**  
Lee la consulta y crea una lista con los resultados, mostrando cada registro con ID, Nombre, RAM, Disco, Funciona.

---

## Personalización

Puedes modificar los campos de la tabla, agregar nuevas funcionalidades (eliminación, exportación, etc.), o adaptar la interfaz XAML a tus necesidades.

---

## Licencia

Este código se distribuye bajo la licencia MIT (o la especificada por el autor, revisar el repositorio).

---

## Autor y Contacto

**Autor:** gtshadow33  
Repositorio: [https://github.com/gtshadow33/BaseComputadoras-_SQLITE](https://github.com/gtshadow33/BaseComputadoras-_SQLITE)

---

## Recursos y Links Útiles

- [Documentación Avalonia](https://avaloniaui.net/docs)
- [Documentación .NET SQLite](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [Ejemplo de aplicaciones con SQLite y C#](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/overview)

---

¿Dudas o sugerencias? Puedes abrir un issue en el repositorio o forkearlo para enviar mejoras.
