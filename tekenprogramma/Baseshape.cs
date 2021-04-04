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
    public abstract class Baseshape
    {
        public double height;
        public double width;
        public double x { get; set; }
        public double y { get; set; }
        public PointerRoutedEventArgs e;

        public List<Location> redoList = new List<Location>();
        public List<Location> undoList = new List<Location>();

        public Location start = null;

        //options modus turned on this shape
        public bool selected = false;
        public bool drawed = false;
        public bool dragging = false;
        public bool resizing = false;
        public bool handle = false;


        public Baseshape(double height, double width, double x, double y)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;

        }

        //public void add()
        //{

        //}

        //public void remove()
        //{

        //}

        public abstract string display(int depth);

        public abstract void select(PointerRoutedEventArgs e, Canvas paintSurface);
        public abstract void deselect(PointerRoutedEventArgs e);

        public abstract void moving(Location location);
        public abstract void undoMoving();
        public abstract void redoMoving();
        public abstract void resize(Location location);
        public abstract void undoResize();
        public abstract void redoResize();

        public abstract bool getIfSelected(double x, double y);
        public abstract bool getHandleIfSelected(double x, double y);
        //public abstract void saving();
        //public abstract void loading();

    }
}
