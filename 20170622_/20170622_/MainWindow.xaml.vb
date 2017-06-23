Imports System.Globalization

Class MainWindow
    Private ActBorder As Class1

    'Private Sub ChangeRect()
    '    Dim gt As GeneralTransform = ActBorder.TransformToVisual(MyCanvas)
    '    Dim r As Rect = gt.TransformBounds(New Rect(New Size(ActBorder.ActualWidth, ActBorder.ActualHeight)))
    '    ActBorder.MyRect = r
    'End Sub
    Private Sub MyCheck()
        Dim ore = BindingOperations.GetBinding(ActBorder, Class1.MyRectProperty)
        MsgBox(ActBorder.MyRect.ToString)

        'ActBorder.MyRect = New Rect(1, 12, 2, 2)

    End Sub
    Private Sub AngleAdd10()
        ActBorder.MyAngle += 10
    End Sub

    'バインディング作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty, strF As String) As Binding
        Dim b As New Binding With {
           .Source = sObj,
           .Path = New PropertyPath(sDp),
           .Mode = BindingMode.TwoWay,
           .StringFormat = strF}
        Return b
    End Function

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        ActBorder = New Class1(MyCanvas, 50, 50, 100, 30, Brushes.Tomato, 20)
        'MyCanvas.Children.Add(ActBorder)

        Dim bindA As Binding = GetMyBinding(ActBorder, Class1.MyAngleProperty, "Angle = {0:0.0}")
        sldAngle.SetBinding(Slider.ValueProperty, bindA)
        tbAngle.SetBinding(TextBlock.TextProperty, bindA)

        Dim bindR As Binding = GetMyBinding(ActBorder, Class1.MyRectProperty, "Rect = {0:#0.0}")
        bindR.Mode = BindingMode.OneWay
        tbRect.SetBinding(TextBlock.TextProperty, bindR)

        'Dim b As New Binding With {
        '    .Source = ro,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.OneWay,
        '    .Converter = New MyConverter,
        '    .ConverterParameter = Me}
        'tbText.SetBinding(TextBlock.TextProperty, b)




        'AddHandler sldAngle.ValueChanged, AddressOf ChangeRect
        AddHandler btCheck.Click, AddressOf MyCheck
        AddHandler btAngleAdd10.Click, AddressOf AngleAdd10

        Dim w1 As New Window1 : w1.Show()

    End Sub

    'Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    '    ActBorder.SetMyRect()

    'End Sub



End Class






