# YouTubeGPT

Esta es una aplicación de muestra que demuestra cómo usar modelos de OpenAI para crear una aplicación de Generación Aumentada por Recuperación (RAG) utilizando videos de YouTube como fuente de datos.

Funciona extrayendo la descripción del video y utilizándola como contexto para el modelo RAG. Luego, el modelo se puede usar para generar una respuesta a un mensaje dado.

La aplicación se divide en dos partes: un servicio de ingestión de datos y una aplicación web. El servicio de ingestión de datos se encarga de descargar los metadatos del video y extraer la descripción. La aplicación web es responsable de proporcionar una interfaz de usuario para interactuar con el modelo.

## Prerrequisitos

- .NET 8
- .NEW Aspire 8
- Acceso a Azure OpenAI Service o una clave API de OpenAI
  - Modelos requeridos:
    - Predeterminado: `text-embedding-ada-3-small`
      - Sobrescribir con `dotnet user-secrets set "Azure:AI:EmbeddingDeploymentName" "<nombre de despliegue de tu modelo>"`
    - Predeterminado: `gpt-4o`
      - Sobrescribir con `dotnet user-secrets set "Azure:AI:ChatDeploymentName" "<nombre de despliegue de tu modelo>"`
- Docker

## Configuración

1. Clona el repositorio

   ```bash
   gh repo clone aaronpowell/YouTubeGPT
   ```

1. Configura el acceso a los modelos (usa Azure OpenAI o una clave API de OpenAI)

   ```bash
   cd YouTubeGPT.AppHost

   # Azure OpenAI Service
   dotnet user-secrets set "ConnectionStrings:OpenAI" "Endpoint=https://<tu-endpoint>.cognitiveservices.azure.com/;Key=<tu-clave>"

   # Clave API de OpenAI
   dotnet user-secrets set "ConnectionStrings:OpenAI" "<tu clave>"
   ```

1. Ejecuta la aplicación

   ```bash
   dotnet run
   ```

1. Abre un navegador y navega a `http://localhost:15015` e ingresa el token de inicio de sesión (consulta las instrucciones enlazadas para saber cómo obtener el token)

## Agregar Datos de YouTube

Esto se maneja mediante el servicio `YouTubeGPT.Ingestion`. Ábrelo desde el panel de Aspire e ingresa una URL de canal de YouTube en el cuadro de entrada proporcionado (por ejemplo: https://youtube.com/@dotnet). El servicio descargará los metadatos del video y extraerá la descripción.

## Búsqueda de Videos

La aplicación web proporciona un cuadro de búsqueda donde puedes ingresar una consulta y el modelo se usará para encontrar el video más relevante para la consulta. La descripción del video se utilizará como contexto para el modelo.

## Licencia

Este proyecto está licenciado bajo la Licencia MIT; consulta el archivo [LICENSE](LICENSE) para más detalles.
