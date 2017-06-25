'WPF、CanvasLeftTopとSliderValueをBinding ( ソフトウェア ) - 午後わてんのブログ - Yahoo!ブログ
'https://blogs.yahoo.co.jp/gogowaten/14988621.html



Imports System.Globalization

Class MainWindow
#Region "Private Sub"
    '右へ10
    Private Sub AddLeft10()
        MyLeft += 10
    End Sub
    '左へ10
    Private Sub SubLeft10()
        MyLeft -= 10
    End Sub
    Private Sub AddAngle10()
        MyAngle += 10
    End Sub
    Private Sub SubAngle10()
        MyAngle -= 10
    End Sub


    'AnglePropertyの変更時に求めると変更直前の値になってしまうので不都合
    Private Shared Sub ChangeTest(dpO As DependencyObject, e As DependencyPropertyChangedEventArgs)
        'Dim mw As MainWindow = DirectCast(dpO, MainWindow)
        'Dim r As Rect = mw.MyExteriorRect
        'r.Offset(e.NewValue, 0.0)
        'Dim b As Border = mw.MyBorder
        'Dim c As Canvas = mw.MyCanvas
        ''Dim s As New Size(b.ActualWidth, b.ActualHeight)
        'Dim s As New Size(b.Width, b.Height)
        'Dim t As GeneralTransform = b.TransformToVisual(c)
        'Dim r As Rect = t.TransformBounds(New Rect(s))
        'Dim diffL As Double = mw.MyLeft - r.Left
        'Dim diffT As Double = mw.MyTop - r.Top
        'mw.MyDiffLeft = diffL
        'mw.MyDiffTop = diffT

    End Sub
    Private Shared Function test(d As DependencyObject, b As Object) As Object

    End Function
#End Region

    '所有者型はBorder？MainWindow？その他？→MainWindowがよさそう？
#Region "DependencyProperty"

    'Left
    Public Shared ReadOnly MyLeftProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyLeft), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public Property MyLeft As Double
        Get
            Return GetValue(MyLeftProperty)
        End Get
        Set(value As Double)
            SetValue(MyLeftProperty, value)
        End Set
    End Property
    'Top
    Public Shared ReadOnly MyTopProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyTop), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public Property MyTop As Double
        Get
            Return GetValue(MyTopProperty)
        End Get
        Set(value As Double)
            SetValue(MyTopProperty, value)
        End Set
    End Property
    'Angle
    'Public Shared ReadOnly MyAngleProperty As DependencyProperty = DependencyProperty.Register(
    '    NameOf(MyAngle), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0, New PropertyChangedCallback(AddressOf ChangeTest),
    '                                                                                New CoerceValueCallback(AddressOf test)))
    Public Shared ReadOnly MyAngleProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyAngle), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public Property MyAngle As Double
        Get
            Return GetValue(MyAngleProperty)
        End Get
        Set(value As Double)
            SetValue(MyAngleProperty, value)
        End Set
    End Property
    'DiffLeft
    Public Shared ReadOnly MyDiffLeftProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyDiffLeft), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public Property MyDiffLeft As Double
        Get
            Return GetValue(MyDiffLeftProperty)
        End Get
        Set(value As Double)
            SetValue(MyDiffLeftProperty, value)
        End Set
    End Property
    'DiffTop
    Public Shared ReadOnly MyDiffTopProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyDiffTop), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public Property MyDiffTop As Double
        Get
            Return GetValue(MyDiffTopProperty)
        End Get
        Set(value As Double)
            SetValue(MyDiffTopProperty, value)
        End Set
    End Property
    'Rect
    Public Shared ReadOnly MyExteriorRectProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyExteriorRect), GetType(Rect), GetType(MainWindow))
    Public ReadOnly Property MyExteriorRect As Rect
        Get
            Return GetValue(MyExteriorRectProperty)
        End Get
    End Property
    Public Shared ReadOnly MyExteriorLeftProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyExteriorLeft), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public ReadOnly Property MyExteriorLeft As Double
        Get
            Return GetValue(MyExteriorLeftProperty)
        End Get
    End Property
    Public Shared ReadOnly MyExteriorTopProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyExteriorTop), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public ReadOnly Property MyExteriorTop As Double
        Get
            Return GetValue(MyExteriorTopProperty)
        End Get
    End Property
    Public Shared ReadOnly MyExteriorWidthProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyExteriorWidth), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public ReadOnly Property MyExteriorWidth As Double
        Get
            Return GetValue(MyExteriorWidthProperty)
        End Get
    End Property
    Public Shared ReadOnly MyExteriorHeihgtProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyExteriorHeight), GetType(Double), GetType(MainWindow), New PropertyMetadata(0.0))
    Public ReadOnly Property MyExteriorHeight As Double
        Get
            Return GetValue(MyExteriorHeihgtProperty)
        End Get
    End Property


