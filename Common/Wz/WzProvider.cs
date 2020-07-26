using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using MapleLib.WzLib;

namespace NineToFive.Wz {
    public class WzProvider {
        /// <summary>
        ///     Loads a target Wz file.
        /// </summary>
        /// <param name="TargetWz">Name of File.wz to be loaded.</param>
        /// <returns>WzFile if found, else null.</returns>
        public static WzFile Load(string TargetWz) {
            string path = $"{Directory.GetCurrentDirectory()}/Wz/{TargetWz}.wz";

            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Could not locate WZ file '{TargetWz}'");
            }

            WzFile wz = new WzFile(path, WzMapleVersion.GMS);
            wz.ParseWzFile(out _);

            return wz;
        }

        public static List<WzImageProperty> GetWzProperties(WzFile wz, string targetPath) {
            return GetWzProperties(wz, null, targetPath);
        }

        /// <summary>
        ///     Parses Wz by traversing along TargetPath.
        /// </summary>
        /// <param name="wz">Wz to parse.</param>
        /// <<param name="directory">Optional directory, argument != null when the function is used recursively</param>
        /// <param name="targetPath">Path to target property</param>
        /// <returns>WzImageProperty at TargetPath location, else null</returns>
        public static List<WzImageProperty> GetWzProperties(WzFile wz, WzDirectory? directory, string targetPath) {
            string[] directories = targetPath.Split("/", 2);
            foreach (WzImage parentNode in (directory ?? wz.WzDirectory).WzImages) {
                
                // if this is evaluated as true, we're able to get the target
                if (parentNode.Name == directories[0]) {
                    return directories.Length == 1 ? parentNode.WzProperties : parentNode.GetFromPath(directories[1]).WzProperties;
                }
            }

            foreach (WzDirectory parentNode in (directory ?? wz.WzDirectory).WzDirectories) {
                
                // unable to get target so pass in current position and remainder of path to recursively find path.
                if (parentNode.Name == directories[0]) { 
                    return GetWzProperties(wz, parentNode, directories[1]);
                }
            }
            
            // if the two loops above fell through, the path is invalid or the target WzImage doesn't exist.
            return null; 
        }

        /// <summary>
        /// Evaluates a string as an equation using two arguments commonly used.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="x">argument 1</param>
        /// <param name="u">argument 2</param>
        /// <returns>evaluated property as a float; must extend nullable because a string can be evaluated to any number.</returns>
        public float? EvaluateProperty(string property, int? x, int? u) {
            //todo: Evaluate
            return null;
        }
    }
}