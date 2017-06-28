Imports System.ComponentModel
Imports System.Windows.Controls.Primitives


Class MainWindow
    Private WithEvents MyExThumb As ExThumb

    Private Sub MyCheck()
        Dim ore = MyExThumb
        Dim root = MyExThumb.testRootCanvas

    End Sub
    Private Sub MyCheck2()
        'MyExThumb.SetPoint(100, 100)
        MyExThumb.SetPoint2(100, 100)
    End Sub
    Private Sub MyMove()
        Dim l As Double = 102 + (-MyExThumb.MyRect.Left) ' MyExThumb.NotifyLeft
        MyExThumb.NotifyLeft = l
        Dim t As Double = 102 + (-MyExThumb.MyRect.Top)
        MyExThumb.NotifyTop = t
    End Sub
    Private Sub SetTextBlockBinding(so As Object, sName As String, tb As TextBlock)
        Dim b As New Binding(sName) With {.Source = so, .StringFormat = sName & " = {0:0.0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
    End Sub
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnCheck.Click, AddressOf MyCheck
        AddHandler btnCheck2.Click, AddressOf MyCheck2
        AddHandler btnMove.Click, AddressOf MyMove

        'MyStackPanel.DataContext = MyExThumb


        Dim ext As New ExThumb(New Border With {.Width = 100, .Height = 100, .Background = Brushes.Red})
        Canvas.SetLeft(ext, 0) : Canvas.SetTop(ext, 0)
        MyCanvas.Children.Add(ext)
        MyExThumb = ext

        Dim b As Binding
        'b = New Binding(NameOf(ExThumb.NotifyAngle)) With {.Source = MyExThumb, .StringFormat = "Angle = {0:0.0}"}
        'tbAngle.SetBinding(TextBlock.TextProperty, b)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.NotifyAngle), tbAngle)

        b = New Binding(NameOf(ExThumb.NotifyAngle)) With {.Source = MyExThumb, .Mode = BindingMode.TwoWay}
        sldAngle.SetBinding(Slider.ValueProperty, b)

        'b = New Binding(NameOf(MyExThumb.MyRect)) With {.Source = MyExThumb, .StringFormat = NameOf(MyExThumb.MyRect) & " = {0:0.0}"}
        'tbRect.SetBinding(TextBlock.TextProperty, b)
        'b = New Binding(NameOf(MyExThumb.NotifyLeft)) With {.Source = MyExThumb, .StringFormat = NameOf(MyExThumb.NotifyLeft) & " = {0:0.0}"}
        'tbLeft.SetBinding(TextBlock.TextProperty, b)
        'b = New Binding(NameOf(MyExThumb.MyOutBounds)) With {.Source = MyExThumb, .StringFormat = NameOf(MyExThumb.MyOutBounds) & " = {0:0.0}"}
        'tbBounds.SetBinding(TextBlock.TextProperty, b)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.DiffPoint), tbRect)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.NotifyLeft), tbLeft)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyOutBounds), tbBounds)

    End Sub

    Private Sub MyExThumb_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles MyExThumb.DragDelta
        MyExThumb.NotifyLeft += e.HorizontalChange
        MyExThumb.NotifyTop += e.VerticalChange
    End Sub
End Class




Public Class ExThumb
    Inherits Thumb
    Implements ComponentModel.INotifyPropertyChanged
    Private RootCanvas As Canvas
    Private RootRotate As RotateTransform
    Public testRootCanvas As Canvas


    'Private Sub SetMyRect()
    '    Dim r As Rect = RootCanvas.TransformToVisual(Me).TransformBounds(New Rect(New Size(RootCanvas.Width, RootCanvas.Height)))
    '    MyRect = r
    '    Call SetBounds()
    '    Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
    '    Dim p1 As Point = gt.Transform(New Point)
    '    Dim p2 As Point = gt.Transform(New Point(RootCanvas.Width, 0))
    '    Dim p3 As Point = gt.Transform(New Point(RootCanvas.Width, RootCanvas.Height))
    '    Dim p4 As Point = gt.Transform(New Point(0, RootCanvas.Height))
    'End Sub

    'Private Sub SetBounds()
    '    'Dim rr As New Rect(MyRect.Left + NotifyLeft, MyRect.Top + NotifyTop, MyRect.Width, MyRect.Height)
    '    'MyOutBounds = rr
    '    With MyRect
    '        MyOutBounds = New Rect(.Left + NotifyLeft, .Top + NotifyTop, .Width, .Height)
    '    End With
    'End Sub

    'Public Sub SetPoint(x As Double, y As Double)
    '    NotifyLeft = x + (-MyRect.Left)
    '    NotifyTop = y + (-MyRect.Top)
    'End Sub

    'OutBoundsの左上座標を指定
    Public Sub SetPoint2(x As Double, y As Double)
        NotifyLeft = x + (-DiffPoint.X)
        NotifyTop = y + (-DiffPoint.Y)
    End Sub

    'DiffPointとOutBoundsの更新、変形時に実行する
    Private Sub SetDiffPointAndOutSize()
        Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(RootCanvas.Width, RootCanvas.Height)))
        DiffPoint = r.Location
        OutSize = r.Size
        Call SetOutBounds()

        Dim p1 As Point = gt.Transform(New Point)
        Dim p2 As Point = gt.Transform(New Point(RootCanvas.Width, 0))
        Dim p3 As Point = gt.Transform(New Point(RootCanvas.Width, RootCanvas.Height))
        Dim p4 As Point = gt.Transform(New Point(0, RootCanvas.Height))
    End Sub
    'OutBoundsの更新、移動時に実行する
    Private Sub SetOutBounds()
        'Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = New Rect(New Point(DiffPoint.X + NotifyLeft, DiffPoint.Y + NotifyTop), OutSize)
        MyOutBounds = r

    End Sub


