# File Format

## Version 9
- **TROR** is present in the map.

## Version 8
- **Limits increased**:

| Limit       | Value  |
|------------|-------|
| MAXSECTORS  | 4096  |
| MAXWALLS    | 16384 |
| MAXSPRITES  | 16384 |

## Version 7
- This is the main map version for commercially released games.

| Limit       | Value  |
|------------|-------|
| MAXSECTORS  | 1024  |
| MAXWALLS    | 8192  |
| MAXSPRITES  | 4096  |

## Data Structure

### General Map Data

| Data Type  | Name        | Description |
|------------|------------|-------------|
| `INT32LE`  | `mapversion` | File format version number (latest in released games is 7, source ports use 8 and 9) |
| `INT32LE`  | `posx` | Player start point, X coordinate |
| `INT32LE`  | `posy` | Player start point, Y coordinate |
| `INT32LE`  | `posz` | Player start point, Z coordinate |
| `INT16LE`  | `ang` | Player starting angle |
| `INT16LE`  | `cursectnum` | Sector number containing the start point |
| `UINT16LE` | `numsectors` | Number of sectors in the map |
| `SECTOR[numsectors]` | `sector` | Information about each sector |
| `UINT16LE` | `numwalls` | Number of walls in the map |
| `WALL[numwalls]` | `wall` | Information about each wall |
| `UINT16LE` | `numsprites` | Number of sprites in the map |
| `SPRITE[numsprites]` | `sprite` | Information about each sprite |

---

## **SECTOR Structure (40 bytes)**

Each **SECTOR** is defined as follows:

| Data Type  | Name        | Description |
|------------|------------|-------------|
| `INT16LE`  | `wallptr` | Index to first wall in sector |
| `INT16LE`  | `wallnum` | Number of walls in sector |
| `INT32LE`  | `ceilingz` | Z-coordinate (height) of ceiling at first point of sector |
| `INT32LE`  | `floorz` | Z-coordinate (height) of floor at first point of sector |
| `INT16LE`  | `ceilingstat` | See bit flags below |
| `INT16LE`  | `floorstat` | See bit flags below |
| `INT16LE`  | `ceilingpicnum` | Ceiling texture (index into ART file) |
| `INT16LE`  | `ceilingheinum` | Slope value (rise/run; 0 = parallel to floor, 4096 = 45 degrees) |
| `INT8`     | `ceilingshade` | Shade offset |
| `UINT8`    | `ceilingpal` | Palette lookup table number (0 = standard colours) |
| `UINT8`    | `ceilingxpanning` | Texture coordinate X-offset for ceiling |
| `UINT8`    | `ceilingypanning` | Texture coordinate Y-offset for ceiling |
| `INT16LE`  | `floorpicnum` | Floor texture (index into ART file) |
| `INT16LE`  | `floorheinum` | Slope value (rise/run; 0 = parallel to floor, 4096 = 45 degrees) |
| `INT8`     | `floorshade` | Shade offset |
| `UINT8`    | `floorpal` | Palette lookup table number (0 = standard colours) |
| `UINT8`    | `floorxpanning` | Texture coordinate X-offset for floor |
| `UINT8`    | `floorypanning` | Texture coordinate Y-offset for floor |
| `UINT8`    | `visibility` | How fast an area changes shade relative to distance |
| `UINT8`    | `filler` | Padding byte |
| `INT16LE`  | `lotag` | Game-specific significance (e.g., triggers) |
| `INT16LE`  | `hitag` | Game-specific significance |
| `INT16LE`  | `extra` | Game-specific significance |

### **ceilingstat & floorstat Bit Flags**
- **Bit 0**: 1 = parallaxing, 0 = not
- **Bit 1**: 1 = sloped, 0 = not
- **Bit 2**: 1 = swap x&y, 0 = not
- **Bit 3**: 1 = double smooshiness
- **Bit 4**: 1 = x-flip
- **Bit 5**: 1 = y-flip
- **Bit 6**: 1 = Align texture to first wall of sector
- **Bits 7-15**: Reserved

---

## **WALL Structure (32 bytes)**

Each **WALL** is defined as follows:

| Data Type  | Name        | Description |
|------------|------------|-------------|
| `INT32LE`  | `x` | X-coordinate of left side of wall |
| `INT32LE`  | `y` | Y-coordinate of left side of wall |
| `INT16LE`  | `point2` | Index to next wall on the right (always in the same sector) |
| `INT16LE`  | `nextwall` | Index to wall on other side of wall (-1 if there is no sector) |
| `INT16LE`  | `nextsector` | Index to sector on other side of wall (-1 if there is no sector) |
| `INT16LE`  | `cstat` | See bit flags below |
| `INT16LE`  | `picnum` | Texture index into ART file |
| `INT16LE`  | `overpicnum` | Texture index into ART file for masked/one-way walls |
| `INT8`     | `shade` | Shade offset of wall |
| `UINT8`    | `pal` | Palette lookup table number (0 = standard colours) |
| `UINT8`    | `xrepeat` | Change pixel size to stretch/shrink textures |
| `UINT8`    | `yrepeat` |  |
| `UINT8`    | `xpanning` | Offset for aligning textures |
| `UINT8`    | `ypanning` |  |
| `INT16LE`  | `lotag` | Game-specific significance (Triggers, etc.) |
| `INT16LE`  | `hitag` | Game-specific significance |
| `INT16LE`  | `extra` | Game-specific significance |

### **cstat Bit Flags**
- **Bit 0**: 1 = Blocking wall
- **Bit 1**: 1 = Bottoms of invisible walls swapped
- **Bit 2**: 1 = Align picture on bottom (for doors)
- **Bit 3**: 1 = x-flipped
- **Bit 4**: 1 = Masking wall
- **Bit 5**: 1 = 1-way wall
- **Bit 6**: 1 = Blocking wall (hitscan / cliptype 1)
- **Bit 7**: 1 = Translucence
- **Bit 8**: 1 = y-flipped
- **Bit 9**: 1 = Translucence reversing
- **Bits 10-15**: Reserved

---

## **SPRITE Structure (44 bytes)**

Each **SPRITE** is defined as follows:

| Data Type  | Name        | Description |
|------------|------------|-------------|
| `INT32LE`  | `x` | X-coordinate of sprite |
| `INT32LE`  | `y` | Y-coordinate of sprite |
| `INT32LE`  | `z` | Z-coordinate of sprite |
| `INT16LE`  | `cstat` | See bit flags below |
| `INT16LE`  | `picnum` | Texture index into ART file |
| `INT8`     | `shade` | Shade offset of wall |
| `UINT8`    | `pal` | Palette lookup table number |
| `UINT8`    | `clipdist` | Size of the movement clipping square |
| `UINT8`    | `xrepeat` | Change pixel size |
| `UINT8`    | `yrepeat` |  |

---

This markdown formatting should now be more structured and readable.