using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shell;
using System.Windows.Media.Effects;

namespace Lin.Core.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    public static class Dialog
    {

        public static event System.Action<Window> NewWindow;
        public static event System.Action<Window> WindowClosed;

        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.RegisterAttached("BorderBackground", typeof(Brush), typeof(Dialog), null);
        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.RegisterAttached("CanClose", typeof(bool?), typeof(Dialog), null);
        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.RegisterAttached("CanMove", typeof(bool?), typeof(Dialog), null);

        #region 设置窗体的宽度
        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(int), typeof(Dialog), new PropertyMetadata(500, null));
        public static int GetWidth(DependencyObject dc)
        {
            return (int)dc.GetValue(WidthProperty);
        }
        public static void SetWidth(DependencyObject dc, int value)
        {
            dc.SetValue(WidthProperty, value);
        }
        #endregion

        #region 设置窗体的高度
        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached("Height", typeof(int), typeof(Dialog), new PropertyMetadata(300, null));
        public static int GetHeight(DependencyObject dc)
        {
            return (int)dc.GetValue(HeightProperty);
        }
        public static void SetHeight(DependencyObject dc, int value)
        {
            dc.SetValue(HeightProperty, value);
        }
        #endregion

        #region 设置窗体的标题
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(Dialog), new PropertyMetadata("--", null));
        public static string GetTitle(DependencyObject dc)
        {
            return (string)dc.GetValue(TitleProperty);
        }
        public static void SetTitle(DependencyObject dc, string value)
        {
            dc.SetValue(TitleProperty, value);
        }
        #endregion

        #region 设置窗体的参数
        public static readonly DependencyProperty ParamsProperty = DependencyProperty.RegisterAttached("Params", typeof(object), typeof(Dialog), null);
        public static object GetParams(DependencyObject dc)
        {
            return dc.GetValue(ParamsProperty);
        }
        public static void SetParams(DependencyObject dc, object value)
        {
            dc.SetValue(ParamsProperty, value);
        }
        #endregion

        #region 传统的显示方式(模态  非模态)
        public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetModel(DependencyObject dc)
        {
            return (bool)dc.GetValue(ModelProperty);
        }
        public static void SetModel(DependencyObject dc, bool value)
        {
            dc.SetValue(ModelProperty, value);
        }
        #endregion

        #region  Resize模式
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.RegisterAttached("ResizeMode", typeof(ResizeMode), typeof(Dialog), new PropertyMetadata(ResizeMode.CanResize, null));
        public static ResizeMode GetResizeMode(DependencyObject dc)
        {
            return (ResizeMode)dc.GetValue(ResizeModeProperty);
        }
        public static void SetResizeMode(DependencyObject dc, ResizeMode value)
        {
            dc.SetValue(ResizeModeProperty, value);
        }
        #endregion

        #region 窗口是否可用作拖放操作
        public static readonly DependencyProperty AllowDropProperty = DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetAllowDrop(DependencyObject dc)
        {
            return (bool)dc.GetValue(AllowDropProperty);
        }
        public static void SetAllowDrop(DependencyObject dc, bool value)
        {
            dc.SetValue(AllowDropProperty, value);
        }
        #endregion

        #region 一个用于描述控件背景的画笔
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(Dialog), new PropertyMetadata(Brushes.White, null));
        public static Brush GetBackground(DependencyObject dc)
        {
            return (Brush)dc.GetValue(BackgroundProperty);
        }
        public static void SetBackground(DependencyObject dc, Brush value)
        {
            dc.SetValue(BackgroundProperty, value);
        }
        #endregion

        #region 该值指示窗口的工作区是否支持透明
        public static readonly DependencyProperty TransparencyProperty = DependencyProperty.RegisterAttached("Transparency", typeof(bool), typeof(Dialog), new PropertyMetadata(false
            , null));
        public static bool GetTransparency(DependencyObject dc)
        {
            return (bool)dc.GetValue(TransparencyProperty);
        }
        public static void SetTransparency(DependencyObject dc, bool value)
        {
            dc.SetValue(TransparencyProperty, value);
        }
        #endregion

        #region 用于描述控件的边框背景的画笔
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(Dialog), new PropertyMetadata(Brushes.Transparent
            , null));
        public static Brush GetBorderBrush(DependencyObject dc)
        {
            return (Brush)dc.GetValue(BorderBrushProperty);
        }
        public static void SetBorderBrush(DependencyObject dc, Brush value)
        {
            dc.SetValue(BorderBrushProperty, value);
        }
        #endregion

        #region 获取或设置控件的边框宽度
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.RegisterAttached("BorderThickness", typeof(Thickness), typeof(Dialog), new PropertyMetadata(new Thickness(0)
           , null));
        public static Thickness GetBorderThickness(DependencyObject dc)
        {
            return (Thickness)dc.GetValue(BorderThicknessProperty);
        }
        public static void SetBorderThickness(DependencyObject dc, Thickness value)
        {
            dc.SetValue(BorderThicknessProperty, value);
        }
        #endregion

        #region 分配给此元素的上下文菜单
        public static readonly DependencyProperty ContextMenuProperty = DependencyProperty.RegisterAttached("ContextMenu", typeof(ContextMenu), typeof(Dialog), new PropertyMetadata(null
           , null));
        public static ContextMenu GetContextMenu(DependencyObject dc)
        {
            return (ContextMenu)dc.GetValue(ContextMenuProperty);
        }
        public static void SetContextMenu(DependencyObject dc, ContextMenu value)
        {
            dc.SetValue(ContextMenuProperty, value);
        }
        #endregion

        #region 获取或设置当鼠标指针悬停在此元素上时显示的光标
        public static readonly DependencyProperty CursorProperty = DependencyProperty.RegisterAttached("Cursor", typeof(Cursor), typeof(Dialog), new PropertyMetadata(null
           , null));
        public static Cursor GetCursor(DependencyObject dc)
        {
            return (Cursor)dc.GetValue(CursorProperty);
        }
        public static void SetCursor(DependencyObject dc, Cursor value)
        {
            dc.SetValue(CursorProperty, value);
        }
        #endregion

        #region   获取或设置元素参与数据绑定时的数据上下文
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(Dialog), new PropertyMetadata(null
           , null));
        public static object GetDataContext(DependencyObject dc)
        {
            return (object)dc.GetValue(DataContextProperty);
        }
        public static void SetDataContext(DependencyObject dc, object value)
        {
            dc.SetValue(DataContextProperty, value);
        }
        #endregion

        #region 获取或设置文本和其他user interface (UI) 元素在控制它们布局的任何父元素中的流动方向
        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.RegisterAttached("FlowDirection", typeof(FlowDirection), typeof(Dialog), new PropertyMetadata(FlowDirection.LeftToRight
           , null));
        public static FlowDirection GetFlowDirection(DependencyObject dc)
        {
            return (FlowDirection)dc.GetValue(FlowDirectionProperty);
        }
        public static void SetFlowDirection(DependencyObject dc, FlowDirection value)
        {
            dc.SetValue(FlowDirectionProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示该元素是否可以接收焦点
        public static readonly DependencyProperty FocusableProperty = DependencyProperty.RegisterAttached("Focusable", typeof(bool), typeof(Dialog), new PropertyMetadata(false
          , null));
        public static bool GetFocusable(DependencyObject dc)
        {
            return (bool)dc.GetValue(FocusableProperty);
        }
        public static void SetFocusable(DependencyObject dc, bool value)
        {
            dc.SetValue(FocusableProperty, value);
        }
        #endregion

        #region  获取或设置一个属性，该属性支持自定义将在此元素捕获键盘焦点时应用于此元素的外观、效果或其他样式特征。
        public static readonly DependencyProperty FocusVisualProperty = DependencyProperty.RegisterAttached("FocusVisual", typeof(Style), typeof(Dialog), new PropertyMetadata(null
          , null));
        public static Style GetFocusVisual(DependencyObject dc)
        {
            return (Style)dc.GetValue(FocusVisualProperty);
        }
        public static void SetFocusVisual(DependencyObject dc, Style value)
        {
            dc.SetValue(FocusVisualProperty, value);
        }
        #endregion

        #region 获取或设置控件的字体系列
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.RegisterAttached("FontFamily", typeof(FontFamily), typeof(Dialog),
            new PropertyMetadata(new FontFamily("微软雅黑"), null));
        public static FontFamily GetFontFamily(DependencyObject dc)
        {
            return (FontFamily)dc.GetValue(FontFamilyProperty);
        }
        public static void SetFontFamily(DependencyObject dc, FontFamily value)
        {
            dc.SetValue(FontFamilyProperty, value);
        }
        #endregion

        #region 获取或设置字号
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.RegisterAttached("FontSize", typeof(double), typeof(Dialog), new PropertyMetadata(SystemFonts.MessageFontSize
          , null));
        public static double GetFontSize(DependencyObject dc)
        {
            return (double)dc.GetValue(FontSizeProperty);
        }
        public static void SetFontSize(DependencyObject dc, double value)
        {
            dc.SetValue(FontSizeProperty, value);
        }
        #endregion

        #region 获取或设置字体在屏幕上的压缩或扩展程度。
        public static readonly DependencyProperty FontStretchProperty = DependencyProperty.RegisterAttached("FontStretch", typeof(FontStretch), typeof(Dialog), new PropertyMetadata(FontStretches.Normal
          , null));
        public static FontStretch GetFontStretch(DependencyObject dc)
        {
            return (FontStretch)dc.GetValue(FontStretchProperty);
        }
        public static void SetFontStretch(DependencyObject dc, FontStretch value)
        {
            dc.SetValue(FontStretchProperty, value);
        }
        #endregion

        #region 获取或设置字体样式
        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.RegisterAttached("FontStyle", typeof(FontStyle), typeof(Dialog), new PropertyMetadata(FontStyles.Normal
          , null));
        public static FontStyle GetFontStyle(DependencyObject dc)
        {
            return (FontStyle)dc.GetValue(FontStyleProperty);
        }
        public static void SetFontStyle(DependencyObject dc, FontStyle value)
        {
            dc.SetValue(FontStyleProperty, value);
        }
        #endregion

        #region   获取或设置指定字体的粗细。
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.RegisterAttached("FontWeight", typeof(FontWeight), typeof(Dialog), new PropertyMetadata(FontWeights.Normal
          , null));
        public static FontWeight GetFontWeight(DependencyObject dc)
        {
            return (FontWeight)dc.GetValue(FontWeightProperty);
        }
        public static void SetFontWeight(DependencyObject dc, FontWeight value)
        {
            dc.SetValue(FontWeightProperty, value);
        }
        #endregion

        #region 该值指示此 System.Windows.FrameworkElement 是否应该强制 按照 System.Windows.FrameworkElement.Cursor 属性所声明的方式呈现光标。
        public static readonly DependencyProperty ForceCursorProperty = DependencyProperty.RegisterAttached("ForceCursor", typeof(bool), typeof(Dialog), new PropertyMetadata(false
          , null));
        public static bool GetForceCursor(DependencyObject dc)
        {
            return (bool)dc.GetValue(ForceCursorProperty);
        }
        public static void SetForceCursor(DependencyObject dc, bool value)
        {
            dc.SetValue(ForceCursorProperty, value);
        }
        #endregion

        #region 获取或设置一个用于描述前景色的画笔
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(Dialog), new PropertyMetadata(Brushes.Black
          , null));
        public static Brush GetForeground(DependencyObject dc)
        {
            return (Brush)dc.GetValue(ForegroundProperty);
        }
        public static void SetForeground(DependencyObject dc, Brush value)
        {
            dc.SetValue(ForegroundProperty, value);
        }
        #endregion

        #region  获取或设置在父元素（如面板或项控件）中构成此元素时应用于此元素的水平对齐特征
        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.RegisterAttached("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Dialog), new PropertyMetadata(HorizontalAlignment.Stretch
          , null));
        public static HorizontalAlignment GetHorizontalAlignment(DependencyObject dc)
        {
            return (HorizontalAlignment)dc.GetValue(HorizontalAlignmentProperty);
        }
        public static void SetHorizontalAlignment(DependencyObject dc, HorizontalAlignment value)
        {
            dc.SetValue(HorizontalAlignmentProperty, value);
        }
        #endregion

        #region   获取或设置窗口的图标
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(Dialog), new PropertyMetadata(null
         , null));
        public static ImageSource GetIcon(DependencyObject dc)
        {
            return (ImageSource)dc.GetValue(IconProperty);
        }
        public static void SetIcon(DependencyObject dc, ImageSource value)
        {
            dc.SetValue(IconProperty, value);
        }
        #endregion

        #region 解释输入范围，在该输入范围内修改从其他输入方法输入的方式
        public static readonly DependencyProperty InputScopeProperty = DependencyProperty.RegisterAttached("InputScope", typeof(InputScope), typeof(Dialog), new PropertyMetadata(null
         , null));
        public static InputScope GetInputScope(DependencyObject dc)
        {
            return (InputScope)dc.GetValue(InputScopeProperty);
        }
        public static void SetInputScope(DependencyObject dc, InputScope value)
        {
            dc.SetValue(InputScopeProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示是否在user interface (UI) 中启用了此元素
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(Dialog), new PropertyMetadata(true
         , null));
        public static bool GetIsEnabled(DependencyObject dc)
        {
            return (bool)dc.GetValue(IsEnabledProperty);
        }
        public static void SetIsEnabled(DependencyObject dc, bool value)
        {
            dc.SetValue(IsEnabledProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值声明此元素是否可以作为其呈现内容的某部分的命中测试结果返回
        public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.RegisterAttached("IsHitTestVisible", typeof(bool), typeof(Dialog), new PropertyMetadata(true
         , null));
        public static bool GetIsHitTestVisible(DependencyObject dc)
        {
            return (bool)dc.GetValue(IsHitTestVisibleProperty);
        }
        public static void SetIsHitTestVisible(DependencyObject dc, bool value)
        {
            dc.SetValue(IsHitTestVisibleProperty, value);
        }
        #endregion

        #region  获取或设置一个值，该值指示是否对此 System.Windows.UIElement 启用操作事件
        public static readonly DependencyProperty IsManipulationEnabledProperty = DependencyProperty.RegisterAttached("IsManipulationEnabled", typeof(bool), typeof(Dialog), new PropertyMetadata(false
         , null));
        public static bool GetIsManipulationEnabled(DependencyObject dc)
        {
            return (bool)dc.GetValue(IsManipulationEnabledProperty);
        }
        public static void SetIsManipulationEnabled(DependencyObject dc, bool value)
        {
            dc.SetValue(IsManipulationEnabledProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示是否将某个控件包含在 Tab 导航中
        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.RegisterAttached("IsTabStop", typeof(bool), typeof(Dialog), new PropertyMetadata(true
         , null));
        public static bool GetIsTabStop(DependencyObject dc)
        {
            return (bool)dc.GetValue(IsTabStopProperty);
        }
        public static void SetIsTabStop(DependencyObject dc, bool value)
        {
            dc.SetValue(IsTabStopProperty, value);
        }
        #endregion

        #region 获取或设置在执行布局时应该应用于此元素的图形转换方式
        public static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.RegisterAttached("LayoutTransform", typeof(Transform), typeof(Dialog), new PropertyMetadata(Transform.Identity
         , null));
        public static Transform GetLayoutTransform(DependencyObject dc)
        {
            return (Transform)dc.GetValue(LayoutTransformProperty);
        }
        public static void SetLayoutTransform(DependencyObject dc, Transform value)
        {
            dc.SetValue(LayoutTransformProperty, value);
        }
        #endregion

        #region 获取或设置窗口左边缘相对于桌面的位置。
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Dialog), new PropertyMetadata(0.0
         , null));
        public static double GetLeft(DependencyObject dc)
        {
            return (double)dc.GetValue(LeftProperty);
        }
        public static void SetLeft(DependencyObject dc, double value)
        {
            dc.SetValue(LeftProperty, value);
        }
        #endregion

        #region 获取或设置元素的最大高度约束
        public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.RegisterAttached("MaxHeight", typeof(double), typeof(Dialog), new PropertyMetadata(double.PositiveInfinity
        , null));
        public static double GetMaxHeight(DependencyObject dc)
        {
            return (double)dc.GetValue(MaxHeightProperty);
        }
        public static void SetMaxHeight(DependencyObject dc, double value)
        {
            dc.SetValue(MaxHeightProperty, value);
        }
        #endregion

        #region 获取或设置元素的最大宽度约束。
        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.RegisterAttached("MaxWidth", typeof(double), typeof(Dialog), new PropertyMetadata(double.PositiveInfinity
        , null));
        public static double GetMaxWidth(DependencyObject dc)
        {
            return (double)dc.GetValue(MaxWidthProperty);
        }
        public static void SetMaxWidth(DependencyObject dc, double value)
        {
            dc.SetValue(MaxWidthProperty, value);
        }
        #endregion

        #region 获取或设置元素的最小高度约束
        public static readonly DependencyProperty MinHeightProperty = DependencyProperty.RegisterAttached("MinHeight", typeof(double), typeof(Dialog), new PropertyMetadata(0.0
       , null));
        public static double GetMinHeight(DependencyObject dc)
        {
            return (double)dc.GetValue(MinHeightProperty);
        }
        public static void SetMinHeight(DependencyObject dc, double value)
        {
            dc.SetValue(MinHeightProperty, value);
        }
        #endregion bv

        #region 获取或设置元素的最小宽度约束
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(Dialog), new PropertyMetadata(0.0
      , null));
        public static double GetMinWidth(DependencyObject dc)
        {
            return (double)dc.GetValue(MinWidthProperty);
        }
        public static void SetMinWidth(DependencyObject dc, double value)
        {
            dc.SetValue(MinWidthProperty, value);
        }
        #endregion

        #region 获取或设置元素的标识名称。该名称提供一个引用，以便当 XAML 处理器在处理过程中构造标记元素之后，代码隐藏（如事件处理程序代码）可以对该元素进行引用
        public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name", typeof(string), typeof(Dialog), new PropertyMetadata("", null));
        public static string GetName(DependencyObject dc)
        {
            return (string)dc.GetValue(NameProperty);
        }
        public static void SetName(DependencyObject dc, string value)
        {
            dc.SetValue(NameProperty, value);
        }
        #endregion

        #region 获取或设置当 System.Windows.UIElement 在user interface (UI) 中呈现时为其整体应用的不透明度因子
        public static readonly DependencyProperty OpacityProperty = DependencyProperty.RegisterAttached("Opacity", typeof(double), typeof(Dialog), new PropertyMetadata(1.0, null));
        public static double GetOpacity(DependencyObject dc)
        {
            return (double)dc.GetValue(OpacityProperty);
        }
        public static void SetOpacity(DependencyObject dc, double value)
        {
            dc.SetValue(OpacityProperty, value);
        }
        #endregion

        #region 获取或设置不透明蒙板，作为应用于此元素已呈现内容的任何 Alpha 通道蒙板的 System.Windows.Media.Brush 实现
        public static readonly DependencyProperty OpacityMaskProperty = DependencyProperty.RegisterAttached("OpacityMask", typeof(Brush), typeof(Dialog), new PropertyMetadata(null, null));
        public static Brush GetOpacityMask(DependencyObject dc)
        {
            return (Brush)dc.GetValue(OpacityMaskProperty);
        }
        public static void SetOpacityMask(DependencyObject dc, Brush value)
        {
            dc.SetValue(OpacityMaskProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示此元素是否合并了主题样式中的样式属性
        public static readonly DependencyProperty DefaultStyleProperty = DependencyProperty.RegisterAttached("DefaultStyle", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetDefaultStyle(DependencyObject dc)
        {
            return (bool)dc.GetValue(DefaultStyleProperty);
        }
        public static void SetDefaultStyle(DependencyObject dc, bool value)
        {
            dc.SetValue(DefaultStyleProperty, value);
        }
        #endregion

        #region 获取或设置控件内的边距
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.RegisterAttached("Padding", typeof(Thickness), typeof(Dialog), new PropertyMetadata(new Thickness(0), null));
        public static Thickness GetPadding(DependencyObject dc)
        {
            return (Thickness)dc.GetValue(PaddingProperty);
        }
        public static void SetPadding(DependencyObject dc, Thickness value)
        {
            dc.SetValue(PaddingProperty, value);
        }
        #endregion

        #region    获取或设置影响此元素呈现位置的转换信息。
        public static readonly DependencyProperty RenderTransformProperty = DependencyProperty.RegisterAttached("RenderTransform", typeof(Transform), typeof(Dialog), new PropertyMetadata(Transform.Identity, null));
        public static Transform GetRenderTransform(DependencyObject dc)
        {
            return (Transform)dc.GetValue(RenderTransformProperty);
        }
        public static void SetRenderTransform(DependencyObject dc, Transform value)
        {
            dc.SetValue(RenderTransformProperty, value);
        }
        #endregion

        #region 获取或设置由 System.Windows.UIElement.RenderTransform 声明的任何可能呈现转换的中心点，相对于元素的边界
        public static readonly DependencyProperty RenderTransformOriginProperty = DependencyProperty.RegisterAttached("RenderTransformOrigin", typeof(Point), typeof(Dialog), new PropertyMetadata(new Point(0, 0), null));
        public static Point GetRenderTransformOrigin(DependencyObject dc)
        {
            return (Point)dc.GetValue(RenderTransformOriginProperty);
        }
        public static void SetRenderTransformOrigin(DependencyObject dc, Point value)
        {
            dc.SetValue(RenderTransformOriginProperty, value);
        }
        #endregion

        #region 获取或设置本地定义的资源字典
        public static readonly DependencyProperty ResourcesProperty = DependencyProperty.RegisterAttached("Resources", typeof(ResourceDictionary), typeof(Dialog), new PropertyMetadata(null, null));
        public static ResourceDictionary GetResources(DependencyObject dc)
        {
            return (ResourceDictionary)dc.GetValue(ResourcesProperty);
        }
        public static void SetResources(DependencyObject dc, ResourceDictionary value)
        {
            dc.SetValue(ResourcesProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示在第一次显示窗口时，窗口是否处于激活状态
        public static readonly DependencyProperty ShowActivatedProperty = DependencyProperty.RegisterAttached("ShowActivated", typeof(bool), typeof(Dialog), new PropertyMetadata(true, null));
        public static bool GetShowActivated(DependencyObject dc)
        {
            return (bool)dc.GetValue(ShowActivatedProperty);
        }
        public static void SetShowActivated(DependencyObject dc, bool value)
        {
            dc.SetValue(ShowActivatedProperty, value);
        }
        #endregion

        #region 获取或设置一个指示窗口是否具有任务栏按钮的值
        public static readonly DependencyProperty ShowInTaskbarProperty = DependencyProperty.RegisterAttached("ShowInTaskbar", typeof(bool), typeof(Dialog), new PropertyMetadata(true, null));
        public static bool GetShowInTaskbar(DependencyObject dc)
        {
            return (bool)dc.GetValue(ShowInTaskbarProperty);
        }
        public static void SetShowInTaskbar(DependencyObject dc, bool value)
        {
            dc.SetValue(ShowInTaskbarProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示窗口是否自动调整自身大小以适应其内容大小
        public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.RegisterAttached("SizeToContent", typeof(SizeToContent), typeof(Dialog), new PropertyMetadata(SizeToContent.Manual, null));
        public static SizeToContent GetSizeToContent(DependencyObject dc)
        {
            return (SizeToContent)dc.GetValue(SizeToContentProperty);
        }
        public static void SetSizeToContent(DependencyObject dc, SizeToContent value)
        {
            dc.SetValue(SizeToContentProperty, value);
        }
        #endregion

        #region  获取或设置一个值，该值决定在呈现过程中，此元素的呈现是否应使用特定于设备的像素设置
        public static readonly DependencyProperty SnapsToDevicePixelsProperty = DependencyProperty.RegisterAttached("SnapsToDevicePixels", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetSnapsToDevicePixels(DependencyObject dc)
        {
            return (bool)dc.GetValue(SnapsToDevicePixelsProperty);
        }
        public static void SetSnapsToDevicePixels(DependencyObject dc, bool value)
        {
            dc.SetValue(SnapsToDevicePixelsProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值决定在用户使用 Tab 键在控件中导航时元素接收焦点的顺序
        public static readonly DependencyProperty TabIndexProperty = DependencyProperty.RegisterAttached("TabIndex", typeof(int), typeof(Dialog), new PropertyMetadata(System.Int32.MaxValue, null));
        public static int GetTabIndex(DependencyObject dc)
        {
            return (int)dc.GetValue(TabIndexProperty);
        }
        public static void SetTabIndex(DependencyObject dc, int value)
        {
            dc.SetValue(TabIndexProperty, value);
        }
        #endregion

        #region 获取或设置一个可用于存储有关此元素的自定义信息的任意对象值
        public static readonly DependencyProperty TagProperty = DependencyProperty.RegisterAttached("Tag", typeof(object), typeof(Dialog), new PropertyMetadata(null, null));
        public static object GetTag(DependencyObject dc)
        {
            return (object)dc.GetValue(TagProperty);
        }
        public static void SetTag(DependencyObject dc, object value)
        {
            dc.SetValue(TagProperty, value);
        }
        #endregion

        #region  获取或设置 System.Windows.Window 的 任务栏缩略图
        public static readonly DependencyProperty TaskbarItemInfoProperty = DependencyProperty.RegisterAttached("TaskbarItemInfo", typeof(TaskbarItemInfo), typeof(Dialog), new PropertyMetadata(null, null));
        public static TaskbarItemInfo GetTaskbarItemInfo(DependencyObject dc)
        {
            return (TaskbarItemInfo)dc.GetValue(TaskbarItemInfoProperty);
        }
        public static void SetTaskbarItemInfo(DependencyObject dc, TaskbarItemInfo value)
        {
            dc.SetValue(TaskbarItemInfoProperty, value);
        }
        #endregion

        #region 获取或设置控件模板
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.RegisterAttached("Template", typeof(ControlTemplate), typeof(Dialog), new PropertyMetadata(null, null));
        public static ControlTemplate GetTemplate(DependencyObject dc)
        {
            return (ControlTemplate)dc.GetValue(TemplateProperty);
        }
        public static void SetTemplate(DependencyObject dc, ControlTemplate value)
        {
            dc.SetValue(TemplateProperty, value);
        }
        #endregion

        #region 获取或设置在user interface (UI) 中为此元素显示的工具提示对象
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached("ToolTip", typeof(object), typeof(Dialog), new PropertyMetadata(null, null));
        public static object GetToolTip(DependencyObject dc)
        {
            return (object)dc.GetValue(ToolTipProperty);
        }
        public static void SetToolTip(DependencyObject dc, object value)
        {
            dc.SetValue(ToolTipProperty, value);
        }
        #endregion

        #region  获取或设置一个值，该值指示窗口是否出现在 Z 顺序的最顶层
        public static readonly DependencyProperty TopmostProperty = DependencyProperty.RegisterAttached("Topmost", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetTopmost(DependencyObject dc)
        {
            return (bool)dc.GetValue(TopmostProperty);
        }
        public static void SetTopmost(DependencyObject dc, bool value)
        {
            dc.SetValue(TopmostProperty, value);
        }
        #endregion

        #region 获取或设置此元素的 Uid
        public static readonly DependencyProperty UidProperty = DependencyProperty.RegisterAttached("Uid", typeof(string), typeof(Dialog), new PropertyMetadata("", null));
        public static string GetUid(DependencyObject dc)
        {
            return (string)dc.GetValue(UidProperty);
        }
        public static void SetUid(DependencyObject dc, string value)
        {
            dc.SetValue(UidProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示在布局过程中是否应该对此元素的大小和位置应用布局舍入
        public static readonly DependencyProperty UseLayoutRoundingProperty = DependencyProperty.RegisterAttached("UseLayoutRounding", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetUseLayoutRounding(DependencyObject dc)
        {
            return (bool)dc.GetValue(UseLayoutRoundingProperty);
        }
        public static void SetUseLayoutRounding(DependencyObject dc, bool value)
        {
            dc.SetValue(UseLayoutRoundingProperty, value);
        }
        #endregion

        #region 获取或设置在父元素（如面板或项控件）中组合此元素时应用于此元素的垂直对齐特征
        public static readonly DependencyProperty VerticalAlignmentProperty = DependencyProperty.RegisterAttached("VerticalAlignment", typeof(VerticalAlignment), typeof(Dialog), new PropertyMetadata(VerticalAlignment.Stretch, null));
        public static VerticalAlignment GetVerticalAlignment(DependencyObject dc)
        {
            return (VerticalAlignment)dc.GetValue(VerticalAlignmentProperty);
        }
        public static void SetVerticalAlignment(DependencyObject dc, VerticalAlignment value)
        {
            dc.SetValue(VerticalAlignmentProperty, value);
        }
        #endregion

        #region 获取或设置此元素的user interface (UI) 可见性
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached("Visibility", typeof(Visibility), typeof(Dialog), new PropertyMetadata(Visibility.Visible, null));
        public static Visibility GetVisibility(DependencyObject dc)
        {
            return (Visibility)dc.GetValue(VisibilityProperty);
        }
        public static void SetVisibility(DependencyObject dc, Visibility value)
        {
            dc.SetValue(VisibilityProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示窗口是处于还原、最小化还是最大化状态
        public static readonly DependencyProperty WindowStateProperty = DependencyProperty.RegisterAttached("WindowState", typeof(WindowState), typeof(Dialog), new PropertyMetadata(WindowState.Normal, null));
        public static WindowState GetWindowState(DependencyObject dc)
        {
            return (WindowState)dc.GetValue(WindowStateProperty);
        }
        public static void SetWindowState(DependencyObject dc, WindowState value)
        {
            dc.SetValue(WindowStateProperty, value);
        }
        #endregion

        #region  获取或设置窗口的边框样式
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.RegisterAttached("WindowStyle", typeof(WindowStyle), typeof(Dialog), new PropertyMetadata(WindowStyle.SingleBorderWindow, null));
        public static WindowStyle GetWindowStyle(DependencyObject dc)
        {
            return (WindowStyle)dc.GetValue(WindowStyleProperty);
        }
        public static void SetWindowStyle(DependencyObject dc, WindowStyle value)
        {
            dc.SetValue(WindowStyleProperty, value);
        }
        #endregion

        #region 获取或设置用于定义元素内容边框的几何图形
        public static readonly DependencyProperty ClipProperty = DependencyProperty.RegisterAttached("Clip", typeof(Geometry), typeof(Dialog), new PropertyMetadata(null, null));
        public static Geometry GetClip(DependencyObject dc)
        {
            return (Geometry)dc.GetValue(ClipProperty);
        }
        public static void SetClip(DependencyObject dc, Geometry value)
        {
            dc.SetValue(ClipProperty, value);
        }
        #endregion

        #region 获取或设置一个值，用于表示是否剪裁此元素的内容（或来自此元素的子元素的内容）以适合包含元素的大小
        public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.RegisterAttached("ClipToBounds", typeof(bool), typeof(Dialog), new PropertyMetadata(false, null));
        public static bool GetClipToBounds(DependencyObject dc)
        {
            return (bool)dc.GetValue(ClipToBoundsProperty);
        }
        public static void SetClipToBounds(DependencyObject dc, bool value)
        {
            dc.SetValue(ClipToBoundsProperty, value);
        }
        #endregion

        #region  获取或设置用于显示 System.Windows.Controls.ContentControl 内容的数据模板
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.RegisterAttached("ContentTemplate", typeof(DataTemplate), typeof(Dialog), new PropertyMetadata(null, null));
        public static DataTemplate GetContentTemplate(DependencyObject dc)
        {
            return (DataTemplate)dc.GetValue(ContentTemplateProperty);
        }
        public static void SetContentTemplate(DependencyObject dc, DataTemplate value)
        {
            dc.SetValue(ContentTemplateProperty, value);
        }
        #endregion

        #region 获取或设置一个模板选择器，以使应用程序编写器能够提供自定义模板选择逻辑
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.RegisterAttached("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(Dialog), new PropertyMetadata(null, null));
        public static DataTemplateSelector GetContentTemplateSelector(DependencyObject dc)
        {
            return (DataTemplateSelector)dc.GetValue(ContentTemplateSelectorProperty);
        }
        public static void SetContentTemplateSelector(DependencyObject dc, DataTemplateSelector value)
        {
            dc.SetValue(ContentTemplateSelectorProperty, value);
        }
        #endregion

        #region 获取或设置一个值，该值指示该元素是否可以接收焦点
        public static readonly DependencyProperty EffectProperty = DependencyProperty.RegisterAttached("Effect", typeof(Effect), typeof(Dialog), new PropertyMetadata(null, null));
        public static Effect GetEffect(DependencyObject dc)
        {
            return (Effect)dc.GetValue(EffectProperty);
        }
        public static void SetEffect(DependencyObject dc, Effect value)
        {
            dc.SetValue(EffectProperty, value);
        }
        #endregion


        //#region 改变弹出的窗口的模态属性
        //public static readonly DependencyProperty WindowProperty = DependencyProperty.RegisterAttached("Window", typeof(Window), typeof(Dialog), new PropertyMetadata(null, null));
        //public static Window GetWindow(DependencyObject dc)
        //{
        //    return (Window)dc.GetValue(WindowProperty);
        //}
        //public static void SetWindow(DependencyObject dc, Window value)
        //{
        //    dc.SetValue(WindowProperty, value);
        //}
        //#endregion

        /// <summary>
        /// 当前处于激活状态的窗口
        /// </summary>
        ///  
        private static Window _CurrentWindow;
        public static Window CurrentWindow
        {
            get
            {
                foreach (Window item in Application.Current.Windows)
                {
                    if (item.IsActive)
                    {
                        return item;
                    }
                }
                return _CurrentWindow;
                //if (Application.Current.MainWindow.IsActive)
                //{
                //    return Application.Current.MainWindow;
                //}
                //if (_CurrentWindow == null)
                //{
                //    return Application.Current.MainWindow;
                //}
                //return _CurrentWindow;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        ///弹出窗口对话框
        /// <param name="content">页面的内容</param>
        /// <param name="title">页面标题</param>
        /// <param name="model">窗口显示模式</param>
        /// <param name="Result">返回的结果</param>
        /// <param name="cancel">取消操作</param>
        /// <param name="width">页面宽度</param>
        /// <param name="height">页面高度</param>
        /// <param name="ShowInTaskbar"></param>
        /// <param name="WindowStartupLocation">窗口显示的位置</param>
        public static void ShowDialog(DependencyObject content, Action<object> Result = null, Action cancel = null)
        {
            Window win = new Window();

            //添加窗口激活事件
            //win.Activated += (object sender, EventArgs e) =>
            //{
            //    _CurrentWindow = sender as Window;
            //};
            //SetWindow(content, win);
            win.Width = GetWidth(content);
            win.Height = GetHeight(content);
            win.Title = GetTitle(content);

            win.Content = content;
            win.ResizeMode = GetResizeMode(content);
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.AllowDrop = GetAllowDrop(content);
            win.AllowsTransparency = GetTransparency(content);
            win.Background = GetBackground(content);
            win.BorderBrush = GetBorderBrush(content);
            win.BorderThickness = GetBorderThickness(content);
            win.ContextMenu = GetContextMenu(content);
            win.Cursor = GetCursor(content);
            win.DataContext = GetDataContext(content);
            win.FlowDirection = GetFlowDirection(content);
            win.Focusable = GetFocusable(content);
            win.FocusVisualStyle = GetFocusVisual(content);
            win.FontFamily = GetFontFamily(content);
            win.FontSize = GetFontSize(content);
            win.FontStretch = GetFontStretch(content);
            win.FontStyle = GetFontStyle(content);
            win.FontWeight = GetFontWeight(content);
            win.ForceCursor = GetForceCursor(content);
            win.Foreground = GetForeground(content);
            win.HorizontalAlignment = GetHorizontalAlignment(content);
            win.Icon = GetIcon(content);
            win.InputScope = GetInputScope(content);
            win.IsEnabled = GetIsEnabled(content);
            win.IsHitTestVisible = GetIsHitTestVisible(content);
            win.IsManipulationEnabled = GetIsManipulationEnabled(content);
            win.IsTabStop = GetIsTabStop(content);
            win.LayoutTransform = GetLayoutTransform(content);
            win.Left = GetLeft(content);
            win.MaxHeight = GetMaxHeight(content);
            win.MaxWidth = GetMaxWidth(content);
            win.MinHeight = GetMinHeight(content);
            win.MinWidth = GetMinWidth(content);
            win.Name = GetName(content);
            win.Opacity = GetOpacity(content);
            win.OpacityMask = GetOpacityMask(content);
            win.OverridesDefaultStyle = GetDefaultStyle(content);
            win.Padding = GetPadding(content);
            win.RenderTransform = GetRenderTransform(content);
            win.RenderTransformOrigin = GetRenderTransformOrigin(content);
            win.Resources = GetResources(content);
            win.ShowActivated = GetShowActivated(content);
            win.ShowInTaskbar = GetShowInTaskbar(content);
            win.SizeToContent = GetSizeToContent(content);
            win.SnapsToDevicePixels = GetSnapsToDevicePixels(content);
            win.TabIndex = GetTabIndex(content);
            win.Tag = GetTag(content);
            win.TaskbarItemInfo = GetTaskbarItemInfo(content);
            win.ToolTip = GetToolTip(content);
            win.Topmost = GetTopmost(content);
            win.Uid = GetUid(content);
            win.UseLayoutRounding = GetUseLayoutRounding(content);
            win.VerticalAlignment = GetVerticalAlignment(content);
            win.Visibility = GetVisibility(content);
            win.WindowState = GetWindowState(content);
            win.WindowStyle = GetWindowStyle(content);
            win.Clip = GetClip(content);
            win.ClipToBounds = GetClipToBounds(content);
            win.ContentTemplate = GetContentTemplate(content);
            win.ContentTemplateSelector = GetContentTemplateSelector(content);
            win.Effect = GetEffect(content); 
            bool windowClosed = false;
            ICloseEnable ce = content as ICloseEnable;
            win.Closed += (object sender, EventArgs args) =>
            {
                if (WindowClosed != null)
                {
                    WindowClosed(win);
                }
                if (ce == null || ce.WindowClosedBehavior == WindowClosedBehavior.CANCEL)
                {
                    if (cancel != null && windowClosed == false)
                    {
                        cancel();
                    }
                }
                else
                {
                    if (Result != null)
                    {
                        windowClosed = true;
                        win.Close();
                        Result(ce.Result);
                    }
                }
            };

            if (ce != null)
            {
                ce.Closed += (object sender, EventArgs args) =>
                {
                    if (Result != null)
                    {
                        windowClosed = true;
                        win.Close();
                        Result(ce.Result);
                    }
                };
                ce.Canceled += (object sender, EventArgs args) =>
                {
                    if (cancel != null)
                    {
                        windowClosed = true;
                        win.Close();
                        cancel();
                    }
                };
            }

            win.IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                if (NewWindow != null)
                {
                    if (win.IsVisible == true)
                    {
                        NewWindow(win);
                    }
                }
            };

            if (GetModel(content))
            {
                win.Owner = CurrentWindow;
                win.ShowDialog();
            }
            else
            {
                win.Show();
            }
        }

    }
}
