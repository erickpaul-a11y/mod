# Universal RimWorld Translator

Prototipo de traducción al español para RimWorld 1.6. Intercepta llamadas a `Translator.Translate(string)`, aplica traducciones de caché y puede consultar MyMemory bajo demanda.

## Qué hace

- Parche Harmony localizado a `Verse.Translator.Translate(string)`. Si la firma no existe, el mod continúa sin parche y lo registra en el log.
- Diccionario local pequeño de palabras y frases frecuentes.
- Escaneo de archivos XML de la carpeta `Languages/<carpeta seleccionada>/Keyed` en los mods cargados. El escaneo solo lee archivos; no los modifica.
- Traducción online opcional, desactivada por defecto, con una petición a la vez y límite de cola.
- Caché persistente guardada de forma atómica en la configuración del juego.
- Ajustes para idioma fuente/destino, carpeta fuente, proveedor, escaneo manual y borrado de caché.

## Límites conocidos

Este mod no es un traductor universal completo. La integración cubre cadenas que llegan a `Translator.Translate(string)` y coinciden con una traducción en caché. No procesa `DefInjected`, texto escrito directamente en código/UI, imágenes, ni todos los textos con variables. Las cadenas con llaves se excluyen para reducir daños a marcadores de formato. Las traducciones online se guardan en caché; una pantalla ya visible no se actualiza hasta que el juego vuelva a solicitar esa cadena.

El escáner tiene un límite de 150 textos por pasada. Solo examina la carpeta seleccionada (normalmente `English`) y su subcarpeta `Keyed`. Para que la caché coincida con los textos que recibe el parche, el idioma activo del juego debe corresponder al idioma fuente escaneado. Algunas estructuras de XML de mods pueden no coincidir con lo que consume el traductor del juego. El proveedor gratuito puede fallar, limitar peticiones o devolver resultados incorrectos.

Al activar la opción online, el texto que no se encuentre en la caché se envía desde tu PC al endpoint configurado. Revisa el proveedor antes de habilitarla.

## Compilar

Requiere Windows, .NET SDK con targeting pack de .NET Framework 4.7.2 y una instalación de RimWorld 1.6. Ejecuta en PowerShell:

```powershell
.\build.ps1 -RimWorldManaged "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed"
```

El directorio indicado debe contener estas DLL reales de la instalación:

- `Assembly-CSharp.dll`
- `UnityEngine.CoreModule.dll`
- `0Harmony.dll`

El script compila `Source/UniversalRimWorldTranslator.csproj` y copia el ensamblado a `Assemblies/UniversalRimWorldTranslator.dll`. No se distribuye un DLL compilado en este repositorio. Las DLL de RimWorld no se incluyen por licencia.

## Instalación y uso

1. Compila con las DLL de tu instalación.
2. Copia el repositorio como mod local en `RimWorld/Mods/UniversalRimWorldTranslator`, conservando `About`, `Languages` y el directorio `Assemblies` generado.
3. Activa el mod y reinicia el juego.
4. En Opciones > Mods, abre los ajustes del traductor. Activa la red solo si aceptas enviar textos al proveedor.
5. Usa el botón de escaneo y revisa `Player.log` si hay errores.

## Comprobaciones aisladas y estado de validación

El workflow `.github/workflows/stub-check.yml` compila los archivos fuente contra stubs mínimos de Verse, Unity y Harmony y ejecuta comprobaciones de diccionario, caché y selección de archivos Keyed. Se puede ejecutar localmente en Windows con .NET SDK:

```powershell
dotnet run --project tests/CompileWithStubs.csproj --configuration Release
```

Estos stubs verifican sintaxis y lógica aislada, no demuestran compatibilidad binaria con el juego. El código todavía necesita compilarse contra las DLL reales de RimWorld 1.6 y probarse dentro del juego. La revisión del código no valida por sí sola la firma/reflexión de Harmony contra la versión instalada, ni el comportamiento de traducción en una partida. Para esa validación hacen falta exactamente las tres DLL indicadas arriba y una ejecución del juego con el mod instalado. No presentes una compilación o prueba en juego como realizada hasta completar esos pasos.
