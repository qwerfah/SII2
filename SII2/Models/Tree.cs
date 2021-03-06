﻿using SII2.Extensions;
using SII2.Helpers;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SII2.Models
{
    /// <summary> Дерево узлов со строковыми идентификаторами. </summary>
    class Tree : Observable
    {
        private Node _root;

        /// <summary> Корневой узел дерева. </summary>
        public Node Root
        {
            get => _root;
            set => Set(ref _root, value);
        }

        public Tree()
        {
            Root = new Node(nameof(Root));
        }

        public Tree(Node root = null)
        {
            Root = (root == null) ? new Node(nameof(Root)) : root;
        }

        /// <summary> Добавить нового потомка  </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="root"></param>
        public void AddNode(Node node, string name, Node root = null)
        {
            if (root == null) root = Root;
            if (root.Name == name)
            {
                (node ?? throw new ArgumentNullException(nameof(node))).Parent = root;
                root.Child.Add(node);
            }
            else
            {
                foreach (Node child in root.Child)
                {
                    AddNode(node, name, child);
                }
            }
        }

        /// <summary> Получить узел дерева по его имени (идентификатору). </summary>
        /// <param name="name"> Имя (идентификатор) узла. </param>
        /// <param name="node"> Узел дерева, с которого начинается поиск (по умолчанию Root). </param>
        public Node GetNode(string name, Node node = null)
        {
            if (node == null) node = Root;
            if (node.Name == name) return node;

            foreach (Node child in node.Child)
            {
                node = GetNode(name, child);
                if (node != null) return node;
            }

            return null;
        }

        /// <summary> Установить ссылку на родителя для каждого узла в дереве. </summary>
        /// <param name="node"> Текущий узел дерева. </param>
        /// <param name="parent"> Родитель текущего узла. </param>
        public void SetParents(Node node = null, Node parent = null)
        {
            if (node == null) node = Root;
            node.Parent = parent;

            foreach (Node child in node.Child)
            {
                SetParents(child, node);
            }
        }

        public void AddToTreeView(TreeViewItem treeView, ICommand[] commands, Node node = null)
        {
            if ((commands ?? throw new ArgumentNullException(nameof(commands))).Length != 2)
            {
                throw new ArgumentException("Контекстное меню содержит ровно два действия.");
            }

            if (node == null) node = Root;

            var item = new TreeViewItem { Header = node.Name };
            var menu = new ContextMenu();

            menu.Items.Add(new MenuItem 
            { 
                Header = "Выбрать в качестве первого узла", 
                Command = commands[0], 
                CommandParameter = item 
            });
            menu.Items.Add(new MenuItem 
            { 
                Header = "Выбрать в качестве второго узла", 
                Command = commands[1], 
                CommandParameter = item 
            });

            item.ContextMenu = menu;
            treeView.Items.Add(item);

            if (!node.Child.Any())
            {
                AddNodeToTreeView(item, node);
            }

            foreach (Node child in node.Child)
            {
                AddToTreeView(item, commands, child);
            }
        }

        private void AddNodeToTreeView(TreeViewItem item, Node node)
        {
            (item ?? throw new ArgumentNullException(nameof(item)))
                .Items.Add(new TreeViewItem { Header = 
                $"1. Максимальная скорость: {(node ?? throw new ArgumentNullException(nameof(node))).Params.MaxSpeed} Мб/с" });
            item.Items.Add(new TreeViewItem { Header = $"2. Максимальная емкость: {node.Params.MaxStorageCapacity} Мб" });
            item.Items.Add(new TreeViewItem { Header = $"3. Год выпуска: {node.Params.ReleaseYear}" });
            item.Items.Add(new TreeViewItem { Header = $"4. Средняя стоимость: {node.Params.AverageCost} Руб/Мб" });
            item.Items.Add(new TreeViewItem { Header = "5. Тип: " +
                ((node.Params.IsGeneralPurpose) ? "общего назначения" : "специального назначения") });

            var application = new TreeViewItem { Header = "6. Применение" };
            item.Items.Add(application);
            if (node.Params.MemoryTypes.Any())
            {
                foreach (MemoryType type in node.Params.MemoryTypes)
                {
                    application.Items.Add(new TreeViewItem { Header = type.Description() });
                }
            }
            else
            {
                application.Items.Add(new TreeViewItem { Header = "Не указано" });
            }
        }
    }
}
