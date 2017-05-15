'Rectangleの枠を太くすると内側に太くなる
'PathにRectangleGeometryを使ったものは線を中心に太くなる(内外両方)


Public Class Window1
    Private MyRectangle1 As Rectangle
    Private MyRectangle2 As Rectangle
    Private MyPathRectAngle1 As Path
    Private MyPathRectAngle2 As Path
    Private MyPathRectAngle3 As Path
    Private MyPathRectAngle4 As Path
    Private MyPathRectAngle5 As Path
    Private MyLine As Line
    Private MyPath As Path

    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler BtnCheck.Click, AddressOf MyCheck

        MyRectangle1 = CreateRectangle(50, 10, 1.0)
        MyCanvas.Children.Add(MyRectangle1)
        Call SetLocate(MyRectangle1, 10, 10)

        MyRectangle2 = CreateRectangle(50, 10, 3.0)
        MyCanvas.Children.Add(MyRectangle2)
        Call SetLocate(MyRectangle2, 65, 10)

        Dim rg As RectangleGeometry
        rg = New RectangleGeometry(New Rect(0, 0, 50, 10))
        MyPathRectAngle1 = New Path With {.Stroke = Brushes.Black, .StrokeThickness = 1.0, .Data = rg}
        MyCanvas.Children.Add(MyPathRectAngle1)
        Call SetLocate(MyPathRectAngle1, 10, 30)

        rg = New RectangleGeometry(New Rect(0, 0, 50, 10))
        MyPathRectAngle2 = New Path With {.Stroke = Brushes.Black, .StrokeThickness = 3.0, .Data = rg}
        MyCanvas.Children.Add(MyPathRectAngle2)
        Call SetLocate(MyPathRectAngle2, 65, 30)

        rg = New RectangleGeometry(New Rect(0, 0, 50, 10))
        MyPathRectAngle3 = New Path With {.Stroke = Brushes.Black, .StrokeThickness = 1.0, .Data = rg}
        MyCanvas.Children.Add(MyPathRectAngle3)
        Call SetLocate(MyPathRectAngle3, 10, 50)
        RenderOptions.SetEdgeMode(MyPathRectAngle3, EdgeMode.Aliased)
        RenderOptions.SetBitmapScalingMode(MyPathRectAngle3, BitmapScalingMode.NearestNeighbor)
        MyPathRectAngle3.UseLayoutRounding = True 'これを有効にするとActualHeightやWidthが0.5減って期待する値になる

        rg = New RectangleGeometry(New Rect(0, 0, 50, 10))
        MyPathRectAngle4 = New Path With {.Stroke = Brushes.Black, .StrokeThickness = 3.0, .Data = rg}
        MyCanvas.Children.Add(MyPathRectAngle4)
        Call SetLocate(MyPathRectAngle4, 65, 50)
        RenderOptions.SetEdgeMode(MyPathRectAngle4, EdgeMode.Aliased)
        RenderOptions.SetBitmapScalingMode(MyPathRectAngle4, BitmapScalingMode.NearestNeighbor)
        'MyPathRectAngle4.UseLayoutRounding = True 'これを有効にするとActualHeightやWidthが0.5減って期待する値になる

        rg = New RectangleGeometry(New Rect(0, 0, 50, 10), 1, 1, New RotateTransform(0))
        MyPathRectAngle5 = New Path With {.Stroke = Brushes.Black, .StrokeThickness = 3.0, .Data = rg}
        MyCanvas.Children.Add(MyPathRectAngle5)
        Call SetLocate(MyPathRectAngle5, 10, 70)
        'RenderOptions.SetEdgeMode(MyPathRectAngle5, EdgeMode.Aliased)
        'MyPathRectAngle5.UseLayoutRounding = True




        MyLine = New Line With {.Stroke = Brushes.Red, .X1 = 120, .X2 = 170, .Y1 = 10, .Y2 = 10}
        MyCanvas.Children.Add(MyLine)

        Dim pf As New PathFigure
        With pf
            .StartPoint = (New Point(120, 20))
            .Segments.Add(New LineSegment(New Point(170, 20), True))
        End With
        Dim pg As New PathGeometry
        pg.Figures.Add(pf)
        MyPath = New Path With {.Stroke = Brushes.Red, .Data = pg}
        MyCanvas.Children.Add(MyPath)


    End Sub
    Private Sub SetLocate(obj As FrameworkElement, x As Double, y As Double)
        Canvas.SetLeft(obj, x)
        Canvas.SetTop(obj, y)
    End Sub

    Private Function CreateRectangle(w As Double, h As Double, b As Double) As Rectangle
        Dim r As New Rectangle
        With r
            .Width = w
            .Height = h
            .StrokeThickness = b
            .Stroke = Brushes.Black
            '.RadiusX = 1
            '.RadiusY = 2
        End With
        Return r
    End Function
    'Private Function CreatePath() As Path

    'End Function

    'イベント用
    Private Sub MyCheck(sender As Object, e As RoutedEventArgs)
        MsgBox($"ActualHeight = {MyPathRectAngle2.ActualHeight}, Height = {MyPathRectAngle2.Height}")
    End Sub
End Class
