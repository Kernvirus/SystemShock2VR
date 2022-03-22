using Assets.Scripts.DebugHelper;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects
{
    class DarkObject
    {
        public int id;
        public bool IsInstance => id > 0;
        public GameObject gameObject = null;
        public string Name
        {
            get
            {
                if (name == null)
                {
                    if (Parent != null)
                        return Parent.Name;
                    else
                        return "NAME NOT SET";
                }
                return name;
            }
        }
        public DarkObject Parent => linkToParent?.dest;

        private Dictionary<Type, Prop> propMap = new Dictionary<Type, Prop>();
        private Dictionary<Type, List<Link>> datalinkMap = new Dictionary<Type, List<Link>>();
        private List<Link> links = new List<Link>();
        private Link linkToParent;
        private string name = null;

        public DarkObject(int id)
        {
            this.id = id;
        }

        public T GetProp<T>() where T : Prop
        {
            Prop prop;
            if (propMap.TryGetValue(typeof(T), out prop))
                return (T)prop;

            var arch = Parent;
            if (arch != null)
            {
                prop = arch.GetProp<T>();
                if (prop != null)
                    return (T)prop;
            }
            return null;
        }

        public bool HasProp<T>() where T : Prop
        {
            return GetProp<T>() != null;
        }

        public bool HasPropDirectly<T>() where T : Prop
        {
            return propMap.TryGetValue(typeof(T), out _);
        }

        public void AddProp(Prop prop)
        {
            propMap.Add(prop.GetType(), prop);

            if (prop.GetType() == typeof(SymNameProp))
            {
                name = ((SymNameProp)prop).Value;
            }
        }

        public T GetComponent<T>() where T : Component
        {
            if (gameObject == null)
                return null;
            return gameObject.GetComponent<T>();
        }

        public DarkObject GetParentWithId(int id)
        {
            DarkObject arch = Parent;
            if (arch == null || arch.id == id)
                return arch;

            return arch.GetParentWithId(id);
        }

        public void AddLink(Link link, Type linkDataType)
        {
            // check to override
            List<Link> links;
            if (!datalinkMap.TryGetValue(linkDataType, out links))
            {
                links = new List<Link>(1);
                datalinkMap.Add(linkDataType, links);
            }
            links.Add(link);

            if (linkDataType == typeof(MetaPropLink) && ((MetaPropLink)link.data).Value == 0)
                linkToParent = link;
        }

        public void AddLink(Link link)
        {
            // check to override
            links.Add(link);
        }

        public List<Link> GetLinks(Type linkType)
        {
            List<Link> links;
            if (datalinkMap.TryGetValue(linkType, out links))
                return links;
            return new List<Link>(0);
        }

        public List<Link> GetLinks(string linkName)
        {
            return links.Where(l => l.name == linkName).ToList();
        }

        public IEnumerable<Prop> Props()
        {
            foreach (var prop in propMap.Values)
            {
                yield return prop;
            }

            var parent = Parent;
            while (parent != null)
            {
                foreach (var prop in parent.propMap.Values)
                {
                    yield return prop;
                }
                parent = parent.Parent;
            }
        }

        public DarkObject GetOwnerOfProp<T>() where T : Prop
        {
            if (HasPropDirectly<T>())
                return this;
            else if (Parent != null)
                return Parent.GetOwnerOfProp<T>();
            else
                return null;
        }

        public void WriteAsComment(CommentBox commentSection)
        {
            commentSection.Id = id;
            commentSection.AddComment("Properties:");
            foreach (var prop in propMap.Values)
            {
                commentSection.AddComment(prop.ToString());
            }
            if (Parent != null)
            {
                commentSection.AddComment("Inherited Properties:");
                foreach (var prop in Parent.Props())
                {
                    commentSection.AddComment(prop.ToString());
                }
            }

            commentSection.AddComment("Links:");
            foreach (var links in datalinkMap.Values)
            {
                foreach (var link in links)
                {
                    commentSection.AddComment("\t" + link.ToString());
                }
            }
            foreach (var link in links)
            {
                commentSection.AddComment("\t" + link.ToString());
            }
        }

        public string FullPath()
        {
            if (Parent != null)
            {
                if (name == null)
                    return Parent.FullPath();
                else
                    return Parent.FullPath() + "/" + name.Replace('/', '_').Replace(':', '_');
            }
            else
            {
                return Name;
            }
        }

        public override string ToString()
        {
            return $"{Name}[{id}]";
        }
    }
}
