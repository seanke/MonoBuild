Documentation on Ken's .ART file format                     by Ken Silverman

I am documenting my ART format to allow you to program your own custom
art utilites if you so desire.  I am still planning on writing the script
system.

All art files must have xxxxx###.ART.  When loading an art file you
should keep trying to open new xxxxx###'s, incrementing the number, until
an art file is not found.


1. long artversion;

   The first 4 bytes in the art format are the version number.  The current
   current art version is now 1.  If artversion is not 1 then either it's the
   wrong art version or something is wrong.

2. long numtiles;

   Numtiles is not really used anymore.  I wouldn't trust it.  Actually
   when I originally planning art version 1 many months ago, I thought I
   would need this variable, but it turned it is was unnecessary.  To get
   the number of tiles, you should search all art files, and check the
   localtilestart and localtileend values for each file.

3. long localtilestart;

   Localtilestart is the tile number of the first tile in this art file.

4. long localtileend;

   Localtileend is the tile number of the last tile in this art file.
   Note:  Localtileend CAN be higher than the last used slot in an art
   file.

         Example:  If you chose 256 tiles per art file:
   TILES000.ART -> localtilestart = 0,   localtileend = 255
   TILES001.ART -> localtilestart = 256, localtileend = 511
   TILES002.ART -> localtilestart = 512, localtileend = 767
   TILES003.ART -> localtilestart = 768, localtileend = 1023

5. short tilesizx[localtileend-localtilestart+1];

   This is an array of shorts of all the x dimensions of the tiles
   in this art file.  If you chose 256 tiles per art file then
   [localtileend-localtilestart+1] should equal 256.

6. short tilesizy[localtileend-localtilestart+1];

   This is an array of shorts of all the y dimensions.

7. long picanm[localtileend-localtilestart+1];

   This array of longs stores a few attributes for each tile that you
   can set inside EDITART.  You probably won't be touching this array, but
   I'll document it anyway.

   Bit:  │31           24│23           16│15            8│7             0│
   ├───────────────┼───────────────┼───────────────┼───────────────┤
   │ | | | | | | | │ | | | | | | | │ | | | | | | | │ | | | | | | | │
   └───────┬───────┼───────────────┼───────────────┼───┬───────────┤
   │ Anim. │  Signed char  │  Signed char  │   │  Animate  │
   │ Speed │   Y-center    │   X-center    │   │   number  │
   └───────┤    offset     │    offset     │   ├───────────┘
   └───────────────┴───────────────┤   └──────────┐
   │ Animate type:│
   │ 00 - NoAnm   │
   │ 01 - Oscil   │
   │ 10 - AnmFd   │
   │ 11 - AnmBk   │
   └──────────────┘
   You probably recognize these:
   Animate speed -            EDITART key: 'A', + and - to adjust
   Signed char x&y offset -   EDITART key: '`', Arrows to adjust
   Animate number&type -      EDITART key: +/- on keypad

8. After the picanm's, the rest of the file is straight-forward rectangular
   art data.  You must go through the tilesizx and tilesizy arrays to find
   where the artwork is actually stored in this file.

   Note:  The tiles are stored in the opposite coordinate system than
   the screen memory is stored.  Example on a 4*4 file:

         Offsets:
         ┌───┬───┬───┬───┐
         │ 0 │ 4 │ 8 │12 │
         ├───┼───┼───┼───┤
         │ 1 │ 5 │ 9 │13 │
         ├───┼───┼───┼───┤
         │ 2 │ 6 │10 │14 │
         ├───┼───┼───┼───┤
         │ 3 │ 7 │11 │15 │
         └───┴───┴───┴───┘

----------------------------------------------------------------------------
If you wish to display the artwork, you will also need to load your
palette.  To load the palette, simply read the first 768 bytes of your
palette.dat and write it directly to the video card - like this:

Example:
long i, fil;

      fil = open("palette.dat",O_BINARY|O_RDWR,S_IREAD);
      read(fil,&palette[0],768);
      close(fil);

      outp(0x3c8,0);
      for(i=0;i<768;i++)
         outp(0x3c9,palette[i]);