Public Class Class1
    Inherits Border 'Borderクラスをを継承
    Private MyScaleTransform As ScaleTransform
    Private MyRotateTransform As RotateTransform
    Private ParentPanel As Canvas

    '依存関係プロパティ
    'Angle
    Public Shared ReadOnly Property MyAngleProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyAngle), GetType(Double), GetType(Class1))
    'Public Shared ReadOnly MyAngleProperty As DependencyProperty =
    'DependencyProperty.Register(NameOf(MyAngle), GetType(Double), GetType(Class1),
    '                            New PropertyMetadata(0.0, New PropertyChangedCallback(AddressOf AnglePropertyChanged)))
    Public Property MyAngle As Double
        Get
            Return GetValue(MyAngleProperty)
        End Get
        Set(value As Double)
            SetValue(MyAngleProperty, value)
            'MsgBox("change angle")
        End Set
    End Property
    'Private Shared Sub AnglePropertyChanged(dObj As DependencyObject, e As DependencyPropertyChangedEventArgs)
    '    'MsgBox("changed angle")
    '    Dim cl1 As Class1 = dObj
    '    Dim gt As GeneralTransform = cl1.TransformToVisual(cl1.ParentPanel) ',Me.Parentだとインスタンス化のときにNothingなのでエラーになる
    '    Dim r As Rect = gt.TransformBounds(New Rect(New Size(cl1.ActualWidth, cl1.ActualHeight)))
    '    cl1.MyRect = r

    'End Sub

    '見た目のRect
    Public Shared ReadOnly Property MyRectProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyRect), GetType(Rect), GetType(Class1))
    Public Property MyRect As Rect
        Get
            Return GetValue(MyRectProperty)
        End Get
        Set(value As Rect)
            SetValue(MyRectProperty, value)
        End Set
    End Property


    Private Property _MyExteriorRect As Rect
    Public ReadOnly Property MyExteriorRect As Rect
        Get
            Return _MyExteriorRect
        End Get
    End Property

    Public Shared ReadOnly MyExteriorLeftProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyExteriorLeft), GetType(Double), GetType(Class1), New PropertyMetadata(0.0))
    Public Property MyExteriorLeft As Double
        Get
            Return GetValue(MyExteriorLeftProperty)
        End Get
        Set(value As Double)
            SetValue(MyExteriorLeftProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyExteriorTopProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyExteriorTop), GetType(Double), GetType(Class1), New PropertyMetadata(0.0))
    Public Property MyExteriorTop As Double
        Get
            Return GetValue(MyExteriorTopProperty)
        End Get
        Set(value As Double)
            SetValue(MyExteriorTopProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyExteriorWidthProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyExteriorWidth), GetType(Double), GetType(Class1), New PropertyMetadata(0.0))
    Public Property MyExteriorWidth As Double
        Get
            Return GetValue(MyExteriorWidthProperty)
        End Get
        Set(value As Double)
            SetValue(MyExteriorWidthProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyExteriorHeightProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyExteriorHeight), GetType(Double), GetType(Class1), New PropertyMetadata(0.0))
    Public Property MyExteriorHeight As Double
        Get
            Return GetValue(MyExteriorHeightProperty)
        End Get
        Set(value As Double)
            SetValue(MyExteriorHeightProperty, value)
        End Set
    End Property


    'バインディングソースの作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty) As Binding
        Dim b As New Binding With {
            .Source = sObj,
            .Path = New PropertyPath(sDp),
            .Mode = BindingMode.TwoWay,
            .NotifyOnSourceUpdated = True,
            .NotifyOnTargetUpdated = True}
        Return b

    End Function


    '見た目のRect
    'Private Sub SetMyRect()
    '    'If Me.Parent Is Nothing Then Exit Sub
    '    Dim gt As GeneralTransform = Me.TransformToVisual(ParentPanel) ',Me.Parentだとインスタンス化のときにNothingなのでエラーになる
    '    Dim r As Rect = gt.TransformBounds(New Rect(New Size(Me.ActualWidth, Me.ActualHeight)))
    '    MyRect = r
    'End Sub
    'Private Shared Sub SetMyRect(child As Class1)
    '    If child.Parent Is Nothing Then Exit Sub
    '    Dim gt As GeneralTransform = child.TransformToVisual(child.ParentPanel) ',Me.Parentだとインスタンス化のときにNothingなのでエラーになる
    '    Dim r As Rect = gt.TransformBounds(New Rect(New Size(child.ActualWidth, child.ActualHeight)))
    '    child.MyRect = r
    'End Sub


    'コンストラクタ
    Public Sub New(panel As Panel, w As Double, h As Double, b As Brush)
        Call Me.New(panel, 0, 0, w, h, b, 0.0)
    End Sub
    Public Sub New(panel As Panel, l As Double, t As Double, w As Double, h As Double, b As Brush, angle As Double)
        ParentPanel = panel
        ParentPanel.Children.Add(Me)
        Width = w : Height = h : Background = b
        MyAngle = angle
        MyExteriorLeft = l : MyExteriorTop = t : MyExteriorWidth = w : MyExteriorHeight = h


        '各種トランスフォームをグループにしてRenderTransformに指定
        MyScaleTransform = New ScaleTransform '拡縮 今回は未使用
        Dim sk As New SkewTransform '並行変形、今回は未使用
        'Dim ro As New RotateTransform '回転
        MyRotateTransform = New RotateTransform

        'グループ作成
        Dim tg As New TransformGroup
        With tg.Children
            .Add(MyScaleTransform) : .Add(sk) : .Add(MyRotateTransform)
        End With
        Me.RenderTransform = tg '指定
        Me.RenderTransformOrigin = New Point(0.5, 0.5) '変形の基準点は中心

        'バインディング
        'ソース：用意した依存関係プロパティ
        'ターゲット：
        BindingOperations.SetBinding(MyRotateTransform, RotateTransform.AngleProperty, GetMyBinding(Me, MyAngleProperty))

        'Dim bindrect As Binding = GetMyBinding(Me, MyExteriorLeftProperty)
        'bindrect.Converter =
        'BindingOperations.SetBinding(Me, MyRectProperty, GetMyBinding(Me, MyExteriorLeftProperty))
    End Sub


    Private Sub Class1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '見た目のRectをAnglePropertyにバインディング
        'これはコンストラクタで行うとまだ描画されていないので正しい値が取得できないのでLoaded
        'Dim bind As New Binding With {
        '    .Source = MyRotateTransform,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay,
        '    .Converter = New MyConverter,
        '    .ConverterParameter = {Me, ParentPanel}}
        'BindingOperations.SetBinding(Me, MyRectProperty, bind)


        Dim bLeft As Binding = GetMyBinding(Me, MyExteriorLeftProperty)
        Dim bTop As Binding = GetMyBinding(Me, MyExteriorTopProperty)
        Dim bW As Binding = GetMyBinding(Me, MyExteriorWidthProperty)
        Dim bH As Binding = GetMyBinding(Me, MyExteriorHeightProperty)
        Dim mb As New MultiBinding
        With mb.Bindings
            .Add(bLeft) : .Add(bTop) : .Add(bW) : .Add(bH)
        End With
        mb.Converter = New MyRectConverter
        mb.Mode = BindingMode.TwoWay
        BindingOperations.SetBinding(Me, MyRectProperty, mb)
    End Sub
End Class

'[C# WPF]Binding ConverterParameterプロパティを使う: C# WPF 入門
'http://cswpf.seesaa.net/article/313843710.html?seesaa_related=related_article

Public Class MyConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        'Throw New NotImplementedException()
        Dim bo As Class1 = DirectCast(parameter(0), Class1)
        Dim panel As Panel = DirectCast(parameter(1), Panel)
        'If bo.Parent Is Nothing Then Return New Rect

        Dim gt As GeneralTransform = bo.TransformToVisual(panel)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(bo.ActualWidth, bo.ActualHeight)))
        Return r

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
        'Dim bo As Class1 = DirectCast(parameter(0), Class1)
        'Return bo.MyAngle
    End Function
End Class

Public Class MyRectConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        'Throw New NotImplementedException()
        Dim r As New Rect(values(0), values(1), values(2), values(3))
        Return r
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
        Dim r As Rect = value
        Dim obj() As Object = New Object() {r.X, r.Y, r.Width, r.Height}
        Return obj
    End Function
End Class