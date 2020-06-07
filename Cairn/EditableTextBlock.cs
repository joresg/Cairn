using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Cairn
{
    class EditableTextBlock : TextBlock    {
        //public Boolean IsInEditMode { get; set; }
        public EditableTextBlockAdorner _adorner;
        public static readonly DependencyProperty StateProp = CustomDepProp.StateProp.AddOwner(typeof(EditableTextBlock), new PropertyMetadata(true));
        public Boolean State
        {
            get { return (Boolean)this.GetValue(StateProp); }
            set { this.SetValue(StateProp, value); }
        }
        public EditableTextBlock()
        {
            //IsInEditMode = true;
            State = false;

        }

        /*
        private static void IsInEditModeUpdate(DependencyObject obj)
        {
            EditableTextBlock textBlock = obj as EditableTextBlock;
            if (null != textBlock)
            {
                //Get the adorner layer of the uielement (here TextBlock)
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);

                //If the IsInEditMode set to true means the user has enabled the edit mode then
                //add the adorner to the adorner layer of the TextBlock.
                if (textBlock.IsInEditMode)
                {
                    if (null == textBlock._adorner)
                    {
                        textBlock._adorner = new EditableTextBlockAdorner(textBlock);

                        //Events wired to exit edit mode when the user presses 
                        //Enter key or leaves the control.
                        textBlock._adorner.TextBoxKeyUp += textBlock.TextBoxKeyUp;
                        textBlock._adorner.TextBoxLostFocus += textBlock.TextBoxLostFocus;
                    }
                    layer.Add(textBlock._adorner);
                }
                else
                {
                    //Remove the adorner from the adorner layer.
                    Adorner[] adorners = layer.GetAdorners(textBlock);
                    if (adorners != null)
                    {
                        foreach (Adorner adorner in adorners)
                        {
                            if (adorner is EditableTextBlockAdorner)
                            {
                                layer.Remove(adorner);
                            }
                        }
                    }

                    //Update the textblock's text binding.
                    BindingExpression expression = textBlock.GetBindingExpression(TextProperty);
                    if (null != expression)
                    {
                        expression.UpdateTarget();
                    }
                }
            }
        }
        */
    }
    public class CustomDepProp : DependencyObject
    {
        public static DependencyProperty StateProp = DependencyProperty.Register(
            "State", typeof(Boolean),
            typeof(CustomDepProp)
            );
        public Boolean State
        {
            get { return (Boolean)this.GetValue(StateProp); }
            set { this.SetValue(StateProp, value); }
        }
    }
}
