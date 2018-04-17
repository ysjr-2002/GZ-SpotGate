using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GZ_SpotGateEx.UIControls
{
    /// <summary>
    /// 图片变换状态的按钮
    /// </summary>
    public class ImageButton : ToggleButton
    {
        public static readonly DependencyProperty NormalImageProperty;

        public static readonly DependencyProperty HoverImageProperty;

        public static readonly DependencyProperty DisableImageProperty;

        public static readonly DependencyProperty TextProperty;

        public ImageButton()
        {
            //DefaultStyleKey = typeof(ImageButtonEx);
        }

        static ImageButton()
        {
            NormalImageProperty = DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(ImageButton));
            HoverImageProperty = DependencyProperty.Register("HoverImage", typeof(ImageSource), typeof(ImageButton));
            DisableImageProperty = DependencyProperty.Register("DisableImage", typeof(ImageSource), typeof(ImageButton));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ImageButton), new PropertyMetadata(""));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Normal Image
        /// </summary>
        public ImageSource NormalImage
        {
            get { return (ImageSource)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }

        /// <summary>
        /// Hover image
        /// </summary>
        public ImageSource HoverImage
        {
            get { return (ImageSource)GetValue(HoverImageProperty); }
            set { SetValue(HoverImageProperty, value); }
        }

        /// <summary>
        /// disable image
        /// </summary>
        public ImageSource DisableImage
        {
            get { return (ImageSource)GetValue(DisableImageProperty); }
            set { SetValue(DisableImageProperty, value); }
        }
    }
}
