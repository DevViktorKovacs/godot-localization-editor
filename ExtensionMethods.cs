using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.Tween;

namespace godotlocalizationeditor
{
    public static class ExtensionMethods
    {
        public static void SetFont(this Control control, string fontPath, int fontSize)
        {
            var dynamicFont = new DynamicFont();

            dynamicFont.FontData = (DynamicFontData)ResourceLoader.Load(fontPath);

            dynamicFont.Size = fontSize;

            control.AddFontOverride("font", dynamicFont);
        }

        public static List<string> GetLines(this File file)
        {
            List<string> result = new List<string>();

            while (!file.EofReached())
            {
                result.Add(file.GetLine());
            }

            return result;        
        }

        public static void DisableTooltips(this ItemList itemlist)
        {
            DebugHelper.PrettyPrintVerbose($"Number of items: {itemlist.GetItemCount()}");

            for (int i = 0; i < itemlist.GetItemCount(); i++)
            {
                itemlist.SetItemTooltipEnabled(i, false);
            }
        
        }

        public static T GetChild<T>(this Node node, bool withPolymorphism = false) where T : Node
        {
            foreach (var child in node.GetChildren())
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    return (T)child;
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    return (T)child;
                }
            }

            return default;
        }

        public static Node GetChildByName(this Node node, string name)
        {
            Node result = null;

            foreach (var child in node.GetChildren())
            {
                var item = child as Node;
                if (item != null)
                {
                    if (item.Name.Equals(name)) result = item;
                }

            }
            return result;
        }

        public static Node GetChildByNameRecursive(this Node node, string name)
        {
            var children = node.GetChildren();

            foreach (var child in children)
            {
                var item = child as Node;

                if (item != null)
                {
                    if (item.Name.Equals(name)) return item;
                }
            }

            foreach (var child in children)
            {
                var grandChild = (child as Node).GetChildByNameRecursive(name);

                if (grandChild != default) return grandChild;
            }

            return default;
        }

        private static IEnumerable<T> GetDescendants<T>(Node node, bool withPolymorphism, List<T> result)
        {
            var children = node.GetChildren();

            foreach (var child in children)
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    result.Add((T)child);
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    result.Add((T)child);
                }
            }

            foreach (var child in children)
            {
                GetDescendants((Node)child, withPolymorphism, result);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="withPolymorphism"> if true, returns any instance which can be inherited from the given type</param>
        /// <returns></returns>
        public static T GetChildRecursive<T>(this Node node, bool withPolymorphism = true) where T : class
        {
            var children = node.GetChildren();

            foreach (var child in children)
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    return (T)child;
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    return (T)child;
                }
            }

            foreach (var child in children)
            {
                var grandChild = (child as Node).GetChildRecursive<T>(withPolymorphism);

                if (grandChild != default) return grandChild;
            }

            return default;
        }

        public static T GetAncestor<T>(this Node node, bool withPolymorphism = true) where T : Node
        {
            var parent = node.GetParent();

            if (parent == default) return default;

            var typeOfParent = parent.GetType();

            if (!withPolymorphism && typeof(T).Equals(typeOfParent))
            {
                return (T)parent;
            }

            if (withPolymorphism && (typeof(T).Equals(typeOfParent) || (typeof(T).IsAssignableFrom(typeOfParent))))
            {
                return (T)parent;
            }

            return parent.GetAncestor<T>();
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static void SetLangugeSpecificTheme(this Control control, string themepath)
        {

                if (themepath != string.Empty)
                {
                    control.Theme = ResourceLoader.Load(themepath) as Theme;

                    DebugHelper.PrettyPrintVerbose($"Loading language specific theme: {themepath}", ConsoleColor.DarkCyan);

                    return;
                }


            control.Theme = default;
        }

    }
}
