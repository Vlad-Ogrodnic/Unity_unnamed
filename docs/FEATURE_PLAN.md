# Feature plan

Lista de lucru pentru MVP, împărțită pe milestones, în ordinea din GDD cap. 13. Fiecare task devine un GitHub issue cu același ID în titlu (de exemplu `[M1-03] Player aim`).

**Cum se citește un task**
- **Depinde de:** ce trebuie să fie gata înainte.
- **Gata când:** criteriile de acceptare; se verifică manual în scena sandbox (plus testele, unde scrie).
- **GDD:** capitolul cu regulile de design.
- Detaliile tehnice (ce clase, unde stau) sunt în [ARCHITECTURE.md](ARCHITECTURE.md).

**Status:** `[ ]` de făcut · `[~]` în lucru · `[x]` gata. Când trecem pe GitHub Issues, statusul se ține acolo, iar aici rămâne doar lista.

## Issues pe GitHub

Un issue acoperă un **sistem întreg** (mai multe task-uri de mai jos); task-urile apar ca checklist în issue. Fiecare issue corespunde în mare unui folder din ARCHITECTURE, ca oamenii care lucrează în paralel să nu modifice aceleași fișiere.

| # | Issue | Task-uri | Milestone |
|---|---|---|---|
| 1 | Fundația proiectului | M0-01…05 | M0 |
| 2 | Player: input, mișcare, aim | M1-01, M1-02 | M1 |
| 3 | Health, damage și atacul ranged | M1-03, M1-04 | M1 |
| 4 | Camera orthographic | M1-06, M3-04 | M1 |
| 5 | Inamici: Chaser, Shooter, Charger, Turret | M1-05, M2-06…08 | M1–M2 |
| 6 | Kit de luptă: melee, bombe, obstacole distructibile | M2-01, M2-04, M2-05 | M2 |
| 7 | Inventar, potions, pickups, moarte și restart | M2-02, M2-03 | M2 |
| 8 | Hit feedback: flash, hit stop, screen shake | M2-09 | M2 |
| 9 | Rooms și uși + template-uri | M3-01, M3-05 | M3 |
| 10 | Generarea etajului + minimap | M3-02, M3-03, M3-06 | M3 |
| 11 | Items și economie: stats, coins, choice room, Forge | M4-02…05 | M4 |
| 12 | Run complet: RunManager, 3 etaje, HUD, meniuri (+ boss opțional) | M4-01, M4-06…08 | M4 |

Issues se creează pe GitHub pe măsură ce ajungem la milestone-ul lor. Numerele din tabel sunt orientative; contează numărul real de pe GitHub.

**Paralelizare:** în fiecare milestone, task-urile fără dependențe între ele pot fi luate de oameni diferiți. Sunt marcate în tabelul de la începutul fiecărui milestone.

---

## M0 · Fundația proiectului

Scop: o bază curată pe care pot lucra mai mulți oameni fără conflicte. Rapid, dar înaintea oricărui feature.

| ID | Task | Depinde de |
|---|---|---|
| M0-01 | Structura de foldere și asmdefs | – |
| M0-02 | Input System: acțiunile jocului | M0-01 |
| M0-03 | Layers și collision matrix | – |
| M0-04 | Contractele de bază (`Game.Core`) | M0-01 |
| M0-05 | Scena sandbox și materialele placeholder | M0-01 |

- [x] **M0-01 Structura de foldere și asmdefs**
  Creează `Assets/_Game/` conform ARCHITECTURE §2, cu `Game.Runtime`, `Game.Editor`, `Game.Tests.EditMode`, `Game.Tests.PlayMode`.
  *Gata când:* proiectul compilează; un test EditMode gol rulează și trece. Codul vechi din `Assets/Scripts/` nu e modificat.

