# Sistema de Gestión Universitaria

Sistema web desarrollado en ASP.NET Core MVC para la gestión integral de procesos académicos universitarios.

## 🎯 Características Principales

- **Gestión de Alumnos**
  - Registro y mantenimiento de información estudiantil
  - Seguimiento académico personalizado
  - Estado académico en tiempo real

- **Gestión de Cursos**
  - Administración de asignaturas y programas
  - Control de cupos y prerrequisitos
  - Calendarización académica

- **Gestión de Catedráticos**
  - Perfiles profesionales
  - Asignación de cursos
  - Control de carga académica

- **Gestión de Notas**
  - Registro de calificaciones
  - Cálculo automático de promedios
  - Generación de reportes académicos

## 📦 Paquetes NuGet y Librerías

### Paquetes NuGet Principales
```bash
# Entity Framework Core
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design

# Herramientas de Desarrollo
Microsoft.VisualStudio.Web.CodeGeneration.Design
Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
```

### Librerías Frontend
```html
<!-- Bootstrap -->
<link href="~/lib/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />

<!-- jQuery y Validaciones -->
<script src="~/lib/jquery/dist/jquery.min.js"></script>
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
<script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>
```

## 🛠️ Tecnologías Utilizadas

- **Backend**
  - ASP.NET Core MVC 7.0
  - Entity Framework Core
  - SQL Server
  - C#

- **Frontend**
  - HTML5
  - CSS3
  - JavaScript
  - Bootstrap

- **Herramientas de Desarrollo**
  - Visual Studio 2022
  - Git
  - GitHub

## 📋 Requisitos Previos

- Visual Studio 2022
- SQL Server 2019 o superior
- .NET 7.0 SDK
- Navegador web moderno

## 🚀 Instalación

1. Clonar el repositorio
```bash
git clone https://github.com/Manuel-Dev-hub/Sistemas_Universitario.git
```

2. Restaurar paquetes NuGet
```bash
dotnet restore
```

3. Actualizar la cadena de conexión en `appsettings.json`

4. Ejecutar migraciones
```bash
dotnet ef database update
```

5. Ejecutar la aplicación
```bash
dotnet run
```

## 💡 Uso

1. Acceder al sistema mediante la URL local
2. Iniciar sesión con credenciales de administrador
3. Navegar por los diferentes módulos del sistema



## 🔄 Estado del Proyecto

El proyecto se encuentra en desarrollo activo, con las siguientes características planeadas para futuras versiones:

- [ ] Dashboard con estadísticas en tiempo real
- [ ] Generación de reportes en PDF
- [ ] Sistema de notificaciones
- [ ] API RESTful
- [ ] Integración con servicios de correo

## 👨‍💻 Autor

- Manuel
- LinkedIn: https://www.linkedin.com/in/manuel-antonio-rios-cardona-475073327
- Portfolio: https://www.portafoliomanuelrios.tech

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para detalles.
