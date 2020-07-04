using System;
using System.Collections.Generic;
using System.IO;
using MapleLib.WzLib;

namespace NineToFive.Wz {
    public class WzProvider {
        /// <summary>
        ///     Loads a target Wz file.
        /// </summary>
        /// <param name="TargetWz">Name of File.wz to be loaded.</param>
        /// <returns>WzFile if found, else null.</returns>
        public static WzFile Load(string TargetWz) {
            string path = $"../../../../Wz/{TargetWz}.wz"; //wtf this is so troll

            if (!File.Exists(path)) { 
                Console.WriteLine($"Could not locate file: {TargetWz}");
                return null;
            }

            WzFile Wz = new WzFile(path, WzMapleVersion.GMS);
            Wz.ParseWzFile(out _);

            return Wz;
        }

        public static List<WzImageProperty> GetWzProperties(WzFile Wz, string TargetPath) {
            return GetWzProperties(Wz, null, TargetPath);
        }

        /// <summary>
        ///     Parses Wz by traversing along TargetPath.
        /// </summary>
        /// <param name="Wz">Wz to parse.</param>
        /// <<param name="Directory">Optional directory, argument != null when the function is used recursively</param>
        /// <param name="TargetPath">Path to target property</param>
        /// <returns>WzImageProperty at TargetPath location, else null</returns>
        public static List<WzImageProperty> GetWzProperties(WzFile Wz, WzDirectory? Directory, string TargetPath) {
            string[] Directories = TargetPath.Split("/", 2);
            foreach (WzImage ParentNode in (Directory ?? Wz.WzDirectory).WzImages) {
                if (ParentNode.Name == Directories[0]) { // if this is evaluated as true, we're able to get the target
                    return Directories.Length == 1 ? ParentNode.WzProperties : ParentNode.GetFromPath(Directories[1]).WzProperties;
                }
            }

            foreach (WzDirectory ParentNode in (Directory ?? Wz.WzDirectory).WzDirectories) {
                if (ParentNode.Name == Directories[0]) { // unable to get target so pass in current position and remainder of path to recursively find path.
                    return GetWzProperties(Wz, ParentNode, Directories[1]);
                }
            }
            
            return null; // if the two loops above fell through, the path is invalid or the target WzImage doesn't exist.
        }

        public int? EvaluateProperty(string Property, int? x, int? u) {
            //todo: Evaluate
            return null;
        }
    }
}