- [x] **M0-02 Input System: acțiunile jocului**
  `GameControls.inputactions` cu action map-ul `Gameplay`: Move (WASD), Aim (poziția mouse-ului), Fire (LMB, ținut), Melee (RMB), Bomb (E, apăsat/eliberat), Potion (Q), Interact (F), Dash (Space), Pause (Esc). Clasa C# generată.
  *Gata când:* acțiunile există și pot fi citite dintr-un script de test. **GDD:** cap. 3, Controale.
  *Notă:* setarea `activeInputHandler` rămâne pe „Both” cât timp codul vechi mai folosește `Input.GetAxis`.

- [x] **M0-03 Layers și collision matrix**
  Layers și matricea din ARCHITECTURE §3.
  *Gata când:* layers există, matricea e setată conform tabelului.

- [x] **M0-04 Contractele de bază**
  `Team`, `DamageInfo`, `IDamageable`, `IAimProvider`, `IInteractable`, `PrefabPool`.
  *Gata când:* compilează; teste EditMode pentru `PrefabPool` (ia/returnează obiecte).

- [x] **M0-05 Scena sandbox și materialele placeholder**
  `Scenes/Sandbox/Sandbox_Template.unity`: podea, câțiva pereți, lumină, camera. Materiale simple: jucător, inamic, proiectil jucător, proiectil inamic, obstacol, pickup.
  *Gata când:* scena se deschide și arată un room gol; fiecare programator își poate face o copie.

---

## M1 · Un room jucabil

Scop: primul lucru jucabil. Jucătorul se mișcă, țintește, trage și omoară un inamic, văzut prin camera finală.

| ID | Task | Depinde de |
|---|---|---|
| M1-01 | Player movement | M0-02 |
| M1-02 | Player aim | M0-02, M0-04 |
| M1-03 | Health și damage | M0-04 |
| M1-04 | Atacul ranged și proiectile | M1-02, M1-03 |
| M1-05 | Inamicul Chaser | M1-03 |
| M1-06 | Camera orthographic pentru un room | M0-05 |

Paralel: M1-01 / M1-02 / M1-03 / M1-06 se pot face simultan.

- [ ] **M1-01 Player movement**
  `PlayerInputReader`, `PlayerMovement`, `PlayerConfig`. Prefab `Player` (capsulă în `Visual`).
  *Gata când:* jucătorul se mișcă în 8 direcții, cu viteză constantă pe diagonală, se oprește la pereți, nu se rotește și nu cade. Viteza se schimbă din `PlayerConfig`. **GDD:** cap. 3.

- [ ] **M1-02 Player aim**
  `PlayerAim` (`IAimProvider`): raycast de la mouse pe planul podelei. Un indicator simplu de direcție în `Visual`.
  *Gata când:* direcția urmează cursorul corect în orice colț al ecranului, la unghiuri de cameră între 60° și 90°. **GDD:** cap. 1, Țintire.

- [ ] **M1-03 Health și damage**
  `Health` (`IDamageable`) cu i-frames opționale și evenimente; `HitFlash` pe `Visual`.
  *Gata când:* teste EditMode/PlayMode: damage scade HP, i-frames blochează damage-ul repetat, la 0 HP se declanșează `Died` o singură dată. Obiectul lovit clipește alb. **GDD:** cap. 3 HP, cap. 4.

- [ ] **M1-04 Atacul ranged și proiectile**
  `RangedAttack`, `Projectile`, `RangedAttackConfig`; proiectile din pool.
  *Gata când:* cât timp e ținut LMB, jucătorul trage cam 3 proiectile/s spre cursor; proiectilele dispar la obstacole și după bătaia maximă; fac 1 damage inamicilor și nu lovesc jucătorul. **GDD:** cap. 4, Atacul ranged.

- [ ] **M1-05 Inamicul Chaser**
  `ChaserBehaviour`, `ContactDamage`, `EnemyConfig`. Prefab `Enemy_Chaser`.
  *Gata când:* inamicul merge spre jucător, îi ia 1 HP la contact, moare din 3–4 proiectile. Jucătorul are i-frames ~1 s și knockback mic după lovitură. **GDD:** cap. 4, Damage primit.

