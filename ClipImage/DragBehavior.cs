using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ClipImage
{
    public class DragBehavior : DependencyObject, IBehavior
    {
        private bool isTap = false;
        private FrameworkElement element;
        private Canvas surface;
        public DependencyObject AssociatedObject
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Attach(DependencyObject associatedObject)
        {

            element = associatedObject as FrameworkElement;
            element.PointerMoved += Element_PointerMoved;
            
        }
        
        

        private void Element_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point=e.GetCurrentPoint((UIElement)sender);
            if (point.Properties.IsLeftButtonPressed)
            {
                var pos = point.Position;
                pos.X = pos.X - element.ActualWidth / 2.0;
                pos.Y = pos.Y - element.ActualHeight / 2.0;

                var left = (double)element.GetValue(Canvas.LeftProperty);
                var top = (double)element.GetValue(Canvas.TopProperty);
                element.SetValue(Canvas.LeftProperty, left + pos.X);
                element.SetValue(Canvas.TopProperty, top + pos.Y);
            }
            
        }
     
        public void Detach()
        {
            if (element != null)
            {
                element.PointerMoved -= Element_PointerMoved;
            }
            
        }
    }
}
