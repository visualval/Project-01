﻿using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU.Drawers
{
    [Serializable]

    public class InspectorDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        private NodeType[] _allowedInspectorNodes = new NodeType[]
        {
            NodeType.CANVAS,
            NodeType.FRAME
        };

        public SelectableFObject FillSelectableFramesArray(FObject document)
        {
            _selectableHambItems.Clear();

            SelectableFObject doc = new SelectableFObject();

            FillNewSelectableItemRecursively(doc, document);

            bool same = CompareIdsRecursively(_doc, doc);

            if (!same)
            {
                _doc = doc;
                _doc.SetAllSelected(true);
            }

            monoBeh.DelegateHolder.UpdateScrollContent();

            return _doc;
        }

        public void FillNewSelectableItemRecursively(SelectableFObject parentItem, FObject parent)
        {
            parentItem.Id = parent.Id;
            parentItem.Type = parent.Type;
            parentItem.Name = parent.Name;

            if (parent.Children.IsEmpty())
                return;

            foreach (FObject child in parent.Children)
            {
                bool isAllowed = _allowedInspectorNodes.Contains(child.Type);

                if (!isAllowed)
                    continue;

                SelectableFObject childItem = new SelectableFObject();
                FillNewSelectableItemRecursively(childItem, child);
                parentItem.Childs.Add(childItem);
            }
        }

        private bool CompareIdsRecursively(SelectableFObject item1, SelectableFObject item2)
        {
            if (item1.Id != item2.Id)
                return false;

            if (item1.Childs.Count != item2.Childs.Count)
                return false;

            for (int i = 0; i < item1.Childs.Count; i++)
            {
                if (!CompareIdsRecursively(item1.Childs[i], item2.Childs[i]))
                    return false;
            }

            return true;
        }

        [SerializeField] List<HamburgerItem> _selectableHambItems = new List<HamburgerItem>();
        public List<HamburgerItem> SelectableHamburgerItems => _selectableHambItems;

        [SerializeField] SelectableFObject _doc = new SelectableFObject();
        public SelectableFObject SelectableDocument => _doc;
    }

    [Serializable]
    public class SelectableFObject
    {
        [SerializeField] string id;
        public string Id { get => id; set => id = value; }

        [SerializeField] string name;
        public string Name { get => name; set => name = value; }

        [SerializeField] NodeType type;
        public NodeType Type { get => type; set => type = value; }

        [SerializeField] bool selected;
        public bool Selected { get => selected; set => selected = value; }

        [SerializeField] List<SelectableFObject> childs = new List<SelectableFObject>();
        public List<SelectableFObject> Childs { get => childs; set => childs = value; }

        public void SetAllSelected(bool value)
        {
            selected = value;

            foreach (SelectableFObject child in childs)
            {
                child.SetAllSelected(value);
            }
        }

        public FObject ConvertToFObject()
        {
            FObject fobject = new FObject
            {
                Id = id,
                Type = type,
                Name = name,
                Children = new List<FObject>()
            };

            foreach (SelectableFObject child in childs)
            {
                //Debug.Log(child.Name);
                fobject.Children.Add(child.ConvertToFObject());
            }

            return fobject;
        }
    }
}