- [ ] **M1-06 Camera orthographic pentru un room**
  `RoomCameraRig`, `CameraConfig`, `CameraFraming` (funcție pură, cu teste).
  *Gata când:* camera orthographic la 75° încadrează tot room-ul la 16:9 și 16:10; unghiul se schimbă din config fără să iasă room-ul din cadru. **GDD:** cap. 1, Camera.

**Demo M1:** în scena sandbox, jucătorul omoară 3 Chasers fără să fie omorât.

---

## M2 · Lupta completă

Scop: tot kit-ul jucătorului și cele 4 tipuri de inamici, într-un singur room.

| ID | Task | Depinde de |
|---|---|---|
| M2-01 | Atacul melee | M1-02, M1-03 |
| M2-02 | Moartea jucătorului și restart | M1-03 |
| M2-03 | Inventar, potions, pickups automate | M1-03 |
| M2-04 | Bombe | M1-02, M1-03, M2-03 |
| M2-05 | Obstacole distructibile | M1-03 |
| M2-06 | Inamicul Shooter | M1-04, M1-05 |
| M2-07 | Inamicul Charger | M1-05 |
| M2-08 | Inamicul Turret | M1-04 |
| M2-09 | Hit stop și screen shake | M1-03 |

Paralel: melee / inventar / inamici / feedback sunt independente.

- [ ] **M2-01 Atacul melee:** arc de ~120° spre aim, ~3× damage-ul unui proiectil, knockback, cooldown ~0,8 s, separat de ranged. *Gata când:* lovește doar inamicii din arc, îi împinge, cooldown-ul se respectă. **GDD:** cap. 4.
- [ ] **M2-02 Moartea jucătorului și restart:** la 0 HP jocul se oprește și apare un ecran simplu „Ai murit — R pentru restart”. *Gata când:* restart-ul readuce room-ul la starea inițială.
- [ ] **M2-03 Inventar, potions, pickups automate:** `PlayerInventory` (coins, potions max 3, bombe), `Pickup`. Q bea un potion (+2 HP, instant, fără încetinire). *Gata când:* pickup-urile se iau la atingere doar dacă e loc; potion-ul nu trece de HP maxim; teste EditMode pentru inventar. **GDD:** cap. 3, Healing; Controale.
- [ ] **M2-04 Bombe:** `BombThrower`, `Bomb`, `BombConfig`. *Gata când:* cât e ținut E apare cercul de aterizare (limitat la distanța maximă); la eliberare bomba zboară în arc peste obstacole, cu umbră pe podea, și explodează la aterizare, lovind tot din rază (inclusiv jucătorul) și distrugând obstacolele distructibile. **GDD:** cap. 7, Bombe.
- [ ] **M2-05 Obstacole distructibile:** `DestructibleObstacle` (dispare la explozie). Obstacolele normale blochează mișcarea și proiectilele.
- [ ] **M2-06 Inamicul Shooter:** păstrează distanța și trage proiectile lente spre jucător.
- [ ] **M2-07 Inamicul Charger:** stă, „telegrafiază” (clipește ~0,5 s), apoi face charge în linie dreaptă; se oprește în pereți.
- [ ] **M2-08 Inamicul Turret:** staționar, trage la intervale fixe într-un pattern simplu.
- [ ] **M2-09 Hit stop și screen shake:** declanșate de melee și de explozii, prin evenimente. Intensitatea din config.

Fiecare inamic: *Gata când* comportamentul e lizibil (jucătorul înțelege ce urmează să facă), valorile sunt în `EnemyConfig`, iar inamicul poate fi omorât cu ambele atacuri. **GDD:** cap. 5 (de scris).

**Demo M2:** un room cu câte un inamic de fiecare tip, învins folosind toate atacurile.

---

## M3 · Dungeon

Scop: un etaj generat, cu rooms conectate și explorare.

