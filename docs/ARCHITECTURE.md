# Arhitectura codului

Cum e organizat codul de gameplay. Regulile generale de lucru (branch-uri, PR-uri, prefabs cu `Visual`) sunt în [CONTRIBUTING.md](../CONTRIBUTING.md); design-ul jocului e în [GDD](gdd/GDD.md) (copie a Google Doc-ului). Aici sunt deciziile tehnice: ce sisteme există, cum comunică și unde stă fiecare lucru.

Dacă un feature nou nu se potrivește cu ce scrie aici, se discută și se actualizează documentul **înainte** de a scrie codul.

---

## 1. Principii

1. **Componente mici, cu o singură responsabilitate.** `Health` nu știe de UI, `PlayerMovement` nu știe de arme. Un obiect de gameplay e o compoziție de componente.
2. **Comunicare prin contracte, nu prin tipuri concrete.** Sistemele vorbesc prin interfețe (`IDamageable`, `IAimProvider`, `IInteractable`) și prin evenimente C# (`event Action<...>`). Proiectilul lovește un `IDamageable`, nu un `Enemy`.
3. **Date separate de logică.** Toate valorile de balans (HP, damage, viteze, cooldowns, prețuri) stau în ScriptableObjects din `_Game/Data/`. În cod nu există numere magice de gameplay.
4. **Logica pură e C# simplu, testabil.** Generarea etajelor, calculul încadrării camerei, stats cu modificatori: clase fără `MonoBehaviour`, cu teste EditMode. `MonoBehaviour`-ul doar le apelează și aplică rezultatul în scenă.
5. **Gameplay separat de grafică.** Root-ul prefab-ului are scripturile și colliderele; copilul `Visual` are doar grafica. Gameplay-ul nu caută nimic în `Visual`; feedback-ul vizual ascultă evenimente (vezi §6).
6. **Fără căutări globale la runtime.** Fără `FindObjectOfType`, `GameObject.Find`, tag-uri în `Update`. Referințele se setează în Inspector sau sunt injectate de cine creează obiectul (de exemplu `FloorBuilder` dă fiecărui `Room` referințele de care are nevoie).
7. **Simplu înainte de general.** Nu construim sisteme generice „pentru viitor”. Când al doilea caz real apare, atunci extragem abstracția.

---

## 2. Structura de foldere

```
Assets/_Game/
├── Scripts/
│   ├── Runtime/            Game.Runtime.asmdef
│   │   ├── Core/           Health, DamageInfo, Team, interfețe comune, pooling, utilitare
│   │   ├── Player/         input, mișcare, aim, inventar, interacțiune
│   │   ├── Combat/         ranged, melee, proiectile, bombe
│   │   ├── Enemies/        comportamente de inamici, contact damage
│   │   ├── Rooms/          Room, Door, spawn de inamici
│   │   ├── Dungeon/        generarea etajului (logică pură) + construirea lui în scenă
│   │   ├── CameraRig/      camera orthographic și încadrarea room-ului
│   │   ├── Items/          pickups, items pasive, stats cu modificatori
│   │   ├── Economy/        coins, Forge, choice room
│   │   ├── Run/            starea run-ului, trecerea între etaje, final de run
│   │   ├── Feedback/       hit flash, hit stop, screen shake
│   │   └── UI/             HUD, minimap, meniuri
│   ├── Editor/             Game.Editor.asmdef (unelte de editor, opțional)
│   └── Tests/
│       ├── EditMode/       Game.Tests.EditMode.asmdef
│       └── PlayMode/       Game.Tests.PlayMode.asmdef
├── Data/                   ScriptableObjects de balans (PlayerConfig, EnemyConfig_Chaser, ...)
├── Input/                  GameControls.inputactions
├── Prefabs/                Player, Enemies/, Projectiles/, Rooms/, Pickups/, UI/
├── Materials/              materialele placeholder, colorate pe categorii
└── Scenes/
    ├── Main/               scenele jocului (MainMenu, Game)
    └── Sandbox/            o scenă personală per programator
```

- **Namespace = folder:** `Game.Core`, `Game.Player`, `Game.Combat` etc.
- **Input:** `GameControls.cs` e generat automat din `Input/GameControls.inputactions` (în `Scripts/Runtime/Player/`, namespace `Game.Player`). Nu se editează de mână; se modifică asset-ul și Unity regenerează clasa.
- **Un singur asmdef de runtime** (`Game.Runtime`). Pentru mărimea proiectului, mai multe asmdefs ar adăuga doar fricțiune; folderele și namespace-urile țin ordinea.
- Codul vechi din `Assets/Scripts/` rămâne neatins până când echipa decide să-l înlocuiască.

