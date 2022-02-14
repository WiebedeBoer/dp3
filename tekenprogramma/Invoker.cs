using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace tekenprogramma
{
    //class invoker
    public class Invoker
    {
        //actions
        public List<ICommand> actionsList = new List<ICommand>();
        public List<ICommand> redoList = new List<ICommand>();
        //elements
        //state 0, remove draw
        //public List<FrameworkElement> removedElements = new List<FrameworkElement>(); //0
        public List<FrameworkElement> drawnElements = new List<FrameworkElement>(); //1
        public List<List<FrameworkElement>> undoElementsList = new List<List<FrameworkElement>>(); //4a keep track of undone
        public List<List<FrameworkElement>> redoElementsList = new List<List<FrameworkElement>>(); //4b keep track of redone
        //state 2, unselect, select
        //public List<FrameworkElement> unselectElements = new List<FrameworkElement>(); //2b
        public List<FrameworkElement> selectElements = new List<FrameworkElement>(); //2a
        public List<List<FrameworkElement>> unselectElementsList = new List<List<FrameworkElement>>(); //4a keep track of undone
        public List<List<FrameworkElement>> reselectElementsList = new List<List<FrameworkElement>>(); //4b keep track of redone
        //groups
        //state 0
        //public List<Group> removedGroups = new List<Group>(); //0 removed groups
        public List<Group> drawnGroups = new List<Group>(); //1 drawn groups  
        public List<List<Group>> undoGroupsList = new List<List<Group>>(); //4a keep track of undone
        public List<List<Group>> redoGroupsList = new List<List<Group>>(); //4b keep track of redone
        //state 2
        //public List<Group> unselectedGroups = new List<Group>(); //2b unselected groups
        public List<Group> selectedGroups = new List<Group>(); //2a selected groups
        public List<List<Group>> unselectGroupsList = new List<List<Group>>(); //4a keep track of undone
        public List<List<Group>> reselectGroupsList = new List<List<Group>>(); //4b keep track of redone
        //counting numbers for execution and elements
        public int counter = 0;
        public int executer = 0;

        public Invoker()
        {
            this.actionsList = new List<ICommand>();
            this.redoList = new List<ICommand>();
        }

        //execute
        public void Execute(ICommand cmd)
        {
            actionsList.Add(cmd);
            redoList.Clear();
            cmd.Execute();
            counter++;
            executer++;
        }

        //undo
        public void Undo()
        {
            if (actionsList.Count >= 1)
            {
                ICommand cmd = actionsList.Last();
                actionsList.RemoveAt(actionsList.Count - 1);
                redoList.Add(cmd);
                cmd.Undo();
                counter--;
            }
        }

        //redo
        public void Redo()
        {
            if (redoList.Count >= 1)
            {
                ICommand cmd = redoList.Last();
                actionsList.Add(cmd);
                redoList.RemoveAt(redoList.Count - 1);
                cmd.Redo();
                counter++;
            }
        }

        //repaint
        public void Repaint()
        {
            //repaint actions
            foreach (ICommand icmd in actionsList)
            {
                icmd.Execute();
            }
        }

        //clear
        public void Clear()
        {
            actionsList.Clear();
        }

    }

}