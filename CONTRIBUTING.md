# Ghid de contribuție

Reguli de lucru pentru toți cei care scriu cod sau modifică scene/prefaburi în proiect.
Dacă ceva de aici nu mai are sens, propune o schimbare printr-un PR la acest fișier.

---

## 1. Setup inițial (o singură dată)

### 1.1 Versiunea de Unity
Toată echipa folosește **exact Unity `6000.6.6f1`**.
Deschiderea proiectului cu altă versiune modifică `ProjectSettings/` și pachetele și generează conflicte.
Dacă Unity Hub propune upgrade, **refuză**.

### 1.2 Git LFS
```powershell
git lfs install
```

### 1.3 UnityYAMLMerge (opțional, recomandat)
`.gitattributes` cere ca scenele, prefaburile și asset-urile să fie îmbinate cu `unityyamlmerge`.
Nu e obligatoriu, dar ajută dacă apare vreodată un conflict pe o scenă sau un prefab. Durează un minut.

Rulează în folderul proiectului (ajustează calea dacă Unity e instalat în alt loc):

```powershell
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "'C:/Program Files/Unity/Hub/Editor/6000.6.6f1/Editor/Data/Tools/UnityYAMLMerge.exe' merge -h -p --force %O %B %A %A"
git config merge.unityyamlmerge.recursive binary
```

Verificare:
```powershell
git config --get-regexp merge.unityyamlmerge
```

### 1.4 IDE
Fișierul `.vsconfig` din repo îi spune lui Visual Studio ce componente să instaleze.
Rider sau VS Code funcționează și ele (pachetele de integrare sunt deja în proiect).

---

## 2. Planificare: GitHub Issues + Projects

- **Un issue = un lucru livrabil.** Fiecare issue conține: descriere, **criterii de acceptare** („e gata când...”), link către secțiunea relevantă din GDD.
- **Board** (GitHub Projects): `Backlog → To do → In progress → In review → Done`.
- **Milestone = sprint.** Tabelul de obiective din raportul de sprint se scoate direct din milestone.
- **Labels:**
  - tip: `gameplay`, `systems`, `ui`, `art-hook`, `bug`, `tech-debt`, `docs`
  - prioritate: `P1` (blocant / esențial), `P2` (important), `P3` (nice to have)
- Îți asignezi issue-ul **înainte** să începi lucrul la el, ca să nu lucreze doi oameni la același lucru.

---

## 3. Branch-uri, commit-uri, PR-uri

Două reguli sunt obligatorii; restul sunt recomandări.

**Obligatoriu:**
1. **Nu se dă push direct pe `main`.** Orice schimbare ajunge în `main` printr-un PR.
2. **Pe o scenă comună lucrează un singur om odată** (detalii în secțiunea 4).

### Branch-uri
- Fiecare branch pornește din `main` actualizat. Nume recomandate:
  - `feat/<descriere-scurta>` → ex. `feat/room-system`
  - `fix/<descriere-scurta>` → ex. `fix/door-trigger`
  - `chore/<descriere>` pentru curățenie / configurare
- Dacă există un issue sau un task din [FEATURE_PLAN](docs/FEATURE_PLAN.md), poți pune ID-ul în nume (`feat/M1-04-ranged-attack`), dar nu e obligatoriu.
- E bine ca branch-urile să fie de scurtă durată și să fie șterse după merge.
- Ține branch-ul la zi: `git pull origin main` des, mai ales înainte de PR.

### Commit-uri
- Mesaje scurte, la imperativ, care spun *ce* face commit-ul: `Add door locking on room enter`.
- Un commit = o schimbare logică. Nu amesteca refactor + feature + reformatare.
- Nu comite niciodată `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, build-uri (sunt deja în `.gitignore` — verifică `git status` înainte de commit).

### Pull Requests
- E mai bine ca PR-urile să fie mici: sunt mai ușor de verificat și de rezolvat la conflicte.
- În descriere scrie pe scurt ce s-a schimbat și **cum se testează** (ce scenă deschizi, ce apeși, ce ar trebui să vezi). Dacă există un issue, adaugă `Closes #<nr-issue>`.
- Un review de la un coleg e recomandat, mai ales pentru schimbări mari sau pe sisteme comune. Schimbările mici pot fi date merge direct de autor.
- Recomandat: **Squash and merge**, apoi ștergerea branch-ului.

---

## 4. Reguli specifice Unity

### Scene
- **Scenele sunt „subțiri”**: logica și conținutul stau în prefaburi; scena doar le pune împreună.
- Fiecare programator lucrează într-o **scenă sandbox personală**: `Assets/_Game/Scenes/Sandbox/<Nume>.unity`.
  Acolo poți face orice; nimeni altcineva nu o modifică.
  O creezi din meniul **Game → Create My Sandbox Scene** (copiază `Sandbox_Template.unity`).
