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

Requiere Windows, .NET SDK con targeting pack de .NET Framework 4.7.2 y una instalación de RimWorld 1.6. El directorio del juego debe contener `Assembly-CSharp.dll` y `UnityEngine.CoreModule.dll`. Harmony suele venir con el mod Harmony de Steam Workshop, así que el script acepta su DLL en una ruta independiente:

```powershell
.\build.ps1 `
  -RimWorldManaged "E:\Juegos\steamapps\common\RimWorld\RimWorldWin64_Data\Managed" `
  -HarmonyDll "E:\Juegos\steamapps\workshop\content\294100\2009463077\1.6\Assemblies\0Harmony.dll"
```

El directorio indicado debe contener estas DLL reales de la instalación:

- `Assembly-CSharp.dll`
- `UnityEngine.CoreModule.dll`
- `0Harmony.dll` (del mod Harmony para RimWorld 1.6; también se acepta dentro de `Managed`)

El script compila `Source/UniversalRimWorldTranslator.csproj` y copia el ensamblado a `Assemblies/UniversalRimWorldTranslator.dll`. No copies `0Harmony.dll` al mod: Harmony es una dependencia requerida y el juego la carga desde su propio mod. Las DLL de RimWorld no se incluyen por licencia.

## Instalación y uso

1. Compila con las DLL de tu instalación.
2. Copia el repositorio como mod local en `RimWorld/Mods/UniversalRimWorldTranslator`, conservando `About`, `Languages` y el directorio `Assemblies` generado.
3. Activa el mod y reinicia el juego.
4. En Opciones > Mods, abre los ajustes del traductor. Activa la red solo si aceptas enviar textos al proveedor.
5. Usa el botón de escaneo y revisa `Player.log` si hay errores.

## Comprobaciones aisladas y estado de validación

El workflow `.github/workflows/stub-check.yml` compila los archivos fuente contra stubs mínimos de Verse, Unity y Harmony y ejecuta comprobaciones de diccionario, caché, análisis de JSON Unicode del proveedor y selección de archivos Keyed. Se puede ejecutar localmente en Windows con .NET SDK:

```powershell
dotnet run --project tests/CompileWithStubs.csproj --configuration Release
```

Los stubs verifican lógica aislada. La compilación contra RimWorld 1.6.4871 se realizó localmente con las DLL de la instalación y Harmony 2.4.3 del mod de Workshop; además, se aplicó el parche Harmony a `Verse.Translator.Translate(string)` en un proceso .NET Framework. Aún falta la prueba funcional dentro del juego con el mod activado; la compilación y el parche aislado no demuestran que las cadenas se traduzcan correctamente durante una partida.

