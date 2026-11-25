# Feature plan

Lista de lucru pentru MVP, împărțită pe milestones, în ordinea din GDD cap. 13. Ce e încă nedecis stă în **De decis înainte de MVP**, iar ce am scos din MVP în **După MVP**, la final.

**Cum se citește un task**
- **Depinde de:** ce trebuie să fie gata înainte.
- **Gata când:** criteriile de acceptare; se verifică manual în scena sandbox (plus testele, unde scrie).
- **GDD:** capitolul cu regulile de design.
- Detaliile tehnice (ce clase, unde stau) sunt în [ARCHITECTURE.md](ARCHITECTURE.md).

**Status:** `[ ]` de făcut · `[~]` în lucru · `[x]` gata. Se bifează aici și în checklist-ul issue-ului.

**ID-urile nu se renumerotează.** Un task scos din MVP își păstrează ID-ul și se mută în secțiunea **După MVP**, ca să rămână valabile referințele din issues, branch-uri și commit-uri.

## Issues pe GitHub

Un issue pe milestone, cu task-urile ca checklist (titlu: `[M2] Lupta completă`), în milestone-ul GitHub cu același nume. PR-ul care termină milestone-ul închide issue-ul (`Closes #<nr>`).

**Paralelizare:** în fiecare milestone, task-urile fără dependențe între ele pot fi luate de oameni diferiți. Sunt marcate sub tabelul fiecărui milestone.

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

- [x] **M1-01 Player movement**
  `PlayerInputReader`, `PlayerMovement`, `PlayerConfig`. Prefab `Player` (capsulă în `Visual`).
  *Gata când:* jucătorul se mișcă în 8 direcții, cu viteză constantă pe diagonală, se oprește la pereți, nu se rotește și nu cade. Viteza se schimbă din `PlayerConfig`. **GDD:** cap. 3.

- [x] **M1-02 Player aim**
  `PlayerAim` (`IAimProvider`): raycast de la mouse pe planul podelei. Un indicator simplu de direcție în `Visual` (ascuns deocamdată: `AimPivot` e dezactivat în prefab).
  *Gata când:* direcția urmează cursorul corect în orice colț al ecranului, la unghiuri de cameră între 60° și 90°. **GDD:** cap. 1, Țintire.

- [x] **M1-03 Health și damage**
  `Health` (`IDamageable`) cu i-frames opționale și evenimente; `HitFlash` pe `Visual`.
  *Gata când:* teste EditMode/PlayMode: damage scade HP, i-frames blochează damage-ul repetat, la 0 HP se declanșează `Died` o singură dată. Obiectul lovit clipește alb. **GDD:** cap. 3 HP, cap. 4.

- [x] **M1-04 Atacul ranged și proiectile**
  `RangedAttack`, `Projectile`, `RangedAttackConfig`; proiectile din pool.
  *Gata când:* cât timp e ținut LMB, jucătorul trage cam 3 proiectile/s spre cursor; proiectilele dispar la obstacole și după bătaia maximă; fac 1 damage inamicilor și nu lovesc jucătorul. **GDD:** cap. 4, Atacul ranged.

- [x] **M1-05 Inamicul Chaser**
  `ChaserBehaviour`, `ContactDamage`, `EnemyConfig`. Prefab `Enemy_Chaser`.
  *Gata când:* inamicul merge spre jucător, îi ia 1 HP la contact, moare din 3–4 proiectile. Jucătorul are i-frames ~1 s și knockback mic după lovitură. **GDD:** cap. 4, Damage primit.

- [x] **M1-06 Camera orthographic pentru un room**
  `RoomCameraRig`, `CameraConfig`, `CameraFraming` (funcție pură, cu teste).
  *Gata când:* camera orthographic la 75° încadrează tot room-ul la 16:9 și 16:10; unghiul se schimbă din config fără să iasă room-ul din cadru. **GDD:** cap. 1, Camera.

**Demo M1:** în scena sandbox, jucătorul omoară 3 Chasers fără să fie omorât.

---

## M2 · Lupta completă

Scop: kit-ul de luptă al jucătorului din MVP și cele 3 tipuri de inamici, într-un singur room.

| ID | Task | Depinde de |
|---|---|---|
| M2-01 | Atacul melee | M1-02, M1-03 |
| M2-02 | Moartea jucătorului și restart (provizoriu) | M1-03 |
| M2-03 | Inventar, potions, pickups automate | M1-03 |
| M2-06 | Inamicul Shooter | M1-04, M1-05 |
| M2-07 | Inamicul Charger | M1-05 |
| M2-09 | Hit stop și screen shake | M1-03 |

