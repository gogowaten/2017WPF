Imports System.Windows.Controls.Primitives


Public Class MyAdorner
    Inherits Adorner
    Private resizeGrip As Thumb
    Private visualChildren As VisualCollection
    Private resizeCanvas As Canvas
    Private MyMarker As Thumb


    Private Function CreateMarkerThumb(myName As String) As Thumb

        Dim r As New FrameworkElementFactory(GetType(Rectangle))
        r.SetValue(Rectangle.FillProperty, Brushes.ForestGreen)
        Dim p As New FrameworkElementFactory(GetType(Path))
        With p
            p.SetValue(Path.StrokeProperty, Brushes.White)
            p.SetValue(Path.DataProperty, New RectangleGeometry(New Rect(0, 0, 10, 10)))
            p.SetValue(Path.HorizontalAlignmentProperty, HorizontalAlignment.Center)
            p.SetValue(Path.VerticalAlignmentProperty, VerticalAlignment.Center)
        End With

        Dim myGrid = New FrameworkElementFactory(GetType(Grid))
        myGrid.AppendChild(r)
        myGrid.AppendChild(p)

        Dim template = New ControlTemplate(GetType(Thumb))
        template.VisualTree = myGrid
        MyMarker.Template = template

        MyMarker = New Thumb With {.Cursor = Cursors.SizeNWSE, .Width = 15, .Height = 15, .Name = myName}
        AddHandler MyMarker.DragDelta, AddressOf ResizeGripDragDelta

        Return MyMarker
    End Function

    Public Sub New(adornedElement As UIElement)
        MyBase.New(adornedElement)

        'resizeGrip = New Thumb With {.Cursor = Cursors.SizeNWSE, .Width = 15, .Height = 15}
        'AddHandler resizeGrip.DragDelta, AddressOf ResizeGripDragDelta

        'Dim myGrid As FrameworkElementFactory = CreateMarker("1")
        'Dim template = New ControlTemplate(GetType(Thumb))
        'template.VisualTree = myGrid
        'resizeGrip.Template = template

        'visualChildren = New VisualCollection(Me)
        'visualChildren.Add(resizeGrip)


        Dim myMarkerCanvas = New FrameworkElementFactory(GetType(Canvas))
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
        'w = Math.Max(resizeGrip.Width, w)
        'h = Math.Max(resizeGrip.Height, h)
        w = Math.Max(sender.width, w)
        h = Math.Max(sender.height, h)
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

    'くっきり表示を指定
    Private Sub MyAdorner_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        RenderOptions.SetEdgeMode(Me, EdgeMode.Aliased)

    End Sub
End Class
