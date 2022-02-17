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
        public int depth; //group depth
        public int id; //group id

        //making
        //composite sub groups
        public List<Group> addedGroups = new List<Group>();
        //composite for creation and selection undo redo
        public List<FrameworkElement> selectedElementsComposite = new List<FrameworkElement>();
        public List<Group> selectedGroupsComposite = new List<Group>();

        //moving
        //undo redo group elements for moving
        public List<List<FrameworkElement>> undoElementsList = new List<List<FrameworkElement>>();
        public List<List<FrameworkElement>> redoElementsList = new List<List<FrameworkElement>>();
        //drawn elements for moving
        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        //selected elements for moving
        public List<FrameworkElement> newSelected = new List<FrameworkElement>();
        
        //variables
        public Invoker invoker;
        public FrameworkElement element;
        public Canvas selectedCanvas;

        public Group(double height, double width, double x, double y, string type, int depth, int id, Canvas selectedCanvas, Invoker invoker, FrameworkElement element) : base(x, y, width, height)
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

        //
        //group creation
        //

        //make new group
        public void MakeGroup(Group group, Canvas selectedCanvas, Invoker invoker)
        {
            //depth variables
            int newdepth = 0;
            //selected elements
            if(invoker.unselectElementsList.Count() > 0)
            {
                List<FrameworkElement> selectedElements = invoker.unselectElementsList.Last();
                if (selectedElements.Count() > 0)
                {
                    List<FrameworkElement> groupElements = new List<FrameworkElement>();
                    foreach (FrameworkElement elm in selectedElements)
                    {
                        elm.Opacity = 0.9;
                        //check if selected is not already grouped element
                        int elmcheck = CheckInGroup(invoker, elm);
                        if (elmcheck == 0)
                        {
                            //add element
                            groupElements.Add(elm);
                            newdepth = 1;
                        }
                    }
                    //add elements to group    
                    this.undoElementsList.Add(groupElements);
                    //shuffle selected elements
                    for (int i = selectedElements.Count(); i > 0; i--)
                    {
                        List<FrameworkElement> lastSelectedElements = invoker.unselectElementsList.Last();
                        invoker.reselectElementsList.Add(lastSelectedElements); //2b+
                        invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
                    }
                }
            }
            //new drawn groups
            List<Group> leftoverGroups = new List<Group>();
            //check drawn group count
            if (invoker.undoGroupsList.Count() > 0)
            {
                List<Group> lastGroups = invoker.undoGroupsList.Last();
                //check drawn group count
                if (lastGroups.Count() > 0)
                {
                    //check selected group count
                    if (invoker.unselectGroupsList.Count() > 0)
                    {
                        List<Group> selectedGroups = invoker.unselectGroupsList.Last();
                        //check selected group count
                        if (selectedGroups.Count() > 0)
                        {
                            //selected groups removal and shuffling
                            for (int s = 0; s < lastGroups.Count(); s++)
                            {
                                bool checkSelected = false;
                                foreach (Group selectedgroup in selectedGroups)
                                {
                                    //add subgroups to this new group
                                    if (selectedgroup.id == lastGroups[s].id)
                                    {
                                        this.addedGroups.Add(selectedgroup);
                                        checkSelected = true;
                                    }                                    
                                }
                                //add only non selected groups
                                if (checkSelected == false)
                                {
                                    leftoverGroups.Add(lastGroups[s]);
                                }
                            }
                            //calculate depth from selected groups
                            foreach (Group selectedgroup in selectedGroups)
                            {
                                if (selectedgroup.depth > newdepth)
                                {
                                    newdepth = 1 + selectedgroup.depth;
                                }
                            }
                            //shuffle selected groups
                            for (int g = selectedGroups.Count(); g > 0; g--)
                            {
                                List<Group> lastSelectedGroups = invoker.unselectGroupsList.Last();
                                invoker.reselectGroupsList.Add(lastSelectedGroups); //2b+
                                invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1); //2a-
                            }
                        }
                    }
                    //else no other selected, then add all left over groups
                    else
                    {
                        foreach (Group lastGroup in lastGroups)
                        {
                            leftoverGroups.Add(lastGroup);
                        }
                    }
                }
            }
            //group depth
            this.depth = newdepth;
            //group id
            this.id = invoker.executer;
            //add group
            leftoverGroups.Add(this);
            invoker.undoGroupsList.Add(leftoverGroups);
        }

        //un group
        public void UnGroup(Canvas selectedCanvas, Invoker invoker)
        {
            //shuffle selected elements
            if (invoker.reselectElementsList.Count() > 0)
            {              
                for (int i = invoker.reselectElementsList.Count(); i > 0; i--)
                {
                    List<FrameworkElement> lastSelectedElements = invoker.reselectElementsList.Last(); //err  
                    invoker.unselectElementsList.Add(lastSelectedElements); //2b+
                    invoker.reselectElementsList.RemoveAt(invoker.reselectElementsList.Count() - 1); //2a-
                }

            }
            //shuffle selected groups
            if (invoker.reselectGroupsList.Count() > 0)
            {
                for (int g = invoker.reselectGroupsList.Count(); g > 0; g--)
                {
                    List<Group> lastSelectedGroups = invoker.reselectGroupsList.Last();
                    invoker.unselectGroupsList.Add(lastSelectedGroups); //2b+
                    invoker.reselectGroupsList.RemoveAt(invoker.reselectGroupsList.Count() - 1); //2a-
                }
            }
            //shuffle drawn groups
            List<Group> lastDrawnGroups = invoker.undoGroupsList.Last();
            invoker.redoGroupsList.Add(lastDrawnGroups); //2b+
            invoker.undoGroupsList.RemoveAt(invoker.undoGroupsList.Count() - 1); //2a-
        }

        //re group
        public void ReGroup(Canvas selectedCanvas, Invoker invoker)
        {
            //shuffle selected elements
            if (invoker.unselectElementsList.Count() > 0)
            {
                for (int i = invoker.unselectElementsList.Count(); i > 0; i--)
                {
                    List<FrameworkElement> lastSelectedElements = invoker.unselectElementsList.Last();
                    invoker.reselectElementsList.Add(lastSelectedElements); //2b+
                    invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
                }

            }
            //shuffle selected groups
            if (invoker.unselectGroupsList.Count() > 0)
            {
                for (int g = invoker.unselectGroupsList.Count(); g > 0; g--)
                {
                    List<Group> lastSelectedGroups = invoker.unselectGroupsList.Last();
                    invoker.reselectGroupsList.Add(lastSelectedGroups); //2b+
                    invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1); //2a-
                }
            }
            //shuffle drawn groups
            List<Group> lastDrawnGroups = invoker.redoGroupsList.Last();
            invoker.undoGroupsList.Add(lastDrawnGroups); //2b+
            invoker.redoGroupsList.RemoveAt(invoker.redoGroupsList.Count() - 1); //2a-
        }

        //
        //moving
        //

        //moving
        public void Moving(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            //fetch moving
            List<Group> lastSelectedGroups = invoker.unselectGroupsList.Last();
            Group selectedgroup = lastSelectedGroups.Last();
            //shuffle group
            invoker.reselectGroupsList.Add(lastSelectedGroups); //2b+
            invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1); //2a-
            //shuffle selected element
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
            //calculate difference in location
            double leftOffset = Convert.ToDouble(selectedelement.ActualOffset.X) - e.GetCurrentPoint(paintSurface).Position.X;
            double topOffset = Convert.ToDouble(selectedelement.ActualOffset.Y) - e.GetCurrentPoint(paintSurface).Position.Y;
            //fetch
            foreach (FrameworkElement drawnElement in invoker.undoElementsList.Last())
            {
                int check = CheckInGroup(invoker, drawnElement);
                //if not selected
                if (check == 0)
                {
                    this.drawnElements.Add(drawnElement); //add
                }
            }
            //elements in group
            if (selectedgroup.undoElementsList.Count() > 0)
            {
                //prepare move
                List<FrameworkElement> PrepareMove = new List<FrameworkElement>();
                //move elements
                foreach (FrameworkElement movedElement in selectedgroup.undoElementsList.Last())
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X) - leftOffset;
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y) - topOffset;
                    location.width = movedElement.Width;
                    location.height = movedElement.Height;
                    invoker.executer++;//acceskey add
                    FrameworkElement madeElement = MovingElement(movedElement, invoker, paintSurface, location);
                    PrepareMove.Add(madeElement);
                    this.drawnElements.Add(madeElement);
                }
                //add moved to group
                selectedgroup.undoElementsList.Add(PrepareMove);
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() >0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubMoving(invoker, subgroup, leftOffset, topOffset, paintSurface); //subgroups
                }            
            }
            //set
            invoker.undoElementsList.Add(this.drawnElements);
            //repaint
            Repaint(invoker,paintSurface);
        }

        //recursively moving subgroups
        private void SubMoving(Invoker invoker, Group selectedgroup, double leftOffset, double topOffset, Canvas paintSurface)
        {
            //elements in group
            if (selectedgroup.undoElementsList.Count() > 0)
            {
                //prepare move
                List<FrameworkElement> PrepareMove = new List<FrameworkElement>();
                //move elements
                foreach (FrameworkElement movedElement in selectedgroup.undoElementsList.Last())
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X) - leftOffset;
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y) - topOffset;
                    location.width = movedElement.Width;
                    location.height = movedElement.Height;
                    invoker.executer++;//acceskey add
                    FrameworkElement madeElement = MovingElement(movedElement, invoker, paintSurface, location);
                    PrepareMove.Add(madeElement);
                    this.drawnElements.Add(madeElement);
                }
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubMoving(invoker, subgroup, leftOffset, topOffset, paintSurface); //subgroups
                }
            }
        }

        //moving element in group
        private FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
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
                returnelement = newRectangle;
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
                returnelement = newEllipse;
            }
            return returnelement;
        }

        //
        //undo redo move resize
        //

        //undo moving or resizing
        public void Undo(Invoker invoker, Canvas paintSurface)
        {
            //fetch for groups
            List<Group> lastSelectedGroups = invoker.reselectGroupsList.Last();
            Group selectedgroup = lastSelectedGroups.Last();
            //shuffle for groups
            invoker.unselectGroupsList.Add(lastSelectedGroups); //2b+
            invoker.reselectGroupsList.RemoveAt(invoker.reselectGroupsList.Count() - 1); //2a-
            //shuffle unselected element
            newSelected = invoker.reselectElementsList.Last();
            invoker.unselectElementsList.Add(newSelected); //2a+
            invoker.reselectElementsList.RemoveAt(invoker.reselectElementsList.Count() - 1); //2b-
            //fetch for repaint
            List<FrameworkElement> lastRedo = invoker.undoElementsList.Last();
            //shuffle for repaint
            invoker.redoElementsList.Add(lastRedo);
            invoker.undoElementsList.RemoveAt(invoker.undoElementsList.Count - 1);
            //elements in group
            if (selectedgroup.undoElementsList.Count() > 0)
            {
                //shuffle elements in group
                List<FrameworkElement> lastElements = selectedgroup.undoElementsList.Last();
                selectedgroup.redoElementsList.Add(lastElements); //2a+
                selectedgroup.undoElementsList.RemoveAt(selectedgroup.undoElementsList.Count() - 1); //2b-
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
            //repaint surface
            Repaint(invoker, paintSurface);          
        }

        private void SubUndo(Group selectedgroup, Invoker invoker)
        {
            //elements in group
            if (selectedgroup.undoElementsList.Count() > 0)
            {
                //shuffle elements in group
                List<FrameworkElement> lastElements = selectedgroup.undoElementsList.Last();
                selectedgroup.redoElementsList.Add(lastElements); //2a+
                selectedgroup.undoElementsList.RemoveAt(selectedgroup.undoElementsList.Count() - 1); //2b-
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
        }

        //redo moving or resizing
        public void Redo(Invoker invoker, Canvas paintSurface)
        {
            //fetch for groups
            List<Group> lastSelectedGroups = invoker.unselectGroupsList.Last(); //err
            Group selectedgroup = lastSelectedGroups.Last();
            //shuffle for groups
            invoker.reselectGroupsList.Add(lastSelectedGroups); //2b+
            invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1); //2a-
            //shuffle selected element
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
            //fetch for repaint
            List<FrameworkElement> lastUndo = invoker.redoElementsList.Last();
            //shuffle for repaint
            invoker.undoElementsList.Add(lastUndo);
            invoker.redoElementsList.RemoveAt(invoker.redoElementsList.Count - 1);
            //elements in group
            if (selectedgroup.redoElementsList.Count() > 0)
            {
                //shuffle elements in group
                List<FrameworkElement> lastElements = selectedgroup.redoElementsList.Last();
                selectedgroup.undoElementsList.Add(lastElements); //2a+
                selectedgroup.redoElementsList.RemoveAt(selectedgroup.redoElementsList.Count() - 1); //2b-
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
            //repaint surface
            Repaint(invoker, paintSurface);
        }

        private void SubRedo(Group selectedgroup,Invoker invoker)
        {
            //elements in group
            if (selectedgroup.redoElementsList.Count() > 0)
            {
                //shuffle elements in group
                List<FrameworkElement> lastElements = selectedgroup.redoElementsList.Last();
                selectedgroup.undoElementsList.Add(lastElements); //2a+
                selectedgroup.redoElementsList.RemoveAt(selectedgroup.redoElementsList.Count() - 1); //2b-
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
        }

        //
        //resizing
        //

        //resize
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            //fetch resizing
            List<Group> lastSelectedGroups = invoker.unselectGroupsList.Last();
            Group selectedgroup = lastSelectedGroups.Last();
            //shuffle
            invoker.reselectGroupsList.Add(lastSelectedGroups); //2b+
            invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1); //2a-
            //calculate difference in size
            double newWidth = ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.X, Convert.ToDouble(selectedelement.ActualOffset.X));
            double newHeight = ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.Y, Convert.ToDouble(selectedelement.ActualOffset.Y));
            double widthOffset = selectedelement.Width - newWidth;
            double heightOffset = selectedelement.Height - newHeight;
            //fetch
            foreach (FrameworkElement drawnElement in invoker.undoElementsList.Last())
            {
                int check = CheckInGroup(invoker, drawnElement);
                //if not selected
                if (check == 0)
                {
                    this.drawnElements.Add(drawnElement); //add
                }
            }
            //elements in group
            if (selectedgroup.undoElementsList.Count() > 0)
            {
                //prepare move
                List<FrameworkElement> PrepareResize = new List<FrameworkElement>();
                //move elements
                foreach (FrameworkElement movedElement in selectedgroup.undoElementsList.Last())
                {                   
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X);
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y);
                    location.width = Convert.ToDouble(movedElement.Width) - widthOffset;
                    location.height = Convert.ToDouble(movedElement.Height) - heightOffset;
                    invoker.executer++; //acceskey add
                    FrameworkElement madeElement = ResizingElement(movedElement, invoker, paintSurface, location);
                    PrepareResize.Add(madeElement);
                    this.drawnElements.Add(madeElement);
                }
                //add moved to group
                selectedgroup.undoElementsList.Add(PrepareResize);
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    subgroup.SubResize(invoker, selectedgroup, widthOffset, heightOffset, paintSurface);
                }
            }
            //set
            invoker.undoElementsList.Add(this.drawnElements);
            //repaint
            Repaint(invoker, paintSurface);           
        }

        //recursively resize subgroups
        private void SubResize(Invoker invoker, Group selectedgroup, double widthOffset, double heightOffset, Canvas paintSurface)
        {
            //elements in group
            if (selectedgroup.undoElementsList.Count() > 0)
            {
                //prepare move
                List<FrameworkElement> PrepareResize = new List<FrameworkElement>();
                //move elements
                foreach (FrameworkElement movedElement in selectedgroup.undoElementsList.Last())
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X);
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y);
                    location.width = Convert.ToDouble(movedElement.Width) - widthOffset;
                    location.height = Convert.ToDouble(movedElement.Height) - heightOffset;
                    invoker.executer++; //acceskey add
                    FrameworkElement madeElement = ResizingElement(movedElement, invoker, paintSurface, location);
                    PrepareResize.Add(madeElement);
                    this.drawnElements.Add(madeElement);
                }
                //add moved to group
                selectedgroup.undoElementsList.Add(PrepareResize);
            }
            //sub groups
            if (selectedgroup.addedGroups.Count() >0)
            {
                selectedgroup.SubResize(invoker, selectedgroup, widthOffset, heightOffset, paintSurface);
            }
        }

        //resizing element in group
        private FrameworkElement ResizingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;           
            //create at new size
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
                Canvas.SetLeft(newRectangle, location.x);
                Canvas.SetTop(newRectangle, location.y);
                returnelement = newRectangle;
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width; //set width
                newEllipse.Height = location.height; //set height 
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                returnelement = newEllipse;
            }
            return returnelement;
        }

        //
        //group selection
        //

        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;

            if (invoker.undoGroupsList.Count() > 0)
            {
                List<Group> checkList = invoker.undoGroupsList.Last();
                if (checkList.Count() > 0)
                {
                    foreach (Group group in checkList)
                    {
                        if (group.undoElementsList.Count() > 0)
                        {
                            foreach (FrameworkElement groupelement in group.undoElementsList.Last())
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
                }
            }
            return counter;
        }

        //check if element in sub group
        public int CheckInSubGroup(Group group, Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            if (group.undoElementsList.Count() > 0)
            {
                foreach (FrameworkElement groupelement in group.undoElementsList.Last())
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

        //see if element is in group and select the group
        public void SelectInGroup(FrameworkElement selectedElement, Invoker invoker)
        {          
            string key = selectedElement.AccessKey;
            List<Group> previousSelected = new List<Group>();
            if (invoker.unselectGroupsList.Count() >0)
            {
                previousSelected = invoker.unselectGroupsList.Last();
            }
            if (invoker.undoGroupsList.Count() >0)
            {
                foreach (Group group in invoker.undoGroupsList.Last())
                {
                    if (group.undoElementsList.Count() >0)
                    {
                        foreach (FrameworkElement drawn in group.undoElementsList.Last())
                        {
                            if (drawn.AccessKey == key)
                            {
                                previousSelected.Add(group);
                                invoker.unselectGroupsList.Add(previousSelected);
                            }
                        }
                    }
                    SelectInGroupHandle(previousSelected, invoker, key, group, group); //subgroup recursive
                }
            }             
        }

        //recursively see if element is in subgroup and select the group
        private void SelectInGroupHandle(List<Group> previousSelected, Invoker invoker, string key, Group group, Group checkgroup)
        {
            if (checkgroup.addedGroups.Count() >0)
            {
                foreach (Group subgroup in checkgroup.addedGroups)
                {
                    if (subgroup.undoElementsList.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in subgroup.undoElementsList.Last())
                        {
                            if (drawn.AccessKey == key)
                            {
                                previousSelected.Add(group);
                                invoker.unselectGroupsList.Add(previousSelected);
                            }
                        }
                    }
                    if (subgroup.addedGroups.Count() > 0)
                    {
                        subgroup.SelectInGroupHandle(previousSelected, invoker, key, group, subgroup); //subgroup recursive
                    }
                }
            }           
        }

        //set group unselected
        public void UnselectGroup(FrameworkElement selectedElement, Invoker invoker)
        {

            string key = selectedElement.AccessKey;
            if (invoker.undoGroupsList.Count() > 0)
            {
                foreach (Group group in invoker.undoGroupsList.Last())
                {
                    if (group.undoElementsList.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.undoElementsList.Last())
                        {
                            if (drawn.AccessKey == key)
                            {
                                if (invoker.unselectGroupsList.Count() > 0)
                                {
                                    //fetch
                                    List<Group> lastUndo = invoker.unselectGroupsList.Last();
                                    //shuffle
                                    invoker.reselectGroupsList.Add(lastUndo);
                                    invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1); 
                                }
                                
                            }
                        }
                    }
                    if (group.addedGroups.Count() >0)
                    {
                        group.UnselectGroupHandle(invoker, key, group); //subgroup recursive
                    }                   
                }
            }
        }
        
        //set subgroup unselected
        private void UnselectGroupHandle(Invoker invoker, string key, Group group)
        {            
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    
                    foreach (FrameworkElement drawn in subgroup.undoElementsList.Last())
                    {
                        if (drawn.AccessKey == key)
                        {
                            if (invoker.unselectGroupsList.Count() > 0)
                            {
                                //fetch
                                List<Group> lastUndo = invoker.unselectGroupsList.Last();
                                //shuffle
                                invoker.reselectGroupsList.Add(lastUndo);
                                invoker.unselectGroupsList.RemoveAt(invoker.unselectGroupsList.Count() - 1);
                            }                               
                        }
                    }
                    if (group.addedGroups.Count() > 0)
                    {
                        subgroup.UnselectGroupHandle(invoker, key, group); //subgroup recursive
                    }
                }
            }          
        }

        //set group unselected
        public void ReselectInGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            if (invoker.undoGroupsList.Count() > 0)
            {
                foreach (Group group in invoker.undoGroupsList.Last())
                {
                    if (group.undoElementsList.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.undoElementsList.Last())
                        {
                            if (drawn.AccessKey == key)
                            {
                                if (invoker.reselectGroupsList.Count() > 0)
                                {
                                    //fetch
                                    List<Group> lastRedo = invoker.reselectGroupsList.Last();
                                    //shuffle
                                    invoker.unselectGroupsList.Add(lastRedo);
                                    invoker.reselectGroupsList.RemoveAt(invoker.reselectGroupsList.Count() - 1);
                                }

                            }
                        }
                    }
                    if (group.addedGroups.Count() > 0)
                    {
                        group.ReselectGroupHandle(invoker, key, group); //subgroup recursive
                    }
                }
            }
        }

        //set subgroup unselected
        private void ReselectGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {

                    foreach (FrameworkElement drawn in subgroup.undoElementsList.Last())
                    {
                        if (drawn.AccessKey == key)
                        {
                            if (invoker.unselectGroupsList.Count() > 0)
                            {
                                //fetch
                                List<Group> lastRedo = invoker.reselectGroupsList.Last();
                                //shuffle
                                invoker.unselectGroupsList.Add(lastRedo);
                                invoker.reselectGroupsList.RemoveAt(invoker.reselectGroupsList.Count() - 1);
                            }
                        }
                    }
                    if (group.addedGroups.Count() > 0)
                    {
                        subgroup.ReselectGroupHandle(invoker, key, group); //subgroup recursive
                    }
                }
            }
        }

        //
        //miscellaneous
        //

        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            //check if undo elements 
            if (invoker.undoElementsList.Count() > 0)
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
        }

        //give smallest number
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
        //saving and loading
        //

        //display lines for saving
        public override string Display(int depth, int maxdepth, Group group)
        {           
            //Display group.
            string str = "";
            if (depth <= maxdepth)
            {               
                //Add group.
                int i = 0;
                while (i < depth)
                {
                    str += "\t";
                    i++;
                }
                //if it has elements
                int groupcount = 0;
                if (group.undoElementsList.Count() > 0)
                {
                    List <FrameworkElement> lastElements = group.undoElementsList.Last();
                    groupcount = lastElements.Count() + group.addedGroups.Count();
                }
                else
                {
                    groupcount = group.addedGroups.Count();
                }
                //write group line with count    
                str = str + "group " + groupcount + "\n";
                //Recursively display child nodes.
                int newdepth = depth + 1; //add depth tab
                if (group.undoElementsList.Count() > 0)
                {
                    foreach (FrameworkElement child in group.undoElementsList.Last())
                    {

                        int j = 0;
                        while (j < newdepth)
                        {
                            str += "\t";
                            j++;
                        }
                        if (child is Rectangle)
                        {

                            str = str + "rectangle " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                        }
                        else
                        {

                            str = str + "ellipse " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                        }
                    }
                }
                //if it has subgroups
                if (group.addedGroups.Count() > 0)
                {
                    foreach (Group subgroup in group.addedGroups)
                    {
                        string substr = subgroup.Display(newdepth, maxdepth, subgroup);
                        str = str + substr;
                    }
                }               
                return str;
            }
            else
            {
                return str;
            }          
        }
    }
}
