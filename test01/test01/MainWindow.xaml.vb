Imports System.ComponentModel
Imports System.Windows.Controls.Primitives

Class MainWindow
    Private TargetThumb As Thumb
    Private TargetExThumb As ExThumb
    Private TargetEx2Thumb As Ex2Thumb


    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnCheck.Click, AddressOf btnCheck_Click
        AddHandler btnSetAngle.Click, AddressOf btnSetAngle_Click
        AddHandler btnSetExAngle.Click, AddressOf btnSetExAngle_Click
        AddHandler btnSetEx2Angle.Click, AddressOf btnSetEx2Angle_Click
        AddHandler btnSetRotateAngle.Click, AddressOf btnSetRotateAngle_Click


        TargetThumb = New Thumb With {.Width = 50, .Height = 50}
        MyCanvas.Children.Add(TargetThumb)
        Canvas.SetLeft(TargetThumb, 300)
        Canvas.SetTop(TargetThumb, 0)

        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(10)
        tg.Children.Add(ro)
        TargetThumb.RenderTransform = tg

        ''AnglePropertyをソースにしてSliderValueにBindingしようとしてもうまくいかない
        Dim b As Binding
        'b = New Binding With {.Source = TargetThumb}
        'b.Path = New PropertyPath(RotateTransform.AngleProperty)
        'sldAngle.SetBinding(Slider.ValueProperty, b)

        ''逆にSliderValueをソースにしてAnglePropertyにBindingすると
        'b = New Binding With {.Source = sldAngle, .Path = New PropertyPath(Slider.ValueProperty)}
        'BindingOperations.SetBinding(ro, RotateTransform.AngleProperty, b)
        ''リンクするけど別の方法でAngleに数値指定するとリンクが解けてしまう

        'これで行けた！ソースにはThumbじゃなくて、その中のRotateTransformを指定する
        ro = GetRotateTransform(TargetThumb)
        b = New Binding With {.Source = ro,
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay}
        sldAngle.SetBinding(Slider.ValueProperty, b)
        'ってことはDependencyPropertyを使ったEx2Thumbはあんまり意味がなくなったかな
        'AnglePropertyが指定しやすいことくらいか


        '                BindingOperations.SetBinding(ro, RotateTransform.AngleProperty, b)
        'BindingOperations.SetBinding(sldAngle, Slider.ValueProperty, b)

        tbAngle.SetBinding(TextBlock.TextProperty, b)

        TargetExThumb = New ExThumb
        MyCanvas.Children.Add(TargetExThumb)
        b = New Binding("Angle") With {.Source = TargetExThumb}
        sldExAngle.SetBinding(Slider.ValueProperty, b)


        TargetEx2Thumb = New Ex2Thumb
        MyCanvas.Children.Add(TargetEx2Thumb)
        'Sliderとのバインディング
        b = New Binding With {
            .Source = TargetEx2Thumb,
            .Path = New PropertyPath(Ex2Thumb.Angle2Property),
            .Mode = BindingMode.TwoWay}
        '↕どちらでも同じ結果だけど↑のほうが打ち間違いがなくていい
        'b = New Binding("Angle2") With {
        '    .Source = TargetEx2Thumb,
        '    .Mode = BindingMode.TwoWay}

        '↓は間違い
        'b = New Binding With {
        '    .Source = TargetEx2Thumb,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay}

        'SliderのValuePropertyにBinding
        sldEx2Angle.SetBinding(Slider.ValueProperty, b)
        'tbEx2Angle.SetBinding(TextBlock.TextProperty, b)


        'TextBlock、Angle2Propertyとのバインディング
        '
        b = New Binding With {
            .Source = TargetEx2Thumb,
            .Path = New PropertyPath(Ex2Thumb.Angle2Property),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "Ex2Thumb.Angle2Property = {0:0.0}"}
        tbEx2Angle.SetBinding(TextBlock.TextProperty, b)


        'TextBlock、RotateTransformのAnglePropertyとのバインディング
        'b = New Binding With {
        '    .Source = TargetEx2Thumb,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay,
        '    .StringFormat = "Rotare.AngleProperty = {0:0.0}"}
        'tbEx2RotateAngle.SetBinding(TextBlock.TextProperty, b)
        '↕違いはSource、上だと動かない
        Dim ExRo As RotateTransform = GetRotateTransform(TargetEx2Thumb)
        b = New Binding With {
            .Source = ExRo,
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "Rotare.AngleProperty = {0:0.0}"}
        tbEx2RotateAngle.SetBinding(TextBlock.TextProperty, b)



    End Sub

    Private Sub btnCheck_Click(sender As Object, e As RoutedEventArgs)

        MsgBox(GetRotateTransform(TargetThumb).Angle)
        MsgBox(GetRotateTransform(TargetExThumb).Angle)
    End Sub

    Private Sub btnSetAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform(TargetThumb).Angle = 30
    End Sub

    Private Function GetRotateTransform(t As UIElement) As RotateTransform
        Dim tg As TransformGroup = t.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = GetType(RotateTransform) Then
                Return c
            End If
        Next
        Return Nothing
    End Function

    Private Sub btnSetExAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform(TargetExThumb).Angle = 30
        'TargetExThumb.Angle = 30
    End Sub

    Private Sub btnSetEx2Angle_Click(sender As Object, e As RoutedEventArgs)
        TargetEx2Thumb.Angle2 = 30
    End Sub

    Private Sub btnSetRotateAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform(TargetEx2Thumb).Angle = 20
    End Sub
