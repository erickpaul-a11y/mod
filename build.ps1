param(
    [Parameter(Mandatory=$true)]
    [string]$RimWorldManaged,
    [string]$HarmonyDll,
    [string]$Configuration = "Release",
    [string]$DotNetExe = "dotnet"
)

$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "Source/UniversalRimWorldTranslator.csproj"
$managed = (Resolve-Path $RimWorldManaged).Path
$required = @("Assembly-CSharp.dll", "UnityEngine.CoreModule.dll")
foreach ($name in $required) {
    if (-not (Test-Path (Join-Path $managed $name))) {
        throw "No se encuentra $name en $managed"
    }
}
$harmony = if ([string]::IsNullOrWhiteSpace($HarmonyDll)) {
    Join-Path $managed "0Harmony.dll"
} else {
    (Resolve-Path $HarmonyDll).Path
}
if (-not (Test-Path $harmony)) {
    throw "No se encuentra 0Harmony.dll. Indica su ruta con -HarmonyDll. Ruta comprobada: $harmony"
}

$env:RIMWORLD_MANAGED = $managed
$env:RIMWORLD_HARMONY = $harmony
& $DotNetExe build $project --configuration $Configuration
if ($LASTEXITCODE -ne 0) { throw "Falló la compilación de .NET." }

$built = Join-Path $PSScriptRoot "Source/bin/$Configuration/net472/UniversalRimWorldTranslator.dll"
if (-not (Test-Path $built)) { throw "No se generó el DLL esperado: $built" }

$out = Join-Path $PSScriptRoot "Assemblies"
New-Item -ItemType Directory -Force -Path $out | Out-Null
Copy-Item $built (Join-Path $out "UniversalRimWorldTranslator.dll") -Force
Write-Host "Compilación completada. DLL copiado en $out"

