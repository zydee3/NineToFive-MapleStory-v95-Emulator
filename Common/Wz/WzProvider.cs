using System;
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
                Console.WriteLine("Could not locate file: {0}", TargetWz);
                return null;
            }

            WzFile Wz = new WzFile(path, WzMapleVersion.GMS);
            Wz.ParseWzFile(out _);

            return Wz;
        }
        
        /// <summary>
        ///     Parses Wz by traversing along TargetPath.
        /// </summary>
        /// <param name="Wz">Wz to parse.</param>
        /// <param name="TargetPath">Path to target property</param>
        /// <returns>WzImageProperty at TargetPath location, else null</returns>
        public static WzImageProperty GetWzProperty(WzFile Wz, string TargetPath) {
            string[] Directories = TargetPath.Split("/", 2);

            foreach (WzImage ParentNode in Wz.WzDirectory.WzImages) {
                if (ParentNode.Name == Directories[0]) {
                    return ParentNode.GetFromPath(Directories[1]);
                }
            }

            return null;
        }
        
        public int? EvaluateProperty(string Property, int? x, int? u) {
            
            
            //todo: Evaluate
            return null;
        }
    }
}