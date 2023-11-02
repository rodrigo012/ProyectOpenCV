# ProyectOpenCV
Comparación Facial con OpenCV y .NET 7.0

El proyecto de Comparación Facial se centra en la aplicación del framework OpenCV, reconocida biblioteca de visión por computadora, en el entorno de desarrollo .NET 7.0. Esta solución está diseñada para analizar y resaltar las diferencias entre imágenes faciales mediante el procesamiento de imágenes, detección de rostros y cálculo de variaciones.

# Características

- Utiliza Emgu.CV,una envoltura .NET para OpenCV, 
- para realizar operaciones de procesamiento de imágenes.
- La arquitectura de .NET 7.0 proporciona una base sólida para la ejecución eficiente del código.
- Ofrece capacidades para la detección y resaltado de variaciones en imágenes faciales.
- Se integra con el módulo de reconocimiento facial de OpenCV para comparar rasgos y características.
# Instalación
Para ejecutar este proyecto:

1. Asegúrate de tener .NET 7.0 instalado.
2. Verifica la instalación de Emgu.CV para el uso de las funcionalidades de OpenCV.
3. Clona el repositorio desde GitHub.
4. Verifica que no haya archivos al comienzo.
5. Crea la carpeta "videos" en "Digital Solution".
6. Integra el video deseado en la carpeta "videos".
7. Ejecuta el proyecto.
8. Al finalizar el proceso del video, se mostrará un mensaje de finalización.
9. El archivo generado estará en la carpeta "Differences" en "Digital Solution".

# Estructura del Proyecto
El proyecto se estructura en varios componentes:

- ComparacionFacial.cs: Contiene la lógica principal para el procesamiento de imágenes y comparación facial.
- OpenCV: Incluye las dependencias y utilidades del framework OpenCV.

# Uso
Este proyecto puede utilizarse para:

- Analizar las diferencias entre imágenes faciales.
- Resaltar variaciones entre rostros en diferentes imágenes.
- Entender el funcionamiento de OpenCV y .NET para procesamiento de imágenes.


# Algunos problemas conocidos incluyen:

- Posibles errores en la detección de rostros en imágenes de baja calidad.
- Limitaciones en la precisión de la comparación facial en situaciones de iluminación variable.