#End Region

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
        'ボタンのクリックイベントに関連付け
        AddHandler btAdd10.Click, AddressOf AddLeft10
        AddHandler btSub10.Click, AddressOf SubLeft10
        AddHandler btAddAngle10.Click, AddressOf AddAngle10
        AddHandler btSubAngle10.Click, AddressOf SubAngle10

        'バインディング
        Dim bindL As Binding = GetMyBinding(Me, MyLeftProperty, "MyLeft = {0:0}")
        MyBorder.SetBinding(LeftProperty, bindL)
        sldCanvasLeft.SetBinding(Slider.ValueProperty, bindL)
        tbCanvasLeft.SetBinding(TextBlock.TextProperty, bindL)
        Dim bindT As Binding = GetMyBinding(Me, MyTopProperty, "MyTop = {0:0}")
        MyBorder.SetBinding(TopProperty, bindT)
        sldCanvasTop.SetBinding(Slider.ValueProperty, bindT)
        tbCanvasTop.SetBinding(TextBlock.TextProperty, bindT)

        'Dim ro As New RotateTransform
        'MyBorder.RenderTransform = ro
        'MyBorder.RenderTransformOrigin = New Point(0.5, 0.5)
        'Dim bindA As Binding = GetMyBinding(Me, MyAngleProperty, "MyAngle = {0:0}")
        'BindingOperations.SetBinding(ro, RotateTransform.AngleProperty, bindA)
        'sldAngle.SetBinding(Slider.ValueProperty, bindA)
        'tbAngle.SetBinding(TextBlock.TextProperty, bindA)
        'MyAngle = 10


        tbDiffLeft.SetBinding(TextBlock.TextProperty, GetMyBinding(Me, MyDiffLeftProperty, "MyDiffLeft = {0:0.00}"))
        tbDiffTop.SetBinding(TextBlock.TextProperty, GetMyBinding(Me, MyDiffTopProperty, "MyDiffTop = {0:0.00}"))
        tbRect.SetBinding(TextBlock.TextProperty, GetMyBinding(Me, MyExteriorRectProperty, "MyRect = {0:0.0}"))
        Dim bELeft As Binding = GetMyBinding(Me, MyExteriorLeftProperty, "")

    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'MyAngle = 10
        MyAngle = 10
        MyLeft = 50
        MyTop = 10
        Dim ro As New RotateTransform
        MyBorder.RenderTransform = ro
        MyBorder.RenderTransformOrigin = New Point(0.5, 0.5)
        Dim bindA As Binding = GetMyBinding(Me, MyAngleProperty, "MyAngle = {0:0}")
        BindingOperations.SetBinding(ro, RotateTransform.AngleProperty, bindA)
        sldAngle.SetBinding(Slider.ValueProperty, bindA)
        tbAngle.SetBinding(TextBlock.TextProperty, bindA)

        'Dim bDl As Binding = GetMyBinding(Me, MyDiffLeftProperty, "")
        'Dim bDt As Binding = GetMyBinding(Me, MyDiffTopProperty, "")
        'Dim mb As New MultiBinding
        'With mb
        '    .Mode = BindingMode.OneWayToSource
        '    .ConverterParameter = Me
        '    .Converter = New SetDiffConverter
        '    With .Bindings
        '        .Add(bDl)
        '        .Add(bDt)
        '    End With
        'End With
        'BindingOperations.SetBinding(Me, MyAngleProperty, mb)


        'Dim bAngle As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty), .Mode = BindingMode.TwoWay,
        '    .Converter = New SetDiffConverter, .ConverterParameter = Me}
        'BindingOperations.SetBinding(Me, MyDiffLeftProperty, bAngle)
        'BindingOperations.SetBinding(Me, MyDiffTopProperty, bAngle)

        'Dim bRotateAngle As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty), .Mode = BindingMode.TwoWay}
        'Dim mb As New MultiBinding
        'With mb
        '    .Mode = BindingMode.TwoWay
        '    .ConverterParameter = Me
        '    .Converter = New SetDiffConverter
        '    With .Bindings
        '        .Add(bRotateAngle)
        '    End With
        'End With
        'BindingOperations.SetBinding(Me, MyDiffLeftProperty, mb)
        'BindingOperations.SetBinding(Me, MyDiffTopProperty, mb)

        'RotateTransformが変化した時にRectを取得
        'Dim bindAngle As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty), .Mode = BindingMode.OneWay,
        '.Converter = New MyConverterRect, .ConverterParameter = Me}
        Dim bindAngle As New Binding With {.Source = Me, .Path = New PropertyPath(MyAngleProperty), .Mode = BindingMode.OneWay,
            .Converter = New MyConverterRect, .ConverterParameter = Me}

        BindingOperations.SetBinding(Me, MyExteriorRectProperty, bindAngle)

        'Diff
        'Dim bindRect As New Binding With {.Source = Me, .Path = New PropertyPath(MyExteriorRectProperty), .Mode = BindingMode.TwoWay,
        '    .Converter = New MyConverterDiff, .ConverterParameter = Me}
        BindingOperations.SetBinding(Me, MyDiffLeftProperty, bindAngle)
        BindingOperations.SetBinding(Me, MyDiffTopProperty, bindAngle)




    End Sub