#Region "Property"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(<System.Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Private Property _MyRect As Rect
    Public Property MyRect As Rect
        Get
            Return _MyRect
        End Get
        Set(value As Rect)
            _MyRect = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    Private Property _DiffPoint As Point
    Public Property DiffPoint As Point
        Get
            Return _DiffPoint
        End Get
        Set(value As Point)
            _DiffPoint = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    Private Property _OutSize As Size
    Public Property OutSize As Size
        Get
            Return _OutSize
        End Get
        Set(value As Size)
            _OutSize = value
            Call NotifyPropertyChanged()
        End Set
    End Property

    Private Property _NotifyAngle As Double
    Public Property NotifyAngle As Double
        Get
            Return _NotifyAngle
        End Get
        Set(value As Double)
            _NotifyAngle = value
            RootRotate.Angle = value
            Call NotifyPropertyChanged()
            'Call SetMyRect()
            Call SetDiffPointAndOutSize()
        End Set
    End Property
    Private Property _NotifyLeft As Double
    Public Property NotifyLeft As Double
        Get
            Return _NotifyLeft
        End Get
        Set(value As Double)
            _NotifyLeft = value
            Canvas.SetLeft(Me, value)
            Call NotifyPropertyChanged()
            'Call SetBounds()
            Call SetOutBounds()
        End Set
    End Property
    Private Property _NotifyTop As Double
    Public Property NotifyTop As Double
        Get
            Return _NotifyTop
        End Get
        Set(value As Double)
            _NotifyTop = value
            Canvas.SetTop(Me, value)
            Call NotifyPropertyChanged()
            'Call SetBounds()
            Call SetOutBounds()
        End Set
    End Property
    Private Property _MyOutBounds As Rect
    Public Property MyOutBounds As Rect
        Get
            Return _MyOutBounds
        End Get
        Set(value As Rect)
            _MyOutBounds = value
            Call NotifyPropertyChanged()

        End Set
    End Property
#End Region



    'これは失敗、DependencyPropertyとPropertyChangedCallbackの組み合わせ
    '回転後の枠を求めるタイミングが早いみたいで変更前の角度が採用されてしまう
    'Public Shared ReadOnly AngleProperty As DependencyProperty = DependencyProperty.Register(
    '    NameOf(Angle), GetType(Double), GetType(ExThumb),
    '    New PropertyMetadata(0.0, New PropertyChangedCallback(AddressOf SetMyRect2)))
    'Public Property Angle As Double
    '    Get
    '        Return GetValue(AngleProperty)
    '    End Get
    '    Set(value As Double)
    '        SetValue(AngleProperty, value)
    '    End Set
    'End Property

    'Public Shared Sub SetMyRect2(obj As DependencyObject, e As DependencyPropertyChangedEventArgs)
    '    Dim t As ExThumb = DirectCast(obj, ExThumb)
    '    Dim c As Canvas = t.RootCanvas
    '    Dim r As Rect = c.TransformToVisual(t).TransformBounds(New Rect(New Size(c.Width, c.Height)))
    '    t.MyRect = r
    'End Sub




    Private Function CreateTemplate() As ControlTemplate
        Dim ct As New ControlTemplate(GetType(Thumb))
        Dim c As New FrameworkElementFactory With {.Name = "RootCanvas", .Type = GetType(Canvas)}
        ct.VisualTree = c

        Return ct
    End Function

    Public Sub New(elm As FrameworkElement)
        Template = CreateTemplate()
        ApplyTemplate()
        RootCanvas = Me.Template.FindName("RootCanvas", Me)
        With RootCanvas
            .Children.Add(elm)
            .Height = elm.Height
            .Width = elm.Width
        End With
        testRootCanvas = RootCanvas

        RootRotate = New RotateTransform
        Dim sc As New ScaleTransform
        Dim sk As New SkewTransform
        Dim tg As New TransformGroup
        With tg.Children
            .Add(sc) : .Add(sk) : .Add(RootRotate)
        End With
        With RootCanvas
            .RenderTransformOrigin = New Point(0.5, 0.5)
            .RenderTransform = tg
            .Background = Brushes.Transparent
        End With

        'BindingOperations.SetBinding(
        '    ro, RotateTransform.AngleProperty,
        '    New Binding With {.Source = Me, .Path = New PropertyPath(AngleProperty), .Mode = BindingMode.TwoWay})

        'Dim b As Binding
        'b = New Binding(NameOf(ExThumb.NotifyAngle))
        'BindingOperations.SetBinding(RootRotate, RotateTransform.AngleProperty, b)

        NotifyAngle = 0.0

    End Sub

    'Private Sub ExThumb_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles Me.DragDelta
    '    NotifyLeft += e.HorizontalChange
    '    NotifyTop += e.VerticalChange

    'End Sub
End Class