End Class


Public Class ExThumb
    Inherits Thumb
    Implements System.ComponentModel.INotifyPropertyChanged
    Public Sub New()
        Me.New(300, 50, 50, 50)
    End Sub
    Public Sub New(x As Double, y As Double, w As Double, h As Double)
        Canvas.SetLeft(Me, x)
        Canvas.SetTop(Me, y)
        Width = w
        Height = h
        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(10)
        Dim sk As New SkewTransform
        Dim st As New ScaleTransform
        With tg.Children
            .Add(st)
            .Add(sk)
            .Add(ro)
        End With
        Me.RenderTransform = tg
        Angle = ro.Angle

        Dim bind As New Binding("Angle") With {.Source = Me, .Mode = BindingMode.TwoWay}
        Me.SetBinding(RotateTransform.AngleProperty, bind)

    End Sub
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub MyPropertyChanged(pName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(pName))
    End Sub

    Private Sub ExThumb_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
        'GetRotateTransform.Angle = Angle
    End Sub

    Private Property _Angle As Double
    Public Property Angle As Double
        Get
            Return _Angle
        End Get
        Set(value As Double)
            _Angle = value
            GetRotateTransform.Angle = value
            'GetRotateTransform.SetValue(RotateTransform.AngleProperty, 100)
            MyPropertyChanged("Angle")
        End Set
    End Property
    Private Function GetRotateTransform() As RotateTransform
        Dim tg As TransformGroup = Me.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = GetType(RotateTransform) Then
                Return c
            End If
        Next
        Return Nothing
    End Function
End Class


Public Class Ex2Thumb
    Inherits Thumb

    Public Sub New()
        Me.New(300, 100, 50, 50)
    End Sub
    Public Sub New(x As Double, y As Double, w As Double, h As Double)
        Canvas.SetLeft(Me, x)
        Canvas.SetTop(Me, y)
        Width = w
        Height = h
        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(50)
        Dim sk As New SkewTransform
        Dim st As New ScaleTransform
        With tg.Children
            .Add(st)
            .Add(sk)
            .Add(ro)
        End With
        Me.RenderTransform = tg
        Angle2 = ro.Angle

        'Angle2とRotateTransformのAnglePropertyをBindingしたい
        Dim bind As Binding
        'bind = New Binding With {.Source = Me, .Path = New PropertyPath(Angle2Property), .Mode = BindingMode.TwoWay}
        'Me.SetBinding(RotateTransform.AngleProperty, bind)

        'bind = New Binding("Angle") With {.Source = Me, .Mode = BindingMode.TwoWay}
        'Me.SetBinding(RotateTransform.AngleProperty, bind)

        'bind = New Binding With {.Source = Me, .Path = New PropertyPath(RotateTransform.AngleProperty), .Mode = BindingMode.TwoWay}
        'BindingOperations.SetBinding(Me, Angle2Property, bind)

        bind = New Binding With {
            .Source = Me,
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay}
        Me.SetBinding(Angle2Property, bind)

        bind = New Binding With {
            .Source = GetRotateTransform(),
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay}
        Me.SetBinding(Angle2Property, bind)
        'BindingOperations.SetBinding(Me, Angle2Property, bind)
    End Sub

    Public Shared ReadOnly Property Angle2Property As DependencyProperty =
        DependencyProperty.Register("Angle2", GetType(Double), GetType(Ex2Thumb))
    Public Property Angle2 As Double
        Get
            Return GetValue(Angle2Property)
        End Get
        Set(value As Double)
            SetValue(Angle2Property, value)
            'GetRotateTransform.Angle = value
        End Set
    End Property
    Private Function GetRotateTransform() As RotateTransform
        Dim tg As TransformGroup = Me.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = GetType(RotateTransform) Then
                Return c
            End If
        Next
        Return Nothing
    End Function
End Class

Public Class MyData
    Public Property Angle As Double

End Class