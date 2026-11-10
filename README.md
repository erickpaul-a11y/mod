# Universal RimWorld Translator

Mod experimental para RimWorld 1.6 que combina un pequeño diccionario local, caché persistente y traducción online opcional.

## Funciones añadidas
- Parche Harmony sobre `Translator.Translate(string)` para consultar la caché cuando el juego traduce una clave.
- Diccionario local inicial de palabras y frases frecuentes.
- Exploración manual de archivos XML dentro de `Languages` de los mods cargados.
- Envío opcional de textos desconocidos a MyMemory en segundo plano, con una sola petición a la vez y una pausa entre solicitudes.
- Caché persistente en la configuración del juego.
- Pantalla de ajustes: activar/desactivar red, elegir códigos de idioma y endpoint, explorar textos, guardar y borrar caché.
- No modifica los archivos originales de otros mods.

## Instalación: todavía requiere compilación
Este repositorio contiene código fuente, no un DLL listo para instalar. No lo coloques directamente en la carpeta Mods esperando que funcione.

Requisitos:
- RimWorld 1.6.
- Visual Studio o .NET SDK compatible con .NET Framework 4.7.2.
- Harmony de RimWorld.
- Referencias a `Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll` y `0Harmony.dll`.

Configura la variable `RIMWORLD_MANAGED` con la carpeta `RimWorldWin64_Data/Managed` de tu instalación y compila `Source/UniversalRimWorldTranslator.csproj` en Release. Copia el ensamblado resultante a `Assemblies/UniversalRimWorldTranslator.dll` dentro de la carpeta del mod.

## Cómo usar
1. Abre el juego y activa el mod.
2. Entra en Opciones > Mods > Universal RimWorld Translator.
3. Configura el código del idioma de origen (`en`), la carpeta del idioma de origen (normalmente `English`) y destino (`es`). El escáner solo lee esa carpeta de idioma para no mezclar textos de idiomas diferentes.
4. Pulsa «Traducir textos detectados en archivos de mods».
5. Espera a que se rellene la caché y vuelve a cargar la partida o el menú para que los textos interceptados puedan usarla.

## Privacidad y límites
- Si activas la traducción online, los textos seleccionados se envían a MyMemory desde tu PC. No envíes contenido privado.
- El servicio gratuito tiene límites y puede fallar. La traducción automática puede ser incorrecta.
- El escáner lee textos XML, pero no reescribe ni crea archivos de idioma que RimWorld pueda cargar directamente. Por tanto, explorar un texto no garantiza que todas las pantallas del mod cambien inmediatamente.
- La cobertura del parche depende de que el texto pase por el método de traducción interceptado. No traduce imágenes, texto dibujado directamente ni todo texto dinámico.
- El diccionario local incluido es inicial y no contiene todavía un vocabulario exhaustivo.
- El código aún no ha sido compilado ni probado dentro de RimWorld 1.6. Considera esta versión una base de desarrollo, no una versión estable.
