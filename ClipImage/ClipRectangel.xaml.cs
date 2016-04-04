using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ClipImage
{
    public sealed partial class ClipRectangle : UserControl
    {
        private RectModel _rectpath;
        public RectModel RectPath
        {
            get { return _rectpath; }
            set { _rectpath = value; }
        }

        private PointModel _points;
        public PointModel Points
        {
            get { return _points; }
            set { _points = value; }
        }


        public double ButtonWidth { get; set; } = 24.0;

        public ClipRectangle()
        {
            this.InitializeComponent();
            RectPath = new RectModel();

            Points = new PointModel(SetStaticPoint, SetRect, ButtonWidth, 100.0);

            this.surface.SizeChanged += Surface_SizeChanged;

            this.surface.DataContext = Points;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Points.X1 = 0;
            Points.Y1 = 0;
            Points.X2 = surface.Width * 2.0 / 3.0;
            Points.Y2 = surface.Height * 2.0 / 3.0; ;

        }

        private void Surface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Points.CanvasRect = new Rect(0, 0, this.surface.ActualWidth, surface.ActualHeight);
            Points.X1 = 0;
            Points.Y1 = 0;
            Points.X2 = surface.Width * 2.0 / 3.0;
            Points.Y2 = surface.Height * 2.0 / 3.0; ;
        }

        private GeometryGroup group = null;
        private void SetRect()
        {
            if (group == null)
            {
                group = new GeometryGroup();
                group.FillRule = FillRule.EvenOdd;//设置规则为减去重叠部分。
            }
            group.Children.Clear();
            group.Children.Add(new RectangleGeometry() { Rect = new Rect { X = 0, Y = 0, Height = surface.ActualHeight, Width = surface.ActualWidth } });//大矩形区域，和Canvas同样大小

            ClipRect = new Rect { X = Points.X1 + ButtonWidth / 2.0, Y = Points.Y1 + ButtonWidth / 2.0, Width = Points.X2 - Points.X1, Height = Points.Y2 - Points.Y1 };//中间透明区域大小
            group.Children.Add(new RectangleGeometry() { Rect = ClipRect });

            RectPath.Group = group;
        }

        public void SetStaticPoint(string flag, double value)
        {
            switch (flag.ToLower())
            {
                case "x1":
                    SetL1Left(value);
                    break;
                case "y1":
                    SetL1Top(value);
                    break;
                case "x2":
                    SetR2Left(value);
                    break;
                case "y2":
                    SetR2Top(value);
                    break;
                default:
                    break;
            }
        }
        private void SetL1Left(double value)
        {

            btn1l.SetValue(Canvas.LeftProperty, value);
            btn2l.SetValue(Canvas.LeftProperty, value);
        }
        private void SetL1Top(double value)
        {
            btn1l.SetValue(Canvas.TopProperty, value);
            btn1r.SetValue(Canvas.TopProperty, value);
        }
        private void SetR2Left(double value)
        {
            btn2r.SetValue(Canvas.LeftProperty, value);
            btn1r.SetValue(Canvas.LeftProperty, value);
        }
        private void SetR2Top(double value)
        {
            btn2r.SetValue(Canvas.TopProperty, value);
            btn2l.SetValue(Canvas.TopProperty, value);
        }

        public class PointModel : INotifyPropertyChanged
        {
            private double _x1;//代表左上Button的Canvas.Left
            public double X1
            {
                get { return _x1; }
                set
                {
                    double abspos = 0 - _buttonWidth / 2.0;//button最左可以到达的位置
                    if (value < abspos)//如果实际位置还小于该最小位置，
                    {
                        _x1 = abspos;//则强制修改Button的位置到最边界处
                        _call?.Invoke("X1", _x1);//通知修改Button位置
                        _rectcall?.Invoke();//修改矩形区域位置
                        return;
                    }

                    if ((_x2 - value) >= _minRectWidth)//如果Button和同行的button间距大于_minRectWidth，属正常情况
                    {
                        _x1 = value;
                        OnPropertyChanged();
                    }
                    else//如果小于该最小间距
                    {
                        _x1 = _x2 - _minRectWidth;//根据最小间距，强制修改Button位置。
                        _call?.Invoke("X1", _x1);//通知修改Button位置
                    }
                    _rectcall?.Invoke();//修改矩形区域位置
                }
            }

            private double _y1;
            public double Y1
            {
                get { return _y1; }
                set
                {
                    double abspos = 0 - _buttonWidth / 2.0;
                    if (value < abspos)
                    {
                        _y1 = abspos;
                        _call?.Invoke("Y1", _y1);
                        _rectcall?.Invoke();
                        return;
                    }
                    if ((_y2 - value) >= _minRectWidth)
                    {
                        _y1 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        _y1 = _y2 - _minRectWidth;
                        _call?.Invoke("Y1", _y1);

                    }
                    _rectcall?.Invoke();
                }
            }

            private double _x2;
            public double X2
            {
                get { return _x2; }
                set
                {

                    double abspos = CanvasRect.Width - _buttonWidth / 2.0;
                    if (value > abspos)
                    {
                        _x2 = abspos;
                        _call?.Invoke("X2", _x2);
                        _rectcall?.Invoke();
                        return;
                    }

                    if ((value - _x1) >= _minRectWidth)
                    {
                        _x2 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        _x2 = _minRectWidth + _x1;
                        _call?.Invoke("X2", _x2);
                    }
                    _rectcall?.Invoke();
                }
            }

            private double _y2;
            public double Y2
            {
                get { return _y2; }
                set
                {
                    double abspos = CanvasRect.Height - _buttonWidth / 2.0;
                    if (value > abspos)
                    {
                        _y2 = abspos;
                        _call?.Invoke("Y2", _y2);
                        _rectcall?.Invoke();
                        return;
                    }

                    if ((value - _y1) >= _minRectWidth)
                    {
                        _y2 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        _y2 = _y1 + _minRectWidth;
                        _call?.Invoke("Y2", _y2);
                    }
                    _rectcall?.Invoke();
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            /// <summary>
            /// 用于限制Button靠近边界和互相靠近的回调方法
            /// </summary>
            private Action<String, double> _call;
            /// <summary>
            /// 用于改变矩形区域大小的回调方法
            /// </summary>
            private Action _rectcall;

            private Rect _canvasRect;//代表中间透明矩形区域
            public Rect CanvasRect
            {
                get { return _canvasRect; }
                set
                {
                    _canvasRect = value;
                    OnPropertyChanged();
                }
            }

            private double _buttonWidth; //Button的宽度
            private double _minRectWidth;//中间透明矩形区域的最小宽度，不能让四个点重合，这儿最小宽度和最小高度都用这个来表示
            public PointModel(Action<string, double> pointAction, Action rectAction, double btnWidth, double minRectWidth)
            {
                _call = pointAction;
                _rectcall = rectAction;
                _buttonWidth = btnWidth;
                _minRectWidth = minRectWidth;
            }
        }
        public class RectModel : INotifyPropertyChanged
        {
            private GeometryGroup _group;
            public GeometryGroup Group
            {
                get { return _group; }
                set
                {
                    _group = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Group"));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }


        /// <summary>
        /// 用于对外提供裁切区域大小，为Rect(X,Y,Width,Height)形式
        /// </summary>
        public static readonly DependencyProperty ClipRectProperty = DependencyProperty.Register("ClipRect", typeof(Rect), typeof(MainPage), new PropertyMetadata(null));
        public Rect ClipRect
        {
            get { return (Rect)GetValue(ClipRectProperty); }
            set { SetValue(ClipRectProperty, value); }
        }
    }
}
