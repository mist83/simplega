﻿using Pointillism.Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Pointillism
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Population population;
        private int currentIndividual = 0;

        public MainWindow()
        {
            InitializeComponent();

            var landscape = System.IO.Path.GetTempFileName() + ".png";
            using (var stream = new FileStream(landscape, FileMode.Create))
            {
                typeof(MainWindow).Assembly.GetManifestResourceStream("Pointillism.images.Landscape.png").CopyTo(stream);
            }

            Original.Source = new BitmapImage(new Uri(landscape));
            // Hack to get static constructor to fire
            Console.WriteLine(new Individual(new Chromosome[] { }).ToString());

            Target.Source = new BitmapImage(new Uri(Utility.Original));

            Loaded += MainWindow_Loaded;
            MouseDown += MainWindow_MouseDown;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
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

        DateTime start = DateTime.Now;

        private void RedrawFunction()
        {
            Title = $"Generation {Utility.Generation}: Determining fitness for individual {currentIndividual}";

            // time for the next generation
            if (currentIndividual >= population.Individuals.Count)
            {
                population.Save();

                // breed the next generation
                currentIndividual = 0;
                var bred = population.Breed();
                MyCanvas.Source = new BitmapImage(new Uri(population.FittestFile));
                Best.Text = population.Best;
                Worst.Text = population.Worst;
                Total.Text = population.Total;
                Average.Text = population.Average;
                var time = DateTime.Now.Subtract(start);
                Time.Text = string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);

                population = bred;

                Utility.Generation++;
                return;
            }

            var individual = population.Individuals[currentIndividual];
            Redraw(individual);
            individual.Score();

            currentIndividual++;
        }

        Bitmap b = new Bitmap(32, 32);
        private static Graphics graphics;

        private void Redraw(Individual individual)
        {
            RedrawToBitmap(individual);
        }

        public static Bitmap RedrawToBitmap(Individual individual)
        {
            //if (graphics == null)
            //    graphics = Graphics.FromImage(b);

            System.Drawing.Color[,] matrix = new System.Drawing.Color[32, 32];
            individual.GrayScales = matrix;

            //graphics.FillRectangle(System.Drawing.Brushes.White, new System.Drawing.RectangleF(0, 0, b.Width, b.Height));

            //MyCanvas.Children.Clear();
            foreach (var chromosome in individual.Chromosomes)
            {
                //var ellipse = new Ellipse { Width = chromosome.Radius, Height = chromosome.Radius, Fill = new SolidColorBrush(chromosome.Color) };
                //Canvas.SetLeft(ellipse, chromosome.X - chromosome.Radius / 2);
                //Canvas.SetTop(ellipse, chromosome.Y - chromosome.Radius / 2);

                //MyCanvas.Children.Add(ellipse);

                var argb = (chromosome.Color.A << 24) | (chromosome.Color.R << 16) | (chromosome.Color.G << 8) | chromosome.Color.B;

                matrix[chromosome.Y, chromosome.X] = System.Drawing.Color.FromArgb(argb);
                //if (Utility.Generation % 10 == 0)
                //{
                //    var brush = new SolidBrush(System.Drawing.Color.FromArgb(argb));
                //    //graphics.FillRectangle(brush, chromosome.X - chromosome.Radius - 2, chromosome.Y - chromosome.Radius - 2, chromosome.Radius, chromosome.Radius);
                //    graphics.FillRectangle(brush, chromosome.X, chromosome.Y, chromosome.Radius, chromosome.Radius);
                //}
            }

            //var grayList = new List<System.Drawing.Color>();
            //var grays = new HashSet<System.Drawing.Color>();
            //if (Utility.Generation % 10 == 0)
            //{
            //    for (var y = 0; y < 256; y++)
            //    {
            //        for (var x = 0; x < 256; x++)
            //        {
            //            var gray = Individual.GetGrayColor(Individual.target[y, x]);

            //            grayList.Add(gray);
            //            grays.Add(gray);

            //            var brush = new SolidBrush(gray);
            //            //graphics.FillRectangle(brush, x, y, 1, 1);
            //        }
            //    }
            //}

            //b.Save(temp, System.Drawing.Imaging.ImageFormat.Png);
            //Utility.Canvas.Tag = matrix;

            if (Utility.Generation % 10 == 0)
            {
                //var stm = new MemoryStream();
                //b.Save(stm, System.Drawing.Imaging.ImageFormat.Png);

                //string tempFile = System.IO.Path.GetTempFileName() + ".pointillism.png";
                //b.Save(tempFile, System.Drawing.Imaging.ImageFormat.Png);
                //MyCanvas.Source = new BitmapImage(new Uri(tempFile));
            }

            return null;
            //Process.Start(temp);
        }

        private void Original_MouseEnter(object sender, MouseEventArgs e)
        {
            ((System.Windows.Controls.Image)sender).Opacity = 0.01;
        }

        private void Original_MouseLeave(object sender, MouseEventArgs e)
        {
            ((System.Windows.Controls.Image)sender).Opacity = 1;
        }
    }

    public static class GraphicsExtensions
    {
        public static void FillCircle(this System.Drawing.Graphics g, System.Drawing.Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
    }
}