---

## 3. Lumea jocului

- **3D, gameplay pe planul XZ.** Y e „în sus”. Tot ce se mișcă are Y fixat (constraints pe Rigidbody).
- **Unități:** 1 unitate = 1 metru. Jucătorul e o capsulă de ~1 m înălțime.
- **Camera:** orthographic, înclinată la 75° (reglabil 60–90°). Vezi §5, Camera.

### Layers (Project Settings → Tags and Layers)

| Layer | Ce conține |
|---|---|
| `Ground` | podeaua (există deja) |
| `Obstacle` | pereți, pietre, obstacole |
| `Player` | jucătorul |
| `Enemy` | inamicii |
| `PlayerProjectile` | proiectilele jucătorului |
| `EnemyProjectile` | proiectilele inamicilor |
| `Pickup` | coins, potions, bombe pe jos |
| `Interactable` | obiecte cu F (choice room, Forge) |
| `RoomTrigger` | triggerele care detectează intrarea în room |

Collision matrix: proiectilele jucătorului lovesc doar `Enemy` și `Obstacle`; cele ale inamicilor doar `Player` și `Obstacle`; `Pickup` și `Interactable` interacționează doar cu `Player`. Astfel nu există friendly fire și nu trebuie verificat în cod.

Perechile care se ciocnesc (toate celelalte combinații între layers de mai sus sunt ignorate):

| Layer | Se ciocnește cu |
|---|---|
| `Ground` | `Ground`, `Player`, `Enemy` |
| `Obstacle` | `Player`, `Enemy`, `PlayerProjectile`, `EnemyProjectile` |
| `Player` | `Ground`, `Obstacle`, `Enemy`, `EnemyProjectile`, `Pickup`, `Interactable`, `RoomTrigger` |
| `Enemy` | `Ground`, `Obstacle`, `Player`, `Enemy`, `PlayerProjectile` |
| `PlayerProjectile` | `Obstacle`, `Enemy` |
| `EnemyProjectile` | `Obstacle`, `Player` |
| `Pickup`, `Interactable`, `RoomTrigger` | `Player` |

`Enemy` × `Enemy` e **provizoriu**: se decide în GDD cap. 5. Dacă unii inamici trebuie să treacă prin alții (de exemplu zburători), primesc un layer separat.

Layers vechi (`Default` etc.) se ciocnesc doar cu `Ground`, `Obstacle`, `Player`, `Enemy`, ca obiectele vechi să rămână solide. Testul EditMode `CollisionMatrixTests` verifică matricea; dacă o schimbi intenționat, actualizează tabelul și testul.

---

## 4. Contracte de bază (`Game.Core`)

```csharp
public enum Team { Player, Enemy }

public readonly struct DamageInfo
{
    public readonly int Amount;
    public readonly Vector3 Direction;      // pentru knockback, normalizat pe XZ
    public readonly float KnockbackForce;
    public readonly GameObject Source;
}

public interface IDamageable
{
    Team Team { get; }
    bool TryTakeDamage(in DamageInfo damage); // false dacă e invulnerabil (i-frames) sau mort
}
```