| ID | Task | Depinde de |
|---|---|---|
| M3-01 | Room și Door | M1-05 |
| M3-02 | Generarea layout-ului (logică pură) | M0-04 |
| M3-03 | Construirea etajului în scenă | M3-01, M3-02 |
| M3-04 | Camera între rooms | M1-06, M3-01 |
| M3-05 | Template-uri de rooms | M3-01 |
| M3-06 | Minimap | M3-03 |

Paralel: M3-01 și M3-02 sunt independente; M3-05 se poate face de un designer imediat ce există M3-01.

- [ ] **M3-01 Room și Door:** stările `Unvisited → Active → Cleared`, ușile se închid la intrare și se deschid când moare ultimul inamic, drop la final. **GDD:** cap. 2, Bucla unui room.
- [ ] **M3-02 Generarea layout-ului:** `FloorLayoutGenerator` cu seed. *Gata când:* teste EditMode: toate rooms accesibile din start, exact un start room și un boss room (cel mai departe de start), un choice room și un Forge, același seed dă același layout. **GDD:** cap. 2, Structura unui etaj.
- [ ] **M3-03 Construirea etajului:** `FloorBuilder` instanțiază template-uri și le leagă ușile. *Gata când:* la fiecare Play apare un etaj diferit, toate ușile duc unde trebuie.
- [ ] **M3-04 Camera între rooms:** tranziție lină la intrarea într-un room nou; înlocuiește logica bazată pe tag-uri.
- [ ] **M3-05 Template-uri de rooms:** ~15 prefabs (mărimi și forme diferite, obstacole, puncte de spawn). Ghid scurt pentru cine face rooms noi. **GDD:** cap. 8 (de scris).
- [ ] **M3-06 Minimap:** arată rooms vizitate, room-ul curent și vecinii văzuți.

**Demo M3:** un etaj complet explorabil, cu inamici în fiecare combat room.

---

## M4 · Run complet

Scop: de la main menu la final de run, 3 etaje.

| ID | Task | Depinde de |
|---|---|---|
| M4-01 | RunManager și 3 etaje | M3-03 |
| M4-02 | Coins și drop-uri | M2-03, M3-01 |
| M4-03 | Items pasive și PlayerStats | M2-03 |
| M4-04 | Choice room | M4-03 |
| M4-05 | Forge | M4-02, M4-03 |
| M4-06 | HUD | M2-03 |
| M4-07 | Main menu, pauză, final de run | M4-01 |
| M4-08 | Boss (opțional) | M2 |

- [ ] **M4-01 RunManager și 3 etaje:** trecerea la etajul următor după boss room; dificultatea crește pe etaje. **GDD:** cap. 2, Structura unui run.
- [ ] **M4-02 Coins și drop-uri:** drop la curățarea unui room și din inamici, după config.
- [ ] **M4-03 Items pasive și PlayerStats:** `ItemDefinition`, `StatModifier`, 10–15 items simple (damage, fire rate, viteză, HP maxim, bătaie...). Teste EditMode pentru stats.
- [ ] **M4-04 Choice room:** 3 items, alegi unul cu F, celelalte dispar.
- [ ] **M4-05 Forge:** upgrades pentru ranged și melee + potions/bombe, plătite cu coins, cumpărate cu F.
- [ ] **M4-06 HUD:** HP segmentat, potions, bombe, coins, prompt-ul de interacțiune.
- [ ] **M4-07 Main menu, pauză, final de run:** start run, pauză cu Esc, ecran de final (moarte / victorie) cu restart.
- [ ] **M4-08 Boss (opțional):** dacă nu e timp, ultimul room al etajului e un room de luptă mai greu. **GDD:** cap. 6.

**Demo M4 = MVP:** un run complet de 3 etaje, jucabil de la main menu până la ecranul de final.

---

## După MVP

Ordinea din GDD cap. 13, Nice to have: meta-progression, Merchant, dash ca item, bosses pe fiecare etaj, rooms cu pressure plates, keybinds custom, setarea unghiului camerei, secret rooms, personaje noi, hub. Se detaliază în task-uri când ajungem acolo.
