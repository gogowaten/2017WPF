Class MainWindow
    Private MyPathRectangle As Path
    Private MyRectangle As Rectangle

    Private Sub Window2_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        MyPathRectangle = CreatePathRectangle(50, 20, 1, 0, 0)
        MyLeftCanvas.Children.Add(MyPathRectangle)
        SetLocate(MyPathRectangle, 65, 40)

        With MyLeftPanel.Children
            .Add(New TextBlock With {.Text = "Pathで四角枠", .HorizontalAlignment = HorizontalAlignment.Center})
            .Add(SetTextBinding(MyPathRectangle, Path.HeightProperty))

            .Add(SetTextBinding(MyPathRectangle, Path.ActualHeightProperty))
            .Add(SetTextBinding(MyPathRectangle, Path.StrokeThicknessProperty))
            .Add(SetSliderBinding(CreateSlider(10, 0.1), MyPathRectangle, Path.StrokeThicknessProperty))

            Dim ra As RectangleGeometry = CType(MyPathRectangle.Data, RectangleGeometry)
            .Add(SetTextBinding(ra, RectangleGeometry.RadiusXProperty))
            .Add(SetSliderBinding(CreateSlider(10, 0.1), ra, RectangleGeometry.RadiusXProperty))
            .Add(SetTextBinding(ra, RectangleGeometry.RadiusYProperty))
            .Add(SetSliderBinding(CreateSlider(10, 0.1), ra, RectangleGeometry.RadiusYProperty))

        End With

        MyRectangle = CreateRectangle(50, 20, 1, 0, 0)
        MyRightCanvas.Children.Add(MyRectangle)
        SetLocate(MyRectangle, 65, 40)
        With MyRightPanel.Children
            .Add(New TextBlock With {.Text = "Rectangleで四角枠", .HorizontalAlignment = HorizontalAlignment.Center})
            .Add(SetTextBinding(MyRectangle, Rectangle.HeightProperty))

            .Add(SetTextBinding(MyRectangle, Path.ActualHeightProperty))
            .Add(SetTextBinding(MyRectangle, Path.StrokeThicknessProperty))
            .Add(SetSliderBinding(CreateSlider(10, 0.1), MyRectangle, Path.StrokeThicknessProperty))

            .Add(SetTextBinding(MyRectangle, Rectangle.RadiusXProperty))
            .Add(SetSliderBinding(CreateSlider(10, 0.1), MyRectangle, Rectangle.RadiusXProperty))
            .Add(SetTextBinding(MyRectangle, Rectangle.RadiusYProperty))
            .Add(SetSliderBinding(CreateSlider(10, 0.1), MyRectangle, Rectangle.RadiusYProperty))

        End With


    End Sub
    Private Function CreateRectangle(w As Double, h As Double, b As Double, rx As Double, ry As Double) As Rectangle
        Dim r As New Rectangle
        With r
            .Width = w
            .Height = h
            .StrokeThickness = b
            .Stroke = Brushes.Black
            .RadiusX = rx
            .RadiusY = ry
        End With
        Return r
    End Function
    Private Function CreatePathRectangle(w As Double, h As Double, b As Double, rx As Double, ry As Double) As Path
        Dim r As New Rect(0, 0, w, h)
        Dim rg As New RectangleGeometry(r, rx, ry)
        Return New Path With {.Data = rg, .Stroke = Brushes.Black, .StrokeThickness = b}
    End Function

    Private Function CreateSlider(max As Double, tf As Double) As Slider
        Return New Slider With {.Maximum = max, .TickFrequency = tf,
            .LargeChange = tf, .IsSnapToTickEnabled = True}
    End Function

    Private Sub SetLocate(obj As FrameworkElement, x As Double, y As Double)
        Canvas.SetLeft(obj, x)
        Canvas.SetTop(obj, y)
    End Sub

    Private Function SetSliderBinding(sld As Slider, fe As DependencyObject, dp As DependencyProperty) As Slider
        Dim b As Binding
        b = New Binding With {.Source = fe,
            .Path = New PropertyPath(dp),
            .StringFormat = dp.ToString & " = {0}"}
        sld.SetBinding(Slider.ValueProperty, b)
        Return sld
    End Function
    Private Function SetTextBinding(e As DependencyObject, dp As DependencyProperty) As TextBlock
        Dim tb As New TextBlock
        Dim b As New Binding With {.Source = e, .Path = New PropertyPath(dp),
            .StringFormat = dp.ToString & " = {0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
        Return tb
    End Function

    Private Sub TestScale()
        Dim st As New ScaleTransform(3, 3)
        MyPathRectangle.RenderTransform = st

    End Sub

    Private Sub BtnCheck_Click(sender As Object, e As RoutedEventArgs) Handles BtnCheck.Click
        'Call TestScale()
        'Dim rg As RectangleGeometry = MyPathRectangle.Data
        'MsgBox(rg.RadiusX)
        Dim gt As GeneralTransform = MyPathRectangle.TransformToVisual(MyLeftCanvas)
        'gt = MyLeftCanvas.TransformToVisual(MyPathRectangle)
        Dim s As Size = MyPathRectangle.RenderSize
        Dim x As Double = Canvas.GetLeft(MyPathRectangle)
        Dim y As Double = Canvas.GetTop(MyPathRectangle)
        Dim r As Rect = gt.TransformBounds(New Rect(x, y, s.Width, s.Height))

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyPathRectangle)
            dc.DrawRectangle(vb, Nothing, r)
        End Using

        Dim bmp As New RenderTargetBitmap(s.Width,
                                          s.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)

        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New System.IO.FileStream("PathRectangle.png", System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using

    End Sub
End Class