- **`Health`** (MonoBehaviour, implementează `IDamageable`): HP întreg, i-frames opționale, evenimente `Damaged(DamageInfo)`, `Healed(int)`, `Died`. Folosit atât de jucător, cât și de inamici. Logica stă în `HealthState` (C# simplu, testat). Valorile vin din `HealthConfig`, din care derivă `PlayerConfig` și `EnemyConfig`, ca fiecare personaj să aibă un singur asset de config.
- **`KnockbackState`**: împingere care scade liniar în timp. Cine se mișcă (jucător, inamici) o folosește în locul mișcării proprii cât e activă.
- **`DisableOnDeath`**: la `Died` oprește componentele din listă și, opțional, tot obiectul.
- **`PlanarMath`**: direcții pe XZ și intersecția unei raze cu podeaua. **`PhysicsLayers.CollisionMaskFor(layer)`**: masca de layers din collision matrix, ca query-urile (proiectile) să urmeze matricea fără măști duplicate în config.
- **`IAimProvider`**: `Vector3 AimDirection` (normalizat, pe XZ) și `Vector3 AimPoint` (punctul de pe podea). Jucătorul îl implementează cu mouse-ul; inamicii care trag pot avea propria implementare. Atacurile depind doar de această interfață, deci modul de țintire se poate schimba fără a atinge armele.
- **`IInteractable`**: `string Prompt`, `bool CanInteract(PlayerContext)`, `void Interact(PlayerContext)`.
- **Pooling:** proiectilele și efectele frecvente folosesc `PrefabPool<T>` (peste `UnityEngine.Pool.ObjectPool<T>`). Cine creează pool-ul le dă instanțelor un callback de `Release`; instanțele nu știu de pool.
- **`PlayerContext`**: ce poate accesa un `IInteractable` la jucător. Acum conține doar `GameObject`-ul jucătorului; inventarul și stats se adaugă când există (M2-03, M4-03).

---

## 5. Sistemele

Secțiunea descrie și sisteme care vin **după MVP** sau care sunt **încă nedecise** (marcate așa), ca să fie clar unde vor sta; nu se implementează înainte de task-ul lor din [FEATURE_PLAN.md](FEATURE_PLAN.md).

### Player
- `PlayerInputReader`: singurul loc care citește Input System. Expune valori (`Move`, `AimScreenPosition`) și evenimente (`MeleePressed`, `BombPressed/Released`, `PotionPressed`, `InteractPressed`; provizoriu `RestartPressed` pentru ecranul de moarte din M2-02, scos în M4-07). Rămâne activ după moarte. Restul componentelor nu știu de Input System.
- `PlayerMovement`: mișcare pe Rigidbody (velocity, interpolare), viteza din `PlayerConfig`.
- `PlayerAim` (`IAimProvider`): raycast de la mouse pe planul podelei. Primește camera prin Inspector sau `SetCamera` (de la cine creează jucătorul).
- `PlayerInventory`: potions (max 3, gol la start), cu limite și eveniment `Changed`; logica în `InventoryState` (testat). `PlayerPotionUse` bea la Q: refuză cu HP plin sau fără potions și anunță `UseFailed` (feedback: `DeniedShake` pe `Visual`). Coins se adaugă în M4-02; bombele doar dacă intră în MVP.
- `PlayerInteractor`: găsește cel mai apropiat `IInteractable`, afișează prompt-ul, apelează `Interact` la F.
- `PlayerStats`: valorile curente (damage, fire rate, viteză...) = config de bază + modificatori de la items/Forge.

### Combat
- `RangedAttack`: trage cât timp `TriggerHeld` e true, după rata de tragere din config (mai târziu din stats); ia proiectile din pool. Nu știe de input: la jucător, `PlayerAttackInput` leagă input-ul de atacuri; un inamic care trage va seta singur `TriggerHeld`.
- `Projectile`: se mișcă singur, verifică coliziunea cu `SphereCast` în direcția mișcării (sigur la viteze mari), lovește primul `IDamageable` sau dispare la obstacol / la bătaia maximă. Ce poate lovi vine din collision matrix pentru layer-ul lui (`PlayerProjectile` / `EnemyProjectile`). Valorile (damage, viteză, bătaie) vin ca `ProjectileStats`, construite de atac.
- `MeleeAttack`: arc în fața aim-ului (`OverlapSphere` + `PlanarMath.IsWithinArc`), damage + knockback la toți `IDamageable` din echipa opusă, cooldown propriu. Evenimente: `Swung(direction)` pentru grafică (`MeleeSwingVisual`) și `Hit(count)` pentru hit stop și screen shake.
- `BombThrower` + `Bomb` (de decis dacă intră în MVP): cerc de aterizare cât e ținută tasta; zbor în arc calculat (lerp pe XZ + parabolă pe Y), umbră pe podea, explozie cu `OverlapSphere` care lovește `IDamageable` și distruge `DestructibleObstacle`.

### Enemies
- Fiecare inamic: `Health` + `ContactDamage` + **un** component de comportament (`ChaserBehaviour`, `ShooterBehaviour`, `ChargerBehaviour`; `TurretBehaviour` după MVP) + `EnemyConfig`.
- Tipurile cu valori proprii își extind config-ul: `ShooterConfig` și `ChargerConfig` derivă din `EnemyConfig`.
- `ShooterBehaviour` e și `IAimProvider` pentru propriul `RangedAttack` (proiectile pe layer-ul `EnemyProjectile`). `ChargerBehaviour` anunță `TelegraphStarted(direction)` / `TelegraphEnded`; pe `Visual`, `TelegraphBlink` clipește și `ChargeDirectionIndicator` arată o săgeată pe podea.
- Comportamentele sunt mașini de stări mici, scrise direct în cod (enum + switch). Fără NavMesh în MVP: rooms sunt mici, mișcarea directă spre jucător + ocolire simplă e suficientă.
- Inamicul nu își caută singur ținta: `Room` îi dă referința la jucător când îl activează.

### Rooms și Dungeon
- `Room`: bounds (pentru cameră), lista de uși, punctele de spawn, starea (`Unvisited → Active → Cleared`), evenimentele `PlayerEntered`, `Cleared`. Când jucătorul intră într-un room necurățat: închide ușile, activează inamicii; când moare ultimul: deschide ușile, dă drop-ul.
- `Door`: deschis / închis, direcția (N/E/S/W), legătura cu room-ul vecin.
- `FloorLayoutGenerator` (C# pur, seed explicit, `System.Random`): produce un `FloorLayout` = grid de celule; fiecare room ocupă 1×1, 2×1, 1×2 sau 2×2 celule și are un tip, plus conexiunile dintre rooms vecine. Tipuri: Start, Combat, Boss, Choice, Forge (în M3 doar Start și Combat; restul în M4). Are teste EditMode (toate rooms accesibile, exact un start room, fără suprapuneri, determinism pe același seed).
- `FloorBuilder`: transformă `FloorLayout` în scenă, alegând template-uri de rooms de mărimea potrivită (prefabs cu `Room`).

### Camera
- `RoomCameraRig`: camera orthographic la unghiul din config. Când jucătorul intră într-un room, calculează mărimea orthographic și poziția din bounds-ul room-ului și face tranziția.
- Calculul încadrării e o funcție pură (`CameraFraming.Compute(playArea, roomArea, pitch, aspect, distance)`), cu teste. Regula: podeaua pe care se poate merge (`playArea`) se vede mereu toată; în rest room-ul cu tot cu pereți (`roomArea`) umple ecranul, deci se taie doar marginea exterioară a pereților. Fundalul apare doar când forma ecranului nu permite ambele (de exemplu 4:3).
- Rig-ul recalculează în fiecare frame, deci unghiul din `CameraConfig` și redimensionarea ferestrei se aplică pe loc. Până la M3-01, room-ul inițial se dă prin două collidere în scenă (`PlayArea` și `Floor`).
- În rooms mai mari decât ecranul (2×1, 1×2, 2×2), camera urmărește jucătorul în limitele room-ului (M3-04).

### Items, economie, run
- `ItemDefinition` (ScriptableObject): nume, descriere, raritate, listă de `StatModifier`. Efectele mai speciale se adaugă abia când apar în listă.
- `Pickup`: se ia automat la atingere dacă inventarul are loc; altfel rămâne pe jos.
- `RunManager`: etajul curent, seed-ul run-ului, trecerea la etajul următor, final de run (moarte / victorie). Un etaj e terminat când boss-ul e învins sau toate combat rooms sunt curățate; atunci apare scara (un `IInteractable`). Un singur obiect în scena `Game`; alte sisteme primesc de la el ce le trebuie, nu îl caută.

---

## 6. Feedback și UI

- Feedback-ul (hit flash, hit stop, screen shake, sunete) **ascultă evenimente** (`Health.Damaged`, lovitura melee, `Bomb.Exploded` dacă bombele intră). Gameplay-ul nu apelează direct efecte.
- `HitFlash` stă pe `Visual` și face flash alb când `Health` de pe root anunță damage. Așa, echipa de artă poate schimba grafica fără să atingă logica. Efectele care schimbă renderere (`HitFlash`, `InvulnerabilityBlink`, `TelegraphBlink`) primesc lista de renderere în Inspector (gol = toate din `Visual`), prin `RendererFlash`.
- `HitStop` (unul pe scenă) oprește timpul câteva frames; `ScreenShake` stă pe cameră și rulează după rig. `ImpactFeedback` le declanșează la `MeleeAttack.Hit`. Efectele vizuale folosesc timp real (`unscaledTime`), ca să meargă și în hit stop.
- `ProvisionalDeathScreen` (M2-02, IMGUI) e temporar și se înlocuiește în M4-07.
- UI-ul se abonează la evenimente (`PlayerInventory.Changed`, `Health.Damaged`) și nu modifică starea jocului.

---

## 7. Configurare și date

- Un ScriptableObject per tip de lucru: `PlayerConfig`, `RangedAttackConfig`, `MeleeAttackConfig`, `BombConfig`, `EnemyConfig`, `CameraConfig`, `FloorGenerationConfig`, `ItemDefinition`.
- Valorile de pornire sunt cele din GDD (cap. 3 și 4). Se reglează în Inspector în timpul playtest-ului, fără a modifica codul.

---

## 8. Teste

- **EditMode** pentru logica pură: generarea etajului, încadrarea camerei, stats cu modificatori, inventarul.
- **PlayMode** doar pentru câteva scenarii critice (proiectilul face damage, ușile se deschid după ce mor inamicii), când se stabilizează sistemele.
- Fiecare feature are o scenă sandbox în care se poate testa manual, conform criteriilor de acceptare din [FEATURE_PLAN.md](FEATURE_PLAN.md).