Paralel: melee / inventar / inamici / feedback sunt independente. (M2-04 și M2-05 sunt la **De decis**, M2-08 în **După MVP**.)

- [x] **M2-01 Atacul melee:** RMB, arc de ~90° spre aim (GDD: ~120°, redus după primul playtest), rază scurtă, ~3× damage-ul unui proiectil, knockback, cooldown ~0,8 s, separat de ranged. *Gata când:* lovește doar inamicii din arc, îi împinge, cooldown-ul se respectă, ranged și melee pot fi alternate. **GDD:** cap. 4, Atacul melee.
- [x] **M2-02 Moartea jucătorului și restart (provizoriu):** la 0 HP jocul se oprește și apare un ecran simplu „Ai murit — R pentru restart”. Tasta R există doar pe acest ecran provizoriu, care se înlocuiește în M4-07. *Gata când:* restart-ul readuce room-ul la starea inițială.
- [x] **M2-03 Inventar, potions, pickups automate:** `PlayerInventory` (deocamdată doar potions: max 3, inventar gol la start; coins vin în M4-02), `Pickup`. Q bea un potion: +2 HP, instant, fără încetinire. Cu HP plin nu se poate bea; apare un mic semnal vizual că n-a mers. *Gata când:* pickup-urile se iau la atingere doar dacă e loc, altfel rămân pe jos; potion-ul nu trece de HP maxim; teste EditMode pentru inventar. **GDD:** cap. 3, Healing și potions; Controale.
- [x] **M2-06 Inamicul Shooter:** păstrează distanța și trage proiectile lente spre jucător.
- [x] **M2-07 Inamicul Charger:** stă, „telegrafiază” (clipește ~0,5 s, iar o săgeată pe podea arată direcția), apoi face charge în linie dreaptă; se oprește în pereți.
- [x] **M2-09 Hit stop și screen shake:** la lovitura melee (și la explozia bombei, dacă bombele intră), prin evenimente. Intensitatea din config. **GDD:** cap. 4, Hit feedback.

Fiecare inamic: *Gata când* comportamentul e lizibil (jucătorul înțelege ce urmează să facă), valorile sunt în `EnemyConfig`, iar inamicul poate fi omorât cu ambele atacuri. **GDD:** cap. 13 (MVP); cap. 5 e încă de scris.

**Demo M2:** un room cu câte un Chaser, Shooter și Charger, învinși folosind ranged și melee.

---

## M3 · Dungeon

Scop: un etaj generat, cu rooms conectate și explorare. În M3 etajul are start room și combat rooms; boss room, choice room și Forge se adaugă în M4.

**Ținta imediată:** M2 + M3, adică un etaj jucabil.

| ID | Task | Depinde de |
|---|---|---|
| M3-01 | Room și Door | M1-05 |
| M3-02 | Generarea layout-ului (logică pură) | M0-04 |
| M3-03 | Construirea etajului în scenă | M3-01, M3-02 |
| M3-04 | Camera între rooms și în rooms mari | M1-06, M3-01 |
| M3-05 | Template-uri de rooms | M3-01 |
| M3-06 | Minimap | M3-03 |

Paralel: M3-01 și M3-02 sunt independente; M3-05 se poate face de un designer imediat ce există M3-01.

- [ ] **M3-01 Room și Door:** stările `Unvisited → Active → Cleared`; ușile unui combat room se închid la intrare și se deschid când moare ultimul inamic; rooms curățate rămân sigure. La curățare room-ul poate lăsa un drop (în M3: potion, cu șansă din config; coins se adaugă în M4-02). **GDD:** cap. 2, Bucla unui room.
- [ ] **M3-02 Generarea layout-ului:** `FloorLayoutGenerator` cu seed, pe grid. Rooms ocupă 1×1, 2×1, 1×2 sau 2×2 celule. În M3: un start room și combat rooms (tipurile Boss, Choice, Forge vin în M4). *Gata când:* teste EditMode: toate rooms accesibile din start, exact un start room, rooms nu se suprapun, ușile leagă doar rooms vecine, același seed dă același layout. **GDD:** cap. 2, Structura unui etaj.
- [ ] **M3-03 Construirea etajului:** `FloorBuilder` instanțiază template-uri potrivite ca mărime și le leagă ușile. *Gata când:* la fiecare Play apare un etaj diferit, toate ușile duc unde trebuie.
- [ ] **M3-04 Camera între rooms și în rooms mari:** tranziție lină la intrarea într-un room nou; în rooms mai mari decât ecranul, camera urmărește jucătorul în limitele room-ului. Înlocuiește logica veche bazată pe tag-uri. **GDD:** cap. 1, Camera.
- [ ] **M3-05 Template-uri de rooms:** ~15 prefabs, în toate cele 4 mărimi, cu obstacole și puncte de spawn. Design-ul rooms depinde de bombe (vezi **De decis**), deci decizia vine înainte de setul final. Ghid scurt pentru cine face rooms noi. **GDD:** cap. 8 (de scris).
- [ ] **M3-06 Minimap:** arată rooms vizitate, room-ul curent și vecinii descoperiți. **GDD:** cap. 2.

