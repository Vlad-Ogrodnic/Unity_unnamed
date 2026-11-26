<!-- Copie a GDD-ului. Versiunea principală e Google Doc-ul echipei; această copie se actualizează manual. Modificările de design se fac în Google Doc, nu aici. -->

# Game Design Document

## Cuprins

1. [Concept](#1-concept)
2. [Core loop și structura unui run](#2-core-loop-și-structura-unui-run)
3. [Jucătorul](#3-jucătorul)
4. [Lupta](#4-lupta)
5. [Inamici](#5-inamici)
6. [Bosses](#6-bosses)
7. [Items și economie](#7-items-și-economie)
8. [Dungeon și level design](#8-dungeon-și-level-design)
9. [Meta-progression](#9-meta-progression)
10. [UI / UX](#10-ui--ux)
11. [Direcție artistică](#11-direcție-artistică)
12. [Audio](#12-audio)
13. [Scope și roadmap](#13-scope-și-roadmap)

---

# 1. Concept

## Structura jocului

**Alegere: Roguelite**

Jocul e format din **runs**. Un run înseamnă urcarea prin mai multe etaje generate procedural, fiecare încheiat cu un boss. La moarte, run-ul se termină și jucătorul o ia de la capăt cu un dungeon nou. Etajele sunt asamblate din **rooms făcute de mână, combinate aleatoriu**. Așa fiecare run e diferit, dar fiecare room e proiectat deliberat.

Între runs jucătorul păstrează o formă limitată de progres (**meta-progression**). Aceasta e gândită să răsplătească skill-ul, nu grind-ul:

- **Unlocks orizontale (majoritatea):** items, arme, abilități și personaje noi care intră în pool-ul de joc. Ele dau jucătorului **mai multe opțiuni**, nu mai multă putere. Items mai puternice sau mai complexe se deblochează mai târziu, după ce jucătorul a învățat bazele. Așa începătorii nu sunt copleșiți de RNG.
- **Vertical upgrades (puține și mici):** bonusuri permanente de stats (de exemplu HP sau damage), **limitate pe tiers**. Înainte de primul boss sunt disponibile doar câteva. Fiecare boss bătut **pentru prima dată** deblochează tier-ul următor. Progresul permanent depinde astfel de skill: nu poți face grind la nesfârșit ca să treci de un obstacol.

Meta currency și ecranul de unlocks sunt accesibile din meniu. Un hub jucabil, ca spațiu între runs, e o posibilă extindere ulterioară.

## Stilul de luptă (overview)

**Alegere: luptă mixtă, ranged + melee**

Jucătorul are două tipuri de atac, ambele îndreptate spre cursor:

- **Atacul principal e ranged:** proiectile trase continuu cât timp e ținut butonul. E instrumentul de bază și cel folosit majoritar.
- **Atacul secundar e melee:** o lovitură de aproape, puternică, gândită pentru situații specifice: inamici care au ajuns lângă jucător, inamici protejați frontal împotriva proiectilelor, curățarea rapidă a unui grup. Melee adaugă un element de risk/reward: faci mai mult damage, dar trebuie să te apropii.

Cele două atacuri se completează, iar items găsite în run le pot modifica pe fiecare separat. Detaliile (damage, cooldowns, interacțiuni) sunt descrise în secțiunea **Lupta**.

## Țintire și control

**Alegere: aim liber la 360°, separat de mișcare (twin-stick)**

Jucătorul se deplasează cu **WASD** și țintește independent cu **mouse-ul**. Atacurile pleacă în direcția cursorului, oricare ar fi direcția de mers. Așa poți să te retragi de un inamic trăgând în el în același timp, mecanica de bază a luptei.

Aim-ul liber e cea mai intuitivă variantă, inclusiv pentru jucătorii mai puțin experimentați, și garantează că orice inamic vizibil poate fi lovit. Pentru că țintirea devine ușoară, dificultatea vine din **poziționare**. Inamicii sunt proiectați să-l forțeze pe jucător să se miște constant: inamici care urmăresc jucătorul, atacuri pe zonă (AoE), proiectile care trebuie evitate și inamici rapizi. Statul pe loc și tragerea de la distanță nu trebuie să fie niciodată o strategie sigură.

## Camera

**Alegere: cameră orthographic, unghi implicit 75°, reglabil**

Jocul folosește o cameră **orthographic**, deci fără perspectivă: obiectele au aceeași mărime oriunde s-ar afla pe ecran, iar liniile paralele rămân paralele. Distanțele, vitezele proiectilelor și hitboxes se citesc la fel în orice zonă a ecranului, lucru esențial pentru un joc în care evitarea proiectilelor e o mecanică centrală. O cameră în perspectivă înclinată ar face ca obiectele din partea de sus a ecranului să pară mai mici și mai departe, iar proiectilele care se îndepărtează să pară mai lente, ceea ce ar îngreuna citirea distanțelor și evitarea atacurilor.

Unghiul implicit e **75°** față de orizontală. Podeaua apare turtită vertical cu ~3%, uniform pe tot ecranul, deci efectul e practic imperceptibil. Unghiul e un **parametru reglabil între 60° și 90°**, iar valoarea finală se stabilește prin playtest.

Camera încadrează câte un room pe ecran. În rooms mai mari decât ecranul, urmărește jucătorul în limitele room-ului. Trecerea dintr-un room în altul se face printr-o tranziție lină.

## Design pillars

Pilonii descriu experiența pe care vrem s-o ofere jocul. Sunt **ghiduri, nu reguli stricte**: ajută la evaluarea ideilor noi și la deciziile de design, dar un feature care nu se aliniază perfect cu ei nu e respins automat. Contează ca, per total, jocul să rămână fidel acestor direcții.

1. **Skill-ul contează mai mult decât norocul.** RNG-ul e binevenit și creează varietate și momente memorabile, inclusiv efecte puternice bazate pe șansă. Totuși, rezultatul unui run trebuie să depindă în principal de priceperea jucătorului. Momentele-cheie, precum boss fights, nu ar trebui decise de noroc. Meta-progression răsplătește progresul real, nu grind-ul.
2. **Mișcarea e totul.** Statul pe loc ar trebui să fie rareori o strategie sigură. Inamicii, layout-ul rooms și atacurile îl încurajează pe jucător să se repoziționeze constant.
3. **Lizibilitate înainte de toate.** Jucătorul trebuie să înțeleagă rapid ce îl poate lovi, de unde și cât de repede. Grafica și efectele vizuale susțin gameplay-ul, nu îl ascund.
4. **Fiecare run e diferit.** Combinațiile de items și de rooms fac ca fiecare încercare să se simtă nouă și să încurajeze strategii diferite.

## Pitch

*TODO*

---

# 2. Core loop și structura unui run

## Bucla principală

**Run → victorie sau moarte → meta currency → unlocks → run nou.** Meta currency se câștigă în funcție de cât de departe a ajuns jucătorul în run.

## Structura unui run

Un run are mai multe **etaje**. Fiecare etaj se termină cu un **boss**; după ce e învins, jucătorul urcă la etajul următor. Învingerea ultimului boss înseamnă câștigarea run-ului.

Inamicii devin mai puternici de la un etaj la altul, dar și jucătorul acumulează items și upgrades pe parcurs. Cele două cresc în paralel, astfel încât **dificultatea resimțită crește încet** pe parcursul run-ului.

*Valori inițiale: 3 etaje în MVP, un run complet de ~20–30 de minute. Scope-ul pornește mic și se poate extinde ulterior.*

## Structura unui etaj

Un etaj e un **grid de rooms** legate prin uși, generat la începutul etajului din rooms făcute de mână. Tipuri de rooms:

- **Start room:** sigur și gol.
- **Combat rooms (majoritatea):** inamici și obstacole.
- **Choice room (garantat):** 3 items, dintre care jucătorul alege unul, iar celelalte dispar.
- **Forge (garantat):** jucătorul cheltuie coins pe upgrades pentru atacul ranged sau melee și pe consumables.
- **Merchant (nu e garantat):** face schimburi, de exemplu un item pe alt item sau HP pe un item.
- **Boss room:** de obicei în cel mai îndepărtat punct față de start.

Jucătorul explorează etajul în ce ordine vrea. Un **minimap** arată rooms vizitate și pe cele vecine descoperite.

## Bucla unui room

Când jucătorul intră într-un combat room nou, **ușile se închid** până sunt eliminați toți inamicii. Apoi se deschid, iar room-ul poate lăsa un drop: coins, healing sau consumables. Rooms curățate rămân sigure. Explorarea completă a etajului aduce mai multe resurse, dar costă HP și timp.

*Posibil pe viitor: items care permit ieșirea dintr-un room înainte de a fi curățat.*

## Resurse în run

- **HP:** nu se regenerează automat (healing-ul e detaliat în cap. 3).
- **Coins:** se cheltuie la Forge și se pierd la finalul run-ului.
- **Items:** modifică atacurile, stats sau comportamentul jucătorului și se cumulează pe parcursul run-ului.
- **Consumables:** potions și bombe, detaliate în cap. 3 și 7.

---

# 3. Jucătorul

## Stats

Jucătorul are un set de stats de bază, modificate de items, de upgrades de la Forge și de vertical upgrades:

- **Max HP** și **viteza de mișcare**
- **Ranged:** damage, fire rate, viteza proiectilului, range
- **Melee:** damage, cooldown, raza și unghiul loviturii

Valorile exacte se stabilesc la balans și stau în date configurabile, nu în cod.

## Controale

| Acțiune | Tastă |
|---|---|
| Mișcare | WASD |
| Aim | Mouse |
| Atac ranged (ținut apăsat) | Click stânga |
| Atac melee | Click dreapta |
| Potion | Q |
| Bombă (ții apăsat pentru țintire, eliberezi pentru aruncare) | E |
| Interacțiune (choice room, Forge, Merchant) | F |
| Dash (doar dacă e deblocat) | Space |
| Pauză | Esc |

Acțiunile folosite în luptă (potion, bombă) stau pe tastele apăsate fără a lua degetele de pe WASD. Interacțiunea se folosește doar în rooms curățate, deci poate sta pe F. Tastele pot fi schimbate din setări (vezi cap. 10).

**Pickups:** coins, potions și bombe se iau **automat, la atingere**. Dacă stocul e plin, pickup-ul rămâne pe jos. Deciziile care costă ceva sau sunt ireversibile (alegerea din choice room, cumpărăturile la Forge, schimburile la Merchant) cer apăsarea tastei **F**, iar un prompt apare deasupra obiectului când jucătorul e aproape.

## HP

HP-ul e **discret**: jucătorul începe cu, de exemplu, 6 HP, iar majoritatea loviturilor iau 1 HP, cele puternice 2. Pe ecran apare ca o bară segmentată, deci jucătorul știe mereu câte lovituri mai poate încasa. După ce e lovit, jucătorul are o scurtă perioadă de **i-frames**.

## Healing și potions

Jucătorul nu se vindecă la ridicarea unui pickup, ci prin **potions** pe care le bea când decide:

- poate avea **maximum 3 potions**;
- fiecare vindecă o cantitate fixă, de exemplu 2 HP;
- băutul e **instant** și nu încetinește jucătorul (de verificat în playtest);
- potions se găsesc ca drops și se cumpără de la Forge.

## Dash

Jucătorul **nu are dash la început**. Dash-ul poate fi obținut ca item sau unlock și oferă o opțiune de mobilitate în plus. Decizia finală se ia după playtest.

---

# 4. Lupta

*Toate valorile din acest capitol sunt de pornire și se reglează prin playtest.*

## Atacul ranged

- Cât ții apăsat click stânga, jucătorul trage automat, cam 3 proiectile pe secundă, în direcția aim-ului.
- Proiectilele au viteză medie și bătaie limitată: dispar după aproximativ 2/3 din lățimea unui room.
- Proiectilele sunt blocate de obstacole.
- Fiecare proiectil face 1 damage, iar un inamic de bază moare din 3–4 lovituri.
- La Forge se pot îmbunătăți damage-ul, rata de tragere, bătaia și viteza proiectilelor.

## Atacul melee

- Click dreapta dă o lovitură în arc, de aproximativ 120°, în direcția aim-ului, cu rază scurtă.
- Face cam 3× damage-ul unui proiectil și aplică knockback.
- Are un cooldown de aproximativ 0,8 s.
- Melee-ul e varianta riscantă, dar puternică: trebuie să intri aproape, dar faci mult damage dintr-o dată.
- Ranged și melee au cooldown-uri separate și pot fi alternate.

## Damage primit și i-frames

- Contactul cu un inamic sau cu un proiectil inamic ia 1 HP. Unii inamici de pe etajele avansate și bosses pot lua 2.
- După o lovitură, jucătorul are aproximativ 1 s de i-frames, în care personajul clipește, plus un knockback mic.

## Hit feedback

- Inamicul lovit clipește alb.
- Lovitura melee și explozia bombei au un mic hit stop (o pauză de câteva frames) și un screen shake mic.
- Nu afișăm cifre de damage, ca ecranul să rămână curat.

---

# 5. Inamici

*TODO: arhetipuri de comportament, lista inamicilor, scalarea dificultății pe etaje.*

---

# 6. Bosses

*TODO: rolul bosses în run, structura unui boss fight (faze, attack patterns), lista bosses.*

---

# 7. Items și economie

*TODO: tipuri de items, coins, choice room, Forge, Merchant, raritate și item pools.*

## Bombe (propunere, de confirmat prin playtest)

Bomba e un consumable cu stoc limitat, folosit pentru damage pe zonă și pentru a distruge obstacole.

- Cât timp ții tasta apăsată, pe podea apare un **cerc care arată unde va ateriza**. Cercul urmează cursorul, până la o distanță maximă.
- Bomba e aruncată **în arc, peste obstacole și inamici**, și explodează la aterizare. Spre deosebire de atacul ranged, nu e blocată de obstacole, deci poate lovi inamicii ascunși.
- Bomba are o **umbră pe podea**, ca să se vadă unde e, la orice unghi al camerei.
- Explozia face damage pe zonă și distruge obstacolele din raza ei. Secret rooms nu intră în MVP.

*Variante de testat: o fracțiune de secundă de fuse după aterizare; items care modifică bomba (de exemplu mină sau shockwave).*

---

# 8. Dungeon și level design

*TODO: reguli de generare a etajelor, ghid de design pentru rooms, obstacole și elemente de mediu.*

---

# 9. Meta-progression

*TODO: meta currency, lista de unlocks orizontale, vertical upgrades și tier-urile lor.*

---

# 10. UI / UX

*TODO: HUD, minimap, meniuri (main menu, pauză, unlocks), ecranul de final de run.*

**Keybinds custom (nice to have):** jucătorul poate schimba tastele din meniul de setări. Tastele din cap. 3 sunt doar default-urile.

---

# 11. Direcție artistică

*TODO (echipa de artă): stil vizual, temă. Constrângeri din gameplay: lizibilitate, cameră orthographic la 75°, grafica separată de logică în prefabs.*

---

# 12. Audio

*TODO: muzică (pe etaje / boss), SFX esențiale pentru feedback.*

---

# 13. Scope și roadmap

## MVP

Scopul MVP-ului e un run complet, jucabil de la început la final, cu placeholder art.

- **Jucătorul:** mișcare, aim 360°, ranged, melee, HP discret cu i-frames, potions, bombe.
- **Camera:** orthographic la 75°, încadrarea fiecărui room și tranziția între rooms.
- **Rooms:** uși care se blochează și se deblochează, obstacole (unele distruse de bombe), aproximativ 15 template-uri făcute de mână.
- **Etaje:** generare pe grid cu start room, combat rooms, choice room, Forge și boss room, plus minimap. 3 etaje.
- **Inamici:** 4 arhetipuri simple: unul care te urmărește, unul care trage de la distanță, unul care face charge, unul staționar (turret).
- **Items și economie:** coins, aproximativ 10–15 items pasive, choice room, upgrades la Forge.
- **UI:** HUD (HP, potions, bombe, coins), main menu, pauză, ecranul de final de run.
- **Boss:** opțional pentru MVP. Dacă nu avem timp pentru un boss, ultimul room al etajului e un room de luptă mai greu.

## Nice to have

- Meta-progression (meta currency, unlocks), primul pe listă după MVP.
- Merchant, dash-ul ca item.
- Câte un boss pe fiecare etaj.
- Rooms cu mecanisme, de exemplu pressure plates care deschid ușile.
- Keybinds custom, setarea unghiului camerei.
- Secret rooms, mai multe personaje, hub.

## Out of scope

Controller, multiplayer, mobile, poveste sau dialoguri.

## Ordinea de implementare

1. **Cleanup:** structura de foldere, asmdefs, Input System, scene de test.
2. **Un room jucabil:** mișcare, aim, atacul ranged, un inamic, camera orthographic.
3. **Lupta completă:** melee, HP și i-frames, potions, bombe, cele 4 tipuri de inamici.
4. **Dungeon:** componenta Room, uși, generarea etajelor, minimap.
5. **Run complet:** coins, Forge, choice room, items, 3 etaje, ecranul de final de run (plus boss, dacă e timp).
6. **Meta-progression și polish.**
