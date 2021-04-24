using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace tekenprogramma
{

    //location class
    public class Location
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Location(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Location()
        {

        }
    }

    //class group
    public class Group : Baseshape
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public string type;
        public int depth;
        public int id;
        //public List<Baseshape> groupitems;
        public Canvas selectedCanvas;

        public List<Baseshape> children = new List<Baseshape>();

        //public List<FrameworkElement> selectedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> unselectedElements = new List<FrameworkElement>();

        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        public List<FrameworkElement> movedElements = new List<FrameworkElement>();

        //public List<Canvas> groupedCanvases = new List<Canvas>();
        //public List<Canvas> ungroupedCanvases = new List<Canvas>();

        public List<Group> addedGroups = new List<Group>();
        //public List<Group> removedGroups = new List<Group>();

        public Invoker invoker;
        public FrameworkElement element;
        public Canvas lastCanvas;

        public Group(double height, double width, double x, double y, string type, int depth, int id, Canvas selectedCanvas, Invoker invoker, FrameworkElement element) : base(height, width, x, y)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.depth = depth;
            this.id = id;
            this.selectedCanvas = selectedCanvas;
            this.invoker = invoker;
            this.element = element;
        }

        //make new group
        public void MakeGroup(Group group, Canvas selectedCanvas, Invoker invoker)
        {
            if (invoker.selectElementsList.Count() >0)
            {
                //get selected elements
                foreach (FrameworkElement elm in invoker.selectElementsList)
                {
                    //check if selected is not already grouped element
                    int elmcheck = CheckInGroup(invoker, elm);
                    if (elmcheck ==0)
                    {
                        this.drawnElements.Add(elm);
                    }
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                }
            }
            if (invoker.selectedGroups.Count() > 0)
            {
                //get selected groups
                foreach (Group selectedgroup in invoker.selectedGroups)
                {
                    this.addedGroups.Add(selectedgroup);
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                }
            }
            invoker.drawnGroups.Add(this);
        }

        //un group
        public void UnGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.drawnGroups.Last();
            if (lastgroup.drawnElements.Count() >0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //add selected
                    invoker.selectElementsList.Add(elm);
                    invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1);
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //add selected
                    invoker.selectedGroups.Add(selectedgroup);
                    invoker.unselectedGroups.RemoveAt(invoker.unselectedGroups.Count() - 1);
                }
            }
            invoker.removedGroups.Add(lastgroup);
            invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
        }

        //re group
        public void ReGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.removedGroups.Last();

            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                }
            }
            invoker.drawnGroups.Add(lastgroup);
            invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
        }

        //moving
        public void Moving(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            FrameworkElement selectedelement = invoker.selectElementsList.Last();
            Group selectedgroup = invoker.selectedGroups.Last();
            //calculate difference in location
            double leftOffset = selectedelement.ActualOffset.X - e.GetCurrentPoint(paintSurface).Position.X;
            double topOffset = selectedelement.ActualOffset.Y - e.GetCurrentPoint(paintSurface).Position.Y;

            foreach (FrameworkElement movedelement in selectedgroup.drawnElements)
            {
                Location location = new Location();
                location.x = e.GetCurrentPoint(paintSurface).Position.X;
                location.y = e.GetCurrentPoint(paintSurface).Position.Y;
                location.width = movedelement.Width;
                location.height = movedelement.Height;

                Moving(movedelement, invoker, paintSurface, location);
            }

            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected element
            invoker.unselectElementsList.Add(selectedelement);
            invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            Repaint(invoker,paintSurface); //repaint
        }

        //undo moving
        public void UndoMoving(Invoker invoker)
        {
            Group selectedgroup = invoker.movedGroups.Last();
            invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() -1);
            invoker.removedGroups.Add(selectedgroup);
        }

        //redo moving
        public void RedoMoving(Invoker invoker)
        {
            Group selectedgroup = invoker.removedGroups.Last();
            invoker.removedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            invoker.movedGroups.Add(selectedgroup);
        }

        //resize
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            FrameworkElement selectedelement = invoker.selectElementsList.Last();
            Group selectedgroup = invoker.selectedGroups.Last();

            //double oldWidth = selectedelement.Width();
            //double oldHeight = selectedelement.Height();


            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected element
            invoker.unselectElementsList.Add(selectedelement);
            invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            Repaint(invoker, paintSurface);//repaint
        }

        //undo resize
        public void UndoResize(Invoker invoker)
        {
            Group selectedgroup = invoker.movedGroups.Last();
            invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            invoker.removedGroups.Add(selectedgroup);
        }

        //redo resize
        public void RedoResize(Invoker invoker)
        {
            Group selectedgroup = invoker.removedGroups.Last();
            invoker.removedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            invoker.movedGroups.Add(selectedgroup);
        }

        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                if (group.drawnElements.Count() >0)
                {
                    foreach (FrameworkElement groupelement in group.drawnElements)
                    {
                        if (groupelement.AccessKey == element.AccessKey)
                        {
                            counter++;
                        }
                    }
                }
            }
            return counter;
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

        //moving element in group
        public void Moving(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
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
        }






        public override string Display(int depth)
        {
            //Display group.
            string str = "";
            //Add group.
            int i = 0;
            while (i < depth)
            {
                str += "\t";
            }
            str = str + "group" + id + "\n";

            //Recursively display child nodes.
            depth = depth + 1;
            foreach (Baseshape child in children)
            {
                //child.Display(depth + 1);

                if (child is Rectangle)
                {
                    int j = 0;
                    while (j < depth)
                    {
                        str += "\t";
                    }
                    str = str + "rectangle " + child.x + " " + child.y + " " + child.width + " " + child.height + "\n";
                }
                else if (child is Ellipse)
                {

                    int j = 0;
                    while (j < depth)
                    {
                        str += "\t";
                    }
                    str = str + "ellipse " + child.x + " " + child.y + " " + child.width + " " + child.height + "\n";
                }
                else
                {
                    child.Display(depth + 1);
                }

            }
            return str;

        }





        //select group
        public override void Select(PointerRoutedEventArgs e, Canvas selectedCanvas)
        {
            foreach (Baseshape baseshape in this.children)
            {
                Shape shape = new Shape(baseshape.x,baseshape.y,baseshape.width,baseshape.height);
                
                if (this.selected)
                {
                    if (baseshape.GetIfSelected(e.GetCurrentPoint(selectedCanvas).Position.X, e.GetCurrentPoint(selectedCanvas).Position.Y))
                    {
                        ICommand select = new Select(shape, e, this.invoker);
                        this.invoker.Execute(select);
                    }
                    else
                    {
                        ICommand deselect = new Deselect(shape, e,this.invoker);
                        this.invoker.Execute(deselect);
                    }

                    if (baseshape.GetHandleIfSelected(e.GetCurrentPoint(selectedCanvas).Position.X, e.GetCurrentPoint(selectedCanvas).Position.Y))
                    {
                        Location location = new Location(e.GetCurrentPoint(selectedCanvas).Position.X, e.GetCurrentPoint(selectedCanvas).Position.Y, 0,0);
                        ICommand resize = new Resize(shape, this.invoker, e, location, selectedCanvas, this.element);
                        this.invoker.Execute(resize);
                    }
                }
            }
            this.selected = true;
        }

        //deselect group
        public override void Deselect(PointerRoutedEventArgs e)
        {
            foreach (Baseshape baseshape in this.children)
            {
                Shape shape = new Shape(baseshape.x, baseshape.y, baseshape.width, baseshape.height);
                if (baseshape.selected)
                {
                    ICommand deselect = new Deselect(shape, e, this.invoker);
                    this.invoker.Execute(deselect);
                }
            }
            this.selected = false;
        }

        //if selected
        public override bool GetIfSelected(double x, double y)
        {
            foreach (Baseshape shape in this.children)
            {
                if (shape.GetIfSelected(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool GetHandleIfSelected(double x, double y)
        {
            bool s = false;
            foreach (Baseshape shape in this.children)
            {
                if (shape.GetHandleIfSelected(x, y))
                {
                    s = true;
                    return true;
                }
            }

            if (s == false)
            {
                for (double i = this.x + this.width - 6; i < this.x + this.width + 6; i++)
                {
                    for (double j = this.y + this.height - 6; j < this.y + this.height + 6; j++)
                    {
                        if (i == x)
                        {
                            if (j == y)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        //Location location = this.undoList.Last();
        //this.redoList.Add(location);
        //this.x = location.x;
        //this.y = location.y;
        //this.width = location.width;
        //this.height = location.height;

        //if (this.redoList.Count() > 0)
        //{
        //    Location location = this.redoList.Last();
        //    this.undoList.Add(location);
        //    this.x = location.x;
        //    this.y = location.y;
        //    this.width = location.width;
        //    this.height = location.height;
        //}

        //bool s = false;
        //FrameworkElement elm = e.OriginalSource as FrameworkElement;
        //foreach (Baseshape shape in this.children)
        //{
        //    if (shape.selected)
        //    {
        //        s = true;
        //        //Location childLocation = new Location(shape.x, shape.y, elm.Width - (shape.start.x - e.GetCurrentPoint(selectedCanvas).Position.X), elm.Height - (shape.start.y - e.GetCurrentPoint(selectedCanvas).Position.Y));
        //        //shape.Resize(childLocation);
        //    }
        //}

        //if (s == false)
        //{
        //    //float percentageWidth = (float)location.width / (float)this.start.width;
        //    //float percentageHeight = (float)location.height / (float)this.start.height;

        //    foreach (Baseshape shape in this.children)
        //    {
        //        //float diffX = ((float)shape.start.x - (float)this.x) * percentageWidth;
        //        //float diffY = ((float)shape.start.y - (float)this.y) * percentageHeight;

        //        //Location childLocation = new Location();
        //        //childLocation.x = this.start.x + Math.Round(diffX);
        //        //childLocation.y = this.start.y + Math.Round(diffY);
        //        //childLocation.width = Math.Round((float)shape.start.width * percentageWidth);
        //        //childLocation.height = Math.Round((float)shape.start.height * percentageHeight);
        //        //shape.Resize(childLocation);
        //    }
        //}





        //public void Add(Baseshape newgroup)
        //{
        //    children.Add(newgroup);
        //}

        //public void Remove(Baseshape newgroup)
        //{
        //    children.Remove(newgroup);
        //}

        //public List<Baseshape> GetGroup()
        //{
        //    return children;
        //}

        //Location location = this.undoList.Last();
        //this.redoList.Add(location);
        //this.x = location.x;
        //this.y = location.y;
        //this.width = location.width;
        //this.height = location.height;

        //if (this.redoList.Count() > 0)
        //{
        //    Location location = this.redoList.Last();
        //    this.undoList.Add(location);
        //    this.x = location.x;
        //    this.y = location.y;
        //    this.width = location.width;
        //    this.height = location.height;
        //}

        //bool s = false;
        //foreach (Baseshape shape in this.children)
        //{
        //    if (shape.selected)
        //    {
        //        //Location childLocation = new Location(e.GetCurrentPoint(selectedCanvas).Position.X, e.GetCurrentPoint(selectedCanvas).Position.Y, shape.width, shape.height);
        //        //shape.Moving(childLocation);
        //        //s = true;
        //    }
        //}

        //if (s == false)
        //{
        //    foreach (Baseshape shape in this.children)
        //    {
        //        double dx = e.GetCurrentPoint(selectedCanvas).Position.X - this.start.x + shape.start.x;
        //        double dy = e.GetCurrentPoint(selectedCanvas).Position.Y - this.start.y + shape.start.y;

        //        Location childLocation = new Location();
        //        childLocation.x = dx;
        //        childLocation.y = dy;
        //        childLocation.width = shape.width;
        //        childLocation.height = shape.height;

        //        //shape.Moving(childLocation);

        //    }

        //}

        //public void UnGroup(Group group, Canvas selectedCanvas, Invoker invoker, FrameworkElement element)
        ////size and place
        //grid.Height = highestHeight;
        //grid.Width = highestLeft;
        //Canvas.SetTop(grid, lowestTop);
        //Canvas.SetLeft(grid, lowestLeft);
        ////add it to selected canvas
        //selectedCanvas.Children.Add(grid);
        ////add to elements
        //invoker.drawnElements.Add(grid);
        //invoker.canvases.Add(grid);
        //}


        //Canvas grid = new Canvas();
        //SolidColorBrush groupbrush = new SolidColorBrush(); //brush
        //groupbrush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
        //groupbrush.Opacity = 0.5; //half opacity
        //grid.Background = groupbrush;
        //size calculation
        //double lowestLeft = 1000;
        //double lowestTop = 1000;
        //double highestLeft = 0;
        //double highestTop = 0;
        //double highestWidth = 0;
        //double highestHeight = 0;

        //lastCanvas = invoker.removedcanvases.Last();
        //foreach (FrameworkElement elm in lastCanvas.Children)
        //{
        //    KeyNumber(elm, invoker);
        //}

        //foreach (Canvas canv in this.ungroupedCanvases)
        //{
        //    this.groupedCanvases.Add(canv);
        //}


        //lastCanvas = invoker.canvases.Last();

        //foreach (FrameworkElement elm in lastCanvas.Children)
        //{
        //    selectedCanvas.Children.Add(elm);
        //    invoker.removedElements.RemoveAt(invoker.removedElements.Count() -1);
        //    invoker.drawnElements.Add(elm);
        //}

        //foreach (Canvas canv in this.groupedCanvases)
        //{
        //    this.ungroupedCanvases.Add(canv);
        //}

        //double top = (double)elm.GetValue(Canvas.TopProperty);
        //double left = (double)elm.GetValue(Canvas.LeftProperty);
        //double width = elm.Width;
        //double height = elm.Height;
        //Group selectedgroup = new Group(left, top, width, height, elm.Name, 0, 0, selectedCanvas, invoker, elm);
        //group.Add(selectedgroup);
        ////re add elements
        //if (elm.Name == "Ellipse")
        //{
        //    Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
        //    newEllipse.Width = width;
        //    newEllipse.Height = height;
        //    SolidColorBrush elmbrush = new SolidColorBrush();//brush
        //    elmbrush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
        //    newEllipse.Fill = elmbrush;//fill color
        //    newEllipse.Name = "Ellipse";//attach name
        //    Canvas.SetLeft(newEllipse, left);//set left position
        //    Canvas.SetTop(newEllipse, top);//set top position
        //    grid.Children.Add(newEllipse);
        //}
        //else if (elm.Name == "Rectangle")
        //{
        //    Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
        //    newRectangle.Width = width; //set width
        //    newRectangle.Height = height; //set height     
        //    SolidColorBrush elmbrush = new SolidColorBrush(); //brush
        //    elmbrush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
        //    newRectangle.Fill = elmbrush; //fill color
        //    newRectangle.Name = "Rectangle"; //attach name
        //    Canvas.SetLeft(newRectangle, left); //set left position
        //    Canvas.SetTop(newRectangle, top); //set top position 
        //    grid.Children.Add(newRectangle);
        //}
        ////group size and place calculation
        //if (elm.ActualOffset.X < lowestLeft)
        //{
        //    lowestLeft = left;
        //}
        //if (elm.ActualOffset.Y < lowestTop)
        //{
        //    lowestTop = top;
        //}
        //if (elm.ActualOffset.X > highestLeft)
        //{
        //    highestLeft = left;
        //    highestWidth = (left + width) - lowestLeft;
        //}
        //if (elm.ActualOffset.Y > highestTop)
        //{
        //    highestTop = top;
        //    highestHeight = (top + height) - lowestTop;
        //}
        ////remove selected
        //selectedCanvas.Children.Remove(elm);
        //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
        //KeyNumber(elm,invoker);



        //getselected canvases and elements

        //foreach (Canvas canv in invoker.selectedCanvases)
        //{
        //elements at canvas
        //get selected elements
        //foreach (FrameworkElement elm in invoker.selectElementsList)
        //{
        //    double top = (double)elm.GetValue(Canvas.TopProperty);
        //    double left = (double)elm.GetValue(Canvas.LeftProperty);
        //    double width = elm.Width;
        //    double height = elm.Height;
        //    //group size and place calculation
        //    if (elm.ActualOffset.X < lowestLeft)
        //    {
        //        lowestLeft = left;
        //    }
        //    if (elm.ActualOffset.Y < lowestTop)
        //    {
        //        lowestTop = top;
        //    }
        //    if (elm.ActualOffset.X > highestLeft)
        //    {
        //        highestLeft = left;
        //        highestWidth = (left + width) - lowestLeft;
        //    }
        //    if (elm.ActualOffset.Y > highestTop)
        //    {
        //        highestTop = top;
        //        highestHeight = (top + height) - lowestTop;
        //    }
        //}
        //add it to selected canvas
        //selectedCanvas.Children.Add(canv);
        //add to elements
        //invoker.drawnElements.Add(canv);
        //invoker.canvases.Add(canv);
        //grouped
        //groupedCanvases.Add(canv);

        //find id        
        //public Group FindID(int id)
        //{
        //    for (int c = 0; c < groupitems.Count; c++)
        //    {
        //        if (groupitems[c].id == id)
        //        {
        //            return groupitems[c];
        //        }
        //        else
        //        {
        //            Group tmp = FindID(id);
        //            if (tmp.id == id)
        //            {
        //                return tmp;
        //            }
        //            //else
        //            //{
        //            //    return c;
        //            //}
        //        }
        //    }
        //} 

        //public double getx()
        //{
        //    double x = this.children.X;
        //    foreach (Baseshape shape in this.children)
        //    {
        //        if (shape.drawed)
        //        {
        //            if (shape.x < x)
        //            {
        //                x = shape.x;
        //            }
        //        }
        //    }
        //    return x;
        //}

        //public double gety()
        //{
        //    double y = this.children.get(0).y;
        //    foreach (Baseshape shape in this.children)
        //    {
        //        if (shape.drawed)
        //        {
        //            if (shape.y < y)
        //            {
        //                y = shape.y;
        //            }
        //        }
        //    }
        //    return y;
        //}

    }
}
