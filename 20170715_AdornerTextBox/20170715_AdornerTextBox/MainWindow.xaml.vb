'C#のWPFでサイズ変更できるTextBoxを作る - Ararami Atudio
'https://araramistudio.jimdo.com/2016/12/08/wpf%E3%81%A7%E3%82%B5%E3%82%A4%E3%82%BA%E5%A4%89%E6%9B%B4%E3%81%A7%E3%81%8D%E3%82%8Btextbox%E3%82%92%E4%BD%9C%E3%82%8B/


Imports System.Windows.Controls.Primitives


Class MainWindow
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Dim tb As New ResizableTextBox With {.Text = "mytext", .Height = 20}
        Canvas.SetLeft(tb, 50) : Canvas.SetTop(tb, 50)
        MyCanvas.Children.Add(tb)
        Dim bo As New ResizableBorder With {.Width = 100, .Height = 30, .Background = Brushes.Red, .Opacity = 0.3}
        MyCanvas.Children.Add(bo)
        Canvas.SetLeft(bo, 50) : Canvas.SetTop(bo, 100)


    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim w1 As New Window1 : w1.Show()

    End Sub
End Class



Public Class ResizingAdorner
    Inherits Adorner
    Private resizeGrip As Thumb
    Private visualChildren As VisualCollection

    Private Function CreatePathMarker(pm As String) As FrameworkElementFactory
        Dim p As New FrameworkElementFactory(GetType(Path))
        With p
            .SetValue(Path.StrokeProperty, New SolidColorBrush(Colors.Cyan))
            .SetValue(Path.DataProperty, Geometry.Parse(pm))
        End With
        Return p
    End Function

    Public Sub New(adornedElement As UIElement)
        MyBase.New(adornedElement)

        resizeGrip = New Thumb With {.Cursor = Cursors.SizeNWSE, .Width = 18, .Height = 18}
        AddHandler resizeGrip.DragDelta, AddressOf ResizeGripDragDelta

        Dim p1 As FrameworkElementFactory = New FrameworkElementFactory(GetType(Path))
        p1.SetValue(Path.FillProperty, New SolidColorBrush(Colors.White))
        'p1.SetValue(Path.FillProperty, New SolidColorBrush(Colors.Blue))
        p1.SetValue(Path.DataProperty, Geometry.Parse("M0,14L14,0L14,14z"))
        'p1.SetValue(Path.DataProperty, New RectangleGeometry(New Rect(0, 0, 14, 14)))
        Dim p2 As FrameworkElementFactory = CreatePathMarker("M0,14L14,0")
        Dim p3 As FrameworkElementFactory = CreatePathMarker("M4,14L14,4")
        Dim p4 As FrameworkElementFactory = CreatePathMarker("M8,14L14,8")
        Dim p5 As FrameworkElementFactory = CreatePathMarker("M12,14L14,12")

        Dim myGrid = New FrameworkElementFactory(GetType(Grid))
        With myGrid
            .SetValue(Grid.MarginProperty, New Thickness(2.0))
            .AppendChild(p1)
            .AppendChild(p2)
            .AppendChild(p3)
            .AppendChild(p4)
            .AppendChild(p5)
        End With

        Dim template = New ControlTemplate(GetType(Thumb))
        template.VisualTree = myGrid
        resizeGrip.Template = template

        visualChildren = New VisualCollection(Me)
        visualChildren.Add(resizeGrip)
    End Sub

    Private Sub ResizeGripDragDelta(sender As Object, e As DragDeltaEventArgs)
        Dim element As FrameworkElement = Me.AdornedElement
        Dim w As Double = element.Width
        Dim h As Double = element.Height
        If w.Equals(Double.NaN) Then w = element.DesiredSize.Width
        If h.Equals(Double.NaN) Then h = element.DesiredSize.Height

        w += e.HorizontalChange
        h += e.VerticalChange
        w = Math.Max(resizeGrip.Width, w)
        h = Math.Max(resizeGrip.Height, h)
        w = Math.Max(element.MinWidth, w)
        h = Math.Max(element.MinHeight, h)
        w = Math.Min(element.MaxWidth, w)
        h = Math.Min(element.MaxHeight, h)

        element.Width = w
        element.Height = h

    End Sub

    Protected Overrides Function ArrangeOverride(finalSize As Size) As Size
        'Return MyBase.ArrangeOverride(finalSize)
        Dim element As FrameworkElement = Me.AdornedElement
        Dim w As Double = resizeGrip.Width
        Dim h As Double = resizeGrip.Height
        Dim x As Double = element.ActualWidth - w
        Dim y As Double = element.ActualHeight - h

        resizeGrip.Arrange(New Rect(x, y, w, h))
        Return finalSize
    End Function

    Protected Overrides ReadOnly Property VisualChildrenCount As Integer
        Get
            'Return MyBase.VisualChildrenCount
            Return visualChildren.Count
        End Get
    End Property

    Protected Overrides Function GetVisualChild(index As Integer) As Visual
        'Return MyBase.GetVisualChild(index)
        Return visualChildren(index)
    End Function
End Class


Public Class ResizableTextBox
    Inherits TextBox
    Protected Overrides Sub OnInitialized(e As EventArgs)
        MyBase.OnInitialized(e)
        AddHandler Me.Loaded, AddressOf InitializeAdorner
    End Sub
    Private Sub InitializeAdorner(sender As Object, e As RoutedEventArgs)
        Dim layer As AdornerLayer = AdornerLayer.GetAdornerLayer(Me)
        Dim adorner As Adorner = New ResizingAdorner(Me)
        layer.Add(adorner)
    End Sub
End Class

Public Class ResizableBorder
    Inherits Border
    Protected Overrides Sub OnInitialized(e As EventArgs)
        MyBase.OnInitialized(e)
        AddHandler Me.Loaded, AddressOf InitializeAdorner
    End Sub
    Private Sub InitializeAdorner(sender As Object, e As RoutedEventArgs)
        Dim layer As AdornerLayer = AdornerLayer.GetAdornerLayer(Me)
        Dim adorner As Adorner = New ResizingAdorner(Me)
        layer.Add(adorner)
    End Sub
End Class