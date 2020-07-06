using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure.Data;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public static class MapWz {
        private const string WzName = "Map";
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapWz));

        /// <summary>
        /// Sets the the field variables of the field being passed in.
        /// </summary>
        /// <param name="Field">Field to be initialized</param>
        /// <param name="MapProperties">List of WzImageProperty loaded from the Field's Image from Wz.</param>
        /// <param name="benchmark">Average time to create template and set: 00:00:00.2348726</param>
        public static void SetField(Field field) {
            if (field == null) return;

            int fieldId = field.Id;
            Dictionary<int, object> templateFields = Server.Worlds[0].Templates[(int) TemplateType.Field];
            if (!templateFields.TryGetValue(fieldId, out object t)) {
                string pathToMapImage = $"Map/Map{fieldId / 100000000}/{fieldId.ToString().PadLeft(9, '0')}.img";
                List<WzImageProperty> fieldProperties = WzProvider.GetWzProperties(WzProvider.Load("Map"), pathToMapImage);
                t = new TemplateField();
                SetTemplateField((TemplateField) t, ref fieldProperties);
                templateFields.Add(fieldId, (TemplateField) t);
            }

            if (t == null) throw new NullReferenceException($"Unable to load field: {fieldId}");


            // t isn't null, so re-set so we don't have to keep typecasting.
            TemplateField template = (TemplateField) t;

            // Create spawn points only where monsters exist.
            Dictionary<int, FieldLifeEntry> mobEntries = template.Life[EntityType.Mob];
            foreach (Foothold foothold in template.Footholds) {
                if (mobEntries.TryGetValue(foothold.ID, out FieldLifeEntry entry)) {
                    field.SpawnPoints.Add(new SpawnPoint(field, entry.ID));
                }
            }

            //todo: load npcs and reactors from template.Life
            field.FieldLimits = new bool[Enum.GetNames(typeof(FieldLimitType)).Length];
            template.FieldLimits.CopyTo(field.FieldLimits, 0);

            field.Footholds = new Foothold[template.Footholds.Length];
            template.Footholds.CopyTo(field.Footholds, 0);

            field.Portals = new Portal[template.Portals.Length];
            template.Portals.CopyTo(field.Portals, 0);

            field.BackgroundMusic = template.BackgroundMusic;
            field.OnFirstUserEnter = template.OnFirstUserEnter;
            field.OnUserEnter = template.OnUserEnter;
            field.ForcedReturn = template.ForcedReturn;
            field.ReturnMap = template.ReturnMap;
            field.Town = template.Town;
            field.Swim = template.Swim;
            field.Fly = template.Fly;
            field.MobCount = template.MobCount;
            field.MobRate = template.MobRate;

            field.VRBottom = template.VRBottom;
            field.VRTop = template.VRTop;
            field.VRLeft = template.VRLeft;
            field.VRRight = template.VRRight;

            Server.Worlds[0].Channels[field.ChannelId].Fields.Add(fieldId, field);
        }

        /// <summary>
        /// Sets the Template of the field.
        /// </summary>
        /// <param name="Field">Field to be initialized</param>
        /// <param name="mapProperties">List of WzImageProperty loaded from the Field's Image from Wz.</param>
        public static void SetTemplateField(TemplateField template, ref List<WzImageProperty> mapProperties) {
            foreach (WzImageProperty node in mapProperties) {
                if (node == null) continue;

                switch (node.Name) {
                    case "back": { }
                        break;
                    case "clock": { }
                        break;
                    case "foothold":
                        LoadFootholds(template, node);
                        break;
                    case "info":
                        LoadInfo(template, node);
                        break;
                    case "ladderRope": { }
                        break;
                    case "life":
                        if (template.LoadLife) LoadLife(template, node);
                        break;
                    case "miniMap":
                        break;
                    case "portal":
                        if (template.LoadPortals) LoadPortals(template, node);
                        break;
                    case "reactor": { }
                        break;
                    default:
                        if (!int.TryParse(node.Name, out int _)) {
                            Log.Info($"Unhandled Map Node: {node.Name,10}({node.GetType()})");
                        }

                        break;
                }
            }
        }

        public static void PrintDirectory(WzImageProperty parent) {
            foreach (WzImageProperty internalProperty in parent.WzProperties) {
                Log.Info($"--{internalProperty.Name}({internalProperty.GetType()})\n----{internalProperty.WzValue}");
                if (internalProperty.Name == "fieldLimit") {
                    foreach (FieldLimitType type in Enum.GetValues(typeof(FieldLimitType))) {
                        Log.Info($"{type} : {type.Check((int) internalProperty.WzValue)}");
                    }
                }
            }
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

                        foothold.ID = childId;
                        foothold.SetEndPoints();
                        footholds.Add((uint) childId, foothold);
                    }
                }
            }

            template.Footholds = footholds.Select(entry => entry.Value).ToArray();
        }

        private static void LoadLife(TemplateField template, WzImageProperty lifeImage) {
            foreach (WzImageProperty life in lifeImage.WzProperties) {
                if (!int.TryParse(life.Name, out int id)) continue;

                EntityType? type = null;
                FieldLifeEntry fieldLife = new FieldLifeEntry();

                foreach (WzImageProperty property in life.WzProperties) {
                    switch (property.Name) {
                        case "id":
                            if (int.TryParse(((WzStringProperty) property).Value, out int lifeId)) {
                                fieldLife.ID = lifeId;
                            }

                            break;
                        case "type":
                            string value = ((WzStringProperty) property).Value;
                            switch (value) {
                                case "m":
                                    type = EntityType.Mob;
                                    break;
                                case "n":
                                    type = EntityType.Npc;
                                    break;
                                case "r":
                                    type = EntityType.Reactor;
                                    break;
                                default:
                                    Log.Info($"Unhandled Entity Type: {value}");
                                    break;
                            }

                            break;
                        case "mobTime":
                            fieldLife.MobTime = ((WzIntProperty) property).Value;
                            break;
                        case "f":
                            fieldLife.Flipped = ((WzIntProperty) property).Value == 1;
                            break;
                        case "hide":
                            fieldLife.Hidden = ((WzIntProperty) property).Value == 1;
                            break;
                        case "fh":
                            fieldLife.FootholdID = ((WzIntProperty) property).Value;
                            break;
                        case "cy":
                            fieldLife.Cy = ((WzIntProperty) property).Value;
                            break;
                        case "rx0":
                            fieldLife.Rx0 = ((WzIntProperty) property).Value;
                            break;
                        case "rx1":
                            fieldLife.Rx1 = ((WzIntProperty) property).Value;
                            break;
                        case "x":
                            fieldLife.X = ((WzIntProperty) property).Value;
                            break;
                        case "y":
                            fieldLife.Y = ((WzIntProperty) property).Value;
                            break;
                        default:
                            Log.Info($"Unhandled Field/Life Property: {property.Name,10}({property.PropertyType})");
                            break;
                    }
                }

                if (type.HasValue) {
                    if (template.Life.TryGetValue(type.Value, out Dictionary<int, FieldLifeEntry> fieldLifeEntries)) {
                        fieldLifeEntries.Add((type == EntityType.Mob ? fieldLife.FootholdID : fieldLife.ID), fieldLife);
                    } else {
                        Log.Info($"Unable to add field life entry: id={id}, type={type}");
                    }
                }
            }
        }

        private static void LoadInfo(TemplateField template, WzImageProperty infoImage) {
            foreach (WzImageProperty property in infoImage.WzProperties) {
                switch (property.Name) {
                    case "bgm":
                        template.BackgroundMusic = ((WzStringProperty) property).Value;
                        break;
                    case "fieldLimit":
                        template.FieldLimits = new bool[Enum.GetNames(typeof(FieldLimitType)).Length];
                        foreach (FieldLimitType type in Enum.GetValues(typeof(FieldLimitType))) {
                            template.FieldLimits[(int) type] = type.Check((int) property.WzValue);
                        }

                        break;
                    case "forcedReturn":
                        template.ForcedReturn = ((WzIntProperty) property).Value;
                        break;
                    case "returnMap":
                        break;
                    case "mobRate":
                        template.MobRate = ((WzFloatProperty) property).Value;
                        break;
                    case "onFirstUserEnter":
                        template.OnFirstUserEnter = ((WzStringProperty) property).Value;
                        break;
                    case "onUserEnter":
                        template.OnUserEnter = ((WzStringProperty) property).Value;
                        break;
                    case "fly":
                        template.Fly = ((WzIntProperty) property).Value == 1;
                        break;
                    case "swim":
                        template.Swim = ((WzIntProperty) property).Value == 1;
                        break;
                    case "town":
                        template.Town = ((WzIntProperty) property).Value == 1;
                        break;
                    case "VRBottom":
                        template.VRBottom = ((WzIntProperty) property).Value;
                        break;
                    case "VRLeft":
                        template.VRLeft = ((WzIntProperty) property).Value;
                        break;
                    case "VRRight":
                        template.VRRight = ((WzIntProperty) property).Value;
                        break;
                    case "VRTop":
                        template.VRTop = ((WzIntProperty) property).Value;
                        break;
                    case "cloud":
                    case "hideMinimap":
                    case "mapMark":
                    case "version":
                        break;
                    default:
                        Log.Info($"Unhandled Map/Info Property: {property.Name,10}({property.GetType()})");
                        break;
                }
            }
        }

        private static void LoadPortals(TemplateField template, WzImageProperty portalImage) {
            List<Portal> portals = new List<Portal>();
            foreach (WzImageProperty portalNode in portalImage.WzProperties) {
                Portal portal = new Portal();
                foreach (WzImageProperty property in portalNode.WzProperties) {
                    switch (property.Name) {
                        case "pn":
                            portal.Name = ((WzStringProperty) property).Value;
                            break;
                        case "pt":
                            portal.TargetPortalID = ((WzIntProperty) property).Value;
                            break;
                        case "tm":
                            portal.TargetMap = ((WzIntProperty) property).Value;
                            break;
                        case "tn":
                            portal.TargetPortalName = ((WzStringProperty) property).Value;
                            break;
                        case "x":
                            portal.X = ((WzIntProperty) property).Value;
                            break;
                        case "y":
                            portal.Y = ((WzIntProperty) property).Value;
                            break;
                        default:
                            Log.Info($"Unhandled Portal Property: {property.Name}");
                            break;
                    }
                }

                portals.Add(portal);
            }

            template.Portals = portals.ToArray();
        }
    }
}