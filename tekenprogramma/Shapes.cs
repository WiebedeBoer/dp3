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

    //shape class
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
        public FrameworkElement selectedElement; //selected element

        public List<FrameworkElement> newDrawn = new List<FrameworkElement>(); //drawn elements
        public List<FrameworkElement> newSelected = new List<FrameworkElement>(); //selected elements

        //public List<List<FrameworkElement>> undoElementsList = new List<List<FrameworkElement>>(); //4a keep track of undone
        //public List<List<FrameworkElement>> redoElementsList = new List<List<FrameworkElement>>(); //4b keep track of redone

        //public List<List<FrameworkElement>> unselectElementsList = new List<List<FrameworkElement>>(); //4a keep track of undone
        //public List<List<FrameworkElement>> reselectElementsList = new List<List<FrameworkElement>>(); //4b keep track of redone

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

        //
        //selecting
        //

        // Selects the shape, state 2
        public void Select(PointerRoutedEventArgs e, Invoker invoker, Canvas paintSurface)
        {
            //fetch
            selectedElement = e.OriginalSource as FrameworkElement;//2af
            //opacity
            selectedElement.Opacity = 0.6; //fill opacity
            //if not first selected
            if (invoker.unselectElementsList.Count() > 0)
            {
                //fetch
                List<FrameworkElement> fetchSelected = invoker.unselectElementsList.Last();
                foreach (FrameworkElement addElement in fetchSelected)
                {
                    newSelected.Add(addElement); //add
                }
            }
            //add
            newSelected.Add(selectedElement);
            invoker.unselectElementsList.Add(newSelected);
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
            /*
            selectedElement = e.OriginalSource as FrameworkElement;//2af
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.selectElements.Add(selectedElement); //2a+
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
            //prepare redo
            PrepareRedo(invoker);
            */
        }

        // Deselects the shape, state 1
        public void Deselect(Invoker invoker, Canvas paintSurface)
        {
            //fetch
            List<FrameworkElement> lastRedo = invoker.unselectElementsList.Last();
            //shuffle
            invoker.reselectElementsList.Add(lastRedo);
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count - 1);
            //fetch
            selectedElement = lastRedo.Last(); //2af  
            //opacity
            selectedElement.Opacity = 1; //fill opacity
            /*
            selectedElement = invoker.selectElements.Last(); //2af  //er1
            selectedElement.Opacity = 1; //fill opacity
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.UnselectGroup(selectedElement, invoker);

            */
        }

        // Reselects the shape, state 2
        public void Reselect(Invoker invoker, Canvas paintSurface)
        {
            //fetch
            List<FrameworkElement> lastUndo = invoker.reselectElementsList.Last();
            //shuffle
            invoker.unselectElementsList.Add(lastUndo);
            invoker.reselectElementsList.RemoveAt(invoker.reselectElementsList.Count - 1);
            //fetch
            selectedElement = lastUndo.Last(); //2af  
            //opacity
            selectedElement.Opacity = 1; //fill opacity
            /*
            selectedElement = invoker.unselectElements.Last(); //2bf
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.selectElements.Add(selectedElement); //2a+
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
            */

        }

        //
        //creation
        //

        //create rectangle, state 1
        public void MakeRectangle(Invoker invoker, Canvas paintSurface)
        {
            //prepare undo
            //if not first draw
            if (invoker.undoElementsList.Count() > 0)
            {
                //fetch
                List<FrameworkElement> fetchDrawn = invoker.undoElementsList.Last();
                foreach (FrameworkElement addElement in fetchDrawn)
                {
                    newDrawn.Add(addElement); //add
                }
            }
            //create
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString(); //access key
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            //add
            newDrawn.Add(newRectangle);
            invoker.undoElementsList.Add(newDrawn);
            //repaint surface
            Repaint(invoker, paintSurface);       
        }

        //create ellipse, state 1
        public void MakeEllipse(Invoker invoker, Canvas paintSurface)
        {
            //prepare undo
            //if not first draw
            if (invoker.undoElementsList.Count() > 0)
            {
                //fetch
                List<FrameworkElement> fetchDrawn = invoker.undoElementsList.Last();
                foreach (FrameworkElement addElement in fetchDrawn)
                {
                    newDrawn.Add(addElement); //add
                }
            }
            //create
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString(); //access key
            newEllipse.Width = width; //set width
            newEllipse.Height = height; //set height
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position   
            //add
            newDrawn.Add(newEllipse);
            invoker.undoElementsList.Add(newDrawn);
            //repaint surface
            Repaint(invoker, paintSurface);
        }

        //
        //undo redo of create
        //

        //undo create, state 0
        public void Remove(Invoker invoker, Canvas paintSurface)
        {
            //fetch
            List<FrameworkElement> lastRedo = invoker.undoElementsList.Last();
            //shuffle
            invoker.redoElementsList.Add(lastRedo);
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count - 1);
            //repaint surface
            Repaint(invoker, paintSurface);
            /*
            //fetch
            List<FrameworkElement> lastRedo = invoker.undoElementsList.Last();
            //shuffle
            invoker.redoElementsList.Add(lastRedo);
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count -1);
            //clear surface
            paintSurface.Children.Clear();
            //check if last in list
            if (invoker.undoElementsList.Count() > 0)
            {
                //fetch
                List<FrameworkElement> lastUndo = invoker.undoElementsList.Last();
                //draw surface
                foreach (FrameworkElement drawelement in lastUndo)
                {
                    paintSurface.Children.Add(drawelement); //add
                }
            }
            */
        }

        //redo create, state 1
        public void Add(Invoker invoker, Canvas paintSurface)
        {
            //fetch
            List<FrameworkElement> lastUndo = invoker.redoElementsList.Last();
            //shuffle
            invoker.undoElementsList.Add(lastUndo);
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count - 1);
            //repaint surface
            Repaint(invoker, paintSurface);
            /*
            //fetch
            List<FrameworkElement> lastUndo = invoker.redoElementsList.Last();
            //shuffle
            invoker.undoElementsList.Add(lastUndo);
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count - 1);
            //check if last in list
            if (invoker.redoElementsList.Count() >0)
            {
                //fetch
                List<FrameworkElement> lastRedo = invoker.redoElementsList.Last();
                //clear surface
                paintSurface.Children.Clear();
                //draw surface
                foreach (FrameworkElement drawelement in lastRedo)
                {
                    paintSurface.Children.Add(drawelement); //add
                }
            }
            */
        }

        //
        //moving, state 3
        //

        //new moving shape
        public void Moving(Invoker invoker, Canvas paintSurface, Location location, FrameworkElement clickedelement)
        {
            //prepare undo
            //fetch
            List<FrameworkElement> fetchDrawn = invoker.undoElementsList.Last();
            //key
            string key = clickedelement.AccessKey;
            foreach (FrameworkElement addElement in fetchDrawn)
            {
                //if not selected
                if (addElement.AccessKey != key)
                {
                    newDrawn.Add(addElement); //add
                }
            }

            //remove selected element from drawn
            //KeyNumber(clickedelement, invoker); //1-
            //remove from selected
            //invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //shuffle selected
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
            //create at new location
            if (clickedelement.Name == "Rectangle")
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
                //add new to drawn
                //invoker.drawnElements.Add(newRectangle); //1+
                //add
                newDrawn.Add(newRectangle);
                invoker.undoElementsList.Add(newDrawn);
            }
            else if (clickedelement.Name == "Ellipse")
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
                //add new to drawn
                //invoker.drawnElements.Add(newEllipse); //1+
                //add
                newDrawn.Add(newEllipse);
                invoker.undoElementsList.Add(newDrawn);
            }
            //prepare redo
            //PrepareRedo(invoker);
            //repaint surface
            Repaint(invoker, paintSurface);
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
        }

        //move back element, state 2
        public void MoveBack(Invoker invoker, Canvas paintSurface)
        {
            //fetch
            List<FrameworkElement> lastRedo = invoker.undoElementsList.Last();
            //shuffle
            invoker.redoElementsList.Add(lastRedo);
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count - 1);
            //shuffle unselected
            newSelected = invoker.reselectElementsList.Last();          
            invoker.unselectElementsList.Add(newSelected); //2a+
            invoker.reselectElementsList.RemoveAt(invoker.reselectElementsList.Count() - 1); //2b-
            //repaint surface
            Repaint(invoker, paintSurface);
            /*
            //shuffle unselected
            prevelement = invoker.movedElements.Last();
            invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1); //2b-
            invoker.selectElements.Add(prevelement); //2a+
            //shuffle moved
            nextelement = invoker.unmovedElements.Last();
            invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1); //3a-
            invoker.unmovedElements.RemoveAt(invoker.unmovedElements.Count() - 1); //3b-
            //add redo
            invoker.undoElements.Add(nextelement); //4a+
            invoker.redoElements.Add(prevelement); //4b+
            //remove and add to drawn
            KeyNumber(nextelement, invoker); //1-
            invoker.drawnElements.Add(prevelement); //1+
            //repaint surface
            Repaint(invoker, paintSurface);
            */
            /*
            //shuffle moved
            List<FrameworkElement> lastUndo = invoker.undoElementsList.Last();
            invoker.redoElementsList.Add(lastUndo); //4b-
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count() - 1); //4a+
            //repaint
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in lastUndo)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            //shuffle unselected
            prevelement = invoker.unselectElements.Last();
            invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1); //2b-
            invoker.selectElements.Add(prevelement); //2a+
            */
        }

        //move back element, state 3
        public void MoveAgain(Invoker invoker, Canvas paintSurface)
        {

            //fetch
            List<FrameworkElement> lastUndo = invoker.redoElementsList.Last();
            //shuffle
            invoker.undoElementsList.Add(lastUndo);
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count - 1);
            //shuffle selected
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
            //repaint surface
            Repaint(invoker, paintSurface);

            /*
            //shuffle selected
            nextelement = invoker.undoElements.Last();
            invoker.unselectElements.Add(nextelement); //2b+
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //shuffle moved
            prevelement = invoker.redoElements.Last();
            invoker.movedElements.Add(prevelement); //3a+
            invoker.unmovedElements.Add(nextelement); //3b+
            //undo redo
            invoker.undoElements.RemoveAt(invoker.undoElements.Count() - 1); ; //4a-
            invoker.redoElements.RemoveAt(invoker.redoElements.Count() - 1); ; //4b-
            //remove and add to drawn
            KeyNumber(prevelement, invoker); //1-
            invoker.drawnElements.Add(nextelement); //1+
            //repaint surface
            Repaint(invoker, paintSurface);
            */
            /*
            //shuffle moved
            List<FrameworkElement> lastRedo = invoker.redoElementsList.Last();
            invoker.undoElementsList.Add(lastRedo); //4b+
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count() - 1); //4a-
            //repaint
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in lastRedo)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            //shuffle selected
            nextelement = invoker.selectElements.Last();
            invoker.unselectElements.Add(nextelement); //2b+
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            */
        }

        //
        //resizing
        //

        //resize shape, state 3
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement clickedelement, Canvas paintSurface, Location location)
        {

            //prepare undo
            //fetch
            List<FrameworkElement> fetchDrawn = invoker.undoElementsList.Last();
            //key
            string key = clickedelement.AccessKey;
            foreach (FrameworkElement addElement in fetchDrawn)
            {
                //if not selected
                if (addElement.AccessKey != key)
                {
                    newDrawn.Add(addElement); //add
                }
            }
            //prepare undo
            //PrepareUndo(invoker);
            //remove selected element from drawn
            //KeyNumber(clickedelement, invoker); //1-
            //remove from selected
            //invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //shuffle selected
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
            //calculate size
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(clickedelement.ActualOffset.X); //set width
            double lh = Convert.ToDouble(clickedelement.ActualOffset.Y); //set height
            double w = ReturnSmallest(ex, lw);
            double h = ReturnSmallest(ey, lh);
            //create at new size
            if (clickedelement.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = w; //set width
                newRectangle.Height = h; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, lw); //set left position
                Canvas.SetTop(newRectangle, lh);//set top position
                //add to drawn
                //invoker.drawnElements.Add(newRectangle); //1+
                //add
                newDrawn.Add(newRectangle);
                invoker.undoElementsList.Add(newDrawn);
            }
            else if (clickedelement.Name == "Ellipse")
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
                //add to drawn
                //invoker.drawnElements.Add(newEllipse); //1+
                //add
                newDrawn.Add(newEllipse);
                invoker.undoElementsList.Add(newDrawn);
            }
            //prepare redo
            //PrepareRedo(invoker);
            //repaint surface
            Repaint(invoker, paintSurface);
        }

        //
        //miscellaneous
        //

        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            //check if undo elements 
            if (invoker.undoElementsList.Count() >0)
            {
                //fetch drawn
                List<FrameworkElement> PrepareDrawn = invoker.undoElementsList.Last();
                //check if drawn elements
                if (PrepareDrawn.Count() > 0)
                {
                    //clear surface
                    paintSurface.Children.Clear();
                    //draw surface
                    foreach (FrameworkElement drawelement in PrepareDrawn)
                    {
                        paintSurface.Children.Add(drawelement); //add
                    }
                }
            }
            else
            {
                //clear surface
                paintSurface.Children.Clear();
            }

            /*
            //clear surface
            paintSurface.Children.Clear();
            //draw surface
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
            */
        }
        
        //prepare redo
        public void PrepareRedo(Invoker invoker)
        {
            
            /*
            //drawn elements for redo
            List<FrameworkElement> PrepareDrawnRedo = new List<FrameworkElement>();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                PrepareDrawnRedo.Add(drawelement); //add
            }
            invoker.undoElementsList.Add(PrepareDrawnRedo);
            //selected elements for redo
            List<FrameworkElement> PrepareSelectedRedo = new List<FrameworkElement>();
            foreach (FrameworkElement selectelement in invoker.selectElements)
            {
                PrepareSelectedRedo.Add(selectelement); //add
            }
            invoker.unselectElementsList.Add(PrepareSelectedRedo);

            */

            /*
            //drawn elements for redo
            List<FrameworkElement> PrepareDrawnRedo = new List<FrameworkElement>();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                PrepareDrawnRedo.Add(drawelement); //add
            }
            invoker.undoElementsList.Add(PrepareDrawnRedo);
            //selected elements for redo
            List<FrameworkElement> PrepareSelectedRedo = new List<FrameworkElement>();
            foreach (FrameworkElement selectelement in invoker.selectElements)
            {
                PrepareSelectedRedo.Add(selectelement); //add
            }
            invoker.unselectElementsList.Add(PrepareSelectedRedo);
            */
        }

        //prepare undo
        public void PrepareUndo(Invoker invoker)
        {

            invoker.drawnElements.Clear();

            if (invoker.undoElementsList.Count() > 0)
            {
                List<FrameworkElement> FetchDrawnUndo = invoker.undoElementsList.Last();
                List<FrameworkElement> PrepareDrawnUndo = new List<FrameworkElement>();
                foreach (FrameworkElement drawelement in FetchDrawnUndo)
                {
                    PrepareDrawnUndo.Add(drawelement); //add
                }
            }

            


            /*
            invoker.drawnElements.Clear();
            List<FrameworkElement> PrepareDrawnUndo = new List<FrameworkElement>();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                PrepareDrawnUndo.Add(drawelement); //add
            }
            invoker.redoElementsList.Add(PrepareDrawnUndo);
            */


            /*
            //drawn elements for undo
            List<FrameworkElement> PrepareDrawnUndo = new List<FrameworkElement>();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                PrepareDrawnUndo.Add(drawelement); //add
            }
            invoker.redoElementsList.Add(PrepareDrawnUndo);
            //selected elements for undo
            List<FrameworkElement> PrepareSelectedUndo = new List<FrameworkElement>();
            foreach (FrameworkElement selectelement in invoker.selectElements)
            {
                PrepareSelectedUndo.Add(selectelement); //add
            }
            invoker.reselectElementsList.Add(PrepareSelectedUndo);
            */
            /*
            //drawn elements for undo
            List<FrameworkElement> PrepareDrawnUndo = new List<FrameworkElement>();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                PrepareDrawnUndo.Add(drawelement); //add
            }
            invoker.redoElementsList.Add(PrepareDrawnUndo);
            //selected elements for undo
            List<FrameworkElement> PrepareSelectedUndo = new List<FrameworkElement>();
            foreach (FrameworkElement selectelement in invoker.selectElements)
            {
                PrepareSelectedUndo.Add(selectelement); //add
            }
            invoker.reselectElementsList.Add(PrepareSelectedUndo);
            */
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
        //selecting for group
        //

        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                if (group.drawnElements.Count() > 0)
                {
                    foreach (FrameworkElement groupelement in group.drawnElements)
                    {
                        if (groupelement.AccessKey == element.AccessKey)
                        {
                            counter++;
                            return counter;
                        }
                    }
                }
                if (group.addedGroups.Count() > 0 && counter == 0)
                {
                    foreach (Group subgroup in group.addedGroups)
                    {
                        counter = CheckInSubGroup(subgroup, invoker, element);
                        if (counter > 0)
                        {
                            return counter;
                        }
                    }
                }
            }
            return counter;
        }

        //check if element in sub group
        public int CheckInSubGroup(Group group, Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            if (group.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement groupelement in group.drawnElements)
                {
                    if (groupelement.AccessKey == element.AccessKey)
                    {
                        counter++;
                        return counter;
                    }
                }
            }
            if (group.addedGroups.Count() > 0 && counter == 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    counter = CheckInSubGroup(subgroup, invoker, element);
                    if (counter > 0)
                    {
                        return counter;
                    }
                }
            }
            return counter;
        }

        //
        //saving
        //
        public async void Saving(Canvas paintSurface, Invoker invoker)
        {
            try
            {
                string lines = "";
                //ungrouped and drawn
                foreach (FrameworkElement child in paintSurface.Children)
                {
                    int elmcheck = CheckInGroup(invoker, child); //see if already in group
                    if (elmcheck == 0)
                    {
                        if (child is Rectangle)
                        {
                            double top = (double)child.GetValue(Canvas.TopProperty);
                            double left = (double)child.GetValue(Canvas.LeftProperty);
                            string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                            lines += str;
                        }
                        else if (child is Ellipse)
                        {
                            double top = (double)child.GetValue(Canvas.TopProperty);
                            double left = (double)child.GetValue(Canvas.LeftProperty);
                            string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                            lines += str;
                        }
                    }
                }
                //grouped and drawn
                foreach (Group group in invoker.drawnGroups)
                {
                    string gstr = group.Display(0,group.depth,group);
                    lines += gstr;
                }
                //create and write to file
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp3data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
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
        public async void Loading(Canvas paintSurface, Invoker invoker)
        {
            //clear previous canvas
            paintSurface.Children.Clear();
            //clear elements
            invoker.drawnElements.Clear();
            invoker.undoElementsList.Clear();
            invoker.redoElementsList.Clear();
            invoker.selectElements.Clear();
            invoker.unselectElementsList.Clear();
            invoker.reselectElementsList.Clear();
            //clear invoker groups
            invoker.drawnGroups.Clear();
            invoker.undoGroupsList.Clear();
            invoker.redoGroupsList.Clear();
            invoker.selectedGroups.Clear();
            invoker.unselectGroupsList.Clear();
            invoker.reselectGroupsList.Clear();
            //clear invoker numbers
            invoker.executer = 0;
            invoker.counter = 0;
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp3data.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(saveFile);
            //load shapes
            string[] readText = Regex.Split(text, "\\n+");
            int i = 0;
            //make groups and shapes
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    invoker.executer++;
                    i++;
                    string notabs = s.Replace("\t", "");
                    string[] line = Regex.Split(notabs, "\\s+");
                    //remake shapes
                    if (line[0] == "ellipse")
                    {
                        Shape shape = new Shape(Convert.ToDouble(line[1]), Convert.ToDouble(line[2]), Convert.ToDouble(line[3]), Convert.ToDouble(line[4]));
                        ICommand place = new MakeEllipses(shape, invoker, paintSurface);
                        invoker.Execute(place);
                    }
                    else if (line[0] == "rectangle")
                    {
                        Shape shape = new Shape(Convert.ToDouble(line[1]), Convert.ToDouble(line[2]), Convert.ToDouble(line[3]), Convert.ToDouble(line[4]));
                        ICommand place = new MakeRectangles(shape, invoker, paintSurface);
                        invoker.Execute(place);
                    }
                    //remake groups
                    else if (line[0] == "group")
                    {
                        FrameworkElement selectedElement = null;
                        Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
                        ICommand place = new MakeGroup(group, paintSurface, invoker);
                        invoker.Execute(place);
                    }   
                }
            }
            int j = 0; //line increment
            int g = 0;//group increment
            //re add elements to groups
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {   
                    string notabs = s.Replace("\t", "");
                    string[] line = Regex.Split(notabs, "\\s+");
                    if (line[0] == "group")
                    {
                        GetGroupElements(readText, j, Convert.ToInt32(line[1]), g, invoker);
                        g++;
                    }
                    j++;
                }
            }
            int maingroup = 0; //main group increment
            int k = 0; //line increment
            //remake subgroups and add elements
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string[] line = Regex.Split(s, "\\s+");
                    int tabcount = s.Length - s.Replace("/", "").Length;

                    //if (s[0] != '\t')
                    if (tabcount <0)
                    { 
                        if (line[0] == "group")
                        {                    
                            GetSubGroups(readText, maingroup, 0, k, k + Convert.ToInt32(line[1]),invoker);
                        }
                    }
                    maingroup++;
                }
                k++;
            }
        }

        //load ellipse
        public void GetEllipse(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");
            //location and size
            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);
            //draw
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
            paintSurface.Children.Add(newEllipse);
            invoker.drawnElements.Add(newEllipse);
        }

        //load rectangle
        public void GetRectangle(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");
            //location and size
            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);
            //draw
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
            paintSurface.Children.Add(newRectangle);
            invoker.drawnElements.Add(newRectangle);
        }

        //re attach element to group
        public void GetGroupElements(string[] readText, int start, int stop, int group, Invoker invoker)
        {
            Group attachgroup = invoker.drawnGroups[group];
            for (int i = start; i < stop; i++)
            {
                FrameworkElement elm = invoker.drawnElements[i];
                attachgroup.drawnElements.Add(elm);
            }
        }

        //re attach subgroups to group
        public void GetSubGroups(string[] readText, int group, int depth, int start, int stop, Invoker invoker)
        {
            Group maingroup = invoker.drawnGroups[group];
            while(start < stop)
            {
                string s = readText[start];
                string notabs = s.Replace("\t", "");
                string tab = "\t";
                int tablength = tab.Length;
                int notablength = notabs.Length;
                int slength = s.Length;
                int subdepth = (slength - notablength) / tablength;

                if (subdepth == (depth + 1))
                {
                    string[] line = Regex.Split(notabs, "\\s+");
                    if (line[0] == "group")
                    {
                        group++;
                        Group subgroup = invoker.drawnGroups[group];
                        maingroup.addedGroups.Add(subgroup);
                        invoker.drawnGroups.RemoveAt(group);

                        GetSubGroups(readText, group, depth + 1, start, start + Convert.ToInt32(line[1]), invoker);
                    }
                    start++;
                }
            }
        }
    }
}