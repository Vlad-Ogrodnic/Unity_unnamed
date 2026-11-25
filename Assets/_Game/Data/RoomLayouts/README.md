# Room layouts

Fiecare fișier `Room_<Nume>.txt` descrie un room. Din ele, meniul **Game → Rooms → Rebuild Room Prefabs** construiește prefab-urile din `Prefabs/Rooms/`. Nu modifica prefab-urile de mână: se suprascriu la rebuild.

## Format

```
size: 1x1
..................
...##........##...
```

- Prima linie: `size: LxH` în celule de grid: `1x1`, `2x1` (lat), `1x2` (adânc) sau `2x2`.
- Apoi câte un rând de caractere pentru fiecare rând de podea, **primul rând = nordul** (sus pe ecran). Un caracter = 1 m.
- Mărimi: `1x1` = 18 × 10, `2x1` = 38 × 10, `1x2` = 18 × 22, `2x2` = 38 × 22 (coloane × rânduri). Pereții nu se desenează.
- Liniile care încep cu `//` sunt comentarii.
- Fișierele `Room_Start*.txt` sunt start rooms (fără inamici); restul sunt combat rooms. Generatorul de etaj le ia din `Data/RoomCatalog.asset`, completat automat la rebuild.

| Caracter | Ce e |
|---|---|
| `.` | podea |
| `#` | obstacol (blochează mișcarea și proiectilele) |
| `X` | obstacol distructibil, **rezervat**: nu se construiește încă (bombele sunt nedecise) |
| `C` | Chaser |
| `S` | Shooter |
| `R` | Charger |

## Reguli (verificate de testul `RoomLayoutTests` și la rebuild)

- Ușile sunt la mijlocul fiecărei laturi de celulă, late de 2 tiles. Primele 2 tiles din fața fiecărei uși trebuie să fie podea.
- Orice tile de podea trebuie să se poată atinge din uși.
- Inamicii stau la cel puțin 3 tiles de orice ușă, ca intrarea să fie corectă.