End Class

Public Class MyConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        '    Throw New NotImplementedException()
        Dim mw As MainWindow = DirectCast(parameter, MainWindow)
        Dim b As Border = mw.MyBorder
        Dim c As Canvas = mw.MyCanvas
        Dim s As New Size(b.Width, b.Height)
        Dim t As GeneralTransform = b.TransformToVisual(c)
        Dim r As Rect = t.TransformBounds(New Rect(s))
        Dim diffL As Double = mw.MyLeft - r.Left
        Dim diffT As Double = mw.MyTop - r.Top
        mw.MyDiffLeft = diffL
        mw.MyDiffTop = diffT
        Return New Object() {}

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class


Public Class SetDiffConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        'Throw New NotImplementedException()
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        'Throw New NotImplementedException()
        Dim mw As MainWindow = parameter
        Dim b As Border = mw.MyBorder
        Dim c As Canvas = mw.MyCanvas
        Dim s As New Size(b.Width, b.Height)
        Dim t As GeneralTransform = b.TransformToVisual(c)
        Dim r As Rect = t.TransformBounds(New Rect(s))
        Dim diffL As Double = mw.MyLeft - r.Left
        Dim diffT As Double = mw.MyTop - r.Top
        mw.MyDiffLeft = diffL
        mw.MyDiffTop = diffT
        Return New Object() {}
    End Function
End Class

'RotateTransformが変化した時にRectを取得
Public Class MyConverterRect
    Implements IValueConverter
    'value = Angle
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        'Throw New NotImplementedException()
        Dim mw As MainWindow = DirectCast(parameter, MainWindow)
        Dim b As Border = mw.MyBorder
        Dim c As Canvas = mw.MyCanvas
        Dim s As New Size(b.Width, b.Height)
        Dim t As GeneralTransform = b.TransformToVisual(c)
        Dim r As Rect = t.TransformBounds(New Rect(s))
        Dim dl As Double = mw.MyLeft - r.Left
        Dim dt As Double = mw.MyTop - r.Top
        mw.MyDiffTop = dt
        mw.MyDiffLeft = dl
        Return r
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        'Throw New NotImplementedException()
        'Dim mw As MainWindow = DirectCast(parameter, MainWindow)
        'Dim d As Double = mw.MyAngle
        'Return d
    End Function
End Class

Public Class MyConverterLeft
    Implements IValueConverter
    'value = Left
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        '    Throw New NotImplementedException()
        Dim mw As MainWindow = DirectCast(parameter, MainWindow)

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class