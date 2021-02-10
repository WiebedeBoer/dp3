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

    class Group
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public string type;
        public int depth;
        public int id;
        public List<Group> groupitems;

        public Group(double height, double width, double x, double y, string type, int depth, int id)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.depth = depth;
            this.id = id;
        }

        public void AddGroup(Group newgroup)
        {
            groupitems.Add(newgroup);
        }

        public void RemoveGroup(Group newgroup)
        {
            groupitems.Remove(newgroup);
        }

        public string Display(int depth)
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
            foreach (Group child in groupitems)
            {
                //child.Display(depth + 1);

                if (child is Rectangle)
                {
                   
                    int j = 0;
                    while(j < depth)
                    {
                        str += "\t";
                    }                    
                    str = str + "rectangle " + child.x + " " + child.y + " " + child.width + " " + child.height + "\n";
                    //return str;
                }
                else if(child is Ellipse)
                {
                    
                    int j = 0;
                    while (j < depth)
                    {
                        str += "\t";
                    }
                    str = str + "ellipse " + child.x + " " + child.y + " " + child.width + " " + child.height + "\n";
                    //return str;
                }
               
                else
                {        
                    str = child.Display(depth + 1);
                    //return str;
                }

            }
            return str;

        }

        /*
        public Group FindID(int id)
        {
            for (int c = 0; c < groupitems.Count; c++)
            {
                if (groupitems[c].id == id)
                {
                    return groupitems[c];
                }
                else
                {
                    Group tmp = FindID(id);
                    if (tmp.id == id)
                    {
                        return tmp;
                    }
                }
            }
        }
        */
    }
}
