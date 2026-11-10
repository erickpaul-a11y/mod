# Universal RimWorld Translator

Traductor comunitario para RimWorld 1.6. Detecta texto traducible mediante un parche de Harmony, consulta una caché local y puede solicitar traducciones online a un proveedor configurable.

> Estado: prototipo inicial. No es todavía un traductor universal terminado ni garantiza traducir todos los textos de todos los mods.

## Objetivos
- No modificar los archivos originales de otros mods.
- Mantener una caché local de traducciones.
- Traducir en segundo plano para no bloquear la partida.
- Configurar idioma de origen/destino y proveedor.
- Permitir correcciones manuales en archivos XML.

## Requisitos
- RimWorld 1.6
- Harmony (incluido en RimWorld)
- .NET Framework 4.7.2 para compilar el ensamblado

## Compilación
Abre `Source/UniversalRimWorldTranslator.csproj`, configura las referencias a `Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll` y `0Harmony.dll` desde la instalación de RimWorld y compila en Release. Copia el DLL resultante a `Assemblies/`.

## Traducción online
El prototipo usa MyMemory como proveedor configurable. El servicio gratuito tiene límites, puede fallar y sus traducciones requieren revisión. No se incluye ninguna clave privada. El juego no espera a la red: el resultado se guarda para la siguiente vez que aparezca el texto.

## Privacidad y seguridad
El texto que se envía al proveedor externo sale de tu PC. No envíes contenido privado. Puedes desactivar la traducción online en la configuración del mod.

## Limitaciones actuales
- RimWorld y algunos mods construyen textos dinámicamente; no todos pasarán por el mismo método de traducción.
- El prototipo no puede traducir imágenes, texto dibujado directamente ni interfaces que eviten el sistema de traducción de RimWorld.
- Para máxima cobertura se necesitará una fase adicional de lectura de archivos de idioma y pruebas con una colección real de mods.
