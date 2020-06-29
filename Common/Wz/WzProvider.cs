using System;
using System.Collections.Generic;
using System.IO;
using MapleLib.WzLib;
using NineToFive.Game;

namespace NineToFive.Wz {
    public class WzProvider {
        /*
         * Loads a target Wz file.
         * @Param TargetWz<string> Name of File.wz to be loaded.
         * @Return WzFile if found, else null.
        */
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
        
        /*
         * Parses Wz<WzFile> by traversing along TargetPath<string>
         * @Param Wz<WzFile> Wz to parse.
         * @Param TargetPath<string> Path to target property<WzImageProperty>
         * @Return WzImageProperty at TargetPath<string> location, else null.
        */
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