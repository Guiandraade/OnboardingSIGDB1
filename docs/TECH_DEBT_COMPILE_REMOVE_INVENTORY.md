# Compile Remove Inventory

This document tracks temporary `<Compile Remove=...>` entries that should be eliminated after structural cleanup.

## Current Inventory

No active `<Compile Remove=...>` entries remain in:

- `src/OnboardingSIGDB1.Domain/OnboardingSIGDB1.Domain.csproj`
- `src/OnboardingSIGDB1.Data/OnboardingSIGDB1.Data.csproj`

## Completed in PR2

- Removed stale Domain exclusions for non-existent files:
  - `Entities\Positions\PositionValidator.cs`
  - `Notifications\Notifiable.cs`
  - `Dto\Employee\EmployeeResponde.cs`
- Removed legacy Data exclusion:
  - `Repositories\EmployeePositionRepository.cs`
- Deleted obsolete file:
  - `src/OnboardingSIGDB1.Data/Repositories/EmployeePositionRepository.cs`

## Removal Policy

1. Confirm no runtime or test dependency on excluded artifact.
2. Remove source file if obsolete.
3. Remove `<Compile Remove=...>` entry.
4. Build + test full solution.
5. Document result in PR notes.


