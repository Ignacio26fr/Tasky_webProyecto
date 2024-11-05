# Tasky

![Tasky Banner](/banner.png) <!-- Aquí puedes agregar la URL de tu imagen tipo banner -->

## Descripción
Tasky es una aplicación de gestión de correos y tareas, desarrollada como parte de un trabajo práctico grupal para la materia **Programación Web 3**. La aplicación se conecta a la API de Google Gmail para gestionar correos entrantes y utiliza Machine Learning (ML) para categorizar y priorizar las tareas de manera eficiente.

## Integrantes del Grupo
1. **Avalos, Mora**
2. **Coscarelli, Agutina**
3. **Franco, Ignacio**
4. **Cernik, Tomás**
5. **Miranda, Ezequiel**
6. **Osnaghi, Pedro**

## Funcionalidades Principales
- **Integración con Gmail**: Conexión con la API de Gmail para recibir y gestionar correos electrónicos entrantes.
- **Clasificación de correos**: Utiliza algoritmos de Machine Learning para categorizar correos por relevancia y prioridad.
- **Gestión de tareas**: Permite la creación, edición y eliminación de tareas basadas en correos entrantes.
- **Notificaciones en tiempo real**: Sistema de alertas para tareas pendientes y correos importantes.
- **Interfaz amigable**: Diseño intuitivo y fácil de usar.

## Tecnologías Utilizadas
- **.NET 6.0**: Framework principal de la aplicación.
- **ASP.NET Core MVC**: Para la estructura del frontend y backend.
- **Entity Framework Core**: ORM para la gestión de la base de datos.
- **Google API**: Para la integración con Gmail.
- **Machine Learning**: Implementado para la clasificación y priorización de correos.
- **Bootstrap**: Para el diseño y la estilización de la interfaz de usuario.

## Instalación y Configuración
1. Clona este repositorio:
   ```bash
   git clone git@github.com:Ignacio26fr/Tasky_webProyecto.git

2. Configura las claves de la API de Google en el archivo de configuración.
3. Ejecuta las migraciones para configurar la base de datos desde la consola NuGet:
   ```bash
   Update-Database 20241022225634_InitialCreate
   Udate-Database 20241026134305_addTaskyTables

## Uso

1. Inicia sesión en la aplicación con tu cuenta de Google.
2. Autoriza la aplicación para acceder a tus correos de Gmail.
3. Explora las funcionalidades de clasificación y gestión de tareas.
