'ExThumb
'┗ControlTemplate
'   ┗Canvas 変形



Imports System.ComponentModel
Imports System.Windows.Controls.Primitives

Public Class ExThumb
    Inherits Thumb
    Implements System.ComponentModel.INotifyPropertyChanged

    Public RootCanvas As Canvas
    'Private MyElement As FrameworkElement
    'Private ParentPanel As Panel 'MyCanvas
    'Private ReadOnly MyRotateTransform As New RotateTransform
    Public ReadOnly MyScaleTransform As New ScaleTransform
    Public ReadOnly MySkewTransform As New SkewTransform


    Private MyImage As Image
    Private MyBorder As Border
    '方法 : INotifyPropertyChanged インターフェイスを実装する
    'https://msdn.microsoft.com/ja-jp/library/ms229614(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub MyPropertyChanged(<System.Runtime.CompilerServices.CallerMemberName()> Optional pName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(pName))
    End Sub
    Public ReadOnly Property MyType As Type

    'DependencyPropertyにすｒ
    Private Property _ExInLeft As Double
    Public Property ExInLeft As Double
        Get
            Return _ExInLeft
        End Get
        Set(value As Double)
            Call RectOffset(value - _ExInLeft, 0) 'OutRectの座標だけ更新
            _ExInLeft = value
            Canvas.SetLeft(Me, value)
            'Call UpdateRect() '見た目の座標取得
            Call MyPropertyChanged()
            Dim h = Me.Height
            Dim h2 = Me.OutRect.Height
        End Set
    End Property
    Private Property _ExInTop As Double
    Public Property ExInTop As Double
        Get
            Return _ExInTop
        End Get
        Set(value As Double)
            Call RectOffset(0, value - _ExInTop) 'OutRectの座標だけ更新
            _ExInTop = value
            Canvas.SetTop(Me, value)
            'Call UpdateRect() '見た目の座標取得
            Call MyPropertyChanged()
        End Set
    End Property
    Private Property _ExElement As FrameworkElement
    Public Property ExElement As FrameworkElement
        Get
            Return _ExElement
        End Get
        Set(value As FrameworkElement)
            _ExElement = value
        End Set
    End Property

    'Locate、内部の座標と変形後の見た目の座標
    '外部に公開するのは見た目の座標
    Private Property _ExOutLeft As Double
    Public Property ExOutLeft As Double
        Get
            Return _ExOutLeft
        End Get
        Set(value As Double)
            _ExOutLeft = value
            'Call MyPropertyChanged("OutsideLeft")
            Call MyPropertyChanged()

        End Set
    End Property
    Private Property _ExOutTop As Double
    Public Property ExOutTop As Double
        Get
            Return _ExOutTop
        End Get
        Set(value As Double)
            _ExOutTop = value
            Call MyPropertyChanged("OutsideTop")
        End Set
    End Property
    Private Property _OutsideWidth As Double
    'Public Property OutsideWidth As Double
    '    Get
    '        Return _OutsideWidth
    '    End Get
    '    Set(value As Double)
    '        _OutsideWidth = value
    '    End Set
    'End Property
    'Private Property _OutsideHeight As Double
    'Public Property OutsideHeight As Double
    '    Get
    '        Return _OutsideHeight
    '    End Get
    '    Set(value As Double)
    '        _OutsideHeight = value
    '    End Set
    'End Property

    '見た目のRect
    Private Property _OutRect As Rect
    Public Property OutRect As Rect
        Get
            Return _OutRect
        End Get
        Set(value As Rect)
            _OutRect = value
            Call MyPropertyChanged()
        End Set
    End Property
    '見た目の座標
    Private Property _OutLocate As Point
    Public Property OutLocate As Point
        Get
            Return _OutLocate
        End Get
        Set(value As Point)
            _OutLocate = value
            Call MyPropertyChanged()
        End Set
    End Property
    'Public Property GapLeft As Double
    'Public Property GapTop As Double

    Public Property ExSize As Size '要る？


    '回転角度、DependencyProperty依存プロパティ
    Public Shared ReadOnly Property AngleDependencyProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(AngleDependency), GetType(Double), GetType(ExThumb))
    Public Property AngleDependency As Double
        Get
            Return GetValue(AngleDependencyProperty)
        End Get
        Set(value As Double)
            SetValue(AngleDependencyProperty, value)
        End Set
    End Property

    '通知プロパティ→依存プロパティに移行した
    'Private Property _AngleNotify As Double
    'Public Property AngleNotify As Double
    '    Get
    '        Return _AngleNotify
    '    End Get
    '    Set(value As Double)
    '        _AngleNotify = value
    '        Call MyPropertyChanged("AngleNotify")
    '        MyRotateTransform.Angle = value
    '        Call UpdateRect()
    '        'Call SetGapLocate()
    '    End Set
    'End Property

    '見た目のRectを取得、実行するのは変形させたときと移動させたとき

    Public Sub UpdateRect()
        'Dim r As Rect = Me.TransformToVisual(Me.Parent).TransformBounds(New Rect(New Size(Me.Width, Me.Height)))
        If Me.Parent Is Nothing Then Exit Sub
        Me.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                  End Sub)
        'Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me.Parent)
        ''Dim gt As GeneralTransform = Me.RenderTransform
        'Dim r As Rect = gt.TransformBounds(New Rect(New Size(Width, Height)))
        'OutRect = r
        OutRect = RootCanvas.TransformToVisual(Me.Parent).TransformBounds(New Rect(New Size(Width, Height)))

    End Sub
    'OutRectの座標更新
    Private Sub RectOffset(xOff As Double, yOff As Double)
        OutRect = Rect.Offset(OutRect, xOff, yOff)
    End Sub

    ''内外のBoundsの差を求める
    'Private Sub SetGapLocate()
    '    If Me.Parent Is Nothing Then Exit Sub
    '    Me.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

    '                                                              End Sub)
    '    Dim r As Rect = RootCanvas.TransformToVisual(Me.Parent).TransformBounds(New Rect(New Size(Width, Height)))
    '    GapLeft = r.X - ExInLeft : GapTop = r.Y - ExInTop

    'End Sub





    'Template作成
    Private Function CreateTemplate() As ControlTemplate
        Dim c As New FrameworkElementFactory(GetType(Canvas), "RootCanvas")
        Dim img As New FrameworkElementFactory(GetType(Image), "TempImage")
        Dim b As New FrameworkElementFactory(GetType(Border), "TempBorder")
        Dim ct As New ControlTemplate(GetType(Thumb))
        'c.AppendChild(img)
        'c.AppendChild(b)
        ct.VisualTree = c
        Return ct
    End Function
    Public Sub New()
        Call MyIni()
    End Sub
    Public Sub New(elm As FrameworkElement, Optional x As Double = 0, Optional y As Double = 0, Optional scX As Double = 1, Optional scY As Double = 1,
                   Optional skX As Double = 0, Optional skY As Double = 0, Optional angle As Double = 0)
        'ParentPanel = parentP
        Call MyIni() 'ControlTemplate作成、設定


        With RootCanvas
            .RenderTransformOrigin = New Point(0.5, 0.5)
            .RenderTransform = CreateRenderTransform(scX, scY, skX, skY, angle)
            '.Background = Brushes.Transparent
            .Background = Brushes.Black
            .Width = elm.Width '要る？
            .Height = elm.Height '要る？
        End With





        Dim t = elm.GetType
        MyType = t
        Select Case t
            Case GetType(Image)
                MyImage = elm
                ExElement = elm
                RootCanvas.Children.Add(elm)
                'Dim img As Image = DirectCast(elm, Image)
                'ExSize = New Size(img.Width, img.Height)
            Case GetType(Border)
                MyBorder = elm
                ExElement = elm
                RootCanvas.Children.Add(elm)
            Case Else
                ExElement = elm
                RootCanvas.Children.Add(elm)
        End Select
        Width = elm.Width : Height = elm.Height

        'Me.Background = Brushes.Red

        Call SetLocate(x, y)
        ExSize = New Size(elm.Width, elm.Height)

        Call SetMyBinding()
        'binding
        'Dim b As New Binding With {.Source = Me, .Path = New PropertyPath(Window.LeftProperty), .Mode = BindingMode.TwoWay}
        'BindingOperations.SetBinding(Me, Canvas.LeftProperty, b)

        'BindingOperations.SetBinding(Me, Canvas.LeftProperty, New Binding(NameOf(ExInLeft)))

        'BindingOperations.SetBinding(Me, Canvas.TopProperty, New Binding(NameOf(ExInTop)))


    End Sub
    '各種バインディング
    Private Sub SetMyBinding()
        ''回転角度
        'Dim b As Binding
        'b = New Binding With {.Source = Me, .Path = New PropertyPath(AngleDependencyProperty)}
        'BindingOperations.SetBinding(MyRotateTransform, RotateTransform.AngleProperty, b)
    End Sub
    Private Sub MyIni()
        Me.Template = CreateTemplate()
        Me.ApplyTemplate()
        RootCanvas = Me.Template.FindName("RootCanvas", Me)

    End Sub
    Private Function CreateRenderTransform(scX As Double, scY As Double, skX As Double, skY As Double, angle As Double) As GeneralTransform
        Dim tg As New TransformGroup
        MyScaleTransform.ScaleX = scX : MyScaleTransform.ScaleY = scY
        MySkewTransform.AngleX = skX : MySkewTransform.AngleY = skY
        'MyRotateTransform.Angle = angle
        Dim MyRotateTransform As New RotateTransform(angle)
        With tg.Children
            .Add(MyScaleTransform)
            .Add(MySkewTransform)
            .Add(MyRotateTransform)
        End With

        'バインディング
        Dim b As Binding
        b = New Binding With {.Source = Me, .Path = New PropertyPath(AngleDependencyProperty)}
        BindingOperations.SetBinding(MyRotateTransform, RotateTransform.AngleProperty, b)

        Return tg
    End Function



    Public Sub SetLocate(x As Double, y As Double)
        Me.ExInLeft = x
        Me.ExInTop = y
        'ExOutLeft = x
        'ExOutTop = y
    End Sub

    'Private Sub ExThumb_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles Me.DragDelta
    '    'Me.ExLeft += e.HorizontalChange
    '    OutsideLeft += e.HorizontalChange
    '    'Me.ExTop += e.VerticalChange
    '    OutsideTop += e.VerticalChange
    'End Sub
End Class

Public Enum ExType
    Image
    Border
    Path
End Enum