**Demo M3:** un etaj complet explorabil, cu inamici în fiecare combat room.

---

## M4 · Run complet

Scop: de la main menu la final de run: 3 etaje cu boss, scară, items și economie.

| ID | Task | Depinde de |
|---|---|---|
| M4-01 | RunManager, 3 etaje și scara | M3-03 |
| M4-02 | Coins și drop-uri | M2-03, M3-01 |
| M4-03 | Items pasive și PlayerStats | M2-03 |
| M4-04 | Choice room | M4-03 |
| M4-05 | Forge | M4-02, M4-03 |
| M4-06 | HUD | M2-03 |
| M4-07 | Main menu, pauză, final de run | M4-01 |
| M4-08 | Boss | M3-03 |

Paralel: M4-02 / M4-03 / M4-06 / M4-08 sunt independente.

- [ ] **M4-01 RunManager, 3 etaje și scara:** etajul se termină când ai învins boss-ul sau ai curățat toate combat rooms; atunci apare scara spre etajul următor, folosită cu F (cu prompt). După etajul 3: victorie. HP-ul, potions, coins și items se păstrează între etaje. Fără creștere de dificultate în MVP. **GDD:** cap. 2, Structura unui run.
- [ ] **M4-02 Coins și drop-uri:** coins doar la curățarea unui room, nu din inamici; drop-urile după config. **GDD:** cap. 2, Bucla unui room.
- [ ] **M4-03 Items pasive și PlayerStats:** `ItemDefinition`, `StatModifier`, 10–15 items pasive. Lista de items se decide întâi în GDD cap. 7. Teste EditMode pentru stats. **GDD:** cap. 3, Stats.
- [ ] **M4-04 Choice room:** 3 items, alegi unul cu F, celelalte dispar; room garantat pe fiecare etaj. **GDD:** cap. 2.
- [ ] **M4-05 Forge:** upgrades pentru ranged și melee + consumables, plătite cu coins, cumpărate cu F; room garantat pe fiecare etaj. Ce vinde exact se decide întâi în GDD cap. 7. **GDD:** cap. 2.
- [ ] **M4-06 HUD:** HP ca bară segmentată, potions, coins, prompt-ul de interacțiune (și bombe, dacă intră). **GDD:** cap. 3.
- [ ] **M4-07 Main menu, pauză, final de run:** minimal, doar cât să ne putem plimba între ecrane la playtest. Main menu: Play / Quit. Pauză (Esc): Resume / Main menu. Final de run: „Victorie” sau „Ai murit”, etajul atins, Restart / Main menu. Înlocuiește ecranul provizoriu din M2-02.
- [ ] **M4-08 Boss:** boss room, de obicei în cel mai îndepărtat punct față de start. Design-ul boss-ului se face întâi în GDD cap. 6. **GDD:** cap. 2, cap. 6.

**Demo M4 = MVP:** un run complet de 3 etaje, jucabil de la main menu până la ecranul de final.

---

## De decis înainte de MVP

- **Bombe (M2-04) și obstacole distructibile (M2-05):** intră sau nu în MVP. Trebuie decis înainte de setul final de template-uri (M3-05), pentru că design-ul rooms depinde de bombe. Dacă intră: design-ul din GDD cap. 7, iar explozia rănește și jucătorul (decis).
- **Locul scării** când etajul se termină prin curățarea tuturor combat rooms, fără boss.

---

## După MVP

Task-urile scoase din MVP (cu ID-ul păstrat) și ideile notate. Se detaliază în task-uri când ajungem la ele.

**Scoase din MVP**
- **M2-08 Inamicul Turret:** staționar.

**Idei notate**
- Mai multe tipuri de inamici.
- Dificultatea pe etaje: **nu** prin multiplicator de HP pentru același inamic (ar fi ciudat ca același inamic să devină mai puternic). Modul exact e de decis.
- Un item care îți dă un avantaj la run-ul următor.

**Din GDD cap. 13, Nice to have:** meta-progression, Merchant, dash ca item, câte un boss pe fiecare etaj, rooms cu pressure plates, keybinds custom, setarea unghiului camerei, secret rooms, personaje noi, hub.
