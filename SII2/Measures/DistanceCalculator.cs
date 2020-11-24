using SII2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SII2.Measures
{
    /// <summary> Класс для вычисления расстояний. </summary>
    class DistanceCalculator
    {
        private readonly string _firstName;
        private readonly string _secondName;
        private readonly Tree _tree;

        public DistanceCalculator(Tree tree, string fname, string sname)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _firstName = fname ?? throw new ArgumentNullException(nameof(fname));
            _secondName = sname ?? throw new ArgumentNullException(nameof(sname));
        }

        public double CalculateDistance(MeasureType type = MeasureType.EuclideanDistance)
        {
            Node node1 = _tree.GetNode(_firstName);
            Node node2 = _tree.GetNode(_secondName);

            if (node1 == null)
            {
                MessageBox.Show($"Не существует узла с именем {_firstName}.");
                return 0.0;
            }
            if (node2 == null)
            {
                MessageBox.Show($"Не существует узла с именем {_secondName}.");
                return 0.0;
            }

            switch (type)
            {
                case MeasureType.EuclideanDistance:
                    return CalculateEuclideanDistance(node1, node2);
                case MeasureType.ManhattanDistance:
                    return CalculateManhattanDistance(node1, node2);
                case MeasureType.TreeDistance:
                    return CalculateTreeDistance(node1, node2);
                case MeasureType.Correlation:
                    return CalculateCorrelationDistance(node1, node2);
                default: return 0.0;
            }
        }

        public double CalculateEuclideanDistance(Node node1, Node node2)
        {
            double a1 = Math.Pow(node1.Params.AverageCost - node2.Params.AverageCost, 2);
            double a2 = Math.Pow(node1.Params.MaxSpeed - node2.Params.MaxSpeed, 2);
            double a3 = Math.Pow(node1.Params.MaxStorageCapacity - node2.Params.MaxStorageCapacity, 2);
            double a4 = Math.Pow(node1.Params.ReleaseYear - node2.Params.ReleaseYear, 2);
            double a5 = Math.Pow(Convert.ToDouble(node1.Params.IsGeneralPurpose) - 
                Convert.ToDouble(node2.Params.IsGeneralPurpose), 2);

            return Math.Sqrt(a1 + a2 + a3 + a4 + a5);
        }

        public double CalculateManhattanDistance(Node node1, Node node2)
        {
            double a1 = Math.Abs(node1.Params.AverageCost - node2.Params.AverageCost);
            double a2 = Math.Abs(node1.Params.MaxSpeed - node2.Params.MaxSpeed);
            double a3 = Math.Abs(node1.Params.MaxStorageCapacity - node2.Params.MaxStorageCapacity);
            double a4 = Math.Abs(node1.Params.ReleaseYear - node2.Params.ReleaseYear);
            double a5 = Math.Abs(Convert.ToDouble(node1.Params.IsGeneralPurpose) -
                Convert.ToDouble(node2.Params.IsGeneralPurpose));

            return (a1 + a2 + a3 + a4 + a5);
        }

        public double CalculateTreeDistance(Node node1, Node node2)
        {
            var distances = new Dictionary<string, double>();
            distances.Add(_firstName, 0);

            var stack = new Stack<Node>();
            stack.Push(node1);

            while (stack.Any())
            {
                Node node = stack.Pop();
                var nodes = (node.Parent == null) ? node.Child :
                            node.Child.Union(new[] { node.Parent });
                foreach (Node child in nodes)
                {
                    if (!distances.TryGetValue(child.Name, out _))
                    {
                        distances.Add(child.Name, distances[node.Name] + 1.0);
                        stack.Push(child);
                    }
                }
            }

            return distances[node2.Name];
        }

        public double CalculateCorrelationDistance(Node node1, Node node2)
        {
            double avg1 = (node1.Params.MaxSpeed + node1.Params.MaxStorageCapacity + 
                          (double)node1.Params.ReleaseYear + node1.Params.AverageCost + 
                          Convert.ToDouble(node1.Params.IsGeneralPurpose)) / 5.0;
            double avg2 = (node2.Params.MaxSpeed + node2.Params.MaxStorageCapacity +
                          (double)node2.Params.ReleaseYear + node2.Params.AverageCost +
                          Convert.ToDouble(node2.Params.IsGeneralPurpose)) / 5.0;

            double[] a1 = new[] 
            {
                node1.Params.MaxSpeed - avg1,
                node1.Params.MaxStorageCapacity - avg1,
                (double)node1.Params.ReleaseYear - avg1,
                node1.Params.AverageCost - avg1,
                Convert.ToDouble(node1.Params.IsGeneralPurpose) - avg1,
            };
            double[] a2 = new[]
            {
                node2.Params.MaxSpeed - avg2,
                node2.Params.MaxStorageCapacity - avg2,
                (double)node2.Params.ReleaseYear - avg2,
                node2.Params.AverageCost - avg2,
                Convert.ToDouble(node2.Params.IsGeneralPurpose) - avg2,
            };

            double numerator = a1.Select((a, i) => a * a2[i]).Sum();
            double denominator = a1.Select(a => a * a).Sum() * a2.Select(a => a * a).Sum();

            return numerator / Math.Sqrt(denominator);
        }
    }
}