- **Scenele comune** (meniu, joc principal etc.) sunt modificate de **o singură persoană odată**.
  Anunță pe grup înainte să lucrezi pe o scenă comună și după ce ai dat push.
- Nu salva o scenă comună dacă nu ai vrut să o modifici (Unity marchează scenele ca modificate foarte ușor — verifică `git status` și dă revert la ce nu ai vrut să schimbi).

### Prefaburi: gameplay separat de grafică
Fiecare prefab de gameplay are structura:

```
Enemy_Slime            ← root: scripturi, Rigidbody, collidere (gameplay)
└── Visual             ← DOAR grafică: mesh/sprite, animator, VFX
```

- Codul de gameplay **nu** face referire la nimic din interiorul `Visual` prin nume sau ierarhie.
  Dacă are nevoie de grafică (ex. animație de lovire), comunică prin evenimente / o componentă dedicată pe `Visual`.
- Echipa de artă înlocuiește doar copilul `Visual`, fără să atingă gameplay-ul.
- Până vin asset-urile finale, `Visual` conține **placeholdere**: primitive Unity (cub, capsulă, sferă) cu materiale simple colorate pe categorii.
- Pentru variații ale aceluiași obiect, folosește **Prefab Variants**, nu copii duplicate.

### Fișiere `.meta`
- Fiecare asset are un `.meta`. **Se comit mereu împreună** (asset + `.meta`).
- Mută și redenumește fișiere **doar din Unity**, nu din Explorer, altfel se pierd referințele.

---

## 5. Structura proiectului

```
Assets/
├── _Game/                  ← tot ce e al nostru
│   ├── Scripts/
│   │   ├── Runtime/        ← Game.Runtime.asmdef
│   │   ├── Editor/         ← Game.Editor.asmdef (unelte de editor)
│   │   └── Tests/
│   │       ├── EditMode/   ← Game.Tests.EditMode.asmdef
│   │       └── PlayMode/   ← Game.Tests.PlayMode.asmdef
│   ├── Prefabs/
│   ├── Data/               ← ScriptableObject-uri (statistici, item-e, inamici)
│   ├── Materials/          ← materiale placeholder
│   ├── Input/              ← .inputactions
│   └── Scenes/
│       └── Sandbox/
├── Plugins/ / ThirdParty/  ← asset-uri externe, nemodificate
└── Settings/               ← setări URP
```

Asset-urile externe (din Asset Store etc.) **nu se modifică direct** — se fac copii / variante în `_Game/`.

---

## 6. Convenții de cod

- **C#, standard .NET:**
  - `PascalCase` pentru clase, metode, proprietăți, evenimente
  - `camelCase` pentru variabile locale și parametri
  - `_camelCase` pentru câmpuri private
  - o clasă publică per fișier, numele fișierului = numele clasei
- **Namespace-uri** pe sistem: `Game.Player`, `Game.Rooms`, `Game.Combat`, `Game.Items`, `Game.UI` etc.
- **Câmpuri în Inspector:** `[SerializeField] private`, nu `public`. Expune prin proprietăți doar ce trebuie.
- **Date în ScriptableObject-uri:** valorile de balans (viață, viteză, damage, rate de tragere, drop chance)
  nu sunt hardcodate în scripturi, ci stau în asset-uri din `_Game/Data/`.
- **Input:** exclusiv **Input System** (pachetul nou). Nu se folosește `Input.GetAxis` / `Input.GetKey`.
- **Referințe între sisteme:** preferă evenimente (`event Action`, UnityEvent) și referințe setate în Inspector
  în locul lui `FindObjectOfType` / `GameObject.Find` / căutări după tag-uri în `Update`.
- **Fără `Debug.Log` lăsat în cod** la merge, cu excepția celor sub un flag de debug.
- **Comentariile** explică *de ce*, nu *ce*. Limba codului și a comentariilor: engleză.

---

## 7. Definition of Done

Un issue e „Done” când:

- [ ] codul compilează **fără erori și fără warning-uri noi**
- [ ] feature-ul a fost testat manual în scena sandbox, conform criteriilor de acceptare
- [ ] logica pură (generare dungeon, calcule de statistici etc.) are **teste EditMode** unde e posibil
- [ ] nu există modificări accidentale în scene comune / `ProjectSettings/`
- [ ] PR-ul are descriere cu „cum se testează”
- [ ] PR-ul a fost făcut merge în `main` (și issue-ul s-a închis, dacă există)

---

## 8. Comunicare
- Deciziile de design importante se trec în **GDD**; deciziile tehnice importante se notează în issue sau PR.
