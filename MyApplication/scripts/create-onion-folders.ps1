$root = Resolve-Path "."
$folders = @(
    "src\Core\Domain\Entities",
    "src\Core\Application\Interfaces",
    "src\Core\Application\Services",
    "src\Infrastructure\Repositories",
    "src\Infrastructure\Services",
    "src\Presentation\Api",
    "src\Presentation\Web",
    "src\CrossCutting\Extensions",
    "tests\Unit",
    "tests\Integration"
)

foreach ($f in $folders) {
    $path = Join-Path $root $f
    New-Item -ItemType Directory -Path $path -Force | Out-Null
    New-Item -ItemType File -Path (Join-Path $path ".gitkeep") -Force | Out-Null
}

# top-level README describing the onion layers
$readmePath = Join-Path $root "src\ONION_ARCHITECTURE_README.md"
@"
Onion architecture layout (created by script)

- src/Core
  - Domain: Entities, value objects, domain events, domain exceptions.
  - Application: DTOs, service interfaces, use cases (application services).
- src/Infrastructure
  - Implementations of persistence, external APIs, file storage, DI composition roots.
- src/Presentation
  - Api: Controllers, DTO mappings
  - Web: Razor UI or SPA integration
- src/CrossCutting
  - Logging, caching, extension methods
- tests
  - Unit and integration test projects

Next steps:
1) In Visual Studio: open __Solution Explorer__, click __Show All Files__, then right-click the new folders/files and choose __Include In Project__ (or add new class library projects and move files into them).
2) Create class library projects per layer (TargetFramework: net8.0) via __Add > New Project__ and reference only inward dependencies (Infrastructure -> Application -> Domain).
"@ | Out-File -FilePath $readmePath -Encoding utf8 -Force