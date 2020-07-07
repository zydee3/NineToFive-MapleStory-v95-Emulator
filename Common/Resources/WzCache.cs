using System.Collections.Generic;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;

namespace NineToFive.Resources {
    public class WzCache {
        public static readonly Dictionary<int, Life>[] Entities = { };

        public static readonly Dictionary<int, TemplateMob.Skill> Skills = new Dictionary<int, TemplateMob.Skill>();
        public static readonly Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public static readonly Dictionary<int, TemplateField> FieldTemplates = new Dictionary<int, TemplateField>();
        public static readonly Dictionary<int, TemplateMob> MobTemplates = new Dictionary<int, TemplateMob>();
    }
}