# CLAUDE.md

Instructions for Claude (or any coding agent) working in this repo. Humans: see [CONTRIBUTING.md](CONTRIBUTING.md).

## The project

University team project (Unity 6000.6.6f1, URP, 3D). A top-down twin-stick **roguelite dungeon crawler**: procedurally assembled floors of handmade rooms, doors lock until the room is cleared, mixed ranged + melee combat, items, a boss per floor, 3 floors per run in the MVP. Scope details: FEATURE_PLAN. Placeholder art only (Unity primitives); the art team replaces visuals later.

## Read before coding

| File | What it is | When to read |
|---|---|---|
| [docs/gdd/GDD.md](docs/gdd/GDD.md) | Game design (Romanian, informal). **Source of truth for gameplay rules and starting values.** Snapshot of the team's Google Doc. | The chapter your task references |
| [docs/FEATURE_PLAN.md](docs/FEATURE_PLAN.md) | Milestones and tasks with IDs, dependencies, acceptance criteria | Always: find your task here |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Folders, namespaces, systems, contracts, layers | Always, before creating any file |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Branches, commits, PRs, prefab rules, Definition of Done | Before committing |

The main GDD is a Google Doc the team edits; `docs/gdd/GDD.md` is a copy updated by hand when the user asks. Don't edit it on your own initiative. If something in it seems outdated or contradicts the user, ask.

`docs/gdd/GDD_alternative.md` and `docs/gdd/GDD.html` are local, gitignored working files. Ignore them unless asked.

## How to work on a task

1. Pick the task by ID from `FEATURE_PLAN.md`; check its dependencies are done. If the user gave no task, ask which one.
2. Read the referenced GDD chapter and the matching ARCHITECTURE section.
3. If the task needs a design decision the GDD doesn't cover, **ask the user** (they are the lead game designer). Don't invent gameplay rules; for pure tuning numbers, pick a sensible default in the config asset and say so.
4. Implement in `Assets/_Game/` following ARCHITECTURE. Keep the change to that one task.
5. Verify (see below) against the task's "Gata când" criteria.
6. Mark the task `[x]` in `FEATURE_PLAN.md` (or `[~]` if partial, with a note) and summarize: what changed, how to test it in the sandbox scene, anything left open.

## Code rules (short version of ARCHITECTURE)

- C#, English identifiers and comments. Namespace mirrors folder: `Game.Player`, `Game.Combat`, …
- Small single-purpose components; communicate through interfaces (`IDamageable`, `IAimProvider`, `IInteractable`) and C# events.
- Every balance value lives in a ScriptableObject config in `Assets/_Game/Data/`. No gameplay magic numbers in code.
- Pure logic (floor generation, camera framing, stat modifiers, inventory) in plain C# classes with EditMode tests.
- Input only through `PlayerInputReader` (Input System). Never `UnityEngine.Input`.
- No `FindObjectOfType` / `GameObject.Find` / tag lookups at runtime. Serialized references or injection by the creator.
- Prefabs: gameplay on the root, graphics only in a `Visual` child; gameplay never reaches into `Visual`.
- Pool frequently spawned objects (projectiles). Avoid per-frame allocations in `Update`.
- `[SerializeField] private` fields, not public fields. No leftover `Debug.Log`.
- Don't build generic systems for hypothetical future needs.

## Unity editor from the command line

The project has `com.unity.pipeline`, and the `unity` CLI is installed. Use it to compile, test and play instead of guessing. Read the `unity-pipeline` skill (via the `unity:using-unity` skill index) before the first use.

```powershell
unity status --until-ready --project-path .
unity command script recompile --project-path .
unity command script recompile_status --project-path .     # repeat until completed / up_to_date
unity command test run --mode editor --project-path .
```

If the Editor isn't reachable, ask the user to open the project in Unity (or focus the Editor window) rather than working blind. Don't create or edit `.unity` / `.prefab` YAML by hand when the Editor can do it.

## Boundaries

- **Code outside `Assets/_Game/`** (`Assets/Scripts/`, `Assets/Prefabs/`, `Assets/Scenes/`) predates the current structure. Don't modify, move or delete it without asking the user.
- Don't touch `ProjectSettings/` or `Packages/manifest.json` unless the task requires it, and say so when you do.
- **GDD (`docs/gdd/`) belongs to the user.** Don't change design text on your own; propose changes in chat.
- **Never write anything about teammates (names, assignments, deadlines) in any file** unless the user explicitly approves it.
- **Git:** commit or push only when the user asks. Work on a branch (e.g. `feat/M1-04-ranged-attack`), never directly on `main`; changes reach `main` only through a PR. Before any push, list for the user exactly which files will be pushed and which are gitignored.
- Talk to the user in Romanian; write code and code comments in English.
