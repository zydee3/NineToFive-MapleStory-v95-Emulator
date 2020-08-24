using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public static class MapWz {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapWz));
        private static readonly WzFile WzFile = WzProvider.Load("Map");

        public static void CopyTemplate(Field field) {
            int fieldId = field.Id;
            if (!WzCache.FieldTemplates.TryGetValue(fieldId, out TemplateField t)) {
                InitializeTemplate(out t, fieldId);
                WzCache.FieldTemplates.Add(fieldId, t);
            }

            // Create spawn points only where monsters exist.
            List<TemplateLife> mobs = t.Life[EntityType.Mob];
            foreach (var mob in mobs) {
                field.SpawnPoints.Add(new SpawnPoint(ref field, mob));
            }

            foreach (var pair in t.Life) {
                if (!pair.Key.IsTemplate()) continue;
                foreach (var lifeEntry in pair.Value) {
                    if (lifeEntry.Type == EntityType.Mob) continue;
                    field.AddLife(lifeEntry.Create());
                }
            }

            field.Footholds = new Foothold[t.Footholds.Length];
            t.Footholds.CopyTo(field.Footholds, 0);

            field.Portals.AddRange(t.Portals);

            field.FieldLimit = t.FieldLimit;
            field.BackgroundMusic = t.BackgroundMusic;
            field.OnFirstUserEnter = t.OnFirstUserEnter;
            field.OnUserEnter = t.OnUserEnter;
            field.ForcedReturn = t.ForcedReturn;
            field.ReturnMap = t.ReturnMap;
            field.Town = t.Town;
            field.Swim = t.Swim;
            field.Fly = t.Fly;
            field.MobCount = t.MobCount;
            field.MobRate = t.MobRate;
            field.VrBottom = t.VRBottom;
            field.VrTop = t.VRTop;
            field.VrLeft = t.VRLeft;
            field.VrRight = t.VRRight;
        }

        private static void InitializeTemplate(out TemplateField templateField, int fieldId) {
            string imgPath = $"Map/Map{fieldId / 100000000}/{fieldId.ToString().PadLeft(9, '0')}.img";
            WzObject wzObject = WzFile.GetObjectFromPath(imgPath, false);
            if (wzObject == null) throw new NullReferenceException($"No map data : {imgPath}");
            templateField = new TemplateField(fieldId);
            LoadInfo(templateField, (WzImageProperty) wzObject["info"]);
            LoadFootholds(templateField, (WzImageProperty) wzObject["foothold"]);
            LoadLife(templateField, (WzImageProperty) wzObject["life"]);
            LoadPortals(templateField, (WzImageProperty) wzObject["portal"]);
            // todo back, clock, reactor
        }

        private static void LoadFootholds(TemplateField template, WzImageProperty footholdsImage) {
            Dictionary<uint, Foothold> footholds = new Dictionary<uint, Foothold>();

            foreach (WzImageProperty collection in footholdsImage.WzProperties) {
                foreach (WzImageProperty parent in collection.WzProperties) {
                    foreach (WzImageProperty child in parent.WzProperties) {
                        if (!int.TryParse(child.Name, out int childId)) continue;
                        Foothold foothold = new Foothold();

                        foreach (WzImageProperty property in child.WzProperties) {
                            switch (property.Name) {
                                case "next":
                                    foothold.Next = ((WzIntProperty) property).Value;
                                    break;
                                case "prev":
                                    foothold.Prev = ((WzIntProperty) property).Value;
                                    break;
                                case "x1":
                                    foothold.X1 = ((WzIntProperty) property).Value;
                                    break;
                                case "x2":
                                    foothold.X2 = ((WzIntProperty) property).Value;
                                    break;
                                case "y1":
                                    foothold.Y1 = ((WzIntProperty) property).Value;
                                    break;
                                case "y2":
                                    foothold.Y2 = ((WzIntProperty) property).Value;
                                    break;
                                case "piece":
                                    break;
                                default:
                                    Log.Info($"Unhandled Field/Foothold Property: {property.Name,10}({property.PropertyType})");
                                    break;
                            }
                        }

                        foothold.Id = childId;
                        foothold.SetEndPoints();
                        footholds.Add((uint) childId, foothold);
                    }
                }
            }

            template.Footholds = footholds.Select(entry => entry.Value).ToArray();
        }

        private static void LoadLife(TemplateField template, WzImageProperty lifeProperty) {
            foreach (WzImageProperty entry in lifeProperty.WzProperties) {
                string type = (entry["type"] as WzStringProperty)?.Value;
                TemplateLife life = type switch {
                    "m" => new TemplateLife(EntityType.Mob),
                    "n" => new TemplateLife(EntityType.Npc),
                    "r" => new TemplateLife(EntityType.Reactor),
                    _   => null
                };
                if (life == null) {
                    Log.Warn($"Unknown life type '{type}'");
                    continue;
                }

                // not default
                life.Id = int.Parse((entry["id"] as WzStringProperty)!.Value);
                life.FootholdId = (entry["fh"] as WzIntProperty)!.Value;
                life.Cy = (entry["cy"] as WzIntProperty)!.Value;
                life.Rx0 = (entry["rx0"] as WzIntProperty)!.Value;
                life.Rx1 = (entry["rx1"] as WzIntProperty)!.Value;
                life.X = (entry["x"] as WzIntProperty)!.Value;
                life.Y = (entry["y"] as WzIntProperty)!.Value;
                // can default
                life.MobTime = (entry["mobTime"] as WzIntProperty)?.Value ?? 1;
                life.Flipped = (entry["f"] as WzIntProperty)?.Value == 1;
                life.Hidden = (entry["hide"] as WzIntProperty)?.Value == 1;

                template.Life[life.Type].Add(life);
            }
        }

        private static void LoadInfo(TemplateField template, WzImageProperty infoImage) {
            template.BackgroundMusic = (infoImage["bgm"] as WzStringProperty)?.Value;
            template.FieldLimit = (uint) ((infoImage["fieldLimit"] as WzIntProperty)?.Value ?? 0);
            template.ForcedReturn = (infoImage["forcedReturn"] as WzIntProperty)?.Value ?? template.FieldId;
            template.ReturnMap = (infoImage["returnMap"] as WzIntProperty)?.Value ?? template.FieldId;
            template.MobRate = (infoImage["mobRate"] as WzFloatProperty)?.Value ?? 1f;
            template.OnFirstUserEnter = (infoImage["onFirstUserEnter"] as WzStringProperty)?.Value;
            template.OnUserEnter = (infoImage["onUserEnter"] as WzStringProperty)?.Value;
            template.Fly = (infoImage["fly"] as WzIntProperty)?.Value == 1;
            template.Swim = (infoImage["swim"] as WzIntProperty)?.Value == 1;
            template.Town = (infoImage["town"] as WzIntProperty)?.Value == 1;
            template.VRBottom = (infoImage["VRBottom"] as WzIntProperty)?.Value ?? 0;
            template.VRLeft = (infoImage["VRLeft"] as WzIntProperty)?.Value ?? 0;
            template.VRRight = (infoImage["VRRight"] as WzIntProperty)?.Value ?? 0;
            template.VRTop = (infoImage["VRTop"] as WzIntProperty)?.Value ?? 0;
        }

        private static void LoadPortals(TemplateField template, WzImageProperty portalImage) {
            List<Portal> portals = new List<Portal>();
            foreach (WzImageProperty entry in portalImage.WzProperties) {
                if (!byte.TryParse(entry.Name, out byte id)) continue;
                Portal portal = new Portal(id) {
                    Name = (entry["pn"] as WzStringProperty)?.Value,
                    TargetPortalId = (entry["pt"] as WzIntProperty)?.Value ?? 0,
                    TargetMap = (entry["tm"] as WzIntProperty)?.Value ?? Field.InvalidField,
                    TargetPortalName = (entry["tn"] as WzStringProperty)?.Value,
                    Location = new Vector2(
                        (entry["x"] as WzIntProperty)!.Value,
                        (entry["y"] as WzIntProperty)!.Value)
                };
                portals.Add(portal);
            }

            template.Portals = portals;
        }
    }
}