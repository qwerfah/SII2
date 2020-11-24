using SII2.Helpers;
using SII2.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System;
using System.Windows;
using SII2.Measures;

namespace SII2.ViewModels
{
    class ApplicationViewModel : Observable
    {
        public readonly string IconPath = Path.GetFullPath("../../../Resources/images.png");

        private Tree _memoryTree;
        private ObservableCollection<TreeViewItem> _memoryTreeViewItems;

        public Tree MemoryTree
        {
            get => _memoryTree;
            set => Set(ref _memoryTree, value);
        }

        public ObservableCollection<TreeViewItem> MemoryTreeViewItems
        {
            get => _memoryTreeViewItems;
            set => Set(ref _memoryTreeViewItems, value);
        }

        private string _filename = "";
        private string _firstNodeName = string.Empty;
        private string _secondNodeName = string.Empty;

        public string Filename
        {
            get => _filename;
            set => Set(ref _filename, value);
        }

        public string FirstNodeName
        {
            get => _firstNodeName;
            set => Set(ref _firstNodeName, value);
        }

        public string SecondNodeName
        {
            get => _secondNodeName;
            set => Set(ref _secondNodeName, value);
        }

        private double _euclideanDistance = 0.0;
        private double _manhattanDistance = 0.0;
        private double _treeDistance = 0.0;
        private double _correlationDistance = 0.0;

        public double EuclideanDistance
        {
            get => _euclideanDistance;
            set => Set(ref _euclideanDistance, value);
        }

        public double ManhattanDistance
        {
            get => _manhattanDistance;
            set => Set(ref _manhattanDistance, value);
        }

        public double TreeDistance
        {
            get => _treeDistance;
            set => Set(ref _treeDistance, value);
        }

        public double CorrelationDistance
        {
            get => _correlationDistance;
            set => Set(ref _correlationDistance, value);
        }

        private ICommand _openFileDialogCommand;
        private ICommand _loadTreeCommand;
        private ICommand _calcDistanceCommand;
        private ICommand _addAsFirstNodeCommand;
        private ICommand _addAsSecondNodeCommand;

        public ICommand OpenFileDialogCommand => _openFileDialogCommand ??= new RelayCommand<object>(_ =>
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog().Value)
            {
                Filename = fileDialog.FileName;
            }
        });

        public ICommand LoadTreeCommand => _loadTreeCommand ??= new RelayCommand<object>(_ =>
        {
            LoadTreeFromFile();
        });

        public ICommand CalcDistanceCommand => _calcDistanceCommand ??= new RelayCommand<object>(obj =>
        {
            var calculator = new DistanceCalculator(MemoryTree, FirstNodeName, SecondNodeName);

            EuclideanDistance = Math.Round(calculator.CalculateDistance(MeasureType.EuclideanDistance), 2);
            ManhattanDistance = Math.Round(calculator.CalculateDistance(MeasureType.ManhattanDistance), 2);
            TreeDistance = Math.Round(calculator.CalculateDistance(MeasureType.TreeDistance), 2);
            CorrelationDistance = Math.Round(calculator.CalculateDistance(MeasureType.Correlation), 2);
        });

        public ICommand AddAsFirstNodeCommand => _addAsFirstNodeCommand ??= new RelayCommand<TreeViewItem>(item =>
        {
            FirstNodeName = (item ?? throw new ArgumentNullException(nameof(item))).Header as string;
        });

        public ICommand AddAsSecondNodeCommand => _addAsSecondNodeCommand ??= new RelayCommand<TreeViewItem>(item =>
        {
            SecondNodeName = (item ?? throw new ArgumentNullException(nameof(item))).Header as string;
        });

        public ApplicationViewModel()
        {
            LoadTreeFromFile();
            MessageBox.Show(IconPath);
        }

        private void LoadTreeFromFile()
        {
            try 
            {
                string fileContent = string.IsNullOrWhiteSpace(Filename) ? 
                    Properties.Resources.tree : File.ReadAllText(Filename);

                MemoryTree = JsonSerializer.Deserialize<Tree>(fileContent);
                MemoryTree.SetParents();

                var root = new TreeViewItem();
                root.Header = "Дерево";
                MemoryTree.AddToTreeView(root, new[] { AddAsFirstNodeCommand, AddAsSecondNodeCommand });

                var items = new ObservableCollection<TreeViewItem>();
                items.Add(root);
                MemoryTreeViewItems = items;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Ошибка при загрузке дерева: неверный путь к файлу.");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Ошибка при загрузке дерева: указанный путь не существует.");
            }
            catch (JsonException)
            {
                MessageBox.Show("Ошибка при загрузке дерева: неверный формат данных.");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                MessageBox.Show($"Ошибка при загрузке дерева: {e.Message}", "Alert");
            }
        }
    }
}
