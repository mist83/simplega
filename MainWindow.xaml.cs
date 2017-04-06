using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Pointillism.Models;

namespace Pointillism
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Population population;
        private int currentIndividual = 0;

        public MainWindow()
        {
            InitializeComponent();

            Utility.Canvas = MyCanvas;

            Loaded += MainWindow_Loaded;
            MouseDown += MainWindow_MouseDown;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += DispatcherTimer_Tick;
            timer.Start();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            population = new Population(100);
            RedrawFunction();
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RedrawFunction();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            RedrawFunction();
        }

        private void RedrawFunction()
        {
            Title = $"Gen {Utility.Generation}: {currentIndividual}";

            // time for the next generation
            if (currentIndividual >= population.Count)
            {
                population.Save();

                // breed the next generation
                // TODO: HAVE TO INTRODUCE MUTATION!!! OTHERWISE WE WILL HIT A MAX AND HAVE TO STOP
                currentIndividual = 0;
                population = population.Breed();
                Utility.Generation++;
                return;
            }

            Redraw(population[currentIndividual]);
            population[currentIndividual].Score();

            currentIndividual++;
        }

        private void Redraw(Individual individual)
        {
            MyCanvas.Children.Clear();

            foreach (var chromosome in individual.Chromosomes)
            {
                var ellipse = new Ellipse { Width = chromosome.Radius, Height = chromosome.Radius, Fill = new SolidColorBrush(chromosome.Color), Opacity = chromosome.Opacity };
                Canvas.SetLeft(ellipse, chromosome.X - chromosome.Radius / 2);
                Canvas.SetTop(ellipse, chromosome.Y - chromosome.Radius / 2);

                MyCanvas.Children.Add(ellipse);
            }
        }

        private Chromosome[] Redraw1()
        {
            var chromosomes = new Chromosome[1000];

            MyCanvas.Children.Clear();
            MyCanvas.Background = Brushes.Black;

            for (var i = 0; i < chromosomes.Length; i++)
            {
                var p = Chromosome.Random();

                //var size = r.Next(5, 30);
                //size = 20;
                //var color = colors[r.Next(colors.Length)];
                //var opacity = r.Next(101) / 100.0;
                var ellipse = new Ellipse { Width = p.Radius, Height = p.Radius, Fill = new SolidColorBrush(p.Color), Opacity = p.Opacity };
                Canvas.SetLeft(ellipse, p.X - p.Radius / 2);
                Canvas.SetTop(ellipse, p.Y - p.Radius / 2);

                MyCanvas.Children.Add(ellipse);

                chromosomes[i] = p;
            }

            return chromosomes;
        }
    }
}
