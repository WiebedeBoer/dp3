using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Windows.Storage;
using Windows.ApplicationModel.Activation;

namespace tekenprogramma
{


    public class Shape
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Invoker invoker;
        public Canvas paintSurface;

        public FrameworkElement prevelement; //prev element
        public FrameworkElement nextelement; //next element

        //file IO
        public string fileText { get; set; }

        //shape
        public Shape(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        // Selects the shape
        public void Select(PointerRoutedEventArgs e)
        {
            //this.selected = true;
        }

        // Deselects the shape
        public void Deselect(PointerRoutedEventArgs e)
        {
            //this.selected = false;
        }

        //
        //repaint
        //
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //
        //creation
        //

        //create rectangle
        public void MakeRectangle(Invoker invoker, Canvas paintSurface)
        {
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            invoker.drawnElements.Add(newRectangle);
            Repaint(invoker, paintSurface); //repaint
        }

        //create ellipse
        public void MakeEllipse(Invoker invoker, Canvas paintSurface)
        {
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position   
            invoker.drawnElements.Add(newEllipse);
            Repaint(invoker, paintSurface); //repaint
        }

        //
        //undo redo
        //

        //undo create
        public void Remove(Invoker invoker, Canvas paintSurface)
        {
            //remove previous
            prevelement = invoker.drawnElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
        }

        //redo create
        public void Add(Invoker invoker, Canvas paintSurface)
        {
            //create next
            nextelement = invoker.removedElements.Last();
            invoker.drawnElements.Add(nextelement);
            invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint
        }

        //
        //moving
        //

        //new moving shape
        public void Moving(Invoker invoker, Canvas paintSurface, Location location, FrameworkElement element)
        {
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
            if (element.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);//set left position
                Canvas.SetTop(newRectangle, location.y); //set top position          
                invoker.drawnElements.Add(newRectangle);
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width;
                newEllipse.Height = location.height;
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                invoker.drawnElements.Add(newEllipse);
            }
            Repaint(invoker, paintSurface); //repaint

        }

        //remove selected element by access key
        public void KeyNumber(FrameworkElement element, Invoker invoker)
        {
            string key = element.AccessKey;
            int inc = 0;
            int number = 0;
            foreach (FrameworkElement drawn in invoker.drawnElements)
            {
                if (drawn.AccessKey == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnElements.RemoveAt(number);
            invoker.removedElements.Add(element);
            invoker.movedElements.Add(element);
        }

        //move back element
        public void MoveBack(Invoker invoker, Canvas paintSurface)
        {
            //remove next
            prevelement = invoker.drawnElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //move back moved element
            nextelement = invoker.movedElements.Last();
            invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
            invoker.drawnElements.Add(nextelement);
            Repaint(invoker, paintSurface); //repaint   
        }

        //move back element
        public void MoveAgain(Invoker invoker, Canvas paintSurface)
        {
            //remove previous
            prevelement = invoker.drawnElements.Last();
            nextelement = invoker.removedElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.movedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            //move again moved element
            invoker.drawnElements.Add(nextelement);
            Repaint(invoker, paintSurface); //repaint   
        }

        //
        //resizing
        //

        //resize shape
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            KeyNumber(element, invoker); //move selected at removed
            //calculate size
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            double w = ReturnSmallest(ex, lw);
            double h = ReturnSmallest(ey, lh);
            //create at new size
            if (element.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = w; //set width
                newRectangle.Height = h; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, lw);
                Canvas.SetTop(newRectangle, lh);
                invoker.drawnElements.Add(newRectangle);
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = w; //set width
                newEllipse.Height = h; //set height 
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, lw);//set left position
                Canvas.SetTop(newEllipse, lh);//set top position
                invoker.drawnElements.Add(newEllipse);
            }
            Repaint(invoker, paintSurface); //repaint
        }

        //give smallest
        public double ReturnSmallest(double first, double last)
        {
            if (first < last)
            {
                return last - first;
            }
            else
            {
                return first - last;
            }
        }

        //
        //saving
        //
        public async void Saving(Canvas paintSurface)
        {

            try
            {
                string lines = "";

                foreach (FrameworkElement child in paintSurface.Children)
                {
                    if (child is Rectangle)
                    {
                        double top = (double)child.GetValue(Canvas.TopProperty);
                        double left = (double)child.GetValue(Canvas.LeftProperty);
                        string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                        lines += str;
                    }
                    else
                    {
                        double top = (double)child.GetValue(Canvas.TopProperty);
                        double left = (double)child.GetValue(Canvas.LeftProperty);
                        string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                        lines += str;
                    }
                }
                //create and write to file
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp2data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
            }
            //file errors
            catch (System.IO.FileNotFoundException)
            {
                fileText = "File not found.";
            }
            catch (System.IO.FileLoadException)
            {
                fileText = "File Failed to load.";
            }
            catch (System.IO.IOException e)
            {
                fileText = "File IO error " + e;
            }
            catch (Exception err)
            {
                fileText = err.Message;
            }

        }

        //
        //loading
        //
        public async void Loading(Canvas paintSurface)
        {
            //clear previous canvas
            paintSurface.Children.Clear();
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp2data.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(saveFile);
            //load shapes
            string[] readText = Regex.Split(text, "\\n+");
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string[] line = Regex.Split(s, "\\s+");
                    if (line[0] == "Ellipse")
                    {
                        this.GetEllipse(s, paintSurface);
                    }
                    else
                    {
                        this.GetRectangle(s, paintSurface);
                    }
                }
            }
        }

        //load ellipse
        public void GetEllipse(String lines, Canvas paintSurface)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
        }

        //load rectangle
        public void GetRectangle(String lines, Canvas paintSurface)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
        }

    }

}