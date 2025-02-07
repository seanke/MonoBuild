using System.Linq;
using MonoBuild.Group;

namespace MonoBuild.Art;

public class GroupArt
{
    public List<RawTile> Tiles { get; private set; } = [];

    public static GroupArt Load(RawGroupFile groupFile)
    {
        var groupArt = new GroupArt();

        var artLumps = groupFile
            .Lumps.Where(x => x.FileName.EndsWith(".ART"))
            .OrderBy(x => x.FileName);

        foreach (var lump in artLumps)
        {
            var artFile = RawArtFile.LoadFromBytes(lump.Data);

            groupArt.Tiles.AddRange(artFile.Tiles);
        }

        return groupArt;
    }
}
