# Compile Remove Inventory

This document tracks temporary `<Compile Remove=...>` entries that should be eliminated after structural cleanup.

## Current Inventory

### `src/OnboardingSIGDB1.Domain/OnboardingSIGDB1.Domain.csproj`

- `Entities\Positions\PositionValidator.cs`
  - Status: file not present in current project tree.
  - Action: remove csproj entry when no back-compat need remains.

- `Notifications\Notifiable.cs`
  - Status: file not present in current project tree.
  - Action: remove csproj entry when no back-compat need remains.

- `Dto\Employee\EmployeeResponde.cs`
  - Status: file not present in current project tree.
  - Action: remove csproj entry when no back-compat need remains.

### `src/OnboardingSIGDB1.Data/OnboardingSIGDB1.Data.csproj`

- `Repositories\EmployeePositionRepository.cs`
  - Status: file exists but is excluded from compilation.
  - Risk: duplicate/legacy repository path can create confusion.
  - Action: keep one canonical repository abstraction/implementation and then remove file + csproj exclusion.

## Removal Policy

1. Confirm no runtime or test dependency on excluded artifact.
2. Remove source file if obsolete.
3. Remove `<Compile Remove=...>` entry.
4. Build + test full solution.
5. Document result in PR notes.

